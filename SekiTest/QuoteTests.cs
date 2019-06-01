using SekiDiscord.Commands;
using Xunit;

namespace SekiTest
{
    public class QuoteTests
    {
        [Fact]
        public void AddQuoteWithAdd()
        {
            int quoteSize = Quotes.QuotesList.Count;

            Quotes.AddQuote("add <Me> Quote with 'add'");

            Assert.Equal("<Me> Quote with 'add'", Quotes.PrintQuote("#" + (quoteSize + 1)));
        }

        [Fact]
        public void AddQuoteWithoutAdd()
        {
            int quoteSize = Quotes.QuotesList.Count;

            Quotes.AddQuote("<Me> Second quote");

            Assert.Equal("<Me> Second quote", Quotes.PrintQuote("#" + (quoteSize + 1)));
        }

        [Fact]
        public void GetQuoteTest()
        {
            Assert.True(!string.IsNullOrWhiteSpace(Quotes.PrintQuote(string.Empty)));
        }
    }
}