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