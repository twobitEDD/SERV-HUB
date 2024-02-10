using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HomeTools.Messenger
{
    // Component of Messenger
    public static class MainMessenger
    {
        // Dictionary of actions for invoke by message
        private static readonly Dictionary<string, List<Action>> messengerCollector;

        // Create and setup
        static MainMessenger() => messengerCollector = new Dictionary<string, List<Action>>();
        
        // Add action with message
        public static void AddMember(Action member, string message)
        {
            if (!messengerCollector.ContainsKey(message))
                messengerCollector.Add(message, new List<Action>());
            
            messengerCollector[message].Add(member);
        }

        // Remove action from messenger
        public static void RemoveMember(Action member, string message)
        {
            if (!messengerCollector.ContainsKey(message))
                return;

            messengerCollector[message].Remove(member);
        }

        // Invoke action by message
        public static void SendMessage(string message)
        {
            if (message == null)
                return;

            if (!messengerCollector.ContainsKey(message))
                return;

            foreach (var actions in messengerCollector[message])
                actions.Invoke();
        }
    }
    
    // Component of Messenger with params
    public static class MainMessenger<T> where T : struct
    {
        // Dictionary of actions for invoke by message
        private static readonly Dictionary<string, List<Action<T>>> messengerCollector;

        // Create and setup
        static MainMessenger() => messengerCollector = new Dictionary<string, List<Action<T>>>();
        
        // Add action with message
        public static void AddMember(Action<T> member, string message)
        {
            if (!messengerCollector.ContainsKey(message))
                messengerCollector.Add(message, new List<Action<T>>());
            
            messengerCollector[message].Add(member);
        }

        // Remove action from messenger
        public static void RemoveMember(Action<T> member, string message)
        {
            if (!messengerCollector.ContainsKey(message))
                return;

            messengerCollector[message].Remove(member);
        }

        // Invoke action by message
        public static void SendMessage(string message, T parameter)
        {
            if (!messengerCollector.ContainsKey(message))
                return;

            foreach (var actions in messengerCollector[message])
                actions.Invoke(parameter);
        }
    }
}
