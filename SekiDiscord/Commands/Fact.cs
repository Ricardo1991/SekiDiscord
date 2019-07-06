using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

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
            Facts = ReadFacts();
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

            var regex = new Regex(Regex.Escape("<random>"));

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

        public static List<string> ReadFacts()
        {
            List<string> facts = new List<string>();
            FactsUsed.Clear();

            if (File.Exists("TextFiles/facts.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/facts.txt");
                    while (sr.Peek() >= 0)
                    {
                        string factS = sr.ReadLine();

                        if (factS.Length > 1 && !(factS[0] == '/' && factS[1] == '/'))
                        {
                            facts.Add(factS);
                        }
                    }

                    sr.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Failed to read facts. " + e.Message);
                }
            }
            else
            {
                //Settings.Default.factsEnabled = false;
                //Settings.Default.Save();
            }

            return facts;
        }
    }
}