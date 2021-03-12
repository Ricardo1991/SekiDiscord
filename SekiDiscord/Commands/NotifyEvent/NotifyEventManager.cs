using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace SekiDiscord.Commands.NotifyEvent
{
    internal class NotifyEventManager
    {
        private const string NOTIFY_FILE_PATH = "TextFiles/notify.json";
        public static List<NotifyEvent> NotifyEvents = new();
        private static readonly Logger logger = new Logger(typeof(NotifyEventManager));

        static NotifyEventManager()
        {
            NotifyEvents = ReadNotifyEvents();
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