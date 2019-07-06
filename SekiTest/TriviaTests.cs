using SekiDiscord.Commands;
using Xunit;

namespace SekiTest
{
    public class TriviaTests
    {
        [Fact]
        public void LoadTriviaTest()
        {
            Assert.True(Trivia.TriviaCount() > 0);
        }

        [Fact]
        public void GetTriviaTest()
        {
            Assert.True(!string.IsNullOrWhiteSpace(Trivia.GetTrivia()));
        }
    }
}