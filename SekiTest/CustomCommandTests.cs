using Xunit;
using SekiDiscord.Commands;
using System.Collections.Generic;

namespace SekiTest
{
    public class CustomCommandTests
    {
        private static string author = "Rakope";
        private static string commandName = "tarr";
        private static string commandContent = "~~~<target> <random>!";

        private static string argument = "boo";
        private static string randomUser = "userboo";
        private static List<string> userList = new List<string> {randomUser};

        [Fact]
        public void AddAndUseCommand()
        {
            Assert.False(CustomCommand.CommandExists(commandName));
            CustomCommand.CustomCommands.Add(new CustomCommand(author,commandName, commandContent));
            Assert.True(CustomCommand.CommandExists(commandName));

            string result = CustomCommand.UseCustomCommand(commandName, argument, author, userList);
            Assert.Equal(result, "~~~" + argument + " " + randomUser+"!");

        }
    }
}
