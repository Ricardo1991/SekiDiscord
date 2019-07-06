using SekiDiscord.Commands;
using System.Collections.Generic;
using Xunit;

namespace SekiTest
{
    public class FactsTests
    {
        [Fact]
        public void LoadFactsTest()
        {
            Assert.True(Fact.FactCount() > 0);
        }

        [Fact]
        public void PrintFactsTest()
        {
            string author = "Ric";
            string target = "Death";
            string randomTarget = "Proper";

            List<string> usersOnline = new List<string>
            {
                author,
                randomTarget,
                target
            };

            Assert.False(string.IsNullOrWhiteSpace(Fact.ShowFact("Proper", usersOnline, author)));
        }
    }
}