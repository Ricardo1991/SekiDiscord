﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace SekiDiscord.DiscordEvents {
    static class CommonEvents {
        private static readonly Logger logger = new Logger(typeof(CommonEvents));

#if DEBUG
        private const UserStatus BOT_PRESENCE_STATUS = UserStatus.Idle;
#else
        private const UserStatus BOT_PRESENCE_STATUS = UserStatus.Online;
#endif

        public static async Task ReadyEvent(DiscordClient c, ReadyEventArgs a) {
            SekiMain.TryReconnect = false;
            await SekiMain.DiscordClient.UpdateStatusAsync(new DiscordActivity(GetRandomStatus(FileHandler.LoadStringListFromFile(FileHandler.StringListFileType.Status))), BOT_PRESENCE_STATUS).ConfigureAwait(false);
            await logger.InfoAsync("Ready!").ConfigureAwait(false);
        }

        public static async Task SocketErrorEvent(DiscordClient c, SocketErrorEventArgs a) {
            SekiMain.TryReconnect = true;
            await logger.ErrorAsync(a.Exception.Message).ConfigureAwait(false);
        }

        public static async Task UnknownEvent(DiscordClient c, UnknownEventArgs a) {
            await logger.InfoAsync("Unknown Event: " + a.EventName).ConfigureAwait(false);
        }

        public static void StartStatusTimer() {
            Timer statusTimer = new Timer(6 * 60 * 60 * 1000); // six hours in milliseconds
            statusTimer.Elapsed += new ElapsedEventHandler(OnUpdateStatusEvent);
            statusTimer.Start();
        }

        internal static async Task SocketClosedEvent(DiscordClient sender, SocketCloseEventArgs e) {
            SekiMain.TryReconnect = true;
            await logger.InfoAsync("Socket Closed").ConfigureAwait(false);
        }

        internal static async Task ResumedEvent(DiscordClient sender, ReadyEventArgs e) {
            SekiMain.TryReconnect = false;
            await logger.InfoAsync("Resumed!").ConfigureAwait(false);
        }

        private static void OnUpdateStatusEvent(object sender, ElapsedEventArgs e) {
            try {
                logger.Info("Attempting to update user status");
                SekiMain.DiscordClient.UpdateStatusAsync(new DiscordActivity(GetRandomStatus(FileHandler.LoadStringListFromFile(FileHandler.StringListFileType.Status))), BOT_PRESENCE_STATUS);
            }
            catch (Exception ex) {
                logger.Error(ex.Message);
            }
        }

        private static string GetRandomStatus(List<string> statuses) {
            if (statuses == null || statuses.Count == 0)
                return null;

            Random r = new Random();
            int i = r.Next(statuses.Count);
            return statuses[i];
        }
    }
}
