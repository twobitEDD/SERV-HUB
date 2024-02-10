using Architecture.Elements;
using Architecture.Pages;
using HomeTools.Handling;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.ModuleReminders
{
    // Handle component for day item of reminders view
    public class ReminderHandlePart : IBehaviorSync
    {
        // Link to class that is responsible for the reminders window
        private static RemindersModule RemindersModule => AreasLocator.Instance.RemindersModule;
        
        // Handle component for item
        private readonly HandleObject handleObject;
        // Handle component for reminder time
        private readonly HandleObject handleObjectText;

        // Link to day item
        private readonly DayView dayView;
        // State of day item
        private bool activeState;

        private bool touched; // Touched flag
        private bool touchedText; // Touched reminder text flag
        private bool readyToClick; // Ready to click flag

        // Timer of auto click when touched
        private const int PressedTimerSample = 37;
        // Current timer of auto click when touched
        private int pressedTimer;

        // Create and setup
        public ReminderHandlePart(GameObject handle, GameObject handleTime, DayView dayView)
        {
            this.dayView = dayView;
            
            // Create handle component for item
            handleObject = new HandleObject(handle);
            handleObject.AddEvent(EventTriggerType.PointerDown, TouchDown);
            handleObject.AddEvent(EventTriggerType.PointerUp, TouchUp);
            handleObject.AddEvent(EventTriggerType.PointerClick, TouchClick);
            
            // Create handle component for reminder text in item
            handleObjectText = new HandleObject(handleTime);
            handleObjectText.AddEvent(EventTriggerType.PointerDown, TouchDown);
            handleObjectText.AddEvent(EventTriggerType.PointerDown, TouchDownText);
            handleObjectText.AddEvent(EventTriggerType.PointerUp, TouchUp);
            handleObjectText.AddEvent(EventTriggerType.PointerClick, TouchClick);
            
            // Add scroll events to reminder text
            AddScrollEventsCustom.AddEventActions(handleTime);
        }

        // Setup default state of handler
        public void SetDefaultState(bool active) => activeState = active;

        public void Start() { }

        public void Update()
        {
            TouchWaitPressed();
        }

        // Wait for auto click when pressed
        private void TouchWaitPressed()
        {
            if (!touched)
                return;

            if (pressedTimer > 0)
            {
                pressedTimer--;
                return;
            }

            TouchedFinished(true);
            touched = false;
        }
        
        // Call the method when click on handler
        private void TouchedFinished(bool vibrate = false)
        {
            // Check scroll motion
            if (!ScrollHandler.AccessByScroll)
                return;
            
            // Check to open track time or activate day reminder item
            if (touchedText && activeState)
                dayView.OpenTimeTracker();
            else
            {
                activeState = !activeState;
                dayView.UpdateActivateState(activeState);
            }
            
            if (vibrate)
                Vibration.Vibrate(25);
            
            // Check permission to receive notifications
            RemindersModule.CurrentSession.CheckToRequest();
        }

        // Touch down method
        private void TouchDown()
        {
            touched = true;
            readyToClick = false;
            pressedTimer = PressedTimerSample;
            touchedText = false;
        }
        
        // Touch down text method
        private void TouchDownText() =>  touchedText = true;

        // Touch up method
        private void TouchUp()
        {
            if (!touched)
                return;

            readyToClick = true;
            touched = false;
        }
        
        // Method call on click
        private void TouchClick()
        {
            if (!readyToClick)
                return;

            TouchedFinished();
        }
    }
}
