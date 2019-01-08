using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SekiDiscord
{
    internal class Program
    {
        private static DiscordClient discord;
        private static CommandsNextModule commands;

        private static StringLibrary StringLib { get; set; } = new StringLibrary();

        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Not enough arguments. Usage: SekiDiscord <api-key>. Quitting.");
                return;
            }
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Starting...");
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            string token = args[0];
            bool quit = false;

            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot
            });

            discord.MessageCreated += async e =>
            {
                if (e.Message.Content.ToLower().StartsWith("!quit"))
                {
                    DiscordMember author = await e.Guild.GetMemberAsync(e.Author.Id);
                    bool isBotAdmin = false;

                    foreach (DiscordRole role in author.Roles)
                    {
                        if (role.Name == "bot-admin")
                        {
                            isBotAdmin = true;
                            break;
                        }
                    }

                    if (author.IsOwner || isBotAdmin)
                    {
                        quit = true;
                        Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Quitting...");
                    }
                }
            };

            commands = discord.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefix = "!"
            });

            SekiCommands.SetStringLibrary(StringLib);
            commands.RegisterCommands<SekiCommands>();

            await discord.ConnectAsync();   //Connect to Discord
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Ready!");

            while (!quit)
            {
                await Task.Delay(5 * 1000);           //Wait a bit
            }

            await discord.DisconnectAsync();
        }
    }
}