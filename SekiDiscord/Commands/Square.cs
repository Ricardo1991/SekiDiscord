using DSharpPlus.CommandsNext;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SekiDiscord.Commands
{
    internal class Square
    {
        public static async Task SquareText(CommandContext ctx)
        {
            Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + "Square Command");
            int MAX_TEXT = 10;

            string text;
            string user = ctx.Member.DisplayName;

            try
            {
                text = ctx.Message.Content.Split(new char[] { ' ' }, 2)[1];
            }
            catch
            {
                text = string.Empty;
            }

            if (text.Length > MAX_TEXT)
            {
                string message = "_farts on " + user + "_";
                await ctx.RespondAsync(message);
                return;
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i <= text.Length - 1; i++)
                {
                    if (i == 0)
                    {
                        builder.Append("```");
                        foreach (char value in text.ToCharArray())
                        {
                            builder.Append(value + " ");
                        }
                        builder.Append("\n");
                    }
                    else if (i == text.Length - 1)
                    {
                        foreach (char value in text.ToCharArray().Reverse())
                        {
                            builder.Append(value + " ");
                        }
                        builder.Append("```");
                    }
                    else
                    {
                        builder.Append(text[i] + new string(' ', text.Length + (text.Length - 3)) + text[text.Length - 1 - i] + "\n");
                    }
                }
                string msg = builder.ToString();
                string message = msg.ToUpper();
                await ctx.RespondAsync(message);
            }
        }
    }
}