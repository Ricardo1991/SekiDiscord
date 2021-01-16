using System;
using System.Globalization;

namespace SekiDiscord {
    public class Logger<T> {
        private Type ClassType { get; set; }

        public Logger() {
            ClassType = typeof(T);
        }

        public void Info(string message) {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}][{1}]: {2}", GetDateTimeString(), ClassType.ToString(), message));
        }

        private string GetDateTimeString() {
            return DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
