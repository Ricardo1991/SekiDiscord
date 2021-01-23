using System;
using System.Collections.Generic;

namespace SekiDiscord.Commands
{
    public static class Fact
    {
        private const int MAX_FACTS_REMEMBER = 300;
        private static List<string> Facts { get; set; }
        private static List<int> FactsUsed { get; set; }

        static Fact()
        {
            FactsUsed = new List<int>();
            Facts = FileHandler.LoadStringListFromFile(FileHandler.StringListFileType.Facts);
        }

        public static int FactCount()
        {
            return Facts.Count;
        }

        public static string ShowFact(string args, List<string> listU, string user)
        {
            Random r = new Random();
            string target;
            int factID;

            if (Facts.Count < 1)
                throw new Exception("No facts loaded.");

            if (string.IsNullOrWhiteSpace(args) || string.Compare(args, "random", StringComparison.OrdinalIgnoreCase) == 0)
                target = listU[r.Next(listU.Count)];        //Random target
            else
                target = args.Trim();

            // Not enough facts to care for anti repeat
            if (Facts.Count <= MAX_FACTS_REMEMBER)
            {
                factID = r.Next(Facts.Count);
            }
            //Anti-repeat
            else
            {
                do factID = r.Next(Facts.Count);
                while (FactsUsed.Contains(factID));

                FactsUsed.Insert(0, factID);
            }

            //Clear the oldest entry if the list reaches the limit
            if (FactsUsed.Count >= MAX_FACTS_REMEMBER)
            {
                FactsUsed.Remove(FactsUsed[FactsUsed.Count - 1]);
            }

            return Useful.FillTags(Facts[factID], user.Trim(), target, listU);
        }
    }
}