using System;
using System.Collections.Generic;


namespace SekiDiscord.Commands
{
    public static class Fortune
    {
        private static List<string> Fortunes { get; set; }

        static Fortune()
        {
            Fortunes = FileHandler.LoadStringListFromFile(FileHandler.StringListFileType.Fortune);
        }

        public static string GetFortune(ulong discordUserID)
        {
            return GetFortune(DateTime.Today, discordUserID);
        }

        public static string GetFortune(DateTime date, ulong discordUserID)
        {
            if (Fortunes == null || Fortunes.Count == 0)
                return null;

            ulong dayFortuneId = discordUserID ^ (ulong)date.Ticks;
            int fortuneID = (int)(dayFortuneId % (ulong)Fortunes.Count);

            return Fortunes[fortuneID];
        }
    }
}