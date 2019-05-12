using MarkovSharp.TokenisationStrategies;
using Newtonsoft.Json;
using SekiDiscord.Commands;
using System;
using System.Collections.Generic;
using System.IO;

namespace SekiDiscord
{
    public class StringLibrary
    {
        public List<string> Rules { get; set; } = new List<string>();
        public List<string> Help { get; set; } = new List<string>();
        public List<string> Trivia { get; set; } = new List<string>();
        public List<string> Kill { get; set; } = new List<string>();
        public List<string> Facts { get; set; } = new List<string>();
        public List<string> Quotes { get; set; } = new List<string>();
        public List<string> Funk { get; set; } = new List<string>();
        public List<string> NickGenStrings { get; set; }
        public List<CustomCommand> CustomCommands { get; set; } = new List<CustomCommand>();
        public List<int> KillsUsed { get; set; } = new List<int>();
        public List<int> FactsUsed { get; set; } = new List<int>();
        public StringMarkov Killgen { get; set; } = new StringMarkov();
        public Dictionary<ulong, HashSet<string>> Pings { get; set; } = new Dictionary<ulong, HashSet<string>>();
        public Dictionary<string, DateTime> Seen { get; set; } = new Dictionary<string, DateTime>();

        public StringLibrary()
        {
            ReloadLibrary();
        }

        public bool ReloadLibrary()
        {
            Trivia = ReadTrivia();              //Trivia strings
            Kill = ReadKills();                 //Read the killstrings
            Facts = ReadFacts();                //Read the factStrings
            NickGenStrings = ReadNickGen();     //For the Nick generator
            Quotes = ReadQuotes();
            Funk = ReadFunk();
            Rules = ReadRules();
            Pings = ReadPings();                //Read ping file
            Seen = ReadSeen();                  //Read seem file
            CustomCommands = CustomCommand.LoadCustomCommands();

            return true;
        }

        public bool ReloadLibrary(string name)
        {
            switch (name.ToLower())
            {
                case "all":
                    ReloadLibrary();
                    break;

                case "rules":
                case "rule":
                    Rules = ReadRules();
                    break;

                case "nick":
                case "nicks":
                    NickGenStrings = ReadNickGen();
                    break;

                case "trivia":
                case "trivias":
                    Trivia = ReadTrivia();
                    break;

                case "kills":
                case "kill":
                    Kill = ReadKills();
                    break;

                case "facts":
                case "fact":
                    Facts = ReadFacts();
                    break;

                case "quotes":
                case "quote":
                    Quotes = ReadQuotes();
                    break;

                case "funk":
                    Funk = ReadFunk();
                    break;

                case "pings":
                case "ping":
                    Pings = ReadPings();
                    break;

                case "seen":
                    Seen = ReadSeen();
                    break;

                default:
                    return false;
            }
            return true;
        }

        public bool SaveLibrary()
        {
            SaveFunk(Funk);
            SaveQuotes(Quotes);
            SavePings(Pings);        // Save pings to file
            SaveSeen(Seen);

            return true;
        }

        public enum LibraryType { All, Rules, Nick, Trivia, Kill, Fact, Quote, Funk, Ping, Seen }

        public bool SaveLibrary(LibraryType type)
        {
            switch (type)
            {
                case LibraryType.All:
                    SaveLibrary();
                    break;

                case LibraryType.Quote:
                    SaveQuotes(Quotes);
                    break;

                case LibraryType.Funk:
                    SaveFunk(Funk);
                    break;

                case LibraryType.Ping:
                    SavePings(Pings);
                    break;

                case LibraryType.Seen:
                    SaveSeen(Seen);
                    break;

                case LibraryType.Rules:
                case LibraryType.Nick:
                case LibraryType.Trivia:
                case LibraryType.Kill:
                case LibraryType.Fact:
                default:
                    return false;
            }
            return true;
        }

        private Dictionary<ulong, HashSet<string>> ReadPings()
        {
            Dictionary<ulong, HashSet<string>> ping = new Dictionary<ulong, HashSet<string>>();

            if (File.Exists("TextFiles/pings.json"))
            {
                try
                {
                    using (StreamReader r = new StreamReader("TextFiles/pings.json"))
                    {
                        string json = r.ReadToEnd();
                        ping = JsonConvert.DeserializeObject<Dictionary<ulong, HashSet<string>>>(json);
                    }
                }
                catch
                {
                }
            }

            return ping;
        }

        private Dictionary<string, DateTime> ReadSeen()
        {
            Dictionary<string, DateTime> seen = new Dictionary<string, DateTime>();

            if (File.Exists("TextFiles/seen.json"))
            {
                try
                {
                    using (StreamReader r = new StreamReader("TextFiles/seen.json"))
                    {
                        string json = r.ReadToEnd();
                        seen = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(json);
                    }
                }
                catch
                {
                }
            }

            return seen;
        }

        private void SavePings(Dictionary<ulong, HashSet<string>> ping)
        {
            try
            {
                using (StreamWriter w = File.CreateText("TextFiles/pings.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(w, ping);
                }
            }
            catch
            {
            }
        }

        private void SaveSeen(Dictionary<string, DateTime> seen)
        {
            try
            {
                using (StreamWriter w = File.CreateText("TextFiles/seen.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(w, seen);
                }
            }
            catch
            {
            }
        }

        private List<string> ReadKills()
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
                catch
                {
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

        internal string getRandomKillString()
        {
            return Killgen.RebuildPhrase(Killgen.Walk());
        }

        private List<string> ReadFacts()
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
                catch
                {
                }
            }
            else
            {
                //Settings.Default.factsEnabled = false;
                //Settings.Default.Save();
            }

            return facts;
        }

        private List<string> ReadRules()
        {
            List<string> rules = new List<string>();
            if (File.Exists("TextFiles/rules.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/rules.txt");
                    while (sr.Peek() >= 0)
                    {
                        rules.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                //Settings.Default.rules_Enabled = false;
                //Settings.Default.Save();
            }

            return rules;
        }

        private List<string> ReadTrivia() //Reads the Trivia stuff
        {
            List<string> trivia = new List<string>();

            if (File.Exists("TextFiles/trivia.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/trivia.txt");
                    while (sr.Peek() >= 0)
                    {
                        trivia.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                //Settings.Default.triviaEnabled = false;
                //Settings.Default.Save();
            }

            return trivia;
        }

        private List<string> ReadNickGen()//These are for the Nick gen
        {
            List<string> nickGenStrings = new List<string>();
            if (File.Exists("TextFiles/nickGen.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/nickGen.txt");
                    while (sr.Peek() >= 0)
                    {
                        nickGenStrings.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                //Settings.Default.nickEnabled = false;
                //Settings.Default.Save();
            }

            return nickGenStrings;
        }

        private List<string> ReadQuotes()
        {
            List<string> quotes = new List<string>();

            if (File.Exists("TextFiles/quotes.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/quotes.txt");
                    while (sr.Peek() >= 0)
                    {
                        quotes.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            return quotes;
        }

        private void SaveQuotes(List<string> quotes)
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/quotes.txt", false))
            {
                foreach (string q in quotes)
                {
                    newTask.WriteLine(q);
                }
            }
        }

        private List<string> ReadFunk()
        {
            List<string> funk = new List<string>();

            if (File.Exists("TextFiles/funk.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/funk.txt");
                    while (sr.Peek() >= 0)
                    {
                        funk.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            return funk;
        }

        private void SaveFunk(List<string> funk)
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/Funk.txt", false))
            {
                foreach (string q in funk)
                {
                    newTask.WriteLine(q);
                }
            }
        }
    }
}