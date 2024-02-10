using System;
using Architecture.Pages;
using Architecture.Statistics;
using HomeTools.Handling;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.StatisticsArea.ArchivedFlows
{
    // Component of archived task handler
    public class TaskHandler : IBehaviorSync
    {
        // Handle component
        private readonly HandleObject handleObject;
        // Animation component of image of task
        private readonly RectTransformSync rectTransformSync;
        // Invoke action when touched
        private readonly Action actionTouched;
        // Archived tasks view scroll component
        private StatisticsScroll statisticsScroll;
        
        private float touchProcess; // Timer of touch time to invoke action
        private const float touchSpeed = 0.02f; // Speed of timer
        private bool touched; // Touched flag
        private bool accessToOpen; // Can open task view flag

        /// <summary>
        /// When the finger moves vertically more
        /// than this distance, then do not allow opening the task view window
        /// </summary>
        private const float maxPressedDistance = 10f; 
        // Current pressed X distance
        private float pressedDistance;

        // Create and setup
        public TaskHandler(GameObject handler, RectTransform image, Action actionTouched)
        {
            // Save touched action
            this.actionTouched = actionTouched;
            
            // Create handle component
            handleObject = new HandleObject(handler);
            AddScrollEventsCustom.AddEventActions(handler);
            handleObject.AddEvent(EventTriggerType.PointerDown, TouchDown);
            handleObject.AddEvent(EventTriggerType.PointerUp, TouchUp);
            
            // Create animation component of rect
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(image);
            rectTransformSync.TargetPosition = image.anchoredPosition + new Vector2(0, 17);
            rectTransformSync.Speed = 0.01f;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync);
        }

        // Add statistics scroll events to handler
        public void AddStatisticsEvents(StatisticsScroll scroll)
        {
            statisticsScroll = scroll;
            handleObject.AddEvent(EventTriggerType.PointerDown, statisticsScroll.TouchDown);
            handleObject.AddEvent(EventTriggerType.PointerUp, statisticsScroll.TouchUp);
        }
        
        public void Start() { }

        // Updates each frame
        public void Update()
        {
            CheckPressedDistance();
            TouchProcess();
            SleepProcess();
        }

        // Call when touched
        private void TouchDown()
        {
            if (!statisticsScroll.CheckSleepMotion())
                return;
            
            touched = true;
            pressedDistance = 0f;
            accessToOpen = true;
        }

        // Call when touched up
        private void TouchUp()
        {
            if (!touched)
                return;
            
            touched = false;
            
            if (accessToOpen)
                actionTouched.Invoke();
        }

        // Process to open view when pressed
        private void TouchProcess()
        {
            if (!touched)
                return;
            
            if (touchProcess <= 1)
            {
                touchProcess += touchSpeed;

                if (touchProcess < 0.3f) 
                    return;
                
                var t = (touchProcess - 0.3f) / (1 - 0.3f);
                rectTransformSync.SetT(1 - t);

                return;
            }
            
            TouchUp();
        }
        
        private void SleepProcess()
        {
            if (touched)
                return;

            if (touchProcess > 0)
                touchProcess -= touchSpeed * 10;
            
            rectTransformSync.SetT(1 - touchProcess);
        }

        // Collect distance to set off access to open task view
        private void CheckPressedDistance()
        {
            if (!touched)
                return;

            var delta = InputHS.DeltaMove;
            pressedDistance += Mathf.Abs(delta.x) + Mathf.Abs(delta.y);

            if (pressedDistance < maxPressedDistance) 
                return;
            
            accessToOpen = false;
            TouchUp();
        }
    }
}
