using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace SekiDiscord.Commands
{
    internal class Fact
    {
        public static List<string> Facts { get; set; }
        public static List<int> FactsUsed { get; set; }

        static Fact()
        {
            Facts = ReadFacts();
            FactsUsed = new List<int>();
        }

        public static string ShowFact(string args, List<string> listU, string nick)
        {
            Random r = new Random();
            string target = "";
            string factString;
            int factID;
            int MAX_FACTS = 300;

            var regex = new Regex(Regex.Escape("<random>"));

            if (Facts.Count < 1)
                return string.Empty;

            if (string.IsNullOrWhiteSpace(args) || string.Compare(args, "random", StringComparison.OrdinalIgnoreCase) == 0)
                target = listU[r.Next(listU.Count)];
            else
                target = args.Trim();

            if (Facts.Count <= MAX_FACTS)
            {
                FactsUsed.Clear();
                factID = r.Next(Facts.Count);
                FactsUsed.Insert(0, factID);
            }
            else
            {
                do factID = r.Next(Facts.Count);
                while (FactsUsed.Contains(factID));
            }

            if (FactsUsed.Count >= MAX_FACTS)
            {
                FactsUsed.Remove(FactsUsed[FactsUsed.Count - 1]);
            }

            FactsUsed.Insert(0, factID);

            factString = Facts[factID];

            return Useful.FillTags(factString, nick.Trim(), target, listU);
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