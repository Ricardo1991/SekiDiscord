using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Globalization;

namespace SekiDiscord.Commands
{
    internal class Seen
    {
        public static void MarkUserSeen(MessageCreateEventArgs e, StringLibrary stringLibrary)
        {
            string username = ((DiscordMember)e.Message.Author).DisplayName.ToLower(CultureInfo.CreateSpecificCulture("en-GB"));

            if (stringLibrary.Seen.ContainsKey(username))
            {
                stringLibrary.Seen[username] = DateTime.UtcNow;
            }
            else
            {
                stringLibrary.Seen.Add(username, DateTime.UtcNow);
            }

            stringLibrary.SaveLibrary(StringLibrary.LibraryType.Seen);
        }

        private static DateTime GetUserSeenUTC(string nick, StringLibrary stringLibrary)
        {
            string user = nick.ToLower(CultureInfo.CreateSpecificCulture("en-GB"));
            if (stringLibrary.Seen.ContainsKey(user))
            {
                return stringLibrary.Seen[user];
            }
            else
                throw new UserNotSeenException("The user " + user + " has not been seen yet, or an error has occured");
        }

        public static string CheckSeen(CommandContext ctx, StringLibrary stringLibrary)
        {
            DateTime seenTime;
            DateTime now = DateTime.UtcNow;
            TimeSpan diff;

            string args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];

            try
            {
                seenTime = GetUserSeenUTC(args, stringLibrary);

                diff = now.Subtract(seenTime);
                string timeDiff = string.Empty;

                if (diff.Days >= 1)
                    if (diff.Days == 1)
                        timeDiff += diff.Days + " day, ";
                    else
                        timeDiff += diff.Days + " days, ";

                if (diff.Hours >= 1)
                    if (diff.Hours == 1)
                        timeDiff += diff.Hours + " hour ago";
                    else
                        timeDiff += diff.Hours + " hours ago";
                else
                    if (diff.Minutes == 1)
                    timeDiff += diff.Minutes + " minute ago";
                else
                    timeDiff += diff.Minutes + " minutes ago";

                return "The user " + args + " was last seen " + timeDiff;
            }
            catch (UserNotSeenException ex)
            {
                return ex.Message;
            }
        }

        public class UserNotSeenException : Exception
        {
            public UserNotSeenException()
            {
            }

            public UserNotSeenException(string user) : base(user)
            {
            }

            public UserNotSeenException(string message, Exception innerException) : base(message, innerException)
            {
            }
        }
    }
}