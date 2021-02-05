using System;
using Xunit;

namespace SekiTest
{
    public class FortuneTests
    {
        private static ulong userID = 123456;

        [Fact]
        public void SameFortuneSameDay()
        {
            string fortune1 = SekiDiscord.Commands.Fortune.GetFortune(userID);
            string fortune2 = SekiDiscord.Commands.Fortune.GetFortune(userID);
            Assert.Equal(fortune1, fortune2);
        }

        [Fact]
        public void DifferentFortuneDifferentDay()
        {
            string fortune1 = SekiDiscord.Commands.Fortune.GetFortune(DateTime.Today, userID);
            string fortune2 = SekiDiscord.Commands.Fortune.GetFortune(DateTime.Today.Add(new TimeSpan(1, 0, 0, 0)), userID);
            Assert.NotEqual(fortune1, fortune2);
        }

        [Fact]
        public void DifferentFortuneDifferentUser()
        {
            string fortune1 = SekiDiscord.Commands.Fortune.GetFortune(userID);
            string fortune2 = SekiDiscord.Commands.Fortune.GetFortune(userID + 1);
            Assert.NotEqual(fortune1, fortune2);
        }
    }
}
