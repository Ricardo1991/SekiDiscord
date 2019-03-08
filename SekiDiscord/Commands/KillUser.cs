using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace SekiDiscord.Commands
{
    internal class KillUser
    {
        private static int MAX_KILLS = 500;
        private static Random r = new Random();

        public static KillResult Kill(CommandContext ctx, StringLibrary stringLibrary)
        {
            string target;
            int killID;
            string killString;
            string nick = ctx.Member.DisplayName;
            List<DiscordMember> listU = Useful.getOnlineUsers(ctx.Channel.Guild);
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
                if (args.ToLower().Trim() == "la kill")
                {
                    return new KillResult(nick + " lost his way", false);
                }
                else if (args.ToLower() == "me baby".Trim())
                {
                    return new KillResult("WASSA WASSA https://www.youtube.com/watch?v=Yk8DAb99QeQ", false);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                        target = listU[r.Next(listU.Count)].DisplayName;
                    else
                        target = args.Trim();

                    if (stringLibrary.Kill.Count <= MAX_KILLS)
                    {
                        stringLibrary.KillsUsed.Clear();
                        killID = r.Next(stringLibrary.Kill.Count);
                        stringLibrary.KillsUsed.Insert(0, killID);
                    }
                    else
                    {
                        do killID = r.Next(stringLibrary.Kill.Count);
                        while (stringLibrary.KillsUsed.Contains(killID));
                    }

                    if (stringLibrary.KillsUsed.Count >= MAX_KILLS)
                    {
                        stringLibrary.KillsUsed.Remove(stringLibrary.KillsUsed[stringLibrary.KillsUsed.Count - 1]);
                    }

                    stringLibrary.KillsUsed.Insert(0, killID);

                    killString = stringLibrary.Kill[killID];

                    killString = Useful.FillTags(killString, nick.Trim(), target, listU);

                    if (killString.ToLower().Contains("<normal>"))
                    {
                        killString = killString.Replace("<normal>", string.Empty).Replace("<NORMAL>", string.Empty);
                        return new KillResult(killString, false);
                    }
                    else
                    {
                        return new KillResult(killString, true);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        internal static KillResult KillRandom(CommandContext ctx, StringLibrary stringLibrary)
        {
            Random r = new Random();
            string target = "";
            string killString;
            string args;
            string nick = ctx.Member.DisplayName;
            KillResult message;

            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                args = string.Empty;
            }

            List<DiscordMember> listU = Useful.getOnlineUsers(ctx.Channel.Guild);

            if (string.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                target = listU[r.Next(listU.Count)].DisplayName;
            else
                target = args.Trim();

            try
            {
                killString = stringLibrary.getRandomKillString();
                killString = Useful.FillTags(killString, nick.Trim(), target, listU).Replace("  ", " ");

                if (killString.ToLower().Contains("<normal>"))
                {
                    killString = killString.Replace("<normal>", string.Empty).Replace("<NORMAL>", string.Empty);
                    message = new KillResult(killString, false);
                }
                else
                    message = new KillResult(killString, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error BOT randomkill :" + ex.Message);
                message = new KillResult("Sorry, i can't think of a random kill right now.", false);
            }

            return message;
        }

        public class KillResult
        {
            public KillResult(string result, bool isAction)
            {
                this.Result = result;
                this.IsAction = isAction;
            }

            public string Result { get; set; }
            public bool IsAction { get; set; }
        }
    }
}