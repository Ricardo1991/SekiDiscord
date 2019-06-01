using SekiDiscord.Commands;
using Xunit;

namespace SekiTest
{
    public class SquareTests
    {
        [Fact]
        public void TestSquareThreeChars()
        {
            string result = Square.SquareText("Boo", "Ricardo");
            Assert.Equal("```B O O \nO   O\nO O B ```", result);
        }

        [Fact]
        public void TestSquareOneChar()
        {
            string result = Square.SquareText("B", "Ricardo");
            Assert.Equal("```B```", result);
        }

        [Fact]
        public void TestSquareManyChars()
        {
            string result = Square.SquareText(new string('B', Square.MAX_TEXT + 1), "Ricardo");
            Assert.Equal("_farts on Ricardo_", result);
        }
    }
}