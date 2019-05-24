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
        public List<string> Trivia { get; set; } = new List<string>();
        public List<string> Facts { get; set; } = new List<string>();
        public List<string> Funk { get; set; } = new List<string>();
        public List<CustomCommand> CustomCommands { get; set; } = new List<CustomCommand>();
        public List<string> NickGenStrings { get; set; }
        public List<int> FactsUsed { get; set; } = new List<int>();
        public Dictionary<ulong, HashSet<string>> Pings { get; set; } = new Dictionary<ulong, HashSet<string>>();
        public Dictionary<string, DateTime> Seen { get; set; } = new Dictionary<string, DateTime>();

        public StringLibrary()
        {
            ReloadLibrary();
        }

        public bool ReloadLibrary()
        {
            Trivia = ReadTrivia();              //Trivia strings
            KillUser.Kills = KillUser.ReadKills();
            Facts = ReadFacts();                //Read the factStrings
            NickGenStrings = ReadNickGen();     //For the Nick generator
            Funk = ReadFunk();
            Rules = ReadRules();
            Pings = ReadPings();                //Read ping file
            Seen = ReadSeen();                  //Read seen file
            CustomCommands = CustomCommand.LoadCustomCommands();

            return true;
        }

        public bool ReloadLibrary(LibraryType type)
        {
            switch (type)
            {
                case LibraryType.All:
                    ReloadLibrary();
                    break;

                case LibraryType.Quote:
                    Quotes.QuotesList = Quotes.ReadQuotes();
                    break;

                case LibraryType.Funk:
                    Funk = ReadFunk();
                    break;

                case LibraryType.Ping:
                    Pings = ReadPings();
                    break;

                case LibraryType.Seen:
                    Seen = ReadSeen();
                    break;

                case LibraryType.Rules:
                    Rules = ReadRules();
                    break;

                case LibraryType.Nick:
                    NickGenStrings = ReadNickGen();
                    break;

                case LibraryType.Trivia:
                    Trivia = ReadTrivia();
                    break;

                case LibraryType.Kill:
                    KillUser.Kills = KillUser.ReadKills();
                    break;

                case LibraryType.Fact:
                    Facts = ReadFacts();
                    break;

                default:
                    return false;
            }
            return true;
        }

        public bool SaveLibrary()
        {
            SaveFunk(Funk);
            Quotes.SaveQuotes(Quotes.QuotesList);
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
                    Quotes.SaveQuotes(Quotes.QuotesList);
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

        private static Dictionary<ulong, HashSet<string>> ReadPings()
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

        private static Dictionary<string, DateTime> ReadSeen()
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

        private static void SavePings(Dictionary<ulong, HashSet<string>> ping)
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

        private static void SaveSeen(Dictionary<string, DateTime> seen)
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
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Failed to read facts. " + e.Message);
                }
            }
            else
            {
                //Settings.Default.factsEnabled = false;
                //Settings.Default.Save();
            }

            return facts;
        }

        private static List<string> ReadRules()
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
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Failed to read rules. " + e.Message);
                }
            }
            else
            {
                //Settings.Default.rules_Enabled = false;
                //Settings.Default.Save();
            }

            return rules;
        }

        private static List<string> ReadTrivia() //Reads the Trivia stuff
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
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Failed to read trivia. " + e.Message);
                }
            }
            else
            {
                //Settings.Default.triviaEnabled = false;
                //Settings.Default.Save();
            }

            return trivia;
        }

        private static List<string> ReadNickGen()//These are for the Nick gen
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
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Failed to read nickGen. " + e.Message);
                }
            }
            else
            {
                //Settings.Default.nickEnabled = false;
                //Settings.Default.Save();
            }

            return nickGenStrings;
        }

        private static List<string> ReadFunk()
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
                catch (Exception e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Failed to read funk. " + e.Message);
                }
            }
            return funk;
        }

        private static void SaveFunk(List<string> funk)
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