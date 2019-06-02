using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class PingUser
    {
        public static Dictionary<ulong, HashSet<string>> Pings { get; set; }

        static PingUser()
        {
            Pings = ReadPings();
        }

        public static async Task PingControl(ulong userNameID, DiscordUser discordUser, string cmd, string args)
        {
            switch (cmd)
            {
                case "add":
                    if (!string.IsNullOrWhiteSpace(args))
                    {
                        if (!Pings.ContainsKey(userNameID))
                        {
                            Pings.Add(userNameID, new HashSet<string>() { args });
                        }
                        else if (Pings.ContainsKey(userNameID))
                        {
                            Pings[userNameID].Add(args);
                        }
                    }
                    break;

                case "remove":
                    if (!string.IsNullOrWhiteSpace(args))
                    {
                        if (Pings.ContainsKey(userNameID))
                        {
                            Pings[userNameID].Remove(args);
                        }
                    }
                    break;

                //case "copy":
                //    if (!string.IsNullOrWhiteSpace(args))
                //    {
                //        bool user = Pings.ContainsKey(username);
                //        bool userToCopyFrom = Pings.ContainsKey(args);
                //        if (!user && userToCopyFrom)
                //        {
                //              Pings.Add(username, Pings[args]);
                //        }
                //        else if (user && userToCopyFrom)
                //        {
                //              Pings[username].UnionWith(Pings[args]);
                //        }
                //    }
                //    break;

                case "info":
                    if (Pings.ContainsKey(userNameID))
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("Your pings: ");
                        foreach (string ping in Pings[userNameID])
                        {
                            stringBuilder.Append("[" + ping + "] ");
                        }
                        await Program.DMUser(discordUser, stringBuilder.ToString().Trim()).ConfigureAwait(false);
                    }
                    else if (!Pings.ContainsKey(userNameID))
                    {
                        await Program.DMUser(discordUser, "You have no pings saved").ConfigureAwait(false);
                    }
                    break;
            }
        }

        public static HashSet<ulong> GetPingedUsers(string message)
        {
            HashSet<ulong> pinged_users = Pings.Where(kvp => kvp.Value.Any(value => Regex.IsMatch(message, @"^.*\b" + value + @"\b.*$"))).Select(kvp => kvp.Key).ToHashSet(); // what the fuck, but it works
            return pinged_users;
        }

        public static async Task SendPings(MessageCreateEventArgs e)
        {
            DiscordChannel channel = await Program.GetDiscordClient.GetChannelAsync(Settings.Default.ping_channel_id).ConfigureAwait(false); //get channel from channel id

            if (!string.IsNullOrWhiteSpace(e.Message.Content) && e.Message.ChannelId != Settings.Default.ping_channel_id)
            {
                string message = e.Message.Content.ToLower(CultureInfo.CreateSpecificCulture("en-GB"));
                HashSet<ulong> pinged_users = GetPingedUsers(message);
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
                    await Program.GetDiscordClient.SendMessageAsync(channel, stringBuilder.ToString()).ConfigureAwait(false);
                }
            }
        }

        public static Dictionary<ulong, HashSet<string>> ReadPings()
        {
            Dictionary<ulong, HashSet<string>> ping = new Dictionary<ulong, HashSet<string>>();

            if (File.Exists("TextFiles/pings.json"))
            {
                try
                {
                    using (StreamReader r = new StreamReader("TextFiles/pings.json"))
                    {
                        string json = r.ReadToEnd();
                        ping = JsonConvert.DeserializeObject<Dictionary<ulong, HashSet<string>>>(json);
                    }
                }
                catch (JsonException)
                {
                }
            }

            return ping;
        }

        public static void SavePings(Dictionary<ulong, HashSet<string>> ping)
        {
            try
            {
                using (StreamWriter w = File.CreateText("TextFiles/pings.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(w, ping);
                }
            }
            catch (JsonException)
            {
            }
        }
    }
}