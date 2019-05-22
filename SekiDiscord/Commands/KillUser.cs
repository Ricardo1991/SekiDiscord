using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace SekiDiscord.Commands
{
    internal class KillUser
    {
        private const int MAX_KILLS = 500;
        private static Random r = new Random();

        public static KillResult Kill(string author, List<DiscordMember> usersOnline, StringLibrary stringLibrary, string target)
        {
            return KillUsername(target, author, usersOnline, stringLibrary);
        }

        private static KillResult KillUsername(string args, string author, List<DiscordMember> usersOnline, StringLibrary stringLibrary)
        {
            string target;
            int killID;
            string killString;

            try
            {
                if (args.ToLower().Trim() == "la kill")
                {
                    return new KillResult(author + " lost his way", false);
                }
                else if (args.ToLower() == "me baby".Trim())
                {
                    return new KillResult("WASSA WASSA https://www.youtube.com/watch?v=Yk8DAb99QeQ", false);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                        target = usersOnline[r.Next(usersOnline.Count)].DisplayName;
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

                    killString = Useful.FillTags(killString, author.Trim(), target, usersOnline);

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

        internal static KillResult KillRandom(string args, string author, List<DiscordMember> usersOnline, StringLibrary stringLibrary)
        {
            Random r = new Random();
            string target = "";
            string killString;
            KillResult message;

            if (string.IsNullOrWhiteSpace(args) || args.ToLower() == "random")
                target = usersOnline[r.Next(usersOnline.Count)].DisplayName;
            else
                target = args.Trim();

            try
            {
                killString = stringLibrary.GetRandomKillString();
                killString = Useful.FillTags(killString, author.Trim(), target, usersOnline).Replace("  ", " ");

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
                Result = result;
                IsAction = isAction;
            }

            public string Result { get; set; }
            public bool IsAction { get; set; }
        }
    }
}