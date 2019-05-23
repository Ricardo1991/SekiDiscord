using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class Seen
    {
        public static void MarkUserSeen(MessageCreateEventArgs e, StringLibrary stringLibrary)
        {
            string username = ((DiscordMember)e.Message.Author).DisplayName.ToLower();

            if (!stringLibrary.Seen.ContainsKey(username))
            {
                stringLibrary.Seen.Add(username, DateTime.UtcNow);
            }
            else if (stringLibrary.Seen.ContainsKey(username))
            {
                stringLibrary.Seen[username] = DateTime.UtcNow;
            }

            stringLibrary.SaveLibrary(StringLibrary.LibraryType.Seen);
        }

        private static DateTime GetUserSeenUTC(string nick, StringLibrary stringLibrary)
        {
            string user = nick.ToLower();
            if (stringLibrary.Seen.ContainsKey(user))
            {
                return stringLibrary.Seen[user];
            }
            else
                return new DateTime(0);
        }

        public static async Task CheckSeen(CommandContext ctx, StringLibrary stringLibrary)
        {
            string args;

            try
            {
                args = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                args = string.Empty;
            }

            string message;
            DateTime seenTime;
            DateTime now = DateTime.UtcNow;
            TimeSpan diff;

            seenTime = GetUserSeenUTC(args, stringLibrary);

            if (seenTime.CompareTo(new DateTime(0)) == 0)
                message = "The user has not been seen yet, or an error has occured";
            else
            {
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

                message = "The user " + args + " was last seen " + timeDiff;
            }

            await ctx.Message.RespondAsync(message).ConfigureAwait(false);
        }
    }
}