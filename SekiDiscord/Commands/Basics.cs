using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace SekiDiscord.Commands
{
    internal class Basics
    {
        public static int Roll(string content)
        {
            Random random = new Random();
            string arg = string.Empty;
            int max = 100;

            try
            {
                arg = content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ");
                int parseMax = int.Parse(arg);

                if (parseMax > 0)
                    max = parseMax;
            }
            catch
            {
            }

            return random.Next(0, max) + 1;
        }

        public static string Shuffle(string content)
        {
            string message = string.Empty;
            string arg;

            Random r = new Random();
            string[] choices;
            List<string> sList = new List<string>();

            try
            {
                arg = content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ");
            }
            catch
            {
                return message;
            }

            if (arg.Contains(','))
                choices = arg.Split(new char[] { ',' });
            else
                choices = arg.Split(new char[] { ' ' });

            foreach (string s in choices)
            {
                sList.Add(s);
            }

            if (sList.Count != 0)
            {
                while (sList.Count > 0)
                {
                    int random = r.Next(sList.Count);
                    message = message + " " + sList[random];
                    sList.Remove(sList[random]);
                }
            }
            return message;
        }

        public static string Choose(string arg, string user)
        {
            Random r = new Random();
            string[] choices;

            if (arg.Contains(','))
                choices = arg.Split(new char[] { ',' });
            else
                choices = arg.Split(new char[] { ' ' });

            if (choices.Length == 0)
                throw new Exception("There are no choices.");

            int random = r.Next(choices.Length);
            return user + ": " + choices[random].Trim();
        }

        public static string PokeRandom(List<DiscordMember> listU, string user)
        {
            int userNumber;
            Random rnd = new Random();

            do
            {
                userNumber = rnd.Next(listU.Count);
            }
            while (listU[userNumber].DisplayName == user);

            return "*pokes " + listU[userNumber].DisplayName + "*";
        }
    }
}