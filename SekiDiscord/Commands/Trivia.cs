using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SekiDiscord.Commands
{
    public static class Trivia
    {
        private static readonly Logger logger = new Logger(typeof(Trivia));

        private static List<string> TriviaList { get; set; }

        static Trivia()
        {
            TriviaList = FileHandler.LoadStringListFromFile(FileHandler.StringListFileType.Trivia);
        }

        public static int TriviaCount()
        {
            if (TriviaList == null)
                return 0;

            return TriviaList.Count;
        }

        public static string GetTrivia()
        {
            if (TriviaList == null || TriviaList.Count == 0)
                throw new Exception("No Trivia loaded");

            Random rnd = new Random();
            string message;

            message = TriviaList[rnd.Next(TriviaList.Count)];

            return message;
        }
    }
}