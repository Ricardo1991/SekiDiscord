using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SekiDiscord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace SekiDiscord
{
    internal class Program
    {
        private static readonly Logger logger = new Logger(typeof(Program));
        private static CommandsNextModule commands;

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
                logger.Error("Not enough arguments. Usage: SekiDiscord <discord-api-key>. Quitting.");
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
            char commandChar = '!';

            //TODO: This should be fixed once .net Core 3.0 is released
            /*if (Settings.Default.UpgradeNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeNeeded = false;
                Settings.Default.Save();
            }*/

            logger.Info("Starting...");

            if (string.IsNullOrWhiteSpace(Settings.Default.apikey))
            {
                string api = string.Empty;
                if (args.Length > 1)
                {
                    api = args[1];
                }
                else
                {
                    logger.Info("Add api key for youtube search (or enter to ignore): ");
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
                    logger.Info("Add api key for Cleverbot (or enter to ignore): ");
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
                await GetDiscordClient.UpdateStatusAsync(new DiscordGame(getRandomStatus())).ConfigureAwait(false);
            };

            GetDiscordClient.UnknownEvent += async unk =>
            {
                await Console.Out.WriteLineAsync(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Unknown Event: " + unk.EventName).ConfigureAwait(false);
            };

            //Register the commands defined on SekiCommands.cs

            commands = GetDiscordClient.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = commandChar.ToString(),
                CaseSensitive = false,
                EnableMentionPrefix = false
            });

            commands.RegisterCommands<SekiCommands>();

            GetDiscordClient.MessageCreated += async e =>
            {
                if (e.Message.Content.StartsWith(commandChar + "quit", StringComparison.OrdinalIgnoreCase))
                {
                    logger.Info("Quit request received, confirming...");

                    if (e.Guild == null)
                    {
                        logger.Info("Message sent via DM, ignoring.");
                        return;
                    }

                    DiscordMember author = await e.Guild.GetMemberAsync(e.Author.Id).ConfigureAwait(false);
                    bool isBotAdmin = Useful.MemberIsBotOperator(author);

                    if (author.IsOwner || isBotAdmin)
                    {
                        logger.Info("Request validated, quitting now...");
                        quit = true;
                    }
                }

                //CustomCommands
                else if (e.Message.Content.StartsWith(commandChar))
                {
                    string arguments = string.Empty;
                    string[] split = e.Message.Content.Split(new char[] { ' ' }, 2);
                    string command = split[0];
                    if (split.Length > 1) arguments = split[1];
                    string senderUsername = Useful.GetUsername(e);

                    List<string> userList;
                    if (e.Channel.Guild != null)
                        userList = Useful.GetOnlineNames(e.Channel.Guild);
                    else
                        userList = new List<string> { senderUsername };

                    string result = CustomCommand.UseCustomCommand(command.TrimStart(commandChar), arguments, senderUsername, userList);
                    if (!string.IsNullOrEmpty(result))
                        await e.Message.RespondAsync(result).ConfigureAwait(false);
                }

                //Bot Talk and Cleverbot
                else if (e.Message.Content.StartsWith(botName + ",", StringComparison.OrdinalIgnoreCase) || e.Message.Content.EndsWith(botName, StringComparison.OrdinalIgnoreCase))
                {
                    await Think(e, botName).ConfigureAwait(false);
                }

                //waifunator
                if (!string.IsNullOrWhiteSpace(e.Message.Content) && e.Message.Author.Id == Settings.Default.limid) //lims shitty id lul
                {
                    if (e.Message.Content.Contains("wife", StringComparison.OrdinalIgnoreCase))
                    {
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_w:")).ConfigureAwait(false);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_a:")).ConfigureAwait(false);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_i:")).ConfigureAwait(false);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_f:")).ConfigureAwait(false);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromName(GetDiscordClient, ":regional_indicator_u:")).ConfigureAwait(false);
                    }
                }

                //Update "last seen" for user that sent the message
                string username = ((DiscordMember)e.Message.Author).DisplayName;
                Seen.MarkUserSeen(username);

                //Ping users, leave this last cause it's sloooooooow
                await PingUser.SendPings(e).ConfigureAwait(false);
            };

            //Connect to Discord
            await GetDiscordClient.ConnectAsync().ConfigureAwait(false);

            botName = GetDiscordClient.CurrentUser.Username;

            Timer statusTimer = new Timer(6 * 60 * 60 * 1000); //six hours in milliseconds
            statusTimer.Elapsed += new ElapsedEventHandler(OnUpdateStatusEvent);
            statusTimer.Start();

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
                        logger.Info("Attempting to Reconnect...");
                        await GetDiscordClient.ConnectAsync().ConfigureAwait(false);
                    }
                }

                //Wait a bit
                await Task.Delay(10 * 1000).ConfigureAwait(false);
            }

            await GetDiscordClient.DisconnectAsync().ConfigureAwait(false);
        }

        private static void OnUpdateStatusEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                logger.Info("Attempting to update user status");
                GetDiscordClient.UpdateStatusAsync(new DiscordGame(getRandomStatus()));
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
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

        private static string getRandomStatus()
        {
            List<string> status = new List<string>();

            if (File.Exists("TextFiles/status.txt"))
            {
                try
                {
                    var sr = new StreamReader("TextFiles/status.txt");
                    while (sr.Peek() >= 0)
                    {
                        status.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch (IOException e)
                {
                    logger.Error("Failed to read status." + e.Message);
                }
            }

            if (status.Count == 0)
                throw new Exception("No status on file");

            Random r = new Random();
            int i = r.Next(status.Count);

            return status[i];
        }
    }
}