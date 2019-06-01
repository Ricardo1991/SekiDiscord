using SekiDiscord.Commands;

namespace SekiDiscord
{
    public static class StringLibrary
    {
        public static bool ReloadLibrary()
        {
            Trivia.TriviaList = Trivia.ReadTrivia();    //Trivia strings
            KillUser.Kills = KillUser.ReadKills();
            Fact.Facts = Fact.ReadFacts();              //Read the factStrings
            Nick.NickGenStrings = Nick.ReadNickGen();        //For the Nick generator
            Funk.FunkList = Funk.ReadFunk();
            PingUser.Pings = PingUser.ReadPings();      //Read ping file
            Seen.SeenTime = Seen.ReadSeen();            //Read seen file
            CustomCommand.CustomCommands = CustomCommand.LoadCustomCommands();

            return true;
        }

        public static bool ReloadLibrary(LibraryType type)
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
                    Funk.FunkList = Funk.ReadFunk();
                    break;

                case LibraryType.Ping:
                    PingUser.Pings = PingUser.ReadPings();
                    break;

                case LibraryType.Seen:
                    Seen.SeenTime = Seen.ReadSeen();
                    break;

                case LibraryType.Nick:
                    Nick.NickGenStrings = Nick.ReadNickGen();
                    break;

                case LibraryType.Trivia:
                    Trivia.TriviaList = Trivia.ReadTrivia();
                    break;

                case LibraryType.Kill:
                    KillUser.Kills = KillUser.ReadKills();
                    break;

                case LibraryType.Fact:
                    Fact.Facts = Fact.ReadFacts();
                    break;

                default:
                    return false;
            }
            return true;
        }

        public static bool SaveLibrary()
        {
            Funk.SaveFunk(Funk.FunkList);
            Quotes.SaveQuotes(Quotes.QuotesList);
            PingUser.SavePings(PingUser.Pings);        // Save pings to file
            Seen.SaveSeen(Seen.SeenTime);

            return true;
        }

        public enum LibraryType { All, Nick, Trivia, Kill, Fact, Quote, Funk, Ping, Seen }

        public static bool SaveLibrary(LibraryType type)
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
                    Funk.SaveFunk(Funk.FunkList);
                    break;

                case LibraryType.Ping:
                    PingUser.SavePings(PingUser.Pings);
                    break;

                case LibraryType.Seen:
                    Seen.SaveSeen(Seen.SeenTime);
                    break;

                case LibraryType.Nick:
                case LibraryType.Trivia:
                case LibraryType.Kill:
                case LibraryType.Fact:
                default:
                    return false;
            }
            return true;
        }
    }
}