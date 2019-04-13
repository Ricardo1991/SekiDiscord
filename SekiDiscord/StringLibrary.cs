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
            ReadHelp();                 //Help text
            ReadTrivia();               //Trivia strings
            ReadKills();                //Read the killstrings
            ReadFacts();                //Read the factStrings
            ReadNickGen();              //For the Nick generator
            ReadQuotes();
            ReadFunk();
            ReadRules();
            ReadPings();                //Read ping file
            ReadSeen();                 //Read seem file
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
                    ReadRules();
                    break;

                case "help":
                    ReadHelp();
                    break;

                case "nick":
                case "nicks":
                    ReadNickGen();
                    break;

                case "trivia":
                case "trivias":
                    ReadTrivia();
                    break;

                case "kills":
                case "kill":
                    ReadKills();
                    break;

                case "facts":
                case "fact":
                    ReadFacts();
                    break;

                case "quotes":
                case "quote":
                    ReadQuotes();
                    break;

                case "funk":
                    ReadFunk();
                    break;

                case "pings":
                case "ping":
                    ReadPings();
                    break;

                case "seen":
                    ReadSeen();
                    break;

                default:
                    return false;
            }
            return true;
        }

        public bool SaveLibrary()
        {
            SaveFunk();
            SaveQuotes();
            SavePings();        // Save pings to file
            SaveSeen();

            return true;
        }

        public bool SaveLibrary(string name)
        {
            switch (name.ToLower())
            {
                case "all":
                    SaveLibrary();
                    break;

                case "rules":
                case "rule":

                    break;

                case "help":

                    break;

                case "nick":
                case "nicks":

                    break;

                case "trivia":
                case "trivias":

                    break;

                case "kills":
                case "kill":

                    break;

                case "facts":
                case "fact":

                    break;

                case "quotes":
                case "quote":
                    SaveQuotes();
                    break;

                case "funk":
                    SaveFunk();
                    break;

                case "pings":
                case "ping":
                    SavePings();
                    break;

                case "seen":
                    SaveSeen();
                    break;

                default:
                    return false;
            }
            return true;
        }

        private async void ReadPings()
        {
            Pings.Clear();
            if (File.Exists("TextFiles/pings.json"))
            {
                try
                {
                    using (StreamReader r = new StreamReader("TextFiles/pings.json"))
                    {
                        string json = await r.ReadToEndAsync();
                        Pings = JsonConvert.DeserializeObject<Dictionary<ulong, HashSet<string>>>(json);
                    }
                }
                catch
                {
                }
            }
        }

        private async void ReadSeen()
        {
            Seen.Clear();
            if (File.Exists("TextFiles/seen.json"))
            {
                try
                {
                    using (StreamReader r = new StreamReader("TextFiles/seen.json"))
                    {
                        string json = await r.ReadToEndAsync();
                        Seen = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(json);
                    }
                }
                catch
                {
                }
            }
        }

        private void SavePings()
        {
            try
            {
                using (StreamWriter w = File.CreateText("TextFiles/pings.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(w, Pings);
                }
            }
            catch
            {
            }
        }

        private void SaveSeen()
        {
            try
            {
                using (StreamWriter w = File.CreateText("TextFiles/seen.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(w, Seen);
                }
            }
            catch
            {
            }
        }

        private void ReadKills()
        {
            Kill.Clear();
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
                            Kill.Add(killS);
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
        }

        internal string getRandomKillString()
        {
            string s = Killgen.RebuildPhrase(Killgen.Walk());

            return s;
        }

        private void ReadFacts()
        {
            Facts.Clear();
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
                            Facts.Add(factS);
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
        }

        private void ReadRules()
        {
            Rules.Clear();
            if (File.Exists("TextFiles/rules.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/rules.txt");
                    while (sr.Peek() >= 0)
                    {
                        Rules.Add(sr.ReadLine());
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
        }

        private void ReadHelp()
        {
            Help.Clear();
            if (File.Exists("TextFiles/help.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/help.txt");
                    while (sr.Peek() >= 0)
                    {
                        Help.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
            else
            {
                //Settings.Default.help_Enabled = false;
                //Settings.Default.Save();
            }
        }

        private void ReadTrivia() //Reads the Trivia stuff
        {
            Trivia.Clear();

            if (File.Exists("TextFiles/trivia.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/trivia.txt");
                    while (sr.Peek() >= 0)
                    {
                        Trivia.Add(sr.ReadLine());
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
        }

        private void ReadNickGen()//These are for the Nick gen
        {
            NickGenStrings = new List<string>();
            NickGenStrings.Clear();
            if (File.Exists("TextFiles/nickGen.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/nickGen.txt");
                    while (sr.Peek() >= 0)
                    {
                        NickGenStrings.Add(sr.ReadLine());
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
        }

        private void ReadQuotes()
        {
            Quotes = new List<string>();
            Quotes.Clear();

            if (File.Exists("TextFiles/quotes.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/quotes.txt");
                    while (sr.Peek() >= 0)
                    {
                        Quotes.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
        }

        private void SaveQuotes()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/quotes.txt", false))
            {
                foreach (string q in Quotes)
                {
                    newTask.WriteLine(q);
                }
            }
        }

        private void ReadFunk()
        {
            Funk = new List<string>();
            Funk.Clear();

            if (File.Exists("TextFiles/funk.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/funk.txt");
                    while (sr.Peek() >= 0)
                    {
                        Funk.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch
                {
                }
            }
        }

        private void SaveFunk()
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/Funk.txt", false))
            {
                foreach (string q in Funk)
                {
                    newTask.WriteLine(q);
                }
            }
        }
    }
}