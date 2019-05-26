using MarkovSharp.TokenisationStrategies;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SekiDiscord.Commands
{
    internal class KillUser
    {
        public static StringMarkov Killgen { get; set; } = new StringMarkov();
        public static List<string> Kills { get; set; } = ReadKills();
        public static List<int> KillsUsed { get; set; } = new List<int>();

        private const int MAX_KILLS = 500;
        private static readonly Random r = new Random();

        public static KillResult Kill(string author, List<string> usersOnline, string target)
        {
            return KillUsername(target, author, usersOnline);
        }

        private static KillResult KillUsername(string args, string author, List<string> usersOnline)
        {
            string target;
            int killID;
            string killString;

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
            catch (Exception ex)
            {
                Console.WriteLine("Error BOT randomkill :" + ex.Message);
                message = new KillResult("Sorry, i can't think of a random kill right now.", false);
            }

            return message;
        }

        public static List<string> ReadKills()
        {
            List<string> kills = new List<string>();
            KillsUsed.Clear();
            Killgen = new StringMarkov();

            if (File.Exists("TextFiles/kills.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/kills.txt");
                    while (sr.Peek() >= 0)
                    {
                        string killS = sr.ReadLine();

                        if (killS.Length > 1 && !(killS[0] == '/' && killS[1] == '/'))
                        {
                            kills.Add(killS);
                            Killgen.Learn(killS);
                        }
                    }

                    sr.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Failed to read kills. " + e.Message);
                }
            }
            else
            {
                //TODO: Save settings
                //Settings.Default.killEnabled = false;
                //Settings.Default.Save();
            }

            return kills;
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