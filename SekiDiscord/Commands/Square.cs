using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SekiDiscord.Commands
{
    public class Square
    {
        public const int MAX_TEXT = 10;

        public static string SquareText(string text, string name)
        {
            if (text == null || text.Length > MAX_TEXT)
            {
                return "_farts on " + name + '_';
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i <= text.Length - 1; i++)
                {
                    if (text.Length == 1) // for single character cases
                    {
                        builder.Append("```" + text + "```");
                    }
                    else if (i == 0)
                    {
                        builder.Append("```");
                        foreach (char value in text.ToCharArray())
                        {
                            builder.Append(value + ' ');
                        }
                        builder.Append('\n');
                    }
                    else if (i == text.Length - 1)
                    {
                        foreach (char value in text.ToCharArray().Reverse())
                        {
                            builder.Append(value + ' ');
                        }
                        builder.Append("```");
                    }
                    else
                    {
                        builder.Append(text[i] + new string(' ', text.Length + (text.Length - 3)) + text[text.Length - 1 - i] + '\n');
                    }
                }
                string msg = builder.ToString();
                return msg.ToUpper(CultureInfo.CreateSpecificCulture("en-GB"));
            }
        }
    }
}