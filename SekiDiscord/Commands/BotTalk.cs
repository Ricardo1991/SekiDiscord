using Cleverbot.Net;
using System;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class BotTalk
    {
        static private CleverbotSession cleverbotSession = null;

        public static async Task<string> BotThinkAsync(string input, string botName)
        {
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
                return answer.Response;
            }
            catch
            {
                cleverbotSession = new CleverbotSession(Settings.Default.CleverbotAPI);
                return "Sorry, but i can't think right now";
            }
        }
    }
}