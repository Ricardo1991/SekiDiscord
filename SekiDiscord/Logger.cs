using System;
using System.Globalization;
using System.Threading.Tasks;

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

        public async Task InfoAsync(string message, string user = "") {
            await Console.Out.WriteLineAsync(string.Format(CultureInfo.InvariantCulture, "[{0}][{1}][{2}]: {3}", GetDateTimeString(), ClassType.ToString(), user, message)).ConfigureAwait(false);
        }

        public async Task ErrorAsync(string message, string user = "") {
            Console.ForegroundColor = ConsoleColor.Red;
            await Console.Error.WriteLineAsync(string.Format(CultureInfo.InvariantCulture, "[{0}][{1}][{2}]ERROR: {3}", GetDateTimeString(), ClassType.ToString(), user, message)).ConfigureAwait(false);
            Console.ResetColor();
        }

        private string GetDateTimeString() {
            return DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
