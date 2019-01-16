using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SekiDiscord.Commands {
    internal class PingUser {
        public static void AddPing(CommandContext e, StringLibrary stringLibrary) {
            string add;
            string args;

            try {
                args = e.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch {
                return;
            }

            if (stringLibrary.Pings == null)
                stringLibrary.Pings = new Dictionary<string, HashSet<string>>();

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "add", true) == 0)
                add = Useful.GetBetween(args, "add ", null);
            else
                add = args;

            if (!string.IsNullOrWhiteSpace(add)) {
                stringLibrary.Pings[e.Member.Username].Add(add);
            }

            stringLibrary.SaveLibrary("pings");
        }

        public static void RemovePing(CommandContext e, StringLibrary stringLibrary) {
            string remove;
            string args;

            try {
                args = e.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch {
                return;
            }

            if (stringLibrary.Pings == null)
                return;

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "remove", true) == 0)
                remove = Useful.GetBetween(args, "remove ", null);
            else
                remove = args;

            if (!string.IsNullOrWhiteSpace(remove)) {
                stringLibrary.Pings[e.Member.Username].Remove(remove);
            }

            stringLibrary.SaveLibrary("pings");
        }

        public static void CopyPing(CommandContext e, StringLibrary stringLibrary) {
            string copy;
            string args;

            try {
                args = e.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch {
                return;
            }

            if (stringLibrary.Pings == null)
                return;

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "copy", true) == 0)
                copy = Useful.GetBetween(args, "copy ", null);
            else
                copy = args;

            if (!string.IsNullOrWhiteSpace(copy)) {
                stringLibrary.Pings[e.Member.Username].UnionWith(stringLibrary.Pings[stringLibrary.Pings.FirstOrDefault(x => x.Key.Contains(copy)).Key]);
            }

            stringLibrary.SaveLibrary("pings");
        }

        public static void Ping(CommandContext e, StringLibrary stringLibrary) {
            string msg = e.Message.Content;

            string[] split_msg = Regex.Split(msg, @"\W");

            HashSet<string> pinged_users = stringLibrary.Pings.Where(kvp => split_msg.Any(kvp.Value.Contains)).Select(kvp => kvp.Key).ToHashSet(); // what the fuck, but it works


        }
    }
}
