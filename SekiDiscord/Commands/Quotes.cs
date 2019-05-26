using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SekiDiscord.Commands
{
    public static class Quotes
    {
        public static List<string> QuotesList { get; set; }

        static Quotes()
        {
            QuotesList = ReadQuotes();
        }

        public static void AddQuote(string args)
        {
            string add;

            if (QuotesList == null)
                QuotesList = new List<string>();

            if (string.Compare(args.Split(new char[] { ' ' }, 2)[0], "add", StringComparison.OrdinalIgnoreCase) == 0)
                add = Useful.GetBetween(args, "add ", null);
            else
                add = args;

            if (!string.IsNullOrWhiteSpace(add))
                QuotesList.Add(add);

            SaveQuotes(QuotesList);
        }

        public static string PrintQuote(string args)
        {
            Random r = new Random();

            if (QuotesList.Count == 0)
                return string.Empty;

            //print random
            if (string.IsNullOrWhiteSpace(args))
            {
                return PrintRandomQuote();
            }
            //Print quote by number
            else if (args.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                string split = args.Split(new char[] { ' ' }, 2)[0];
                string message;

                try
                {
                    int number = Convert.ToInt32(split.TrimStart('#'), CultureInfo.CreateSpecificCulture("en-GB"));

                    if (number <= QuotesList.Count && number > 0)
                        message = QuotesList[number - 1];
                    else
                        message = "Quote number " + number + " does not exist";
                }
                catch (FormatException)
                {
                    return "Invalid input";
                }
                catch (OverflowException)
                {
                    return "Number Overflow";
                }

                return message;
            }
            //search
            else
            {
                string[] queries = args.Trim().ToLower(CultureInfo.CreateSpecificCulture("en-GB")).Split(' ');
                List<string> restults = SearchQuotes(queries);
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

        private static List<string> SearchQuotes(string[] queries)
        {
            List<string> searchResults = new List<string>();

            foreach (string quote in QuotesList)
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

        private static string PrintRandomQuote()
        {
            Random r = new Random();
            int i = r.Next(QuotesList.Count);
            return QuotesList[i];
        }

        public static string QuoteCount()
        {
            return QuotesList.Count.ToString(CultureInfo.CreateSpecificCulture("en-GB"));
        }

        public static List<string> ReadQuotes()
        {
            List<string> quotes = new List<string>();

            if (File.Exists("TextFiles/quotes.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/quotes.txt");
                    while (sr.Peek() >= 0)
                    {
                        quotes.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Failed to read quotes. " + e.Message);
                }
            }
            return quotes;
        }

        public static void SaveQuotes(List<string> quotes)
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/quotes.txt", false))
            {
                foreach (string q in quotes)
                {
                    newTask.WriteLine(q);
                }
            }
        }
    }
}