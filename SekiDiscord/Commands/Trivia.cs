﻿using System;
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
            TriviaList = ReadTrivia();
        }

        public static int TriviaCount()
        {
            return TriviaList.Count;
        }

        public static string GetTrivia()
        {
            Random rnd = new Random();
            string message;

            message = TriviaList[rnd.Next(TriviaList.Count)];

            return message;
        }

        //Reads Trivia messages into memory
        public static List<string> ReadTrivia()
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
                    logger.Error("Failed to read trivia. " + e.Message);
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