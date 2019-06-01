using System;
using System.Collections.Generic;
using System.Globalization;

namespace SekiDiscord.Commands
{
    internal class Basics
    {
        public static int Roll(string content)
        {
            Random random = new Random();
            int max = 100;

            try
            {
                string arg = content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ", StringComparison.OrdinalIgnoreCase);
                int parseMax = int.Parse(arg, CultureInfo.CreateSpecificCulture("en-GB"));

                if (parseMax > 0)
                    max = parseMax;
            }
            catch (IndexOutOfRangeException)
            {
            }
            catch (FormatException)
            {
            }
            catch (OverflowException)
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

            arg = content.Split(new char[] { ' ' }, 2)[1].Trim().Replace("  ", " ", StringComparison.OrdinalIgnoreCase);

            if (arg.Contains(',', StringComparison.OrdinalIgnoreCase))
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

            if (arg.Contains(',', StringComparison.OrdinalIgnoreCase))
                choices = arg.Split(new char[] { ',' });
            else
                choices = arg.Split(new char[] { ' ' });

            if (choices.Length == 0)
                throw new Exception("There are no choices.");

            int random = r.Next(choices.Length);
            return user + ": " + choices[random].Trim();
        }

        public static string PokeRandom(List<string> listU, string user)
        {
            int userNumber;
            Random rnd = new Random();

            do
            {
                userNumber = rnd.Next(listU.Count);
            }
            while (listU[userNumber] == user);

            return "*pokes " + listU[userNumber] + "*";
        }
    }
}