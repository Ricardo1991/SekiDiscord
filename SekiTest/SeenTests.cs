using Xunit;

namespace SekiTest
{
    public class SeenTests
    {
        [Fact]
        public void UserSeenTest()
        {
            SekiDiscord.Commands.Seen.MarkUserSeen("testUser");
            Assert.StartsWith("The user testUser was last seen", SekiDiscord.Commands.Seen.CheckSeen("testUser"));
        }

        [Fact]
        public void UserNotSeenTest()
        {
            Assert.Equal("The user nothere has not been seen yet, or an error has occured", SekiDiscord.Commands.Seen.CheckSeen("nothere"));
        }
    }
}