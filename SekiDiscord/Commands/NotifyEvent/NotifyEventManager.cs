using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SekiDiscord.Commands.NotifyEvent
{
    public class NotifyEventManager
    {
        private const string NOTIFY_FILE_PATH = "TextFiles/notify.json";
        public static Dictionary<string,NotifyEvent> NotifyEvents = new();
        private static readonly Logger logger = new Logger(typeof(NotifyEventManager));

        public static void LoadAndEnableEvents()
        {
            NotifyEvents = ReadNotifyEvents();
            foreach (KeyValuePair<string, NotifyEvent> a in NotifyEvents)
            {
                if (a.Value.Enabled)
                    a.Value.EnableEvent();
            }
        }

        public static bool AddEvent(NotifyEvent notifyEvent)
        {
            try
            {
                NotifyEvents.Add(notifyEvent.Name, notifyEvent);

            }
            catch(ArgumentException)
            {
                return false;
            }
            
            SaveNotifyEvents(NotifyEvents);
            return true;
        }

        public static bool AddEvent(string inputFormat)
        {
            (string, DateTime, TimeSpan) formatedInput = InputArgumentToEventData(inputFormat);
            NotifyEvent newEvent = new NotifyEvent(formatedInput.Item1, formatedInput.Item2, formatedInput.Item3);
            return AddEvent(newEvent);
        }

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

        public static bool SubscribeUserToEvent(ulong userID, ulong guildID, string eventName)
        {
            NotifyEvent selectedEvent = NotifyEvents.GetValueOrDefault(eventName);
            bool result = selectedEvent.SubscribeUser(userID, guildID);
            SaveNotifyEvents(NotifyEvents);
            return result;
        }

        public static bool UnsubscribeUserToEvent(ulong userID, ulong guildID, string eventName)
        {

            NotifyEvent selectedEvent = NotifyEvents.GetValueOrDefault(eventName);
            bool result = selectedEvent.UnsubscribeUser(userID, guildID);
            SaveNotifyEvents(NotifyEvents);
            return result;
        }

        public static bool EnableEvent(string eventName)
        {
            try
            {
                NotifyEvent selectedEvent = NotifyEvents.GetValueOrDefault(eventName);
                selectedEvent.EnableEvent();
                SaveNotifyEvents(NotifyEvents);
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
                NotifyEvent selectedEvent = NotifyEvents.GetValueOrDefault(eventName);
                selectedEvent.DisableEvent();
                SaveNotifyEvents(NotifyEvents);
            }
            catch
            {
                return false;
            }
           
            return true;
        }

        private static Dictionary<string, NotifyEvent> ReadNotifyEvents()
        {
            Dictionary<string, NotifyEvent> notifyEvents = new();

            if (File.Exists(NOTIFY_FILE_PATH))
            {
                try
                {
                    using StreamReader r = new StreamReader(NOTIFY_FILE_PATH);
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
                JsonSerializer serializer = new JsonSerializer();
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