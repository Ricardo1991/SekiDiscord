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
        private static readonly Logger logger = new(typeof(NotifyEvent));

        private bool enabled = false;
        private DateTime eventStart;
        private HashSet<(ulong, ulong)> eventSubscribers = new();
        private string name;
        private TimeSpan repeatPeriod;
        private Timer triggerEventTimer;

        public NotifyEvent(string name, DateTime eventStart, TimeSpan repeatPeriod)
        {
            this.EventStart = eventStart;
            this.RepeatPeriod = repeatPeriod;
            this.Name = name;
        }

        public bool Enabled { get => enabled; set => enabled = value; }
        public string Name { get => name; set => name = value; }
        public DateTime EventStart { get => eventStart; set => eventStart = value; }
        public TimeSpan RepeatPeriod { get => repeatPeriod; set => repeatPeriod = value; }
        public HashSet<(ulong, ulong)> EventSubscribers { get => eventSubscribers; set => eventSubscribers = value; }

        /// <summary>
        /// Disables the event, and stops the timer
        /// </summary>
        public void DisableEvent()
        {
            Enabled = false;
            triggerEventTimer.Stop();
        }

        /// <summary>
        /// Enables the Event, and sets up the timer
        /// </summary>
        public void EnableEvent()
        {
            Enabled = true;
            logger.Info("Enabled event " + Name);
            SetupFirstTimer();
        }

        /// <summary>
        /// Calculates the minutes left until the next notification should be fired.
        /// </summary>
        /// <param name="eventStart">DateTime with the start of the event</param>
        /// <param name="repeatPeriod">TimeSpan with the repeat period of the notification</param>
        /// <returns>int with the number of minutes remaining</returns>
        public static int TimeForNextNotification(DateTime eventStart, TimeSpan repeatPeriod)
        {
            DateTime now = DateTime.Now.ToUniversalTime();

            if (now > eventStart)
            {
                double a = (now - eventStart).TotalMinutes;
                return Convert.ToInt32(Math.Floor(a % repeatPeriod.TotalMinutes));
            }
            else
            {
                double a = (eventStart - now).TotalMinutes;
                return Convert.ToInt32(Math.Ceiling(a + repeatPeriod.TotalMinutes));
            }
        }

        private async void OnNotifyEventTriggerAsync(object sender, ElapsedEventArgs e)
        {
            SetupTimer(RepeatPeriod.TotalMinutes * 60 * 1000);

            try
            {
                logger.Info("Attempting to sent notification");

                StringBuilder message = new("Event \"");
                message.Append(Name).Append("\" was raised for");

                foreach ((ulong, ulong) userobj in EventSubscribers)
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
            return EventSubscribers.Add((userID, userGuild));
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
            bool removeSuccessful = EventSubscribers.Remove((userID, userGuild));

            if (EventSubscribers.Count == 0)
                Enabled = false;

            return removeSuccessful;
        }

        private static async Task<string> GetUserMentionAsync((ulong, ulong) userGuildPair)
        {
            DiscordGuild guild = await SekiMain.DiscordClient.GetGuildAsync(userGuildPair.Item2);

            DiscordMember member = guild.Members.Where(mem => mem.Id.Equals(userGuildPair.Item1)).FirstOrDefault();
            return member.Mention;
        }

        private void SetupFirstTimer()
        {
            double interval = TimeForNextNotification(EventStart, RepeatPeriod) * 60 * 1000; // span minutes converted to milliseconds)

            if (interval == 0)
                interval = RepeatPeriod.TotalMinutes * 60 * 1000;

            SetupTimer(interval);
        }

        private void SetupTimer(double interval)
        {
            if (triggerEventTimer != null)
                triggerEventTimer.Stop();

            triggerEventTimer = new Timer(interval)
            {
                AutoReset = false,
                Interval = interval
            };
            triggerEventTimer.Elapsed += new ElapsedEventHandler(OnNotifyEventTriggerAsync);
            triggerEventTimer.Start();
        }
    }
}