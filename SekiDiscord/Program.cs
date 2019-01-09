using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
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
                    string result = Commands.CustomCommand.UseCustomCommand(command.TrimStart('!'), arguments, e, StringLib);
                    if (!string.IsNullOrEmpty(result))
                        await e.Message.RespondAsync(result);
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

            while (!quit)
            {
                //Wait a bit
                await Task.Delay(5 * 1000);
            }

            await discord.DisconnectAsync();
        }
    }
}