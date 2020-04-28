using System;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace SekiDiscord.Commands
{
    internal class Fortune
    {
        private static List<string> Fortunes { get; set; }

        static Fortune()
        {
            Fortunes = ReadFortune();
        }

        public static string GetFortune(DateTime date, DiscordUser discordUser)
        {
            ulong dayFortuneId = discordUser.Id ^ (ulong)date.Ticks;
            int fortuneID = (int)(dayFortuneId % (ulong)Fortunes.Count);

            return Fortunes[fortuneID];
        }

        public static List<string> ReadFortune()
        {
            List<string> fortunes = new List<string>();

            if (File.Exists("TextFiles/facts.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/fortune.txt");
                    while (sr.Peek() >= 0)
                    {
                        string fortuneLine = sr.ReadLine();

                        if (fortuneLine.Length > 1 && !(fortuneLine[0] == '/' && fortuneLine[1] == '/'))
                        {
                            fortunes.Add(fortuneLine);
                        }
                    }

                    sr.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Failed to read fortunes. " + e.Message);
                }
            }
            else
            {
                //Settings.Default.fortuneEnabled = false;
                //Settings.Default.Save();
            }

            return fortunes;
        }
    }
}