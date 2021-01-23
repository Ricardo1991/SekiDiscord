using MarkovSharp.TokenisationStrategies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SekiDiscord.Commands
{
    internal class KillUser
    {
        private static readonly Logger logger = new Logger(typeof(KillUser));

        public static StringMarkov Killgen { get; set; }
        public static List<string> Kills { get; set; }
        public static List<int> KillsUsed { get; set; }
        private const int MAX_KILLS = 500;

        static KillUser()
        {
            KillsUsed = new List<int>();
            Killgen = new StringMarkov();
            Kills = FileHandler.LoadStringListFromFile(FileHandler.StringListFileType.Kills);
        }

        public static KillResult Kill(string author, List<string> usersOnline, string target)
        {
            return KillUsername(target, author, usersOnline);
        }

        private static KillResult KillUsername(string args, string author, List<string> usersOnline)
        {
            string target;
            int killID;
            string killString;

            Random r = new Random();

            if (args.ToLower(CultureInfo.CreateSpecificCulture("en-GB")).Trim() == "la kill")
            {
                return new KillResult(author + " lost his way", false);
            }
            if (args.ToLower(CultureInfo.CreateSpecificCulture("en-GB")).Trim() == "me baby")
            {
                return new KillResult("WASSA WASSA https://www.youtube.com/watch?v=Yk8DAb99QeQ", false);
            }

            if (string.IsNullOrWhiteSpace(args) || args.ToLower(CultureInfo.CreateSpecificCulture("en-GB")) == "random")
                target = usersOnline[r.Next(usersOnline.Count)];
            else
                target = args.Trim();

            if (Kills.Count <= MAX_KILLS)
            {
                KillsUsed.Clear();
                killID = r.Next(Kills.Count);
                KillsUsed.Insert(0, killID);
            }
            else
            {
                do killID = r.Next(Kills.Count);
                while (KillsUsed.Contains(killID));
            }

            if (KillsUsed.Count >= MAX_KILLS)
            {
                KillsUsed.Remove(KillsUsed[KillsUsed.Count - 1]);
            }

            KillsUsed.Insert(0, killID);

            killString = Kills[killID];

            killString = Useful.FillTags(killString, author.Trim(), target, usersOnline);

            if (killString.Contains("<normal>", StringComparison.OrdinalIgnoreCase))
            {
                killString = killString.Replace("<normal>", string.Empty, StringComparison.OrdinalIgnoreCase);
                return new KillResult(killString, false);
            }
            else
            {
                return new KillResult(killString, true);
            }
        }

        internal static KillResult KillRandom(string args, string author, List<string> usersOnline)
        {
            Random r = new Random();
            string target;
            string killString;
            KillResult message;

            if (string.IsNullOrWhiteSpace(args) || string.Compare(args, "random", StringComparison.OrdinalIgnoreCase) == 0)
                target = usersOnline[r.Next(usersOnline.Count)];
            else
                target = args.Trim();

            try
            {
                killString = GetRandomKillString();
                killString = Useful.FillTags(killString, author.Trim(), target, usersOnline).Replace("  ", " ", StringComparison.OrdinalIgnoreCase);

                if (killString.Contains("<normal>", StringComparison.OrdinalIgnoreCase))
                {
                    killString = killString.Replace("<normal>", string.Empty, StringComparison.OrdinalIgnoreCase);
                    message = new KillResult(killString, false);
                }
                else
                    message = new KillResult(killString, true);
            }
            catch (IOException ex)
            {
                logger.Error("Error BOT randomkill: " + ex.Message);
                message = new KillResult("Sorry, i can't think of a random kill right now.", false);
            }

            return message;
        }

        private static string GetRandomKillString()
        {
            return Killgen.RebuildPhrase(Killgen.Walk());
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