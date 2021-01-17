using System;
using System.Collections.Generic;
using System.IO;

namespace SekiDiscord {
    public static class FileHandler {
        private static readonly Logger logger = new Logger(typeof(FileHandler));

        public static List<string> GetStatusList() {
            List<string> status = new List<string>();

            if (File.Exists("TextFiles/status.txt")) {
                try {
                    using (StreamReader sr = new StreamReader("TextFiles/status.txt")) {
                        while (sr.Peek() >= 0) {
                            status.Add(sr.ReadLine());
                        }
                    }
                }
                catch (IOException e) {
                    logger.Error("Failed to read status." + e.Message);
                }
            }

            if (status.Count == 0) {
                throw new IOException("No status on file");
            }
            return status;
        }
    }
}
