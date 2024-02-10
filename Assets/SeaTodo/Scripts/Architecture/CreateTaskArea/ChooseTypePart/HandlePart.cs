using Architecture.Pages;
using HomeTools.Handling;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.CreateTaskArea.ChooseTypePart
{
    // Handle part of task type item
    public class HandlePart : IBehaviorSync
    {
        private readonly HandleObject handleObject; // Handle object
        private readonly ChooseTypeView chooseTypeView; // Link to view 
        private readonly MainPart mainPart; // Link to main part of items

        // States of item
        private bool activeState;
        private bool touched;
        private bool touchedText;
        private bool readyToClick;

        // Timer when user touch item
        private const int pressedTimerSample = 37;
        private int pressedTimer; // Current touch timer

        // Create Handle part
        public HandlePart(ChooseTypeView chooseTypeView, MainPart mainPart, GameObject handler)
        {
            // Save main
            this.mainPart = mainPart;
            this.chooseTypeView = chooseTypeView;
            
            // Create handle part of item
            handleObject = new HandleObject(handler);
            handleObject.AddEvent(EventTriggerType.PointerDown, TouchDown);
            handleObject.AddEvent(EventTriggerType.PointerDown, TouchDownText);
            handleObject.AddEvent(EventTriggerType.PointerUp, TouchUp);
            handleObject.AddEvent(EventTriggerType.PointerClick, TouchClick);
            
            // Add scroll events to handler
            AddScrollEventsCustom.AddEventActions(handler);
        }

        // Set default state
        public void SetDefaultState(bool active)
        {
            activeState = active;
        }

        public void Start() { }

        public void Update()
        {
            TouchWaitPressed();
        }

        // Process when touched
        private void TouchWaitPressed()
        {
            if (!touched)
                return;

            if (pressedTimer > 0)
            {
                pressedTimer--;
                return;
            }

            TouchedFinished();
        }
        
        // Finished touch
        private void TouchedFinished()
        {
            // Check if there was a long movement
            if (!ScrollHandler.AccessByScroll)
                return;

            // Open track goal view or activate item
            if (touchedText && activeState)
            {
                chooseTypeView.OpenGoalPanel();
            }
            else
            {
                activeState = !activeState;
                mainPart.SetupCurrentType(chooseTypeView.FlowType);
            }
        }
        
        // When touched text
        private void TouchDownText() => touchedText = true;

        // When touched item
        private void TouchDown()
        {
            touched = true;
            readyToClick = false;
            touchedText = false;
            pressedTimer = pressedTimerSample;
        }

        // When touched up item
        private void TouchUp()
        {
            if (!touched)
                return;

            readyToClick = true;
            touched = false;
        }

        // When clicked
        private void TouchClick()
        {
            if (!readyToClick)
                return;

            TouchedFinished();
        }
        
        public void Dispose()
        {
            SyncWithBehaviour.Instance.RemoveObserver(this);
            handleObject?.Dispose();
        }
    }
}
