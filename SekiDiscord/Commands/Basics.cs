using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class Basics
    {
        public static async Task Roll(CommandContext ctx)
        {
            string nick = ctx.Member.DisplayName;
            Random random = new Random();
            string arg = string.Empty;
            int max = 100;

            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ");
                int parseMax = int.Parse(arg);

                if (parseMax > 0)
                    max = parseMax;
            }
            catch
            {
            }

            int number = random.Next(0, max) + 1;

            nick = nick.Replace("\r", "");
            string message = nick + " rolled a " + number;
            await ctx.RespondAsync(message);
        }

        public static async Task Shuffle(CommandContext ctx)
        {
            string message = string.Empty;
            string arg;

            Random r = new Random();
            string[] choices;
            List<string> sList = new List<string>();

            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ");
            }
            catch
            {
                return;
            }

            if (arg.Contains(','))
                choices = arg.Split(new char[] { ',' });
            else
                choices = arg.Split(new char[] { ' ' });

            foreach (string s in choices)
            {
                sList.Add(s);
            }

            if (sList.Count != 0)
            {
                while (sList.Count > 0)
                {
                    int random = r.Next(sList.Count);
                    message = message + " " + sList[random];
                    sList.Remove(sList[random]);
                }

                await ctx.RespondAsync(message);
            }
        }

        public static async Task Choose(CommandContext ctx)
        {
            string message = string.Empty;
            string arg;
            string user = ctx.Member.DisplayName;

            Random r = new Random();
            string[] choices;
            List<string> sList = new List<string>();

            try
            {
                arg = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ");
            }
            catch
            {
                return;
            }

            if (arg.Contains(','))
                choices = arg.Split(new char[] { ',' });
            else
                choices = arg.Split(new char[] { ' ' });

            if (choices.Length != 0)
            {
                int random = r.Next(choices.Length);
                message = user + ": " + choices[random].Trim();
                await ctx.RespondAsync(message);
            }
        }

        public static async Task PokeRandom(CommandContext ctx)
        {
            int userNumber;
            Random rnd = new Random();
            List<DiscordMember> listU = Useful.getOnlineUsers(ctx.Channel.Guild);

            do
            {
                userNumber = rnd.Next(listU.Count);
            }
            while (listU[userNumber].DisplayName == ctx.Member.DisplayName);

            string message = "*pokes " + listU[userNumber].DisplayName + "*";
            await ctx.RespondAsync(message);
        }
    }
}