using System;
using Xunit;
using SekiDiscord;

namespace SekiTest
{
    public class QuoteTests
    {
        [Fact]
        public void AddQuote()
        {
            StringLibrary stringLibrary = new StringLibrary();
            int quoteSize = stringLibrary.Quotes.Count;

            SekiDiscord.Commands.Quotes.AddQuote("add <Me> Quote with 'add'", stringLibrary);

            Assert.True(quoteSize + 1 == stringLibrary.Quotes.Count);

            SekiDiscord.Commands.Quotes.AddQuote("<Me> Second quote", stringLibrary);

            Assert.True(quoteSize + 2 == stringLibrary.Quotes.Count);
        }
    }
}