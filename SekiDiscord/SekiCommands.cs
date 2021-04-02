using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SekiDiscord.Commands;
using SekiDiscord.Commands.NotifyEvent;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SekiDiscord
{
#pragma warning disable CA1822 // Mark members as static

    public class SekiCommands
    {
        private static readonly Logger logger = new Logger(typeof(SekiCommands));

        [Command("quote")]
        [Description("Show or add quotes. Add argument \"add\" after the command to add quote. If a different argument is used it will perform a search. If no arguments are added, a random quote is shown")]     // this will be displayed to tell users what this command does when they invoke help
        [Aliases("q")]
        public async Task Quote(CommandContext ctx)
        {
            logger.Info("Quote Command", Useful.GetDiscordName(ctx));

            string arg = Useful.GetCommandArguments(ctx.Message.Content);

            if (string.Compare(arg.Split(new char[] { ' ' }, 2)[0], "add", StringComparison.OrdinalIgnoreCase) == 0)  // add
            {
                Quotes.AddQuote(arg);
            }
            else // lookup or random
            {
                await ctx.RespondAsync(Quotes.PrintQuote(arg)).ConfigureAwait(false);
            }
        }

        [Command("quote-add")]
        [Description("Add quotes. Example: !quote-add <rya> r u a boo?")]
        [Aliases("q-add")]
        public async Task QuoteAdd(CommandContext ctx)
        {
            logger.Info("Quote Add Command", Useful.GetDiscordName(ctx));

            string arg = Useful.GetCommandArguments(ctx.Message.Content);
            Quotes.AddQuote(arg);
        }

        [Command("quote-count")]
        [Description("Show how many quotes are loaded")]
        [Aliases("qc", "qcount")]
        public async Task QuoteCount(CommandContext ctx)
        {
            logger.Info("Quote Count Command", Useful.GetDiscordName(ctx));

            await ctx.RespondAsync(Quotes.QuoteCount()).ConfigureAwait(false);
        }

        [Command("kill")]
        [Description("Perform a kill action on a user. Indicate user with arguments, or leave black for a random target.")]
        public async Task Kill(CommandContext ctx)
        {
            logger.Info("kill Command", Useful.GetDiscordName(ctx));

            KillUser.KillResult result = KillUser.Kill(Useful.GetUsername(ctx), Useful.GetOnlineNames(ctx.Channel.Guild), Useful.GetCommandArguments(ctx.Message.Content));

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
        [Aliases("kill-random","random-kill")]
        public async Task RandomKill(CommandContext ctx)
        {
            logger.Info("rkill Command", Useful.GetDiscordName(ctx));

            KillUser.KillResult result = KillUser.KillRandom(Useful.GetCommandArguments(ctx.Message.Content), Useful.GetUsername(ctx), Useful.GetOnlineNames(ctx.Channel.Guild));

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
        [Aliases("cmd-add")]
        public async Task AddCustomCommand(CommandContext ctx)
        {
            logger.Info("addcmd Command", Useful.GetDiscordName(ctx));

            string[] splits = ctx.Message.Content.Split(new char[] { ' ' }, 3);

            if (CustomCommand.CommandExists(splits[0]) == true)
            {
                string message = "Command " + splits[0] + " already exists.";
                await ctx.RespondAsync(message).ConfigureAwait(false);
            }

            CustomCommand.CustomCommands.Add(new CustomCommand(ctx.User.Username, splits[1], splits[2]));
            CustomCommand.SaveCustomCommands();
        }

        [Command("removecmd")]
        [Description("Remove a command to the custom commands list")]
        [Aliases("cmd-remove")]
        public async Task RemoveCustomCommand(CommandContext ctx)
        {
            logger.Info("removecmd Command", Useful.GetDiscordName(ctx));

            if (Useful.MemberIsBotOperator(ctx.Member) || ctx.Member.IsOwner)
            {
                string[] splits = ctx.Message.Content.Split(new char[] { ' ' }, 3);

                CustomCommand.RemoveCommandByName(splits[1]);
                CustomCommand.SaveCustomCommands();
                string message = "Command " + splits[1] + " removed.";
                await ctx.RespondAsync(message).ConfigureAwait(false);
            }
        }

        [Command("ping")]
        [Description("Add, Remove or Copy words or phrases that the user will be mentioned at")]
        [Aliases("p")]
        public async Task Ping(CommandContext ctx)
        {
            logger.Info("ping Command", Useful.GetDiscordName(ctx));

            string msg, cmd, args;

            try
            {
                msg = Useful.GetCommandArguments(ctx.Message.Content.ToLowerInvariant()); // remove !p or !ping
                cmd = msg.Split(new char[] { ' ' }, 2)[0]; // get command word
                args = Useful.GetBetween(msg, cmd, null).TrimStart(); // get words after command, add a space to cmd word so args doesnt start with one
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await PingUser.PingControl(ctx.Member.Id, ctx.Message.Author, cmd, args).ConfigureAwait(false);
                PingUser.SavePings(PingUser.Pings);
            }
        }

        [Command("roll")]
        [Description("Roll a number between 0 and the indicated number. 100 will be used as default if no valid number is presented")]
        public async Task Roll(CommandContext ctx)
        {
            logger.Info("Roll Command", Useful.GetDiscordName(ctx));

            try
            {
                int number = Basics.Roll(ctx.Message.Content);
                string message = Useful.GetUsername(ctx) + " rolled a " + number;
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
            logger.Info("Shuffle Command", Useful.GetDiscordName(ctx));

            string result = Basics.Shuffle(ctx.Message.Content);

            if (!string.IsNullOrWhiteSpace(result))
                await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("choose")]
        [Description("Choose a word from the argument list, randomly")]
        public async Task Choose(CommandContext ctx)
        {
            logger.Info("Choose Command", Useful.GetDiscordName(ctx));

            try
            {
                string arg = Useful.GetCommandArguments(ctx.Message.Content).Trim().Replace("  ", " ", StringComparison.OrdinalIgnoreCase);
                string result = Basics.Choose(arg, Useful.GetUsername(ctx));
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
            logger.Info("Square Command", Useful.GetDiscordName(ctx));

            string message = Square.SquareText(Useful.GetCommandArguments(ctx.Message.Content), Useful.GetUsername(ctx));
            await ctx.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("funk")]
        [Description("Provide link to a song from the stored list")]
        public async Task Funk(CommandContext ctx)
        {
            logger.Info("Funk Command", Useful.GetDiscordName(ctx));

            if (string.IsNullOrEmpty(Useful.GetCommandArguments(ctx.Message.Content))) { // lookup or random
                await ctx.Message.RespondAsync(Commands.Funk.PrintFunk()).ConfigureAwait(false);
            }
            else {
                Commands.Funk.AddFunk(ctx.Message.Content);
            }
        }

        [Command("poke")]
        [Description("poke a user randomly")]
        public async Task Poke(CommandContext ctx)
        {
            logger.Info("Poke Command", Useful.GetDiscordName(ctx));

            string result = Basics.PokeRandom(Useful.GetOnlineNames(ctx.Channel.Guild), Useful.GetUsername(ctx));
            await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("youtube")]
        [Description("Search youtube for the arguments provided, and return the top result")]
        [Aliases("yt")]
        public async Task YoutubeSearch(CommandContext ctx)
        {
            logger.Info("Youtube Command", Useful.GetDiscordName(ctx));

            try
            {
                string result = Youtube.YoutubeSearch(Useful.GetCommandArguments(ctx.Message.Content));
                await ctx.Message.RespondAsync(result).ConfigureAwait(false);
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
        }

        [Command("nick")]
        [Description("Generate a nickname")]
        public async Task Nick(CommandContext ctx)
        {
            logger.Info("Nick Command", Useful.GetDiscordName(ctx));

            string result = Commands.Nick.NickGen(ctx.Message.Content, Useful.GetUsername(ctx));

            await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("fact")]
        [Description("Show a random fun made up fact")]
        public async Task Fact(CommandContext ctx)
        {
            logger.Info("Fact Command", Useful.GetDiscordName(ctx));

            string result = Commands.Fact.ShowFact(Useful.GetCommandArguments(ctx.Message.Content), Useful.GetOnlineNames(ctx.Channel.Guild), Useful.GetUsername(ctx));

            await ctx.Message.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("seen")]
        [Description("Check how long ago a user was last seen")]
        public async Task Seen(CommandContext ctx)
        {
            logger.Info("Seen Command", Useful.GetDiscordName(ctx));

            string message = Commands.Seen.CheckSeen(Useful.GetCommandArguments(ctx.Message.Content));
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("trivia")]
        [Description("Get a piece of trivia")]
        public async Task Trivia(CommandContext ctx)
        {
            logger.Info("Trivia Command", Useful.GetDiscordName(ctx));

            try
            {
                await ctx.Message.RespondAsync(Commands.Trivia.GetTrivia()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, Useful.GetDiscordName(ctx));
            }
        }

        [Command("version")]
        [Description("Get GIT information about current build")]
        [Aliases("v")]
        public async Task Version(CommandContext ctx)
        {
            logger.Info("Version Command", Useful.GetDiscordName(ctx));

            await ctx.Message.RespondAsync(Commands.Version.VersionString).ConfigureAwait(false);
        }

        [Command("fortune")]
        [Description("Get today's fortune for yourself")]
        public async Task Fortune(CommandContext ctx)
        {
            logger.Info("Fortune Command", Useful.GetDiscordName(ctx));

            string message = Commands.Fortune.GetFortune(ctx.User.Id);
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("event-subscribe")]
        [Description("Subscribe to a named event")]
        public async Task EventSubscribe(CommandContext ctx)
        {
            logger.Info("Event Subscribe Command", Useful.GetDiscordName(ctx));
            string arguments = Useful.GetCommandArguments(ctx.Message.Content).Trim();

            bool result = NotifyEventManager.SubscribeUserToEvent(ctx.User.Id, ctx.Guild.Id, arguments);

            string message = result ? "Added sucessfully to " + arguments : "Error";
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("event-unsubscribe")]
        [Description("Unsubscribe to a named event")]
        public async Task EventUnsubscribe(CommandContext ctx)
        {
            logger.Info("Event Unsubscribe Command", Useful.GetDiscordName(ctx));
            string arguments = Useful.GetCommandArguments(ctx.Message.Content).Trim();

            bool result = NotifyEventManager.UnsubscribeUserToEvent(ctx.User.Id, ctx.Guild.Id, arguments);

            string message = result ? "Removed sucessfully from " + arguments : "Error";
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("event-enable")]
        [Description("Enable a named event")]
        public async Task EventEnable(CommandContext ctx)
        {
            logger.Info("Event Enable Command", Useful.GetDiscordName(ctx));
            string arguments = Useful.GetCommandArguments(ctx.Message.Content).Trim();

            try
            {
                NotifyEventManager.EnableEvent(arguments);
                await ctx.Message.RespondAsync("Enabled " + arguments + " sucessfully").ConfigureAwait(false);
            }
            catch (ArgumentException ex)
            {
                await ctx.Message.RespondAsync("Error, event probably not found").ConfigureAwait(false);
                logger.Error("Error enabling event: " + ex.Message, Useful.GetDiscordName(ctx));
            }
        }

        [Command("event-disable")]
        [Description("Disable a named event")]
        public async Task EventDisable(CommandContext ctx)
        {
            logger.Info("Event Disable Command", Useful.GetDiscordName(ctx));
            string arguments = Useful.GetCommandArguments(ctx.Message.Content).Trim();

            try
            {
                NotifyEventManager.DisableEvent(arguments);
                await ctx.Message.RespondAsync("Disabled " + arguments + " sucessfully").ConfigureAwait(false);
            }
            catch (ArgumentException ex)
            {
                await ctx.Message.RespondAsync("Error, event probably not found").ConfigureAwait(false);
                logger.Error("Error disabling event: " + ex.Message, Useful.GetDiscordName(ctx));
            }
        }

        [Command("event-create")]
        [Description("Create a named event. Example: !event-create genshin; 1 January 2021, 06:00; 1:0:0:0")]
        public async Task EventCreate(CommandContext ctx)
        {
            logger.Info("Create Event Command", Useful.GetDiscordName(ctx));
            string arguments = Useful.GetCommandArguments(ctx.Message.Content);

            string message;

            try
            {
                NotifyEventManager.AddEvent(arguments);
                message = "Event added. Now activate it manually";
            }
            catch (Exception ex)
            {
                message = "Event not created, error: " + ex.Message;
            }

            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("event-delete")]
        [Description("Delete a named event. Example: !event-delete genshin")]
        [RequireRolesAttribute("bot-admin", "Administrator")]
        public async Task EvenDelete(CommandContext ctx)
        {
            logger.Info("Remove Event Command", Useful.GetDiscordName(ctx));
            string arguments = Useful.GetCommandArguments(ctx.Message.Content);

            string message;

            try
            {
                NotifyEventManager.RemoveEvent(arguments);
                message = "Event Removed.";
            }
            catch (Exception ex)
            {
                message = "Event not removed. Error: " + ex.Message;
            }

            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("event-list")]
        [Description("List saved events. Argument \"extra\" for more information")]
        public async Task EventList(CommandContext ctx)
        {
            logger.Info("List Event Command", Useful.GetDiscordName(ctx));
            string arguments = Useful.GetCommandArguments(ctx.Message.Content);

            if (NotifyEventManager.NotifyEventCount() == 0)
            {
                await ctx.Message.RespondAsync("No Events saved").ConfigureAwait(false);
                return;
            }

            string[] events = NotifyEventManager.getNotifyEventDetails(arguments.Trim().ToLower() == "extra");

            StringBuilder builder = new StringBuilder().Append("```");

            foreach (string eventDetail in events)
            {
                builder.AppendLine(eventDetail);
            }

            builder.Append("```");
            await ctx.Message.RespondAsync(builder.ToString()).ConfigureAwait(false);
        }
    }

#pragma warning restore CA1822 // Mark members as static
}