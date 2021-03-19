using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SekiDiscord.Commands.NotifyEvent
{
    public class NotifyEventManager
    {
        public const string NOTIFY_FILE_PATH = "TextFiles/notify.json";
        private static Dictionary<string, NotifyEvent> NotifyEvents;
        private static readonly Logger logger = new(typeof(NotifyEventManager));

        public static void LoadAndEnableEvents()
        {
            NotifyEvents = ReadNotifyEvents();
            foreach (KeyValuePair<string, NotifyEvent> a in NotifyEvents)
            {
                if (a.Value.Enabled)
                    a.Value.EnableEvent();
            }
        }

        public static int NotifyEventCount()
        {
            if (NotifyEvents != null)
                return NotifyEvents.Count;
            else return 0;
        }

        public static string[] getNotifyEventDetails(bool sendExtraDetail)
        {
            List<string> list = new();

            foreach (KeyValuePair<string, NotifyEvent> eventN in NotifyEvents)
            {
                StringBuilder sb = new();
                {
                    sb.Append("Enabled: ").Append(eventN.Value.Enabled)
                        .Append("; Subscribers: ").Append(eventN.Value.EventSubscribers.Count)
                        .Append("; Name: ").Append(eventN.Value.Name);
                    if (sendExtraDetail)
                        sb.Append("; Minutes until next trigger: ").Append(NotifyEvent.TimeForNextNotification(eventN.Value.EventStart, eventN.Value.RepeatPeriod));
                }
                list.Add(sb.ToString());
            }

            return list.ToArray();
        }

        /// <summary>
        /// Add event by NotifyEvent object
        /// </summary>
        /// <param name="notifyEvent">NotifyEvent object already instatiated with the parsed data</param>
        public static void AddEvent(NotifyEvent notifyEvent)
        {
            try
            {
                NotifyEvents.Add(notifyEvent.Name, notifyEvent);
            }
            catch
            {
                throw;
            }

            SaveNotifyEvents(NotifyEvents);
        }

        /// <summary>
        /// Add event by argument string
        /// </summary>
        /// <param name="inputFormat">string that should be composed of (string, DateTime, TimeSpan), each separated by ";"</param>
        public static void AddEvent(string inputFormat)
        {
            try
            {
                (string, DateTime, TimeSpan) formatedInput = InputArgumentToEventData(inputFormat);
                NotifyEvent newEvent = new NotifyEvent(formatedInput.Item1, formatedInput.Item2, formatedInput.Item3);
                AddEvent(newEvent);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Converts the request string into a triplet of (string, DateTime, TimeSpan)
        /// </summary>
        /// <param name="inputFormat">string that should be composed of (string, DateTime, TimeSpan), each separated by ";"</param>
        /// <returns>(string, DateTime, TimeSpan) triplet containing event name, event start DateTime, and repeat period TimeSpan</returns>
        public static (string, DateTime, TimeSpan) InputArgumentToEventData(string inputFormat)
        {
            string[] argumentSplit = inputFormat.Split(';', 3);

            if (argumentSplit.Length < 3)
            {
                logger.Error("invalid input on InputArgumentToEventData: " + inputFormat);
                throw new ArgumentException("input format invalid");
            }

            string name = argumentSplit[0].Trim();

            DateTime eventStart = StringToDateTime(argumentSplit[1]);
            TimeSpan repeatPeriod = StringToTimeSpan(argumentSplit[2]);

            return (name, eventStart, repeatPeriod);
        }

        private static TimeSpan StringToTimeSpan(string v)
        {
            return TimeSpan.Parse(v);
        }

        private static DateTime StringToDateTime(string dateInput)
        {
            return DateTime.Parse(dateInput);
        }

        /// <summary>
        /// Subscribe user from event, by name.
        /// </summary>
        /// <param name="userID">ulong with user ID</param>
        /// <param name="guildID">ulong with guild ID where the request originated</param>
        /// <param name="eventName">event name to subscribe to</param>
        /// <returns>boolean with the success of the operation</returns>
        public static bool SubscribeUserToEvent(ulong userID, ulong guildID, string eventName)
        {
            NotifyEvent selectedEvent = NotifyEvents.GetValueOrDefault(eventName);
            bool result = selectedEvent.SubscribeUser(userID, guildID);
            SaveNotifyEvents(NotifyEvents);
            return result;
        }

        /// <summary>
        /// Unsubscribe user from event, by name.
        /// </summary>
        /// <param name="userID">ulong with user ID</param>
        /// <param name="guildID">ulong with guild ID where the request originated</param>
        /// <param name="eventName">event name to unsubscribe from</param>
        /// <returns>boolean with the success of the operation</returns>
        public static bool UnsubscribeUserToEvent(ulong userID, ulong guildID, string eventName)
        {
            NotifyEvent selectedEvent = NotifyEvents.GetValueOrDefault(eventName);
            bool result = selectedEvent.UnsubscribeUser(userID, guildID);
            SaveNotifyEvents(NotifyEvents);
            return result;
        }

        /// <summary>
        /// Enables an event by name
        /// </summary>
        /// <param name="eventName">Name of the event to enable</param>
        public static void EnableEvent(string eventName)
        {
            try
            {
                NotifyEvents.TryGetValue(eventName, out NotifyEvent selectedEvent);

                if (selectedEvent == null)
                    throw new NullReferenceException(eventName + " event not found");
                selectedEvent.EnableEvent();
                SaveNotifyEvents(NotifyEvents);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Disables an event by name
        /// </summary>
        /// <param name="eventName">Name of the event to disable</param>
        public static void DisableEvent(string eventName)
        {
            try
            {
                NotifyEvents.TryGetValue(eventName, out NotifyEvent selectedEvent);

                if (selectedEvent == null)
                    throw new NullReferenceException(eventName + " event not found");
                selectedEvent.DisableEvent();
                SaveNotifyEvents(NotifyEvents);
            }
            catch
            {
                throw;
            }
        }

        private static Dictionary<string, NotifyEvent> ReadNotifyEvents()
        {
            Dictionary<string, NotifyEvent> notifyEvents = new();

            if (File.Exists(NOTIFY_FILE_PATH))
            {
                try
                {
                    using StreamReader r = new(NOTIFY_FILE_PATH);
                    string json = r.ReadToEnd();
                    notifyEvents = JsonConvert.DeserializeObject<Dictionary<string, NotifyEvent>>(json);
                    logger.Info("Loaded notification events");
                }
                catch (JsonException e)
                {
                    logger.Error("Could not load notification events: " + e.Message);
                }
            }

            logger.Info("Loaded " + notifyEvents.Count + " events");
            return notifyEvents;
        }

        private static void SaveNotifyEvents(Dictionary<string, NotifyEvent> notifyEvents)
        {
            try
            {
                using StreamWriter w = File.CreateText(NOTIFY_FILE_PATH);
                JsonSerializer serializer = new();
                serializer.Serialize(w, notifyEvents);
                logger.Info("Loaded notification events");
            }
            catch (JsonException e)
            {
                logger.Error("Could not save notification events: " + e.Message);
            }
        }
    }
}