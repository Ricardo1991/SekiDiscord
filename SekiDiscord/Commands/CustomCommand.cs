using System;
using System.Collections.Generic;
using System.IO;

namespace SekiDiscord.Commands
{
    public class CustomCommand
    {
        private static readonly Logger logger = new Logger(typeof(CustomCommand));
        
        private const string customCommandFilePath = "TextFiles/customCommands.txt";

        public static List<CustomCommand> CustomCommands { get; set; }

        public string Name { get; set; }

        public string Format { get; set; }

        public string Author { get; set; }

        static CustomCommand()
        {
            CustomCommands = LoadCustomCommands();
        }

        public CustomCommand(string author, string name, string format)
        {
            Name = name;
            Format = format;
            Author = author;
        }

        public static List<CustomCommand> LoadCustomCommands()
        {
            List<CustomCommand> command = new List<CustomCommand>();

            if (File.Exists(customCommandFilePath))
            {
                try
                {
                    StreamReader sr = new StreamReader(customCommandFilePath);
                    while (sr.Peek() >= 0)
                    {
                        string line = sr.ReadLine();
                        string[] splitLine = line.Split(new char[] { ' ' }, 3);

                        command.Add(new CustomCommand(splitLine[0], splitLine[1], splitLine[2]));
                    }
                    sr.Close();
                }
                catch (IndexOutOfRangeException)
                {
                    logger.Error("Not enough arguments on a custom command, while loading file");
                }
            }

            return command;
        }

        internal static string UseCustomCommand(string command, string arguments, string nick, List<string> listU)
        {
            string response;
            CustomCommand customCommand;

            if (CommandExists(command, CustomCommands) == false)
            {
                //message = new Privmsg(CHANNEL, "Command " + cmd + " doesn't exist.");
                //sendMessage(message);

                return string.Empty;
            }

            customCommand = GetCustomCommandByName(command, CustomCommands);

            if (customCommand == null)
            {
                return string.Empty;
            }

            response = customCommand.Format;

            response = Useful.FillTags(response, nick.Trim(), arguments, listU);
            return response;
        }

        public static void SaveCustomCommands(List<CustomCommand> commands)
        {
            using StreamWriter newTask = new StreamWriter(customCommandFilePath, false);
            foreach (CustomCommand q in commands)
            {
                newTask.WriteLine(q.Author + " " + q.Name + " " + q.Format);
            }
        }

        public static bool CommandExists(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (string.Compare(q.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            }

            return false;
        }

        public static CustomCommand GetCustomCommandByName(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (string.Compare(q.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return q;
            }

            return null;
        }

        public static void RemoveCommandByName(string name, List<CustomCommand> commands)
        {
            foreach (CustomCommand q in commands)
            {
                if (string.Compare(q.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    commands.Remove(GetCustomCommandByName(name, commands));
                    return;
                }
            }
        }
    }
}