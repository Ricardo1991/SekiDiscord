﻿using DSharpPlus.Entities;
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
    public class PingUser
    {
        private static readonly Logger logger = new Logger(typeof(PingUser));
        private const string PINGS_FILE_PATH = "TextFiles/pings.json";

        public static Dictionary<ulong, HashSet<string>> Pings { get; set; }

        static PingUser()
        {
            Pings = ReadPings();
        }

        public static void PingControlAdd(ulong userNameID, string args)
        {
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
        }

        public static void PingControlRemove(ulong userNameID, string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
            {
                if (Pings.ContainsKey(userNameID))
                {
                    Pings[userNameID].Remove(args);
                }
            }
        }

        public static async Task PingControlInfo(DiscordMember member)
        {
            if (Pings.ContainsKey(member.Id))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("Your pings: ");
                foreach (string ping in Pings[member.Id])
                {
                    stringBuilder.Append("[" + ping + "] ");
                }
                await DMUser(member, stringBuilder.ToString().Trim()).ConfigureAwait(false);
            }
            else if (!Pings.ContainsKey(member.Id))
            {
                await DMUser(member, "You have no pings saved").ConfigureAwait(false);
            }
        }


        public static HashSet<ulong> GetPingedUsers(string message)
        {
            HashSet<ulong> pinged_users = Pings.Where(kvp => kvp.Value.Any(value => Regex.IsMatch(message, @"^.*\b" + value + @"\b.*$"))).Select(kvp => kvp.Key).ToHashSet(); // what the fuck, but it works
            return pinged_users;
        }

        public static async Task SendPings(MessageCreateEventArgs e)
        {
            DiscordChannel channel = await SekiMain.DiscordClient.GetChannelAsync(Settings.Default.ping_channel_id).ConfigureAwait(false); // get channel from channel id

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
                        member = e.Guild.Members.Values.Where(mem => mem.Id.Equals(user)).FirstOrDefault();
                        if (member != null)
                            mentions += member.Mention + " ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(mentions))
                {
                    // Update "last seen" for user that sent the message
                    if (e.Guild.Members.TryGetValue(e.Message.Author.Id, out DiscordMember discordMember))
                    {
                        string author_nickname = discordMember.DisplayName;
                        if (author_nickname == null)
                            author_nickname = e.Message.Author.Username;
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append(mentions + "at " + e.Message.Channel.Mention + "\n" + "<" + author_nickname + "> " + e.Message.Content);
                        await SekiMain.DiscordClient.SendMessageAsync(channel, stringBuilder.ToString()).ConfigureAwait(false);
                    }
                }
            }
        }

        public static async Task DMUser(DiscordMember discordMember, string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                await discordMember.SendMessageAsync(msg).ConfigureAwait(false);
            }
        }

        public static Dictionary<ulong, HashSet<string>> ReadPings()
        {
            Dictionary<ulong, HashSet<string>> ping = new Dictionary<ulong, HashSet<string>>();

            if (File.Exists(PINGS_FILE_PATH))
            {
                try
                {
                    using StreamReader r = new StreamReader(PINGS_FILE_PATH);
                    string json = r.ReadToEnd();
                    ping = JsonConvert.DeserializeObject<Dictionary<ulong, HashSet<string>>>(json);
                }
                catch (JsonException e)
                {
                    logger.Error("COULD NOT READ PINGS: " + e.Message);
                }
            }
            return ping;
        }

        public static void SavePings(Dictionary<ulong, HashSet<string>> ping)
        {
            try
            {
                using StreamWriter w = File.CreateText(PINGS_FILE_PATH);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(w, ping);
            }
            catch (JsonException e)
            {
                logger.Error("COULD NOT SAVE PINGS: " + e.Message);
            }
        }
    }
}