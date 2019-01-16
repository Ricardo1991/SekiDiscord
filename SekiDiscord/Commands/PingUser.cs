using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SekiDiscord.Commands
{
    internal class PingUser
    {
        public static void AddPing(CommandContext e, StringLibrary stringLibrary)
        {
            string msg = e.Message.Content.ToLower();
            string add;
            string args;

            try
            {
                args = msg.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                return;
            }

            if (stringLibrary.Pings == null)
                stringLibrary.Pings = new Dictionary<string, HashSet<string>>();

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "add", true) == 0)
                add = Useful.GetBetween(args, "add ", null);
            else
                add = args;

            if (!string.IsNullOrWhiteSpace(add))
            {
                string username = e.Member.Username.ToLower();
                if (!stringLibrary.Pings.ContainsKey(username))
                {
                    Console.WriteLine("doesnt exists");

                    stringLibrary.Pings.Add(username, new HashSet<string>() { add });
                }
                else if (stringLibrary.Pings.ContainsKey(username))
                {
                    Console.WriteLine("exists");

                    stringLibrary.Pings[username].Add(add);
                }
                else
                {
                    return;
                }

                foreach (KeyValuePair<string, HashSet<string>> pair in stringLibrary.Pings)
                {
                    foreach (string s in pair.Value)
                    {
                        Console.WriteLine("{0}, {1}", pair.Key, s);
                    }
                }
            }

            stringLibrary.SaveLibrary("pings");
        }

        public static void RemovePing(CommandContext e, StringLibrary stringLibrary)
        {
            string msg = e.Message.Content.ToLower();
            string remove;
            string args;

            try
            {
                args = msg.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                return;
            }

            if (stringLibrary.Pings == null)
                return;

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "remove", true) == 0)
                remove = Useful.GetBetween(args, "remove ", null);
            else
                remove = args;

            if (!string.IsNullOrWhiteSpace(remove))
            {
                string username = e.Member.Username.ToLower();
                if (stringLibrary.Pings.ContainsKey(username))
                {
                    stringLibrary.Pings[username].Remove(remove);
                }
                else
                {
                    return;
                }

                foreach (KeyValuePair<string, HashSet<string>> pair in stringLibrary.Pings)
                {
                    foreach (string s in pair.Value)
                    {
                        Console.WriteLine("{0}, {1}", pair.Key, s);
                    }
                }
            }

            stringLibrary.SaveLibrary("pings");
        }

        public static void CopyPing(CommandContext e, StringLibrary stringLibrary)
        {
            string msg = e.Message.Content.ToLower();
            string copy;
            string args;

            try
            {
                args = msg.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                return;
            }

            if (stringLibrary.Pings == null)
                return;

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "copy", true) == 0)
                copy = Useful.GetBetween(args, "copy ", null);
            else
                copy = args;

            if (!string.IsNullOrWhiteSpace(copy))
            {
                string username = e.Member.Username.ToLower();
                bool requester = stringLibrary.Pings.ContainsKey(username);
                bool source = stringLibrary.Pings.ContainsKey(copy);
                if (!requester && source)
                {
                    stringLibrary.Pings.Add(username, stringLibrary.Pings[copy]);
                }
                else if (requester && source)
                {
                    stringLibrary.Pings[username].UnionWith(stringLibrary.Pings[copy]);
                }
                else
                {
                    return;
                }

                foreach (KeyValuePair<string, HashSet<string>> pair in stringLibrary.Pings)
                {
                    foreach (string s in pair.Value)
                    {
                        Console.WriteLine("{0}, {1}", pair.Key, s);
                    }
                }
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