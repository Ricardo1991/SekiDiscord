using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SekiDiscord
{
    public static class Useful
    {
        public static bool MemberIsBotOperator(DiscordMember member)
        {
            foreach (DiscordRole role in member.Roles)
            {
                if (role.Name == "bot-admin")
                {
                    return true;
                }
            }
            return false;
        }

        public static List<DiscordMember> GetOnlineMembers(DiscordGuild discordGuild)
        {
            List<DiscordMember> ul = new List<DiscordMember>();

            foreach (DiscordMember u in discordGuild.Members)
            {
                try
                {
                    if (u.Presence.Status == UserStatus.Online || u.Presence.Status == UserStatus.Idle)
                        ul.Add(u);
                }
                catch
                {
                }
            }
            return ul;
        }

        public static List<string> GetOnlineNames(DiscordGuild discordGuild)
        {
            List<string> ul = new List<string>();

            foreach (DiscordMember u in discordGuild.Members)
            {
                try
                {
                    if (u.Presence.Status == UserStatus.Online || u.Presence.Status == UserStatus.Idle)
                        ul.Add(u.DisplayName);
                }
                catch
                {
                }
            }
            return ul;
        }

        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;

            if (string.IsNullOrWhiteSpace(strSource)) return string.Empty;

            if (!strSource.Contains(strStart, StringComparison.Ordinal))
            {
                throw new Exception("Source does not contain Start");
            }

            //Get to the end
            if (string.IsNullOrEmpty(strEnd))
            {
                if (string.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;

                End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }

            if (strSource.Contains(strStart, StringComparison.Ordinal) && strSource.Contains(strEnd, StringComparison.Ordinal))
            {
                if (string.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start, StringComparison.Ordinal);
                if (End < 0) End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                if (string.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
        }

        public static string FillTags(string template, string user, string target, List<string> userlist)
        {
            var regex = new Regex(Regex.Escape("<random>"));
            Random r = new Random();
            string randomTarget;

            template = template.Replace("<TARGET>", target.ToUpper(CultureInfo.CreateSpecificCulture("en-GB")), StringComparison.Ordinal)
                .Replace("<USER>", user.ToUpper(CultureInfo.CreateSpecificCulture("en-GB")), StringComparison.Ordinal);
            template = template.Replace("<target>", target, StringComparison.Ordinal).Replace("<user>", user, StringComparison.Ordinal);

            while (template.Contains("<random>", StringComparison.OrdinalIgnoreCase))
            {
                do
                {
                    randomTarget = userlist[r.Next(userlist.Count)];
                } while (string.Compare(target, randomTarget, StringComparison.OrdinalIgnoreCase) == 0 && userlist.Count > 1);

                template = regex.Replace(template, randomTarget, 1);
            }

            return template;
        }
    }
}
