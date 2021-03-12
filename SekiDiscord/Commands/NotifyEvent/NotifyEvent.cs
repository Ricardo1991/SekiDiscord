using System;
using System.Collections.Generic;

namespace SekiDiscord.Commands.NotifyEvent
{
    internal class NotifyEvent
    {
        private string name;
        private DateTime eventStart;
        private TimeSpan repeatPeriod;

        private bool enabled = false;
        private HashSet<ulong> eventSubscribers = new();

        public NotifyEvent(string name, DateTime eventStart, TimeSpan repeatPeriod)
        {
            this.eventStart = eventStart;
            this.repeatPeriod = repeatPeriod;
            this.name = name;
        }

        public bool Enabled { get => enabled; set => enabled = value; }

        /// <summary>
        /// Subscribe a user to the event
        /// Returns true if the user was added, and false if it already existed.
        /// </summary>
        /// <param name="userID">Discord ID of user to subscribe</param>
        /// <returns></returns>
        public bool SubscribeUser(ulong userID)
        {
            return eventSubscribers.Add(userID);
        }

        /// <summary>
        /// Unsubscribe a user from this event
        /// Returns true if the user was found and removed, and false if it was not.
        /// </summary>
        /// <param name="userID">Discord ID of user to unsubscribe</param>
        /// <returns></returns>
        public bool UnsubscribeUser(ulong userID)
        {
            bool removeSuccessful = eventSubscribers.Remove(userID);

            if (eventSubscribers.Count == 0)
                Enabled = false;

            return removeSuccessful;
        }
    }
}