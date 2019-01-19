using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SekiDiscord.Commands {

    internal class PingUser {

        public static async Task PingControl(CommandContext e, StringLibrary stringLibrary, string cmd, string args) {
            string username = e.Member.Username.ToLower(); // get message creators username in lower case
            switch (cmd) {
                case "add":
                    if (!string.IsNullOrWhiteSpace(args)) {
                        if (!stringLibrary.Pings.ContainsKey(username)) {
                            stringLibrary.Pings.Add(username, new HashSet<string>() { args });
                        }
                        else if (stringLibrary.Pings.ContainsKey(username)) {
                            stringLibrary.Pings[username].Add(args);
                        }
                    }
                    break;

                case "remove":
                    if (!string.IsNullOrWhiteSpace(args)) {
                        if (stringLibrary.Pings.ContainsKey(username)) {
                            stringLibrary.Pings[username].Remove(args);
                        }
                    }
                    break;

                case "copy":
                    if (!string.IsNullOrWhiteSpace(args)) {
                        bool user = stringLibrary.Pings.ContainsKey(username);
                        bool userToCopyFrom = stringLibrary.Pings.ContainsKey(args);
                        if (!user && userToCopyFrom) {
                            stringLibrary.Pings.Add(username, stringLibrary.Pings[args]);
                        }
                        else if (user && userToCopyFrom) {
                            stringLibrary.Pings[username].UnionWith(stringLibrary.Pings[args]);
                        }
                    }
                    break;

                case "info":
                    if (stringLibrary.Pings.ContainsKey(username)) {
                        var discordUser = e.Message.Author;
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.Append("Your pings: ");
                        foreach (string ping in stringLibrary.Pings[username]) {
                            stringBuilder.Append(ping + " ");
                        }
                        await Program.DMUser(discordUser, stringBuilder.ToString());
                    }
                    else if (!stringLibrary.Pings.ContainsKey(username)) {
                        await e.RespondAsync("You have no pings saved :(");
                    }
                    break;
            }
            stringLibrary.SaveLibrary("pings");
        }

        public static HashSet<string> Ping(MessageCreateEventArgs e, StringLibrary stringLibrary) {
            string message = e.Message.Content.ToLower();
            HashSet<string> pinged_users = stringLibrary.Pings.Where(kvp => kvp.Value.Any(value => Regex.IsMatch(message, @"^.*" + value + ".*$"))).Select(kvp => kvp.Key).ToHashSet(); // what the fuck, but it works
            return pinged_users;
        }
    }
}