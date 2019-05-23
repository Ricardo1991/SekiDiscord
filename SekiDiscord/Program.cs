using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SekiDiscord.Commands;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace SekiDiscord
{
    internal class Program
    {
        private static CommandsNextModule commands;

        private static StringLibrary StringLib { get; set; } = new StringLibrary();

        public static async Task DMUser(DiscordUser user, string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                await GetDiscordClient.SendMessageAsync(await GetDiscordClient.CreateDmAsync(user).ConfigureAwait(false), msg).ConfigureAwait(false);
            }
        }

        public static DiscordClient GetDiscordClient { get; set; }

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Not enough arguments. Usage: SekiDiscord <discord-api-key>. Quitting.");
                return;
            }

            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            string token = args[0];
            bool quit = false;
            bool tryReconnect = false;
            string botName = string.Empty;

            //TODO: This should be fixed once .net Core 3.0 is released
            /*if (Settings.Default.UpgradeNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeNeeded = false;
                Settings.Default.Save();
            }*/

            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Starting...");

            if (string.IsNullOrWhiteSpace(Settings.Default.apikey))
            {
                string api = string.Empty;
                if (args.Length > 1)
                {
                    api = args[1];
                }
                else
                {
                    Console.WriteLine("Add api key for youtube search (or enter to ignore): ");
                    api = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(api))
                {
                    Settings.Default.apikey = api;
                    //TODO: This should be fixed once .net Core 3.0 is released
                    //Settings.Default.Save();
                }
            }
            if (string.IsNullOrWhiteSpace(Settings.Default.CleverbotAPI))
            {
                string api = string.Empty;
                if (args.Length > 2)
                {
                    api = args[2];
                }
                else
                {
                    Console.WriteLine("Add api key for Cleverbot (or enter to ignore): ");
                    api = Console.ReadLine();
                }
                if (!string.IsNullOrWhiteSpace(api))
                {
                    Settings.Default.CleverbotAPI = api;
                    //TODO: This should be fixed once .net Core 3.0 is released
                    //Settings.Default.Save();
                }
            }

            GetDiscordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            GetDiscordClient.SocketErrored += async a =>
            {
                tryReconnect = true;
                await Console.Out.WriteLineAsync(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Error: " + a.Exception.Message).ConfigureAwait(false);
            };

            GetDiscordClient.Ready += async a =>
            {
                await Console.Out.WriteLineAsync(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Ready!").ConfigureAwait(false);
                tryReconnect = false;
            };

            GetDiscordClient.UnknownEvent += async unk =>
            {
                await Console.Out.WriteLineAsync(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Unknown Event: " + unk.EventName).ConfigureAwait(false);
            };

            GetDiscordClient.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("!quit"))
                {
                    DiscordMember author = await e.Guild.GetMemberAsync(e.Author.Id).ConfigureAwait(false);
                    bool isBotAdmin = Useful.MemberIsBotOperator(author);

                    if (author.IsOwner || isBotAdmin)
                    {
                        quit = true;
                        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Quitting...");
                    }
                }

                //CustomCommands
                else if (e.Message.Content.StartsWith('!'))
                {
                    string arguments = string.Empty;
                    string[] split = e.Message.Content.Split(new char[] { ' ' }, 2);
                    string command = split[0];
                    if (split.Length > 1) arguments = split[1];
                    string result = CustomCommand.UseCustomCommand(command.TrimStart('!'), arguments, e, StringLib);
                    if (!string.IsNullOrEmpty(result))
                        await e.Message.RespondAsync(result).ConfigureAwait(false);
                }

                //Bot Talk and Cleverbot
                else if (e.Message.Content.StartsWith(botName + ",", StringComparison.OrdinalIgnoreCase))
                {
                    if (e.Message.Content.EndsWith('?'))
                    {
                    }
                    else
                    {
                        await Think(e, botName).ConfigureAwait(false);
                    }
                }
                else if (e.Message.Content.EndsWith(botName, StringComparison.OrdinalIgnoreCase))
                {
                    await Think(e, botName).ConfigureAwait(false);
                }

                //waifunator
                if (!string.IsNullOrWhiteSpace(e.Message.Content) && e.Message.Author.Id == Settings.Default.limid) //lims shitty id lul
                {
                    if (e.Message.Content.ToLower().Contains("wife"))
                    {
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_w:")).ConfigureAwait(false);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_a:")).ConfigureAwait(false);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_i:")).ConfigureAwait(false);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_f:")).ConfigureAwait(false);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_u:")).ConfigureAwait(false);
                    }
                }

                //Update "last seen" for user that sent the message
                Seen.MarkUserSeen(e, StringLib);
                //Ping users, leave this last cause it's sloooooooow
                await PingUser.SendPings(e, StringLib).ConfigureAwait(false);
            };

            //Register the commands defined on SekiCommands.cs

            commands = GetDiscordClient.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!",
                CaseSensitive = false,
                EnableMentionPrefix = false
            });

            SekiCommands.SetStringLibrary(StringLib);
            commands.RegisterCommands<SekiCommands>();

            //Connect to Discord
            await GetDiscordClient.ConnectAsync().ConfigureAwait(false);

            botName = GetDiscordClient.CurrentUser.Username;

            while (!quit)
            {
                if (tryReconnect)
                {
                    try
                    {
                        await GetDiscordClient.DisconnectAsync().ConfigureAwait(false);
                    }
                    finally
                    {
                        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Attempting to Reconnect...");
                        await GetDiscordClient.ConnectAsync().ConfigureAwait(false);
                    }
                }

                //Wait a bit
                await Task.Delay(10 * 1000).ConfigureAwait(false);
            }

            await GetDiscordClient.DisconnectAsync().ConfigureAwait(false);
        }

        private static async Task Think(MessageCreateEventArgs e, string bot)
        {
            if (string.IsNullOrWhiteSpace(Settings.Default.CleverbotAPI))
                return;

            string input = e.Message.Content;

            //Show the "bot is typing..." message
            await e.Channel.TriggerTypingAsync().ConfigureAwait(false);

            string response = await BotTalk.BotThinkAsync(input, bot).ConfigureAwait(false);
            await e.Message.RespondAsync(response).ConfigureAwait(false);
        }
    }
}