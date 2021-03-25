using SekiDiscord.Commands.NotifyEvent;
using System;
using System.IO;
using Xunit;

namespace SekiTest
{
    public class NotifyEventTests
    {
        public NotifyEventTests()
        {
            NotifyEventManager.LoadAndEnableEvents();
            if (File.Exists(NotifyEventManager.NOTIFY_FILE_PATH))
                File.Delete(NotifyEventManager.NOTIFY_FILE_PATH);
        }

        [Fact]
        public void CalculateAlarm15Minutes()
        {
            DateTime eventStart = DateTime.UtcNow.Subtract(new TimeSpan(3, 30, 0));
            TimeSpan repeatPeriod = new TimeSpan(0, 45, 0);

            Assert.Equal(15, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateAlarm0Minutes()
        {
            DateTime eventStart = DateTime.UtcNow.Subtract(new TimeSpan(3, 0, 0));
            TimeSpan repeatPeriod = new TimeSpan(0, 45, 0);

            Assert.Equal(0, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateAlarmNow()
        {
            DateTime eventStart = DateTime.UtcNow;
            TimeSpan repeatPeriod = new TimeSpan(0, 45, 0);

            Assert.Equal(0, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateAlarmFuture()
        {
            DateTime eventStart = DateTime.UtcNow.AddHours(1);
            TimeSpan repeatPeriod = new TimeSpan(0, 45, 0);

            Assert.Equal(60, NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateAlarmTomorrow()
        {
            DateTime eventStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 2, 00, 00, DateTimeKind.Utc);
            DateTime now = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 23, 00, 00, DateTimeKind.Utc);
            TimeSpan repeatPeriod = new TimeSpan(1, 0, 0, 0);

            Assert.Equal(3 * 60, NotifyEvent.TimeForNextNotification(now, eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateAlarmLaterToday()
        {
            DateTime eventStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 14, 00, 00, DateTimeKind.Utc);
            DateTime now = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 20, 00, DateTimeKind.Utc).AddDays(10);
            TimeSpan repeatPeriod = new TimeSpan(1, 0, 0, 0);

            Assert.Equal(14 * 60 - 20, NotifyEvent.TimeForNextNotification(now, eventStart, repeatPeriod));
        }

        [Fact]
        public void CalculateOldAlarm()
        {
            DateTime eventStart = DateTime.UtcNow.ToUniversalTime().Subtract(new TimeSpan(15, 3, 30, 0, 0)); //Event Start set to 15 days, 3 hours and 30 minutes ago
            TimeSpan repeatPeriod = new TimeSpan(1, 0, 0, 0);

            Assert.Equal(24 * 60 - (3 * 60 + 30), NotifyEvent.TimeForNextNotification(eventStart, repeatPeriod));
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

        [Fact]
        public void NoRepatedEventNames()
        {
            //Example for  event starting at 6am utc, repeating every day
            string userInput = "Genshin3; 13 March 2021, 06:00AM; 1:0:0:0";

            NotifyEventManager.AddEvent(userInput);
            Assert.Throws<ArgumentException>(() => NotifyEventManager.AddEvent(userInput));

            Assert.Equal(1, NotifyEventManager.NotifyEventCount());
        }

        [Fact]
        public void NoRepeatedUserSubscriber()
        {
            //Example for  event starting at 6am utc, repeating every day
            string userInput = "Genshin2; 13 March 2021, 06:00AM; 1:0:0:0";

            NotifyEventManager.AddEvent(userInput);

            ulong userID = 132;
            ulong userGuild = 321;

            NotifyEventManager.SubscribeUserToEvent(userID, userGuild, "Genshin2");
            Assert.False(NotifyEventManager.SubscribeUserToEvent(userID, userGuild, "Genshin2"));
        }

        [Fact]
        public void EventNotFoundError()
        {
            Assert.Throws<NullReferenceException>(() => NotifyEventManager.EnableEvent("Booba"));
        }
    }
}