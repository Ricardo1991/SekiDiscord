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
            FunkList.Add(content);
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