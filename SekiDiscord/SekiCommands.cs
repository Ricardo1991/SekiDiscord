using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SekiDiscord.Commands;
using System;
using System.Threading.Tasks;

namespace SekiDiscord
{
    public class SekiCommands
    {
        private static StringLibrary stringLibrary;

        internal StringLibrary StringLibrary { get => stringLibrary; set => stringLibrary = value; }

        public static void SetStringLibrary(StringLibrary lib)
        {
            stringLibrary = lib;
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
                Quotes.AddQuote(ctx, stringLibrary);
            }
            else //lookup or random
            {
                string result = Quotes.PrintQuote(ctx, stringLibrary);
                await ctx.RespondAsync(result);
            }
        }

        [Command("kill")]
        public async Task Kill(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Kill Command");
            KillUser.KillResult result = KillUser.Kill(ctx, StringLibrary);

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
            KillUser.KillResult result = KillUser.KillRandom(ctx, StringLibrary);

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
    }
}