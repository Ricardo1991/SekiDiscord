using Xunit;

namespace SekiTest
{
    public class PingTests
    {
        private static ulong userID = 123456;
        private static string pingTriggerWord = "testPingWord";

        [Fact]
        public void PingAddAndDetectAndRemove()
        {
            SekiDiscord.Commands.PingUser.PingControlAdd(userID, pingTriggerWord);
            Assert.Contains(userID, SekiDiscord.Commands.PingUser.GetPingedUsers("Hello " + pingTriggerWord));
            SekiDiscord.Commands.PingUser.PingControlRemove(userID, pingTriggerWord);
            Assert.DoesNotContain(userID, SekiDiscord.Commands.PingUser.GetPingedUsers("Hello " + pingTriggerWord));

        }
    }
}
