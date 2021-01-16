using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SekiDiscord.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SekiDiscord
{
    public class SekiCommands
    {
        [Command("quote")]
        [Description("Show or add quotes. Add argument \"add\" after the command to add quote. If a different argument is used it will perform a search. If no arguments are shown, a random quote is shown")]     // this will be displayed to tell users what this command does when they invoke help
        [Aliases("q")]
        public async Task Quote(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Quote Command");
            string arg;

            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch (IndexOutOfRangeException)
            {
                arg = string.Empty;
            }

            if (string.Compare(arg.Split(new char[] { ' ' }, 2)[0], "add", StringComparison.OrdinalIgnoreCase) == 0)  //add
            {
                Quotes.AddQuote(arg);
            }
            else //lookup or random
            {
                string result = Quotes.PrintQuote(arg);
                await ctx.RespondAsync(result).ConfigureAwait(false);
            }
        }

        [Command("qcount")]
        [Description("Show how many quotes are loaded")]     // this will be displayed to tell users what this command does when they invoke help
        [Aliases("qc")]
        public async Task QuoteCount(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Quote Count Command");

            string result = Quotes.QuoteCount();
            await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("kill")]
        [Description("Perform a kill action on a user. Indicate user with arguments, or leave black for a random target.")]
        public async Task Kill(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Kill Command");

            string author = Useful.GetUsername(ctx);
            List<string> usersOnline = Useful.GetOnlineNames(ctx.Channel.Guild);

            string args;

            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch (IndexOutOfRangeException)
            {
                args = string.Empty;
            }

            KillUser.KillResult result = KillUser.Kill(author, usersOnline, args);

            switch (result.IsAction)
            {
                case true:
                    await ctx.RespondAsync("*" + result.Result + "*").ConfigureAwait(false);
                    break;

                case false:
                    await ctx.RespondAsync(result.Result).ConfigureAwait(false);
                    break;
            }
        }

        [Command("rkill")]
        [Description("Perform a randomly generated kill action on a user. Indicate user with arguments, or leave black for a random target.")]
        public async Task RKill(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "RKill Command");

            string args;
            string author = Useful.GetUsername(ctx);
            List<string> listU = Useful.GetOnlineNames(ctx.Channel.Guild);

            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch (IndexOutOfRangeException)
            {
                args = string.Empty;
            }

            KillUser.KillResult result = KillUser.KillRandom(args, author, listU);

            switch (result.IsAction)
            {
                case true:
                    await ctx.RespondAsync("*" + result.Result.Trim() + "*").ConfigureAwait(false);
                    break;

                case false:
                    await ctx.RespondAsync(result.Result).ConfigureAwait(false);
                    break;
            }
        }

        [Command("addcmd")]
        [Description("Add a command to the custom commands list")]
        public async Task AddCustomCommand(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "addcmd Command");
            string nick = ctx.User.Username;
            string[] splits;
            string message;
            splits = ctx.Message.Content.Split(new char[] { ' ' }, 3);

            if (CustomCommand.CommandExists(splits[0], CustomCommand.CustomCommands) == true)
            {
                message = "Command " + splits[0] + " already exists.";
                await ctx.RespondAsync(message).ConfigureAwait(false);
            }

            CustomCommand.CustomCommands.Add(new CustomCommand(nick, splits[1], splits[2]));
            CustomCommand.SaveCustomCommands(CustomCommand.CustomCommands);
        }

        [Command("removecmd")]
        [Description("Remove a command to the custom commands list")]
        public async Task RemoveCustomCommand(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "removecmd Command");

            string[] splits;

            splits = ctx.Message.Content.Split(new char[] { ' ' }, 3);

            if (Useful.MemberIsBotOperator(ctx.Member) || ctx.Member.IsOwner)
            {
                CustomCommand.RemoveCommandByName(splits[1], CustomCommand.CustomCommands);
                CustomCommand.SaveCustomCommands(CustomCommand.CustomCommands);
                string message = "Command " + splits[1] + " removed.";
                await ctx.RespondAsync(message).ConfigureAwait(false);
            }
        }

        [Command("ping")]
        [Description("Add, Remove or Copy words or phrases that the user will be mentioned at")]
        [Aliases("p")]                          // alternative names for the command
        public async Task Ping(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "ping Command");

            string msg, cmd, args;

            try
            {
                msg = ctx.Message.Content.ToLower(CultureInfo.CreateSpecificCulture("en-GB")).Split(new char[] { ' ' }, 2)[1]; // remove !p or !ping
                cmd = msg.Split(new char[] { ' ' }, 2)[0]; // get command word
                args = Useful.GetBetween(msg, cmd, null).TrimStart(); // get words after command, add a space to cmd word so args doesnt start with one
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                ulong userNameID = ctx.Member.Id; // get message creators username in lower case
                var discordUser = ctx.Message.Author;
                await PingUser.PingControl(userNameID, discordUser, cmd, args).ConfigureAwait(false);
                PingUser.SavePings(PingUser.Pings);
            }
        }

        [Command("roll")]
        [Description("Roll a number between 0 and the indicated number. 100 will be used as default if no valid number is presented")]
        public async Task Roll(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Roll Command");

            string nick = Useful.GetUsername(ctx);
            string message;

            try
            {
                int number = Basics.Roll(ctx.Message.Content);
                message = nick + " rolled a " + number;
                await ctx.RespondAsync(message).ConfigureAwait(false);
            }
            catch (FormatException)
            {
                await ctx.RespondAsync("Wrong Format").ConfigureAwait(false);
            }
            catch (OverflowException)
            {
                await ctx.RespondAsync("Number too large").ConfigureAwait(false);
            }
        }

        [Command("shuffle")]
        [Description("Shuffle provided words randomly. Can be phrases if separated by commas")]
        public async Task Shuffle(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Shuffle Command");
            string result = Basics.Shuffle(ctx.Message.Content);

            if (!string.IsNullOrWhiteSpace(result))
                await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("choose")]
        [Description("Choose a word from the argument list, randomly")]
        public async Task Choose(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Choose Command");

            string user = Useful.GetUsername(ctx);

            try
            {
                string arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ", StringComparison.OrdinalIgnoreCase);
                string result = Basics.Choose(arg, user);
                await ctx.RespondAsync(result).ConfigureAwait(false);
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
        }

        [Command("square")]
        [Description("squarify a word. Limited to lenght of 10 characters")]
        [Aliases("s")]
        public async Task SquareText(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Square Command");
            string text = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            string message = Square.SquareText(text, Useful.GetUsername(ctx));
            await ctx.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("funk")]
        [Description("Provide link to a song from the stored list")]
        public async Task Funk(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Funk Command");
            string arg;
            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch (IndexOutOfRangeException)
            {
                arg = string.Empty;
            }

            if (string.IsNullOrEmpty(arg)) //lookup or random
            {
                await ctx.Message.RespondAsync(Commands.Funk.PrintFunk()).ConfigureAwait(false);
            }
            else
                Commands.Funk.AddFunk(ctx.Message.Content);
        }

        [Command("poke")]
        [Description("poke a user randomly")]
        public async Task Poke(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Poke Command");

            List<string> listU = Useful.GetOnlineNames(ctx.Channel.Guild);
            string user = Useful.GetUsername(ctx);

            string result = Basics.PokeRandom(listU, user);
            await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("youtube")]
        [Description("Search youtube for the arguments provided, and return the top result")]
        [Aliases("yt")]                          // alternative names for the command
        public async Task YoutubeSearch(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Youtube Command");

            try
            {
                string query = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
                string result = Youtube.YoutubeSearch(query);
                await ctx.Message.RespondAsync(result).ConfigureAwait(false);
            }
            catch (IndexOutOfRangeException)
            {
            }
        }

        [Command("nick")]
        [Description("Generate a nickname")]
        public async Task Nick(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Nick Command");

            string args = ctx.Message.Content;
            string nick = Useful.GetUsername(ctx);
            string result = Commands.Nick.NickGen(args, nick);

            await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("fact")]
        [Description("Show a random fun made up fact")]
        public async Task Fact(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Fact Command");

            List<string> listU = Useful.GetOnlineNames(ctx.Channel.Guild);
            string user = Useful.GetUsername(ctx);
            string args;
            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch (IndexOutOfRangeException)
            {
                args = string.Empty;
            }

            string result = Commands.Fact.ShowFact(args, listU, user);

            await ctx.Message.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("seen")]
        [Description("Check how long ago a user was last seen")]
        public async Task Seen(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Seen Command");

            string args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            string message = Commands.Seen.CheckSeen(args);
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("trivia")]
        [Description("Get a piece of trivia")]
        public async Task Trivia(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Trivia Command");

            string message = Commands.Trivia.GetTrivia();
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("version")]
        [Description("Get GIT information about current build")]
        [Aliases("v")]
        public async Task Version(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Version Command");

            await ctx.Message.RespondAsync(Commands.Version.VersionString).ConfigureAwait(false);
        }

        [Command("fortune")]
        [Description("Get today's fortune for yourself")]
        public async Task Fortune(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Fortune Command");

            string message = Commands.Fortune.GetFortune(DateTime.Today, ctx.User);
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }
    }
}