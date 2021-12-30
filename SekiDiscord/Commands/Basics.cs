using System;
using System.Text;
using System.Collections.Generic;

namespace SekiDiscord.Commands
{
    public static class Basics
    {

        public static string RollMany(string format){

            if(!format.Contains('d')){
                //format error
            }

            string[] splitS = format.Split('d');

            if(splitS.Length<2){
                //format error 2
            }

            int diceCount = Int32.Parse(splitS[0]);
            int diceSize = Int32.Parse(splitS[1]);

            StringBuilder resultsBuilder = new StringBuilder();
            for(int i = 0; i<diceCount; i++)
            {
                resultsBuilder.Append(Basics.Roll(diceSize) + " ");
            }

            return resultsBuilder.ToString();
        }

        public static int Roll(int inputMax = 100)
        {
            Random random = new Random();
            return random.Next(0, inputMax) + 1;
        }

        public static string Shuffle(string content)
        {
            string message = string.Empty;
            string arg;

            Random r = new Random();
            string[] choices;
            List<string> sList = new List<string>();

            arg = content.Trim().Replace("  ", " ", StringComparison.OrdinalIgnoreCase);

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