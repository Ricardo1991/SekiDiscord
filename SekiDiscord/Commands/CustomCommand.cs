using System;
using System.Collections.Generic;
using System.IO;

namespace SekiDiscord.Commands
{
    public class CustomCommand
    {
        private static readonly Logger logger = new Logger(typeof(CustomCommand));
        
        private const string CUSTOM_COMMANDS_FILE_PATH = "TextFiles/customCommands.txt";

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

            if (File.Exists(CUSTOM_COMMANDS_FILE_PATH))
            {
                try
                {
                    StreamReader sr = new StreamReader(CUSTOM_COMMANDS_FILE_PATH);
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

        public static string UseCustomCommand(string command, string arguments, string nick, List<string> listU)
        {
            string response;
            CustomCommand customCommand;

            if (CommandExists(command) == false)
            {
                //message = new Privmsg(CHANNEL, "Command " + cmd + " doesn't exist.");
                //sendMessage(message);

                return string.Empty;
            }

            customCommand = GetCustomCommandByName(command);

            if (customCommand == null)
            {
                return string.Empty;
            }

            response = customCommand.Format;

            response = Useful.FillTags(response, nick.Trim(), arguments, listU);
            return response;
        }

        public static void SaveCustomCommands()
        {
            using StreamWriter newTask = new StreamWriter(CUSTOM_COMMANDS_FILE_PATH, false);
            foreach (CustomCommand q in CustomCommands)
            {
                newTask.WriteLine(q.Author + " " + q.Name + " " + q.Format);
            }
        }

        public static bool CommandExists(string commandName)
        {
            foreach (CustomCommand q in CustomCommands)
            {
                if (string.Compare(q.Name, commandName, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            }

            return false;
        }

        private static CustomCommand GetCustomCommandByName(string name)
        {
            foreach (CustomCommand q in CustomCommands)
            {
                if (string.Compare(q.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return q;
            }

            return null;
        }

        public static bool RemoveCommandByName(string name)
        {
            foreach (CustomCommand q in CustomCommands)
            {
                if (string.Compare(q.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return CustomCommands.Remove(GetCustomCommandByName(name));
                    
                }
            }
            return false;
        }
    }
}