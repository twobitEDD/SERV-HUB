using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HomeTools.Messenger
{
    // Class for add events to Event Trigger component
    public static class AddUnityEvents<T>
    {
        // Add action with int
        public static void AddPointer(GameObject gameObject, EventTriggerType pointerType, Action<T> action, T eventData)
        {
            // Try get Event Trigger component
            var eventTrigger = gameObject.GetComponent<EventTrigger>();
        
            // Check if should add
            if (eventTrigger == null)
                eventTrigger = gameObject.AddComponent<EventTrigger>();
            
            // Add action to event
            eventTrigger.triggers.Add(new EventTrigger.Entry 
            {
                eventID = pointerType, 
                callback = CreateTrigger(action, eventData)
            });
        }

        // Add action with int to Event Trigger
        public static void AddPointer(EventTrigger eventTrigger, EventTriggerType pointerType, Action<T> action, T eventData)
        {
            eventTrigger.triggers.Add(new EventTrigger.Entry 
            {
                eventID = pointerType, 
                callback = CreateTrigger(action, eventData)
            });
        }

        // Add Trigger action
        private static EventTrigger.TriggerEvent CreateTrigger(Action<T> action, T eventData)
        {
            var touchDownCallback = new Action<T>(action);
            var trigger = new EventTrigger.TriggerEvent();
            trigger.AddListener((call) => touchDownCallback(eventData));

            return trigger;
        }
    }
}
