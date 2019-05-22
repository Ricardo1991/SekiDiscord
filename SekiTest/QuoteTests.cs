using SekiDiscord;
using Xunit;

namespace SekiTest
{
    public class QuoteTests
    {
        [Fact]
        public void AddQuoteWithAdd()
        {
            StringLibrary stringLibrary = new StringLibrary();
            int quoteSize = stringLibrary.Quotes.Count;

            SekiDiscord.Commands.Quotes.AddQuote("add <Me> Quote with 'add'", stringLibrary);

            Assert.True(string.Compare(SekiDiscord.Commands.Quotes.PrintQuote("#" + (quoteSize + 1), stringLibrary), "<Me> Quote with 'add'") == 0);
        }

        [Fact]
        public void AddQuoteWithoutAdd()
        {
            StringLibrary stringLibrary = new StringLibrary();
            int quoteSize = stringLibrary.Quotes.Count;

            SekiDiscord.Commands.Quotes.AddQuote("<Me> Second quote", stringLibrary);

            Assert.True(string.Compare(SekiDiscord.Commands.Quotes.PrintQuote("#" + (quoteSize + 1), stringLibrary), "<Me> Second quote") == 0);
        }
    }
}