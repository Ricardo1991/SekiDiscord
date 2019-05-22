using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SekiDiscord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SekiDiscord
{
    public class SekiCommands
    {
        internal static StringLibrary StringLibrary { get; set; }

        public static void SetStringLibrary(StringLibrary lib)
        {
            StringLibrary = lib;
        }

        [Command("quote")]
        [Description("Show or add quotes")]     // this will be displayed to tell users what this command does when they invoke help
        [Aliases("q")]                          // alternative names for the command
        public async Task Quote(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Quote Command");
            string arg;

            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                arg = string.Empty;
            }

            if (string.Compare(arg.ToLower().Split(new char[] { ' ' }, 2)[0], "add") == 0)  //add
            {
                Quotes.AddQuote(arg, StringLibrary);
            }
            else //lookup or random
            {
                string result = Quotes.PrintQuote(arg, StringLibrary);
                await ctx.RespondAsync(result);
            }
        }

        [Command("qcount")]
        [Description("Show how many quotes are loaded")]     // this will be displayed to tell users what this command does when they invoke help
        [Aliases("qc")]
        public async Task QuoteCount(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Quote Count Command");

            string result = Quotes.QuoteCount(StringLibrary);
            await ctx.RespondAsync(result);
        }

        [Command("kill")]
        public async Task Kill(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Kill Command");

            string author = ctx.Member.DisplayName;
            List<DiscordMember> usersOnline = Useful.getOnlineUsers(ctx.Channel.Guild);

            string args;

            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                args = string.Empty;
            }

            KillUser.KillResult result = KillUser.Kill(author, usersOnline, StringLibrary, args);

            switch (result.IsAction)
            {
                case true:
                    await ctx.RespondAsync("*" + result.Result + "*");
                    break;

                case false:
                    await ctx.RespondAsync(result.Result);
                    break;
            }
        }

        [Command("rkill")]
        public async Task RKill(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "RKill Command");

            string args;
            string author = ctx.Member.DisplayName;
            List<DiscordMember> listU = Useful.getOnlineUsers(ctx.Channel.Guild);

            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                args = string.Empty;
            }

            KillUser.KillResult result = KillUser.KillRandom(args, author, listU, StringLibrary);

            switch (result.IsAction)
            {
                case true:
                    await ctx.RespondAsync("*" + result.Result.Trim() + "*");
                    break;

                case false:
                    await ctx.RespondAsync(result.Result);
                    break;
            }
        }

        [Command("addcmd")]
        [Description("Add a command to the custom commands list")]
        public async Task AddCustomCommand(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "addcmd Command");
            string nick = ctx.User.Username;
            string[] splits;
            string message;
            splits = ctx.Message.Content.Split(new char[] { ' ' }, 3);

            if (CustomCommand.CommandExists(splits[0], StringLibrary.CustomCommands) == true)
            {
                message = "Command " + splits[0] + " already exists.";
                await ctx.RespondAsync(message);
            }

            StringLibrary.CustomCommands.Add(new CustomCommand(nick, splits[1], splits[2]));
            CustomCommand.SaveCustomCommands(StringLibrary.CustomCommands);
        }

        [Command("removecmd")]
        [Description("Remove a command to the custom commands list")]
        public async Task RemoveCustomCommand(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "removecmd Command");

            string[] splits;

            splits = ctx.Message.Content.Split(new char[] { ' ' }, 3);

            if (Useful.MemberIsBotOperator(ctx.Member) || ctx.Member.IsOwner)
            {
                CustomCommand.RemoveCommandByName(splits[1], StringLibrary.CustomCommands);
                CustomCommand.SaveCustomCommands(StringLibrary.CustomCommands);
                string message = "Command " + splits[1] + " removed.";
                await ctx.RespondAsync(message);
            }
        }

        [Command("ping")]
        [Description("Add, Remove or Copy words or phrases that the user will be mentioned at")]
        [Aliases("p")]                          // alternative names for the command
        public async Task Ping(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "ping Command");

            string msg, cmd, args;

            try
            {
                msg = ctx.Message.Content.ToLower().Split(new char[] { ' ' }, 2)[1]; // remove !p or !ping
                cmd = msg.Split(new char[] { ' ' }, 2)[0]; // get command word
                args = Useful.GetBetween(msg, cmd + " ", null); // get words after command, add a space to cmd word so args doesnt start with one
            }
            catch
            {
                msg = cmd = args = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await PingUser.PingControl(ctx, StringLibrary, cmd, args);
            }
        }

        [Command("roll")]
        [Description("Roll a number between 0 and 100")]
        public async Task Roll(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Roll Command");

            string nick = ctx.Member.DisplayName;
            nick = nick.Replace("\r", "");

            int number = Basics.Roll(ctx.Message.Content);

            string message = nick + " rolled a " + number;
            await ctx.RespondAsync(message);
        }

        [Command("shuffle")]
        [Description("Shuffle words randomly. Can be phrases if separated by commas")]
        public async Task Shuffle(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Shuffle Command");
            string result = Basics.Shuffle(ctx.Message.Content);

            if (!string.IsNullOrWhiteSpace(result))
                await ctx.RespondAsync(result);
        }

        [Command("choose")]
        [Description("Choose an item from the presented list randomly")]
        public async Task Choose(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Choose Command");

            string arg;
            string user = ctx.Member.DisplayName;

            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ");
                string result = Basics.Choose(arg, user);
                await ctx.RespondAsync(result);
            }
            catch
            {
                return;
            }
        }

        [Command("square")]
        [Description("square a word")]
        [Aliases("s")]
        public async Task SquareText(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Square Command");
            await Square.SquareText(ctx);
        }

        [Command("funk")]
        [Description("gimme music")]
        public async Task Funk(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Funk Command");
            string arg;
            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                arg = string.Empty;
            }

            if (string.IsNullOrEmpty(arg)) //lookup or random
                await Commands.Funk.PrintFunk(ctx, StringLibrary);
            else
                Commands.Funk.AddFunk(ctx, StringLibrary);
        }

        [Command("poke")]
        [Description("poke someone randomly")]
        public async Task Poke(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Poke Command");

            List<DiscordMember> listU = Useful.getOnlineUsers(ctx.Channel.Guild);
            string user = ctx.Member.DisplayName;

            string result = Basics.PokeRandom(listU, user);
            await ctx.RespondAsync(result);
        }

        [Command("youtube")]
        [Description("poke someone randomly")]
        [Aliases("yt")]                          // alternative names for the command
        public async Task YoutubeSearch(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Youtube Command");

            string query;

            try
            {
                query = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];

                string result = Youtube.YoutubeSearch(query);

                await ctx.Message.RespondAsync(result);
            }
            catch
            {
            }
        }

        [Command("nick")]
        [Description("generate a nickname")]
        public async Task Nick(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Nick Command");
            await Commands.Nick.NickGen(ctx, StringLibrary);
        }

        [Command("fact")]
        [Description("Show a random fun made up fact")]
        public async Task Fact(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Fact Command");
            await Commands.Fact.ShowFact(ctx, StringLibrary);
        }

        [Command("seen")]
        [Description("Check how long ago a user was last seen")]
        public async Task Seen(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Seen Command");
            await Commands.Seen.CheckSeen(ctx, StringLibrary);
        }
    }
}