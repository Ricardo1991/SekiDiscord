using System;
using System.Collections.Generic;
using System.IO;

namespace SekiDiscord.Commands
{
    internal class Funk
    {
        public static List<string> FunkList { get; set; }

        static Funk()
        {
            FunkList = FileHandler.LoadStringListFromFile(FileHandler.StringListFileType.Funk);
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

            SaveFunk(FunkList);
        }

        public static void SaveFunk(List<string> funk)
        {
            using StreamWriter newTask = new StreamWriter(FileHandler.FileEnumToString(FileHandler.StringListFileType.Funk), false);
            foreach (string q in funk)
            {
                newTask.WriteLine(q);
            }
        }
    }
}