using DSharpPlus.CommandsNext;
using System;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class Funk
    {
        public static async Task PrintFunk(CommandContext ctx, StringLibrary stringLibrary)
        {
            if (stringLibrary.Funk.Count == 0)
                return;

            Random r = new Random();
            int i = r.Next(stringLibrary.Funk.Count);

            await ctx.Message.RespondAsync(stringLibrary.Funk[i]).ConfigureAwait(false);
        }

        public static void AddFunk(CommandContext ctx, StringLibrary stringLibrary)
        {
            //if (userlist.UserIsMuted(nick) || !Settings.Default.funkEnabled) return;

            string args;
            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                args = string.Empty;
            }

            try
            {
                string[] splits = ctx.Message.Content.Split();
                if (string.Compare(splits[0].ToLower(), "add") == 0)
                    args = args.Replace("add ", string.Empty);

                stringLibrary.Funk.Add(args);

                stringLibrary.SaveLibrary(StringLibrary.LibraryType.Funk);
            }
            catch
            {
            }
        }
    }
}