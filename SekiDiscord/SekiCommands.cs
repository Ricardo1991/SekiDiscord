using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SekiDiscord.Commands;
using SekiDiscord.Commands.NotifyEvent;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SekiDiscord
{
#pragma warning disable CA1822 // Mark members as static

    public class SekiCommands : BaseCommandModule
    {
        private static readonly Logger logger = new Logger(typeof(SekiCommands));

        [Command("quote")]
        [Description("Show or add quotes. Add argument \"add\" after the command to add quote. If a different argument is used it will perform a search. If no arguments are shown, a random quote is shown")]     // this will be displayed to tell users what this command does when they invoke help
        public async Task Quote(CommandContext ctx)
        {
            logger.Info("Quote Command: Random overload", Useful.GetDiscordName(ctx));
            await ctx.RespondAsync(Quotes.PrintQuote(null)).ConfigureAwait(false);
        }

        [Command("quote")]
        [Description("Show or add quotes. Add argument \"add\" after the command to add quote. If a different argument is used it will perform a search. If no arguments are shown, a random quote is shown")]     // this will be displayed to tell users what this command does when they invoke help
        [Aliases("q")]
        public async Task Quote(CommandContext ctx, [RemainingText] string arg)
        {
            logger.Info("Quote Command", Useful.GetDiscordName(ctx));

            if (arg != null && string.Compare(arg.Split(new char[] { ' ' }, 2)[0], "add", StringComparison.OrdinalIgnoreCase) == 0)  // add
            {
                Quotes.AddQuote(arg);
            }
            else // lookup or random
            {
                await ctx.RespondAsync(Quotes.PrintQuote(arg)).ConfigureAwait(false);
            }
        }

        [Command("qcount")]
        [Description("Show how many quotes are loaded")]
        [Aliases("qc")]
        public async Task QuoteCount(CommandContext ctx)
        {
            logger.Info("Quote Count Command", Useful.GetDiscordName(ctx));

            await ctx.RespondAsync(Quotes.QuoteCount()).ConfigureAwait(false);
        }

        [Command("kill")]
        [Description("Perform a kill action on a user. Indicate user with arguments, or leave black for a random target.")]
        public async Task Kill(CommandContext ctx)
        {
            logger.Info("kill Command: Random overload", Useful.GetDiscordName(ctx));

            KillUser.KillResult result = KillUser.Kill(Useful.GetUsername(ctx), Useful.GetOnlineNames(ctx.Channel.Guild));

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

        [Command("kill")]
        [Description("Perform a kill action on a user. Indicate user with arguments, or leave black for a random target.")]
        public async Task Kill(CommandContext ctx, [RemainingText] string arg)
        {
            logger.Info("kill Command", Useful.GetDiscordName(ctx));

            KillUser.KillResult result = KillUser.Kill(Useful.GetUsername(ctx), Useful.GetOnlineNames(ctx.Channel.Guild), arg);

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
            logger.Info("rkill Command", Useful.GetDiscordName(ctx));

            KillUser.KillResult result = KillUser.KillRandom(Useful.GetUsername(ctx), Useful.GetOnlineNames(ctx.Channel.Guild));

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

        [Command("rkill")]
        [Description("Perform a randomly generated kill action on a user. Indicate user with arguments, or leave black for a random target.")]
        public async Task RKill(CommandContext ctx, [RemainingText] string arg)
        {
            logger.Info("rkill Command", Useful.GetDiscordName(ctx));

            KillUser.KillResult result = KillUser.KillRandom(Useful.GetCommandArguments(arg), Useful.GetUsername(ctx), Useful.GetOnlineNames(ctx.Channel.Guild));

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
        public async Task AddCustomCommand(CommandContext ctx, string commandName, [RemainingText] string format)
        {
            logger.Info("addcmd Command", Useful.GetDiscordName(ctx));

            if (CustomCommand.CommandExists(commandName) == true)
            {
                string message = "Command " + commandName + " already exists.";
                await ctx.RespondAsync(message).ConfigureAwait(false);
            }

            if (string.IsNullOrWhiteSpace(format)) return;

            CustomCommand.CustomCommands.Add(new CustomCommand(ctx.User.Username, commandName, format));
            CustomCommand.SaveCustomCommands();
        }

        [Command("removecmd")]
        [Description("Remove a command to the custom commands list")]
        public async Task RemoveCustomCommand(CommandContext ctx, string commandName)
        {
            logger.Info("removecmd Command", Useful.GetDiscordName(ctx));

            if (Useful.MemberIsBotOperator(ctx.Member) || ctx.Member.IsOwner)
            {
                if (CustomCommand.RemoveCommandByName(commandName)){
                    CustomCommand.SaveCustomCommands();
                    await ctx.RespondAsync("Command " + commandName + " removed.").ConfigureAwait(false);
                }
                await ctx.RespondAsync("Command " + commandName + " not found.").ConfigureAwait(false);
            }
        }

        [Command("ping")]
        [Description("Add, Remove or Copy words or phrases that the user will be mentioned at")]
        [Aliases("p")]
        public async Task Ping(CommandContext ctx, string cmd, [RemainingText] string args)
        {
            logger.Info("ping Command", Useful.GetDiscordName(ctx));

            switch (cmd)
            {
                case "add":
                    PingUser.PingControlAdd(ctx.Member.Id, args);
                    break;

                case "remove":
                    PingUser.PingControlRemove(ctx.Member.Id, args);
                    break;

                case "info":
                    if (ctx.Guild.Members.TryGetValue(ctx.Message.Author.Id, out DiscordMember member))
                    {
                        await PingUser.PingControlInfo(member).ConfigureAwait(false);
                    }
                    break;
            }
            PingUser.SavePings(PingUser.Pings); 
        }


        [Command("roll")]
        [Description("Roll a number between 0 and 100")]
        public async Task Roll(CommandContext ctx)
        {
            logger.Info("Roll Command", Useful.GetDiscordName(ctx));

            try
            {
                int number = Basics.Roll(100);
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

        [Command("roll")]
        [Description("Roll a number between 0 and the indicated number.")]
        public async Task Roll(CommandContext ctx, int max)
        {
            logger.Info("Roll Command", Useful.GetDiscordName(ctx));

            try
            {
                int number = Basics.Roll(max);
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
        public async Task Shuffle(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("Shuffle Command", Useful.GetDiscordName(ctx));

            string result = Basics.Shuffle(args);

            if (!string.IsNullOrWhiteSpace(result))
                await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("choose")]
        [Description("Choose a word from the argument list, randomly")]
        public async Task Choose(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("Choose Command", Useful.GetDiscordName(ctx));

            try
            {
                string arg = Useful.GetCommandArguments(args).Trim().Replace("  ", " ", StringComparison.OrdinalIgnoreCase);
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
        public async Task SquareText(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("Square Command", Useful.GetDiscordName(ctx));

            string message = Square.SquareText(Useful.GetCommandArguments(args), Useful.GetUsername(ctx));
            await ctx.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("funk")]
        [Description("Provide link to a song from the stored list")]
        public async Task Funk(CommandContext ctx)
        {
            logger.Info("Funk Command", Useful.GetDiscordName(ctx));


            await ctx.Message.RespondAsync(Commands.Funk.PrintFunk()).ConfigureAwait(false);

        }
        [Command("funk")]
        [Description("Provide link to a song from the stored list")]
        public void Funk(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("Funk Command", Useful.GetDiscordName(ctx));

            if (!string.IsNullOrEmpty(Useful.GetCommandArguments(args)))
            {
                Commands.Funk.AddFunk(args);
            }
        }

        [Command("poke")]
        [Description("poke a user randomly")]
        public async Task Poke(CommandContext ctx, params string[] names)
        {
            logger.Info("Poke Command", Useful.GetDiscordName(ctx));

            string result = Basics.PokeRandom(Useful.GetOnlineNames(ctx.Channel.Guild), Useful.GetUsername(ctx));
            await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("youtube")]
        [Description("Search youtube for the arguments provided, and return the top result")]
        [Aliases("yt")]
        public async Task YoutubeSearch(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("Youtube Command", Useful.GetDiscordName(ctx));

            try
            {
                string result = Youtube.YoutubeSearch(Useful.GetCommandArguments(args));
                await ctx.Message.RespondAsync(result).ConfigureAwait(false);
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }
        }

        [Command("nick")]
        [Description("Generate a nickname")]
        public async Task Nick(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("Nick Command", Useful.GetDiscordName(ctx));

            string result = Commands.Nick.NickGen(args, Useful.GetUsername(ctx));

            await ctx.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("fact")]
        [Description("Show a random fun made up fact")]
        public async Task Fact(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("Fact Command", Useful.GetDiscordName(ctx));

            string result = Commands.Fact.ShowFact(args, Useful.GetOnlineNames(ctx.Channel.Guild), Useful.GetUsername(ctx));

            await ctx.Message.RespondAsync(result).ConfigureAwait(false);
        }

        [Command("seen")]
        [Description("Check how long ago a user was last seen")]
        public async Task Seen(CommandContext ctx, string userName)
        {
            logger.Info("Seen Command", Useful.GetDiscordName(ctx));

            string message = Commands.Seen.CheckSeen(Useful.GetCommandArguments(userName));
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
        public async Task EventSubscribe(CommandContext ctx, string eventName)
        {
            logger.Info("Event Subscribe Command", Useful.GetDiscordName(ctx));

            bool result = NotifyEventManager.SubscribeUserToEvent(ctx.User.Id, ctx.Guild.Id, eventName);

            string message = result ? "Added sucessfully to " + eventName : "Error";
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("event-unsubscribe")]
        [Description("Unsubscribe to a named event")]
        public async Task EventUnsubscribe(CommandContext ctx, string eventName)
        {
            logger.Info("Event Unsubscribe Command", Useful.GetDiscordName(ctx));

            bool result = NotifyEventManager.UnsubscribeUserToEvent(ctx.User.Id, ctx.Guild.Id, eventName);

            string message = result ? "Removed sucessfully from " + eventName : "Error";
            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }

        [Command("event-enable")]
        [Description("Enable a named event")]
        public async Task EventEnable(CommandContext ctx, string eventName)
        {
            logger.Info("Event Enable Command", Useful.GetDiscordName(ctx));

            try
            {
                NotifyEventManager.EnableEvent(eventName);
                await ctx.Message.RespondAsync("Enabled " + eventName + " sucessfully").ConfigureAwait(false);
            }
            catch (ArgumentException ex)
            {
                await ctx.Message.RespondAsync("Error, event probably not found").ConfigureAwait(false);
                logger.Error("Error enabling event: " + ex.Message, Useful.GetDiscordName(ctx));
            }
        }

        [Command("event-disable")]
        [Description("Disable a named event")]
        public async Task EventDisable(CommandContext ctx, string eventName)
        {
            logger.Info("Event Disable Command", Useful.GetDiscordName(ctx));

            try
            {
                NotifyEventManager.DisableEvent(eventName);
                await ctx.Message.RespondAsync("Disabled " + eventName + " sucessfully").ConfigureAwait(false);
            }
            catch (ArgumentException ex)
            {
                await ctx.Message.RespondAsync("Error, event probably not found").ConfigureAwait(false);
                logger.Error("Error disabling event: " + ex.Message, Useful.GetDiscordName(ctx));
            }
        }

        [Command("event-create")]
        [Description("Create a named event. Example: !event-create genshin; 1 January 2021, 06:00; 1:0:0:0")]
        public async Task EventCreate(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("Create Event Command", Useful.GetDiscordName(ctx));

            string message;

            try
            {
                NotifyEventManager.AddEvent(args);
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
        [RequireRoles(RoleCheckMode.Any, "bot-admin", "Administrator")]
        public async Task EvenDelete(CommandContext ctx, string eventName)
        {
            logger.Info("Remove Event Command", Useful.GetDiscordName(ctx));

            string message;

            try
            {
                NotifyEventManager.RemoveEvent(eventName);
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
        public async Task EventList(CommandContext ctx, [RemainingText] string args)
        {
            logger.Info("List Event Command", Useful.GetDiscordName(ctx));

            if (NotifyEventManager.NotifyEventCount() == 0)
            {
                await ctx.Message.RespondAsync("No Events saved").ConfigureAwait(false);
                return;
            }

            string[] events = NotifyEventManager.getNotifyEventDetails(!string.IsNullOrWhiteSpace(args) && args.Trim().ToLower() == "extra");

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