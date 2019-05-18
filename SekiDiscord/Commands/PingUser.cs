using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class PingUser
    {
        public static async Task PingControl(CommandContext e, StringLibrary stringLibrary, string cmd, string args)
        {
            ulong username = e.Member.Id; // get message creators username in lower case
            switch (cmd)
            {
                case "add":
                    if (!string.IsNullOrWhiteSpace(args))
                    {
                        if (!stringLibrary.Pings.ContainsKey(username))
                        {
                            stringLibrary.Pings.Add(username, new HashSet<string>() { args });
                        }
                        else if (stringLibrary.Pings.ContainsKey(username))
                        {
                            stringLibrary.Pings[username].Add(args);
                        }
                    }
                    break;

                case "remove":
                    if (!string.IsNullOrWhiteSpace(args))
                    {
                        if (stringLibrary.Pings.ContainsKey(username))
                        {
                            stringLibrary.Pings[username].Remove(args);
                        }
                    }
                    break;

                //case "copy":
                //    if (!string.IsNullOrWhiteSpace(args))
                //    {
                //        bool user = stringLibrary.Pings.ContainsKey(username);
                //        bool userToCopyFrom = stringLibrary.Pings.ContainsKey(args);
                //        if (!user && userToCopyFrom)
                //        {
                //            stringLibrary.Pings.Add(username, stringLibrary.Pings[args]);
                //        }
                //        else if (user && userToCopyFrom)
                //        {
                //            stringLibrary.Pings[username].UnionWith(stringLibrary.Pings[args]);
                //        }
                //    }
                //    break;

                case "info":
                    if (stringLibrary.Pings.ContainsKey(username))
                    {
                        var discordUser = e.Message.Author;
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("Your pings: ");
                        foreach (string ping in stringLibrary.Pings[username])
                        {
                            stringBuilder.Append("[" + ping + "] ");
                        }
                        await Program.DMUser(discordUser, stringBuilder.ToString().Trim());
                    }
                    else if (!stringLibrary.Pings.ContainsKey(username))
                    {
                        await e.RespondAsync("You have no pings saved :(");
                    }
                    break;
            }
            stringLibrary.SaveLibrary(StringLibrary.LibraryType.Ping);
        }

        public static HashSet<ulong> GetPingedUsers(MessageCreateEventArgs e, StringLibrary stringLibrary)
        {
            string message = e.Message.Content.ToLower();
            HashSet<ulong> pinged_users = stringLibrary.Pings.Where(kvp => kvp.Value.Any(value => Regex.IsMatch(message, @"^.*\b" + value + @"\b.*$"))).Select(kvp => kvp.Key).ToHashSet(); // what the fuck, but it works
            return pinged_users;
        }

        public static async Task SendPings(MessageCreateEventArgs e, StringLibrary stringLibrary)
        {
            DiscordChannel channel = await Program.GetDiscordClient.GetChannelAsync(Settings.Default.ping_channel_id); //get channel from channel id

            if (!string.IsNullOrWhiteSpace(e.Message.Content) && e.Message.ChannelId != Settings.Default.ping_channel_id)
            {
                HashSet<ulong> pinged_users = GetPingedUsers(e, stringLibrary);
                string mentions = string.Empty;
                DiscordMember member;

                foreach (ulong user in pinged_users) // loop and get mention strings
                {
                    if (user != e.Message.Author.Id)
                    {
                        member = e.Guild.Members.Where(mem => mem.Id.Equals(user)).FirstOrDefault();
                        if (member != null)
                            mentions += member.Mention + " ";
                    }
                }

                if (!string.IsNullOrWhiteSpace(mentions))
                {
                    string author_nickname = ((DiscordMember)e.Message.Author).DisplayName;
                    if (author_nickname == null)
                        author_nickname = e.Message.Author.Username;
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(mentions + "at " + e.Message.Channel.Mention + "\n" + "<" + author_nickname + "> " + e.Message.Content);
                    await Program.GetDiscordClient.SendMessageAsync(channel, stringBuilder.ToString());
                }
            }
        }
    }
}