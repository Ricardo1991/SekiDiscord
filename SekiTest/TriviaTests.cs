using SekiDiscord.Commands;
using Xunit;

namespace SekiTest
{
    public class TriviaTests
    {
        [Fact]
        public void LoadTriviaTest()
        {
            Trivia.TriviaList = Trivia.ReadTrivia();
            Assert.True(Trivia.TriviaList.Count > 0);
        }

        [Fact]
        public void GetTriviaTest()
        {
            Trivia.TriviaList = Trivia.ReadTrivia();
            Assert.True(!string.IsNullOrWhiteSpace(Trivia.GetTrivia()));
        }
    }
}