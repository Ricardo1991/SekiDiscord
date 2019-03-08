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
        private static DiscordClient discord;
        private static CommandsNextModule commands;

        private static StringLibrary StringLib { get; set; } = new StringLibrary();

        public static async Task DMUser(DiscordUser user, string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                await discord.SendMessageAsync(await discord.CreateDmAsync(user), msg);
            }
        }

        public static DiscordClient GetDiscordClient
        {
            get
            {
                return discord;
            }
            set
            {
                discord = value;
            }
        }

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

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            discord.SocketErrored += async a =>
            {
                tryReconnect = true;
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Error: " + a.Exception.Message);
            };

            discord.Ready += async a =>
            {
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Ready!");
                tryReconnect = false;
            };

            discord.UnknownEvent += async unk =>
            {
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Unknown Event: " + unk.EventName);
            };

            discord.MessageCreated += async e =>
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
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":regional_indicator_w:"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":regional_indicator_a:"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":regional_indicator_i:"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":regional_indicator_f:"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(discord, ":regional_indicator_u:"));
                    }
                }

                //Update "last seen" for user that sent the message
                Seen.MarkUserSeen(e, StringLib);
                //Ping users, leave this last cause it's sloooooooow
                await PingUser.SendPings(e, StringLib);
            };

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            SekiCommands.SetStringLibrary(StringLib);
            commands.RegisterCommands<SekiCommands>();

            //Connect to Discord
            await discord.ConnectAsync();

            botName = discord.CurrentUser.Username;
            while (!quit)
            {
                if (tryReconnect)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Attempting to Reconnect...");
                    await discord.ConnectAsync();
                }

                //Wait a bit
                await Task.Delay(10 * 1000);
            }

            await discord.DisconnectAsync();
        }
    }
}