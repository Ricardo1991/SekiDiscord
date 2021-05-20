using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace SekiDiscord
{
    public static class Useful
    {
        public static string GetUsername(CommandContext ctx)
        {
            if (ctx.Member != null) {
                return ctx.Member.DisplayName;
            }
            return ctx.User.Username;
        }

        internal static string GetUsername(MessageCreateEventArgs e) {
            if (e.Message.Author != null) {
                return e.Message.Author.Username;
            }
            return e.Author.Username;
        }

        public static string GetDiscordName(CommandContext ctx) {
            return ctx.User.Username + '#' + ctx.User.Discriminator;
        }

        public static bool MemberIsBotOperator(DiscordMember member)
        {
            return member != null && member.Roles.Any(role => role.Name.Equals("bot-admin", StringComparison.Ordinal));
        }

        private static readonly List<UserStatus> UserStatuses = new List<UserStatus>() { UserStatus.Online, UserStatus.Idle };

        public static List<DiscordMember> GetOnlineMembers(DiscordGuild discordGuild)
        {
            if (discordGuild != null) {
                return discordGuild.Members.Values
                    .Where(user => user.Presence != null && UserStatuses.Contains(user.Presence.Status) && user.Id != Settings.Default.ChibiID)
                    .ToList();
            }
            return null;
        }

        public static List<string> GetOnlineNames(DiscordGuild discordGuild)
        {
            if (discordGuild != null) {
                return discordGuild.Members
                    .Where(user => user.Value.Presence != null && UserStatuses.Contains(user.Value.Presence.Status) && user.Value.Id != Settings.Default.ChibiID)
                    .Select(user => user.Value.DisplayName)
                    .ToList();
            }
            return null;
        }

        public static string GetBetween(string strSource, string strStart, string strEnd)
        {
            if (string.IsNullOrWhiteSpace(strSource)) return string.Empty;

            if (!strSource.Contains(strStart, StringComparison.Ordinal))
            {
                throw new ArgumentException("Source does not contain Start");
            }

            int Start = GetBetweenStringStart(strStart, strSource);
            int End = strSource.Length;

            if (!string.IsNullOrEmpty(strEnd) && strSource.Contains(strEnd, StringComparison.Ordinal)) {
                End = strSource.IndexOf(strEnd, Start, StringComparison.Ordinal);
                if (End < 0) End = strSource.Length;
            }

            return strSource[Start..End];
        }

        private static int GetBetweenStringStart(string strStart, string strSource) {
            if (string.IsNullOrEmpty(strStart)) {
                return 0;
            }
            return strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
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