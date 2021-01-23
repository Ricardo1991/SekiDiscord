using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using SekiDiscord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SekiDiscord.DiscordEvents {
    static class MessageEvents {
        private static readonly Logger logger = new Logger(typeof(MessageEvents));

        public static async Task MessageCreatedEvent(MessageCreateEventArgs a) {
            if (a.Message.Content.StartsWith(Settings.Default.commandChar + "quit", StringComparison.OrdinalIgnoreCase)) {
                await CheckForQuitEvent(a).ConfigureAwait(false);
            } else if (a.Message.Content.StartsWith(Settings.Default.commandChar)) {
                await CheckForCustomCommand(a).ConfigureAwait(false);
            } else if (a.Message.Content.StartsWith(SekiMain.BotName + ",", StringComparison.OrdinalIgnoreCase) || a.Message.Content.EndsWith(SekiMain.BotName, StringComparison.OrdinalIgnoreCase)) {
                await CheckForCleverBot(a).ConfigureAwait(false);
            }

            await Waifunator(a.Message).ConfigureAwait(false);

            // Update "last seen" for user that sent the message
            string username = ((DiscordMember)a.Message.Author).DisplayName;
            Seen.MarkUserSeen(username);

            // Ping users, leave this last cause it's sloooooooow
            await PingUser.SendPings(a).ConfigureAwait(false);
        }

        private static async Task CheckForQuitEvent(MessageCreateEventArgs a) {
            logger.Info("Quit request received, confirming...");

            if (a.Guild == null) {
                logger.Info("Message sent via DM, ignoring.");
                return;
            }

            DiscordMember author = await a.Guild.GetMemberAsync(a.Author.Id).ConfigureAwait(false);
            if (author.IsOwner || Useful.MemberIsBotOperator(author)) {
                logger.Info("Request validated, quitting now...");
                SekiMain.Quit = true;
            }
        }

        private static async Task CheckForCustomCommand(MessageCreateEventArgs a) {
            string[] split = a.Message.Content.Split(new char[] { ' ' }, 2);
            string arguments = split.Length > 1 ? split[1] : string.Empty;
            string senderUsername = Useful.GetUsername(a);

            string result = CustomCommand.UseCustomCommand(split[0].TrimStart(Settings.Default.commandChar), arguments, senderUsername, GetUserList(a.Channel.Guild, senderUsername));
            if (!string.IsNullOrEmpty(result)) {
                await a.Message.RespondAsync(result).ConfigureAwait(false);
            }
        }

        private static async Task CheckForCleverBot(MessageCreateEventArgs a) {
            if (string.IsNullOrWhiteSpace(Settings.Default.CleverbotAPI)) return;

            string input = a.Message.Content;

            //Show the "bot is typing..." message
            await a.Channel.TriggerTypingAsync().ConfigureAwait(false);

            string response = await BotTalk.BotThinkAsync(input, SekiMain.BotName).ConfigureAwait(false);
            await a.Message.RespondAsync(response).ConfigureAwait(false);
        }

        private static List<string> GetUserList(DiscordGuild guild, string senderUsername) {
            if (guild != null) {
                return Useful.GetOnlineNames(guild);
            }
            return new List<string> { senderUsername };
        }

        private static async Task Waifunator(DiscordMessage message) {
            if (!string.IsNullOrWhiteSpace(message.Content) && message.Author.Id.Equals(Settings.Default.limid) && message.Content.Contains("wife", StringComparison.OrdinalIgnoreCase)) {
                await message.CreateReactionAsync(DiscordEmoji.FromName(SekiMain.DiscordClient, ":regional_indicator_w:")).ConfigureAwait(false);
                await message.CreateReactionAsync(DiscordEmoji.FromName(SekiMain.DiscordClient, ":regional_indicator_a:")).ConfigureAwait(false);
                await message.CreateReactionAsync(DiscordEmoji.FromName(SekiMain.DiscordClient, ":regional_indicator_i:")).ConfigureAwait(false);
                await message.CreateReactionAsync(DiscordEmoji.FromName(SekiMain.DiscordClient, ":regional_indicator_f:")).ConfigureAwait(false);
                await message.CreateReactionAsync(DiscordEmoji.FromName(SekiMain.DiscordClient, ":regional_indicator_u:")).ConfigureAwait(false);
            }
        }
    }
}
