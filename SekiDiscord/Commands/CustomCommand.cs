using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SekiDiscord.Commands
{
    public class CustomCommand
    {
        public static List<CustomCommand> CustomCommands { get; set; }
        private string name;
        private string format;
        private string author;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Format
        {
            get
            {
                return format;
            }

            set
            {
                format = value;
            }
        }

        public string Author
        {
            get
            {
                return author;
            }

            set
            {
                author = value;
            }
        }

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

            if (File.Exists("TextFiles/customCommands.txt"))
            {
                try
                {
                    StreamReader sr = new StreamReader("TextFiles/customCommands.txt");
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
                    Console.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ", CultureInfo.CreateSpecificCulture("en-GB")) + "Not enough arguments on a custom command, while loading file");
                }
            }
            else
            {
                //TODO: Settings
                //Settings.Default.help_Enabled = false;
                //Settings.Default.Save();
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
            using StreamWriter newTask = new StreamWriter("TextFiles/customCommands.txt", false);
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