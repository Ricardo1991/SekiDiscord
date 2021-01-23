using DSharpPlus.Entities;
using System;
using System.Collections.Generic;


namespace SekiDiscord.Commands
{
    internal class Fortune
    {
        private static List<string> Fortunes { get; set; }

        static Fortune()
        {
            Fortunes = FileHandler.LoadStringListFromFile(FileHandler.StringListFileType.Fortune);
        }

        public static string GetFortune(DateTime date, DiscordUser discordUser)
        {
            if (Fortunes == null || Fortunes.Count == 0)
                return null;

            ulong dayFortuneId = discordUser.Id ^ (ulong)date.Ticks;
            int fortuneID = (int)(dayFortuneId % (ulong)Fortunes.Count);

            return Fortunes[fortuneID];
        }
    }
}