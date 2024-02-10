using HomeTools.Messenger;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.Pages
{
    // Class for adding scroll events for other objects
    public static class AddScrollEventsCustom
    {
        public static void AddEventActions(GameObject gameObject)
        {
            var eventTrigger = gameObject.GetComponent<EventTrigger>();
        
            if (eventTrigger == null)
                eventTrigger = gameObject.AddComponent<EventTrigger>();
        
            AddUnityEvents<BaseEventData>.AddPointer(eventTrigger, EventTriggerType.PointerDown,
                PageScroll.Instance.PointerDown, new BaseEventData(EventSystem.current));
        
            AddUnityEvents<BaseEventData>.AddPointer(eventTrigger, EventTriggerType.PointerUp,
                PageScroll.Instance.PointerUp, new BaseEventData(EventSystem.current));
        
            AddUnityEvents<BaseEventData>.AddPointer(eventTrigger, EventTriggerType.PointerEnter,
                PageScroll.Instance.PointerEnter, new BaseEventData(EventSystem.current));
        
            AddUnityEvents<BaseEventData>.AddPointer(eventTrigger, EventTriggerType.PointerExit,
                PageScroll.Instance.PointerExit, new BaseEventData(EventSystem.current));
        }
    }
}
