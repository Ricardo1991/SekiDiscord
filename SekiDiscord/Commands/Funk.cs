using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SekiDiscord.Commands
{
    internal class Funk
    {
        public static List<string> FunkList { get; set; }

        static Funk()
        {
            FunkList = new List<string>();
        }

        public static string PrintFunk()
        {
            if (FunkList.Count == 0)
                return string.Empty;

            Random r = new Random();
            int i = r.Next(FunkList.Count);

            return FunkList[i];
        }

        public static void AddFunk(string content)
        {
            string args;
            try
            {
                args = content.Split(new char[] { ' ' }, 2)[1];
            }
            catch (IndexOutOfRangeException)
            {
                return;
            }

            string[] splits = content.Split();
            if (string.Compare(splits[0], "add", StringComparison.OrdinalIgnoreCase) == 0)
                args = args.Replace("add ", string.Empty, StringComparison.OrdinalIgnoreCase);

            FunkList.Add(args);

            StringLibrary.SaveLibrary(StringLibrary.LibraryType.Funk);
        }

        public static List<string> ReadFunk()
        {
            List<string> funk = new List<string>();

            if (File.Exists("TextFiles/funk.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/funk.txt");
                    while (sr.Peek() >= 0)
                    {
                        funk.Add(sr.ReadLine());
                    }
                    sr.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Failed to read funk. " + e.Message);
                }
            }
            return funk;
        }

        public static void SaveFunk(List<string> funk)
        {
            using (StreamWriter newTask = new StreamWriter("TextFiles/Funk.txt", false))
            {
                foreach (string q in funk)
                {
                    newTask.WriteLine(q);
                }
            }
        }
    }
}