using System;
using HomeTools.Messenger;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HomeTools.Handling
{
    // Component for add touch events to object (Create Event Trigger and add events)
    public class HandleObject : IDisposable
    {
        private int id; // Id of event trigger
        public GameObject GameObject { get; } // Game Object with Event Trigger
        // Create
        public HandleObject(GameObject gameObject) => GameObject = gameObject;
        // Add events
        public void AddEvent(EventTriggerType eventType, Action action)
        {
            id = EventTriggerHandling.AddEventTrigger(GameObject, eventType, action);
        }
        // Dispose component and remove handling
        public void Dispose()
        {
            EventTriggerHandling.RemoveMember(id);
            UnityEngine.Object.Destroy(GameObject);
        }
    }
}
