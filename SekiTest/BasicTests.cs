using SekiDiscord.Commands;
using System;
using Xunit;

namespace SekiTest
{
    public class BasicTests
    {
        [Fact]
        public void Roll20Test()
        {
            int randomNumber = Basics.Roll("!roll 20");

            //Test with random numbers are bad. Might fail or pass depending on randomness.
            Assert.True(randomNumber > 0 && randomNumber < 21);
        }

        [Fact]
        public void RollBigNumberTest()
        {
            Assert.Throws<OverflowException>(() => Basics.Roll("!roll 1000000000000"));
        }

        [Fact]
        public void RollDefaultTest()
        {
            int randomNumber = Basics.Roll("!roll");

            //Test with random numbers are bad. Might fail or pass depending on randomness.
            Assert.True(randomNumber >= 0 && randomNumber < 100);
        }

        [Fact]
        public void RollTextTest()
        {
            Assert.Throws<FormatException>(() => Basics.Roll("!roll Hello"));
        }
    }
}