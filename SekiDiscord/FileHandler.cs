using System;
using System.Collections.Generic;
using System.IO;

namespace SekiDiscord {
    public static class FileHandler {
        private static readonly Logger logger = new Logger(typeof(FileHandler));

        public enum StringListFileType
        {
            Facts,
            Fortune,
            Funk,
            Kills,
            NickGen,
            Quotes,
            Status,
            Trivia
        }

        public static List<string> LoadStringListFromFile(StringListFileType fileType)
        {
            List<string> loadList = new List<string>();
            string filePath = FileEnumToString(fileType);

            if (!File.Exists(filePath))
            {
                logger.Error("File \""+ filePath + "\" not found");
                throw new FileNotFoundException("The file was not found", filePath);
            }

            try
            {
                using StreamReader sr = new StreamReader("TextFiles/status.txt");
                while (sr.Peek() >= 0)
                {
                    loadList.Add(sr.ReadLine());
                }
            }
            catch (IOException e)
            {
                logger.Error("Failed to read status." + e.Message);
            }

            return loadList;
        }

        public static string FileEnumToString(StringListFileType fileType)
        {
            switch (fileType)
            {
                case StringListFileType.Facts:
                    return "TextFiles/facts.txt";

                case StringListFileType.Fortune:
                    return "TextFiles/fortune.txt";

                case StringListFileType.Funk:
                    return "TextFiles/funk.txt";

                case StringListFileType.Kills:
                    return "TextFiles/kills.txt";

                case StringListFileType.NickGen:
                    return "TextFiles/nickGen.txt";

                case StringListFileType.Quotes:
                    return "TextFiles/quotes.txt";

                case StringListFileType.Status:
                    return "TextFiles/status.txt";

                case StringListFileType.Trivia:
                    return "TextFiles/trivia.txt";

                default: break;
            }
            throw new Exception("Undefined fileType");
        }
    }
}