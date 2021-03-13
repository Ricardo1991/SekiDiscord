using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SekiDiscord.Commands.NotifyEvent
{
    public class NotifyEvent
    {
        private static readonly Logger logger = new Logger(typeof(NotifyEvent));

        private bool enabled = false;
        private DateTime eventStart;
        private HashSet<(ulong, ulong)> eventSubscribers = new();
        private string name;
        private TimeSpan repeatPeriod;
        private Timer triggerEventTimer;

        public NotifyEvent(string name, DateTime eventStart, TimeSpan repeatPeriod)
        {
            this.eventStart = eventStart;
            this.repeatPeriod = repeatPeriod;
            this.Name = name;

            SetupFirstTimer();
        }

        private bool Enabled { get => enabled; set => enabled = value; }
        public string Name { get => name; set => name = value; }


        public void DisableEvent()
        {
            Enabled = false;
            triggerEventTimer.Stop();
        }

        public void EnableEvent()
        {
            Enabled = true;
            SetupFirstTimer();
        }

        public static int TimeForNextNotification(DateTime eventStart, TimeSpan repeatPeriod)
        {
            DateTime now = DateTime.Now;
            double a;

            if(now>eventStart)
            {
                a = (now - eventStart).TotalMinutes;
                return Convert.ToInt32(Math.Floor(a % repeatPeriod.TotalMinutes));
            }
            else
            {
                a = (eventStart - now).TotalMinutes;
                return Convert.ToInt32(Math.Ceiling(a + repeatPeriod.TotalMinutes));
            }

        }

        public async void OnNotifyEventTriggerAsync(object sender, ElapsedEventArgs e)
        {
            SetupTimer();

            try
            {
                logger.Info("Attempting to sent notification");

                StringBuilder message = new("Event \"");
                message.Append(Name).Append("\" was raised for");

                foreach ((ulong, ulong) userobj in eventSubscribers)
                {
                    string memberMention = await GetUserMentionAsync(userobj);

                    if (memberMention != null)
                    {
                        message.Append(' ').Append(memberMention);
                    }
                }

                DiscordChannel channel = await SekiMain.DiscordClient.GetChannelAsync(Settings.Default.ping_channel_id).ConfigureAwait(false); // get channel from channel id
                await SekiMain.DiscordClient.SendMessageAsync(channel, message.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Subscribe a user to the event
        /// Returns true if the user was added, and false if it already existed.
        /// </summary>
        /// <param name="userID">Discord ID of user to subscribe</param>
        /// <param name="userGuild">Discord Guild ID of user to subscribe</param>
        /// <returns></returns>
        public bool SubscribeUser(ulong userID, ulong userGuild)
        {
            return eventSubscribers.Add((userID, userGuild));
        }

        /// <summary>
        /// Unsubscribe a user from this event
        /// Returns true if the user was found and removed, and false if it was not.
        /// </summary>
        /// <param name="userID">Discord ID of user to unsubscribe</param>
        /// <param name="userGuild">Discord Guild ID of user to unsubscribe</param>
        /// <returns></returns>
        public bool UnsubscribeUser(ulong userID, ulong userGuild)
        {
            bool removeSuccessful = eventSubscribers.Remove((userID, userGuild));

            if (eventSubscribers.Count == 0)
                Enabled = false;

            return removeSuccessful;
        }

        private async Task<string> GetUserMentionAsync((ulong, ulong) userGuildPair)
        {
            DiscordGuild guild = await SekiMain.DiscordClient.GetGuildAsync(userGuildPair.Item2);

            DiscordMember member = guild.Members.Where(mem => mem.Id.Equals(userGuildPair.Item1)).FirstOrDefault();
            return member.Mention;
        }

        private void SetupFirstTimer()
        {
            triggerEventTimer = new Timer(TimeForNextNotification(eventStart, repeatPeriod) * 60 * 1000); // span minutes converted to milliseconds)
            triggerEventTimer.Elapsed += new ElapsedEventHandler(OnNotifyEventTriggerAsync);
            triggerEventTimer.Start();
        }

        private void SetupTimer()
        {
            triggerEventTimer.Interval = repeatPeriod.TotalMinutes * 60 * 1000;
            triggerEventTimer.Stop();
            triggerEventTimer.Start();
        }
    }
}