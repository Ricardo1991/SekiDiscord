using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace SekiDiscord.DiscordEvents {
    static class CommonEvents {
        private static readonly Logger logger = new Logger(typeof(CommonEvents));

        public static async Task ReadyEvent(ReadyEventArgs a) {
            SekiMain.TryReconnect = false;
            await SekiMain.DiscordClient.UpdateStatusAsync(new DiscordGame(GetRandomStatus(FileHandler.GetStatusList()))).ConfigureAwait(false);
            await logger.InfoAsync("Ready!").ConfigureAwait(false);
        }

        public static async Task SocketErrorEvent(SocketErrorEventArgs a) {
            SekiMain.TryReconnect = true;
            await logger.ErrorAsync(a.Exception.Message).ConfigureAwait(false);
        }

        public static async Task UnknownEvent(UnknownEventArgs a) {
            await logger.InfoAsync("Unknown Event: " + a.EventName).ConfigureAwait(false);
        }

        public static void StartStatusTimer() {
            Timer statusTimer = new Timer(6 * 60 * 60 * 1000); // six hours in milliseconds
            statusTimer.Elapsed += new ElapsedEventHandler(OnUpdateStatusEvent);
            statusTimer.Start();
        }

        private static void OnUpdateStatusEvent(object sender, ElapsedEventArgs e) {
            try {
                logger.Info("Attempting to update user status");
                SekiMain.DiscordClient.UpdateStatusAsync(new DiscordGame(GetRandomStatus(FileHandler.GetStatusList())));
            }
            catch (Exception ex) {
                logger.Error(ex.Message);
            }
        }

        private static string GetRandomStatus(List<string> statuses) {
            Random r = new Random();
            int i = r.Next(statuses.Count);
            return statuses[i];
        }
    }
}
