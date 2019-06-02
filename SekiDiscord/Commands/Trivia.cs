using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SekiDiscord.Commands
{
    public class Trivia
    {
        public static List<string> TriviaList { get; set; }

        static Trivia()
        {
            TriviaList = ReadTrivia();
        }

        public static string GetTrivia()
        {
            Random rnd = new Random();
            string message;

            message = TriviaList[rnd.Next(TriviaList.Count)];

            return message;
        }

        public static List<string> ReadTrivia() //Reads the Trivia stuff
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
                catch (IOException e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Failed to read trivia. " + e.Message);
                }
            }
            else
            {
                //Settings.Default.triviaEnabled = false;
                //Settings.Default.Save();
            }

            return trivia;
        }
    }
}