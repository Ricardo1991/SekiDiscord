using SekiDiscord;
using System;
using System.Collections.Generic;
using Xunit;

namespace SekiTest
{
    public class UsefulTests
    {
        [Fact]
        public void FillTagsTest()
        {
            string killString = "<user> kills <target>. <USER> in all caps screams to <TARGET> in all caps and winks at <random>";
            string expected = "Ric kills Death. RIC in all caps screams to DEATH in all caps and winks at Proper";

            string author = "Ric";
            string target = "Death";
            string randomTarget = "Proper";

            List<string> usersOnline = new List<string>
            {
                randomTarget,
                target
            };

            string result = Useful.FillTags(killString, author.Trim(), target, usersOnline);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestGetBetweenWithEnd()
        {
            string testString = "Hello my friend.";

            string result = Useful.GetBetween(testString, "Hello", ".");

            Assert.Equal(" my friend", result);
        }

        [Fact]
        public void TestGetBetweenNoEnd()
        {
            string testString = "Hello my friend.";

            string result = Useful.GetBetween(testString, "Hello", null);

            Assert.Equal(" my friend.", result);
        }

        [Fact]
        public void TestGetBetweenNoMatch()
        {
            string testString = "Hello my friend.";

            Assert.Throws<ArgumentException>(() => Useful.GetBetween(testString, "hello", null));
        }
        [Fact]
        public void TestTimeSpanFormatter() {

            DateTime test1 = DateTime.UtcNow;
            DateTime test2 = test1.AddDays(1).AddMinutes(30);
            TimeSpan span = test2 - test1;
            
            Assert.Equal("1 days, 0h30", Useful.TimeSpanFormatter(span));


            test1 = DateTime.UtcNow;
            test2 = test1.AddHours(1).AddMinutes(30);
            span = test2 - test1;

            Assert.Equal("1h30", Useful.TimeSpanFormatter(span));

            test1 = DateTime.UtcNow;
            test2 = test1.AddHours(11).AddMinutes(30);
            span = test2 - test1;

            Assert.Equal("11h30", Useful.TimeSpanFormatter(span));

            test1 = DateTime.UtcNow;
            test2 = test1.AddHours(25).AddMinutes(30);
            span = test2 - test1;

            Assert.Equal("1 days, 1h30", Useful.TimeSpanFormatter(span));

            //zero
            test1 = DateTime.UtcNow;
            test2 = test1;
            span = test2 - test1;

            Assert.Equal("0h00", Useful.TimeSpanFormatter(span));
        }
    }
}