﻿using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SekiDiscord.Commands
{
    internal class PingUser
    {
        public static void PingControl(CommandContext e, StringLibrary stringLibrary, string cmd, string args)
        {
            string username = e.Member.Username.ToLower(); // get message creators username in lower case

            switch (cmd)
            {
                case "add":
                    if (!stringLibrary.Pings.ContainsKey(username))
                    {
                        stringLibrary.Pings.Add(username, new HashSet<string>() { args });
                    }
                    else if (stringLibrary.Pings.ContainsKey(username))
                    {
                        stringLibrary.Pings[username].Add(args);
                    }
                    break;

                case "remove":
                    if (stringLibrary.Pings.ContainsKey(username))
                    {
                        stringLibrary.Pings[username].Remove(args);
                    }
                    break;

                case "copy":
                    bool user = stringLibrary.Pings.ContainsKey(username);
                    bool userToCopyFrom = stringLibrary.Pings.ContainsKey(args);
                    if (!user && userToCopyFrom)
                    {
                        stringLibrary.Pings.Add(username, stringLibrary.Pings[args]);
                    }
                    else if (user && userToCopyFrom)
                    {
                        stringLibrary.Pings[username].UnionWith(stringLibrary.Pings[args]);
                    }
                    break;
            }
            stringLibrary.SaveLibrary("pings");
        }

        public static HashSet<string> Ping(MessageCreateEventArgs e, StringLibrary stringLibrary)
        {
            string message = e.Message.Content.ToLower();
            string[] split_msg = Regex.Split(message, @"\W");

            HashSet<string> pinged_users = stringLibrary.Pings.Where(kvp => split_msg.Any(kvp.Value.Contains)).Select(kvp => kvp.Key).ToHashSet(); // what the fuck, but it works
            return pinged_users;
        }
    }
}