﻿using System;
using System.Collections.Generic;

namespace SekiDiscord.Commands
{
    public static class Quotes
    {
        public static void AddQuote(string args, StringLibrary stringLibrary)
        {
            string add;

            if (stringLibrary.Quotes == null)
                stringLibrary.Quotes = new List<string>();

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "add", true) == 0)
                add = Useful.GetBetween(args, "add ", null);
            else
                add = args;

            if (!string.IsNullOrWhiteSpace(add))
                stringLibrary.Quotes.Add(add);

            stringLibrary.SaveLibrary(StringLibrary.LibraryType.Quote);
        }

        public static string PrintQuote(string args, StringLibrary stringLibrary)
        {
            Random r = new Random();

            if (stringLibrary.Quotes.Count == 0)
                return string.Empty;

            //print random
            if (string.IsNullOrWhiteSpace(args))
            {
                return PrintRandomQuote(stringLibrary);
            }
            //Print quote by number
            else if (args.StartsWith("#"))
            {
                string split = args.Split(new char[] { ' ' }, 2)[0];
                int number = 0;
                string message;

                try
                {
                    number = Convert.ToInt32(split.Replace("#", string.Empty));

                    if (number <= stringLibrary.Quotes.Count && number > 0)
                        message = stringLibrary.Quotes[number - 1];
                    else
                        message = "Quote number " + number + " does not exist";
                }
                catch
                {
                    message = "Invalid input";
                }
                return message;
            }
            //search
            else
            {
                string[] queries = args.Trim().ToLower().Split(' ');
                List<string> restults = SearchQuotes(queries, stringLibrary);
                string message = string.Empty;

                if (restults.Count > 0)
                {
                    if (restults.Count > 1)
                    {
                        message = "Found " + restults.Count + " quotes. Showing one of them:\n";
                    }
                    message += restults[r.Next(restults.Count)];

                    return message;
                }
                else
                {
                    message = "No Quotes Found!";
                    return message;
                }
            }
        }

        private static List<string> SearchQuotes(string[] queries, StringLibrary stringLibrary)
        {
            List<string> results = new List<string>();

            foreach (string quote in stringLibrary.Quotes)
            {
                bool add = true;
                foreach (string query in queries)
                {
                    if (!quote.ToLower().Contains(query))
                    {
                        add = false;
                    }
                }
                if (add)
                    results.Add(quote);
            }

            return results;
        }

        private static string PrintRandomQuote(StringLibrary stringLibrary)
        {
            Random r = new Random();
            string message;
            int i = r.Next(stringLibrary.Quotes.Count);
            message = stringLibrary.Quotes[i];

            return message;
        }

        public static string QuoteCount(StringLibrary stringLibrary)
        {
            string message = stringLibrary.Quotes.Count.ToString();
            return message;
        }
    }
}