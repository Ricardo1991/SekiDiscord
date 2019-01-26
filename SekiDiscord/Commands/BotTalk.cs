﻿using Cleverbot.Net;
using DSharpPlus.EventArgs;
using System;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class BotTalk
    {
        static private CleverbotSession cleverbotSession = null;

        public static async Task BotThink(MessageCreateEventArgs e, StringLibrary stringLibrary, string botName)
        {
            string message;

            if (string.IsNullOrWhiteSpace(Settings.Default.CleverbotAPI))
                return;

            string input = e.Message.Content;

            //Remove bot name from message input
            if (input.StartsWith(botName, StringComparison.OrdinalIgnoreCase))
            {
                input = input.Substring(input.IndexOf(',') + 1).Trim();
            }
            else if (input.TrimEnd().EndsWith(botName, StringComparison.OrdinalIgnoreCase))
            {
                input = input.Substring(0, input.LastIndexOf(botName, StringComparison.OrdinalIgnoreCase)).Trim();
            }

            try
            {
                if (cleverbotSession == null)
                    cleverbotSession = new CleverbotSession(Settings.Default.CleverbotAPI);

                CleverbotResponse answer = await cleverbotSession.GetResponseAsync(input);
                message = answer.Response;
            }
            catch
            {
                message = "Sorry, but i can't think right now";
                cleverbotSession = new CleverbotSession(Settings.Default.CleverbotAPI);
            }

            await e.Message.RespondAsync(message);
        }
    }
}