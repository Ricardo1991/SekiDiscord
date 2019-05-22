using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
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

        public static List<DiscordMember> getOnlineUsers(DiscordGuild discordGuild)
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

        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;

            if (string.IsNullOrWhiteSpace(strSource)) return string.Empty;

            if (string.IsNullOrEmpty(strEnd))
            {
                if (string.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;

                End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
            else if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                if (string.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                if (End < 0) End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                if (string.IsNullOrEmpty(strStart))
                    Start = 0;
                else
                    Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.Length;
                return strSource.Substring(Start, End - Start);
            }
        }

        public static string FillTags(string template, string user, string target, List<DiscordMember> userlist)
        {
            var regex = new Regex(Regex.Escape("<random>"));
            Random r = new Random();
            string randomTarget;

            template = template.Replace("<TARGET>", target.ToUpper()).Replace("<USER>", user.ToUpper());
            template = template.Replace("<target>", target).Replace("<user>", user);

            while (template.Contains("<random>"))
            {
                do
                {
                    randomTarget = userlist[r.Next(userlist.Count)].DisplayName;
                } while (string.Compare(target, randomTarget, true) == 0 || userlist.Count < 2);

                template = regex.Replace(template, randomTarget, 1);
            }

            return template;
        }
    }
}