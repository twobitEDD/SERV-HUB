using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HomeTools.Messenger
{
    // Component for collect actions of handling
    public static class EventTriggerHandling
    {
        // Message collector
        private static readonly Dictionary<int, Action> messengerCollector;
        private static int globalId; // For generate uniq id

        // Create and setup
        static EventTriggerHandling()
        {
            messengerCollector = new Dictionary<int, Action>();
        }

        // Add action to Event trigger by id and Messages collector
        public static int AddEventTrigger(GameObject gameObject, EventTriggerType eventType, Action action)
        {
            var id = ++globalId;
            AddUnityEvents<int>.AddPointer(gameObject, eventType, SendMessage, id);
            AddMember(action, id);

            return id;
        }

        // Add action to Message collector with uniq id
        private static void AddMember(Action member, int message) => messengerCollector.Add(message, member);

        // Remove action from Message collector with uniq id
        public static void RemoveMember(int message)
        {
            if (!messengerCollector.ContainsKey(message))
                return;

            messengerCollector.Remove(message);
        }

        // Invoke action by id
        private static void SendMessage(int message)
        {
            if (!messengerCollector.ContainsKey(message))
                return;
            
            messengerCollector[message].Invoke();
        }
    }
}
