using SekiDiscord.Commands;
using System;
using System.Collections.Generic;
using Xunit;

namespace SekiTest
{
    public class BasicTests
    {
        private static string randomUser = "userboo";
        private static List<string> userList = new List<string> { randomUser };
        private static string author = "Rakope";

        [Fact]
        public void PokeRandomTest()
        {
            string poke = Basics.PokeRandom(userList, author);
            Assert.Equal(poke, "*pokes " + randomUser + "*");
        }

        [Fact]
        public void Roll20Test()
        {
            int randomNumber = Basics.Roll(20);

            //Test with random numbers are bad. Might fail or pass depending on randomness.
            Assert.True(randomNumber > 0 && randomNumber < 21);
        }

        [Fact]
        public void RollManyTestSimple()
        {
            string result = Basics.RollMany("1d1");
            Assert.True(result.Equals("1"));
        }

        [Fact]
        public void RollManyTest()
        {
            string result = Basics.RollMany("2d20");
            string[] resultsSplit = result.Split();
            int firstDice = Int32.Parse(resultsSplit[0]);
            int secondDice = Int32.Parse(resultsSplit[1]);

            Assert.True(firstDice > 0 && firstDice <= 20);
            Assert.True(secondDice > 0 && secondDice <= 20);
        }

        [Fact]
        public void RollDefaultTest()
        {
            int randomNumber = Basics.Roll();

            //Test with random numbers are bad. Might fail or pass depending on randomness.
            Assert.True(randomNumber >= 0 && randomNumber < 100);
        }

        [Fact]
        public void RollNegativeTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Basics.Roll(-10));
        }
    }
}