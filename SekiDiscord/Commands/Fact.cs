using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class Fact
    {
        public static async Task ShowFact(CommandContext ctx, StringLibrary stringLibrary)
        {
            Random r = new Random();
            string target = "";
            string factString;
            int factID;
            List<DiscordMember> listU = Useful.getOnlineUsers(ctx.Channel.Guild);
            int MAX_FACTS = 300;
            string nick = ctx.Member.DisplayName;

            string args;

            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                args = string.Empty;
            }

            var regex = new Regex(Regex.Escape("<random>"));

            if (stringLibrary.Facts.Count < 1)
                return;

            if (string.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                target = listU[r.Next(listU.Count)].DisplayName;
            else
                target = args.Trim();

            if (stringLibrary.Facts.Count <= MAX_FACTS)
            {
                stringLibrary.FactsUsed.Clear();
                factID = r.Next(stringLibrary.Facts.Count);
                stringLibrary.FactsUsed.Insert(0, factID);
            }
            else
            {
                do factID = r.Next(stringLibrary.Facts.Count);
                while (stringLibrary.FactsUsed.Contains(factID));
            }

            if (stringLibrary.FactsUsed.Count >= MAX_FACTS)
            {
                stringLibrary.FactsUsed.Remove(stringLibrary.FactsUsed[stringLibrary.FactsUsed.Count - 1]);
            }

            stringLibrary.FactsUsed.Insert(0, factID);

            factString = stringLibrary.Facts[factID];

            factString = Useful.FillTags(factString, nick.Trim(), target, listU);

            await ctx.RespondAsync(factString).ConfigureAwait(false);
        }
    }
}