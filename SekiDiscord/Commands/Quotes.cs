using System;
using System.Collections.Generic;
using System.Globalization;

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
            else if (args.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                string split = args.Split(new char[] { ' ' }, 2)[0];
                string message;

                try
                {
                    int number = Convert.ToInt32(split.TrimStart('#'), CultureInfo.CreateSpecificCulture("en-GB"));

                    if (number <= stringLibrary.Quotes.Count && number > 0)
                        message = stringLibrary.Quotes[number - 1];
                    else
                        message = "Quote number " + number + " does not exist";
                }
                catch
                {
                    return "Invalid input";
                }
                return message;
            }
            //search
            else
            {
                string[] queries = args.Trim().ToLower(CultureInfo.CreateSpecificCulture("en-GB")).Split(' ');
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
            List<string> searchResults = new List<string>();

            foreach (string quote in stringLibrary.Quotes)
            {
                bool addToResults = true;
                foreach (string query in queries)
                {
                    if (!quote.ToLower(CultureInfo.CreateSpecificCulture("en-GB")).Contains(query, StringComparison.OrdinalIgnoreCase))
                    {
                        addToResults = false;
                    }
                }
                if (addToResults)
                {
                    searchResults.Add(quote);
                }
            }

            return searchResults;
        }

        private static string PrintRandomQuote(StringLibrary stringLibrary)
        {
            Random r = new Random();
            int i = r.Next(stringLibrary.Quotes.Count);
            return stringLibrary.Quotes[i];
        }

        public static string QuoteCount(StringLibrary stringLibrary)
        {
            return stringLibrary.Quotes.Count.ToString(CultureInfo.CreateSpecificCulture("en-GB"));
        }
    }
}