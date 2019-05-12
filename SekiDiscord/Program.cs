using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using SekiDiscord.Commands;
using System;
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
                await discordClient.SendMessageAsync(await discordClient.CreateDmAsync(user), msg);
            }
        }

        public static DiscordClient discordClient { get; set; }

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Not enough arguments. Usage: SekiDiscord <discord-api-key> <google-api-key>. Quitting.");
                return;
            }
            //TODO: This should be fixed once .net Core 3.0 is released
            /*if (Settings.Default.UpgradeNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeNeeded = false;
                Settings.Default.Save();
            }*/
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

            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Starting...");
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            string token = args[0];
            bool quit = false;
            bool tryReconnect = false;
            string botName = string.Empty;

            discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            discordClient.SocketErrored += async a =>
            {
                tryReconnect = true;
                await Console.Out.WriteLineAsync(DateTime.Now.ToString("[HH:mm:ss] ") + "Error: " + a.Exception.Message);
            };

            discordClient.Ready += async a =>
            {
                await Console.Out.WriteLineAsync(DateTime.Now.ToString("[HH:mm:ss] ") + "Ready!");
                tryReconnect = false;
            };

            discordClient.UnknownEvent += async unk =>
            {
                await Console.Out.WriteLineAsync(DateTime.Now.ToString("[HH:mm:ss] ") + "Unknown Event: " + unk.EventName);
            };

            discordClient.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("!quit"))
                {
                    DiscordMember author = await e.Guild.GetMemberAsync(e.Author.Id);
                    bool isBotAdmin = Useful.MemberIsBotOperator(author);

                    if (author.IsOwner || isBotAdmin)
                    {
                        quit = true;
                        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Quitting...");
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
                        await e.Message.RespondAsync(result);
                }

                //Bot Talk and Cleverbot
                else if (e.Message.Content.StartsWith(botName + ",", StringComparison.OrdinalIgnoreCase))
                {
                    if (e.Message.Content.EndsWith('?'))
                    {
                    }
                    else
                    {
                        await BotTalk.BotThink(e, StringLib, botName);
                    }
                }
                else if (e.Message.Content.EndsWith(botName, StringComparison.OrdinalIgnoreCase))
                {
                    await BotTalk.BotThink(e, StringLib, botName);
                }

                //waifunator
                if (!string.IsNullOrWhiteSpace(e.Message.Content) && e.Message.Author.Id == Settings.Default.limid) //lims shitty id lul
                {
                    if (e.Message.Content.ToLower().Contains("wife"))
                    {
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discordClient, ":regional_indicator_w:"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discordClient, ":regional_indicator_a:"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discordClient, ":regional_indicator_i:"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discordClient, ":regional_indicator_f:"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discordClient, ":regional_indicator_u:"));
                    }
                }

                //Update "last seen" for user that sent the message
                Seen.MarkUserSeen(e, StringLib);
                //Ping users, leave this last cause it's sloooooooow
                await PingUser.SendPings(e, StringLib);
            };

            //Register the commands defined on SekiCommands.cs
            commands = discordClient.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!",
                CaseSensitive = false,
                EnableMentionPrefix = false
            });

            SekiCommands.SetStringLibrary(StringLib);
            commands.RegisterCommands<SekiCommands>();

            //Connect to Discord
            await discordClient.ConnectAsync();

            botName = discordClient.CurrentUser.Username;
            while (!quit)
            {
                if (tryReconnect)
                {
                    try
                    {
                        await discordClient.DisconnectAsync();
                    }
                    finally
                    {
                        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Attempting to Reconnect...");
                        await discordClient.ConnectAsync();
                    }
                }

                //Wait a bit
                await Task.Delay(10 * 1000);
            }

            await discordClient.DisconnectAsync();
        }
    }
}