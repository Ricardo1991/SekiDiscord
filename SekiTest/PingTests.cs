using Xunit;

namespace SekiTest
{
    public class PingTests
    {
        private static ulong userID = 123456;
        private static string pingTriggerWord = "testPingWord";

        [Fact]
        public async System.Threading.Tasks.Task PingAddAndDetectAndRemove()
        {
            await SekiDiscord.Commands.PingUser.PingControl(userID, null, "add", pingTriggerWord);
            Assert.Contains(userID, SekiDiscord.Commands.PingUser.GetPingedUsers("Hello " + pingTriggerWord));
            await SekiDiscord.Commands.PingUser.PingControl(userID, null, "remove", pingTriggerWord);
            Assert.DoesNotContain(userID, SekiDiscord.Commands.PingUser.GetPingedUsers("Hello " + pingTriggerWord));

        }
    }
}
