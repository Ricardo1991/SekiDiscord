using SekiDiscord;
using Xunit;

namespace SekiTest
{
    public class QuoteTests
    {
        private StringLibrary stringLibrary;

        public QuoteTests()
        {
            stringLibrary = new StringLibrary();
        }

        [Fact]
        public void AddQuoteWithAdd()
        {
            int quoteSize = stringLibrary.Quotes.Count;

            SekiDiscord.Commands.Quotes.AddQuote("add <Me> Quote with 'add'", stringLibrary);

            Assert.Equal("<Me> Quote with 'add'", SekiDiscord.Commands.Quotes.PrintQuote("#" + (quoteSize + 1), stringLibrary));
        }

        [Fact]
        public void AddQuoteWithoutAdd()
        {
            int quoteSize = stringLibrary.Quotes.Count;

            SekiDiscord.Commands.Quotes.AddQuote("<Me> Second quote", stringLibrary);

            Assert.Equal("<Me> Second quote", SekiDiscord.Commands.Quotes.PrintQuote("#" + (quoteSize + 1), stringLibrary));
        }
    }
}