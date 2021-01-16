using System;
using System.Globalization;

namespace SekiDiscord {
    public class Logger {
        private Type ClassType { get; set; }

        public Logger(Type classType) {
            ClassType = classType;
        }

        public void Info(string message, string user = "") {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}][{1}][{2}]: {3}", GetDateTimeString(), ClassType.ToString(), user, message));
        }

        public void Error(string message, string user = "") {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}][{1}][{2}]ERROR: {3}", GetDateTimeString(), ClassType.ToString(), user, message));
            Console.ResetColor();
        }

        private string GetDateTimeString() {
            return DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
