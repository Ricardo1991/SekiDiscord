using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using SekiDiscord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            string botName = string.Empty;

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            discord.SocketErrored += async a =>
            {
                quit = true;
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Error... " + a.Exception.Message);
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

                //Ping users, leave this last cause it's sloooooooow
                var ping_channel = await discord.GetChannelAsync(Settings.Default.ping_channel_id);
                if (!string.IsNullOrWhiteSpace(e.Message.Content) && e.Message.ChannelId != Settings.Default.ping_channel_id)
                {
                    HashSet<string> pinged_users = PingUser.Ping(e, StringLib);
                    string mentions = string.Empty;
                    DiscordMember member;

                    foreach (string user in pinged_users) // loop and get mention strings
                    {
                        if (user != e.Message.Author.Username.ToLower())
                        {
                            member = e.Guild.Members.Where(mem => mem.Username.ToLower().Contains(user)).First();
                            mentions += member.Mention + " ";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(mentions))
                    {
                        string author_nickname = e.Message.Channel.Guild.Members.Where(x => x.Id.Equals(e.Message.Author.Id)).Select(x => x.Nickname).First();
                        if (author_nickname == null)
                            author_nickname = e.Message.Author.Username;
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(mentions + "at " + e.Message.Channel.Mention + "\n" + "<" + author_nickname + "> " + e.Message.Content);
                        await discord.SendMessageAsync(ping_channel, stringBuilder.ToString());
                    }
                }
            };

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            SekiCommands.SetStringLibrary(StringLib);
            commands.RegisterCommands<SekiCommands>();

            //Connect to Discord
            await discord.ConnectAsync();
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Ready!");

            botName = discord.CurrentUser.Username;
            while (!quit)
            {
                //Wait a bit
                await Task.Delay(5 * 1000);
            }

            await discord.DisconnectAsync();
        }
    }
}