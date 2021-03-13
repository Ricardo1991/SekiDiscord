using System;
using SekiDiscord.Commands.NotifyEvent;
using Xunit;

namespace SekiTest
{
    public class NotifyEventTests
    {

        [Fact]
        public void CalculateAlarm15Minutes()
        {
            DateTime eventStart = DateTime.Now.Subtract(new TimeSpan(6,15,0));
            TimeSpan repeatPeriod = new TimeSpan(0,45,0);

            Assert.Equal(15, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateAlarm0Minutes()
        {
            DateTime eventStart = DateTime.Now.Subtract(new TimeSpan(6, 0, 0));
            TimeSpan repeatPeriod = new TimeSpan(0, 45, 0);

            Assert.Equal(0, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateAlarmNow()
        {
            DateTime eventStart = DateTime.Now;
            TimeSpan repeatPeriod = new TimeSpan(0, 45, 0);

            Assert.Equal(0, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateAlarmFutureFar()
        {
            DateTime eventStart = DateTime.Now.AddHours(1);
            TimeSpan repeatPeriod = new TimeSpan(0, 45, 0);

            Assert.Equal(105, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }
        [Fact]
        public void CalculateAlarmFutureSoon()
        {
            DateTime eventStart = DateTime.Now.AddMinutes(5);
            TimeSpan repeatPeriod = new TimeSpan(0, 45, 0);

            Assert.Equal(50, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }

        [Fact]
        public void ParseUserInput()
        {
            //Example for  event starting at 6am utc, repeating every day
            string userInput = "Genshin Impact Daily login; 13 March 2021, 06:00AM; 1:0:0:0";
            DateTime date = new DateTime(2021, 03, 13, 6, 0, 0, DateTimeKind.Utc);
            TimeSpan span = new TimeSpan(1, 0, 0, 0);

            (string, DateTime, TimeSpan) result = NotifyEventManager.InputArgumentToEventData(userInput);
            Assert.Equal("Genshin Impact Daily login", result.Item1);
            Assert.Equal(date, result.Item2);
            Assert.Equal(span, result.Item3);
        }
    }
}
