using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace SekiDiscord.Commands.NotifyEvent
{
    internal class NotifyEventManager
    {
        private const string NOTIFY_FILE_PATH = "TextFiles/notify.json";
        public static List<NotifyEvent> NotifyEvents = new();
        private static readonly Logger logger = new Logger(typeof(NotifyEventManager));
        private static List<Timer> TimerList = new();

        static NotifyEventManager()
        {
            NotifyEvents = ReadNotifyEvents();
        }

        public static void AddEvent(NotifyEvent notifyEvent)
        {
            NotifyEvents.Add(notifyEvent);
            SaveNotifyEvents(NotifyEvents);
            //Todo: refresh event timers to add new one
        }

        public static bool SubscribeUserToEvent(ulong userID, ulong guildID, string eventName)
        {
            NotifyEvent selectedEvent = NotifyEvents.Where(e => e.Name.Equals(eventName)).First();
            return selectedEvent.SubscribeUser(userID, guildID);
        }

        public static bool UnsubscribeUserToEvent(ulong userID, ulong guildID, string eventName)
        {

            NotifyEvent selectedEvent = NotifyEvents.Where(e => e.Name.Equals(eventName)).First();
            return selectedEvent.UnsubscribeUser(userID, guildID);
        }

        public static bool EnableEvent(string eventName)
        {
            try
            {
                NotifyEvent selectedEvent = NotifyEvents.Where(e => e.Name.Equals(eventName)).First();
                selectedEvent.EnableEvent();
            }
            catch
            {
                return false;
            }
           
            return true;
        }

        public static bool DisableEvent(string eventName)
        {
            try
            {
                NotifyEvent selectedEvent = NotifyEvents.Where(e => e.Name.Equals(eventName)).First();
                selectedEvent.DisableEvent();
            }
            catch
            {
                return false;
            }
           
            return true;
        }

        private static List<NotifyEvent> ReadNotifyEvents()
        {
            List<NotifyEvent> notifyEvents = new();

            if (File.Exists(NOTIFY_FILE_PATH))
            {
                try
                {
                    using StreamReader r = new StreamReader(NOTIFY_FILE_PATH);
                    string json = r.ReadToEnd();
                    notifyEvents = JsonConvert.DeserializeObject<List<NotifyEvent>>(json);
                }
                catch (JsonException e)
                {
                    logger.Error("COULD NOT READ NOTIFY EVENTS: " + e.Message);
                }
            }

            logger.Info("Loaded " + notifyEvents.Count + " events");
            return notifyEvents;
        }

        private static void SaveNotifyEvents(List<NotifyEvent> notifyEvents)
        {
            try
            {
                using StreamWriter w = File.CreateText(NOTIFY_FILE_PATH);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(w, notifyEvents);
            }
            catch (JsonException e)
            {
                logger.Error("COULD NOT SAVE NOTIFY EVENTS: " + e.Message);
            }
        }
    }
}