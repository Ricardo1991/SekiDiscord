using DSharpPlus;
using DSharpPlus.CommandsNext;
using SekiDiscord.DiscordEvents;
using System;
using System.Threading.Tasks;

namespace SekiDiscord {
    public class SekiMain {
        private static readonly Logger logger = new Logger(typeof(SekiMain));

        public static DiscordClient DiscordClient { get; set; }
        public static bool Quit { get; set; }
        public static bool TryReconnect { get; set; }
        public static string BotName { get; set; }

        private static string[] Arguments { get; set; }
        private static CommandsNextModule Commands { get; set; }

        public SekiMain(string[] args) {
            Arguments = args;
            SetupApiKeys();
        }

        public async Task StartupAsync() {
            logger.Info("Starting...");

            DiscordClient = new DiscordClient(new DiscordConfiguration {
                Token = Arguments[0],
                TokenType = TokenType.Bot
            });

            SetupCommands();

            DiscordClient.SocketErrored += CommonEvents.SocketErrorEvent;

            DiscordClient.Ready += CommonEvents.ReadyEvent;

            DiscordClient.UnknownEvent += CommonEvents.UnknownEvent;

            DiscordClient.MessageCreated += MessageEvents.MessageCreatedEvent;

            SekiDiscord.Commands.NotifyEvent.NotifyEventManager.LoadAndEnableEvents();

            // Connect to Discord:
            await DiscordClient.ConnectAsync().ConfigureAwait(false);

            BotName = DiscordClient.CurrentUser.Username;

            CommonEvents.StartStatusTimer();

            await Update().ConfigureAwait(false);

            await DiscordClient.DisconnectAsync().ConfigureAwait(false);
        }

        private static async Task Update() {
            while (!Quit) {
                if (TryReconnect) {
                    try {
                        await DiscordClient.DisconnectAsync().ConfigureAwait(false);
                    } finally {
                        logger.Info("Attempting to Reconnect...");
                        await DiscordClient.ConnectAsync().ConfigureAwait(false);
                    }
                }
                // Wait a bit
                await Task.Delay(10 * 1000).ConfigureAwait(false);
            }
        }

        private static void SetupCommands() {
            Commands = DiscordClient.UseCommandsNext(new CommandsNextConfiguration {
                StringPrefix = Settings.Default.commandChar.ToString(),
                CaseSensitive = false,
                EnableMentionPrefix = false
            });
            Commands.RegisterCommands<SekiCommands>();
        }

        private static void SetupApiKeys() {
            if (string.IsNullOrWhiteSpace(Settings.Default.apikey)) {
                string api = GetApiKey(1, "Add api key for youtube search (or enter to ignore): ");
                if (!string.IsNullOrWhiteSpace(api)) {
                    Settings.Default.apikey = api;
                }
            }
            if (string.IsNullOrWhiteSpace(Settings.Default.CleverbotAPI)) {
                string api = GetApiKey(2, "Add api key for Cleverbot (or enter to ignore): ");
                if (!string.IsNullOrWhiteSpace(api)) {
                    Settings.Default.CleverbotAPI = api;
                }
            }

            Settings.Default.Save();
        }

        private static string GetApiKey(int index, string message) {
            if (Arguments.Length > index) {
                return Arguments[index];
            }
            logger.Info(message);
            return Console.ReadLine();
        }
    }
}
