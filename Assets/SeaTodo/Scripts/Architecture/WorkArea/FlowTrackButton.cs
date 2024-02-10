using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TrackArea;
using HomeTools.Handling;
using HomeTools.Messenger;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.WorkArea
{
    // Track button of task item
    public class FlowTrackButton : IBehaviorSync
    {
        // Link to track task progress page
        private TrackArea.TrackArea TrackArea => AreasLocator.Instance.TrackArea;
        // Link to calendar view
        private static Calendar WorkCalendar => AreasLocator.Instance.WorkArea.WorkCalendar;
        // Additional component for task item behaviour
        private readonly FlowTrackButtonLogic flowTrackButtonLogic;
        private readonly WorkFlowItem workFlowItem; // Task item 
        // Handle component
        private readonly HandleObject handleObject;
        // Animation component of button rect
        private readonly RectTransformSync rectTransformSync;
        // Speed of animation
        private const float scaleTransformSpeed = 0.4f;
        
        private Flow flowData; // Task

        // Animation params
        private const float processSpeed = 0.025f;
        private const float processSleep = 0.7f;
        
        // Button states
        public bool Touched { get; private set; }
        private bool readyToClick;
        private float trackModeTimer;
        private bool moveModeEnabled;

        // Create and setup
        public FlowTrackButton(RectTransform rectTransform, RectTransform buttonArea, WorkFlowItem workFlowItem)
        {
            // Save component
            this.workFlowItem = workFlowItem;
            // Create additional component for button behaviour
            flowTrackButtonLogic = new FlowTrackButtonLogic(rectTransform);
            
            // Create animation component of button rect
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.Speed = scaleTransformSpeed;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            rectTransformSync.TargetScale = Vector3.one * 1.1f;
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.WorkAreaYear);

            // Create handle component
            handleObject = new HandleObject(buttonArea.gameObject);
            handleObject.AddEvent(EventTriggerType.PointerDown, TouchDown);
            handleObject.AddEvent(EventTriggerType.PointerUp, TouchUp);
            handleObject.AddEvent(EventTriggerType.PointerExit, TouchExit);
            handleObject.AddEvent(EventTriggerType.PointerClick, TouchClick);
            
            MainMessenger<(int,int)>.AddMember(TrackedOutOfButton, AppMessagesConst.TrackedInStatisticArea);
        }

        // Update button by new task
        public void UpdateFlowData(Flow flow)
        {
            flowData = flow;
            flowTrackButtonLogic.UpdateFlowData(flow);
            flowTrackButtonLogic.UpdateTrackedDay();
        }

        // Setup move button mode when task moves
        public void SetMoveMode()
        {
            flowTrackButtonLogic.SetMoveMode();
            moveModeEnabled = true;
        }
        
        // Setup default button mode when task placed
        public void SetDefaultMode()
        {
            if (!Touched)
                flowTrackButtonLogic.SetDefaultMode();
            moveModeEnabled = false;
        }

        // Touch down method
        private void TouchDown()
        {
            if (moveModeEnabled)
                return;
            
            trackModeTimer = 0;
            Touched = true;
            readyToClick = false;
            rectTransformSync.Stop();
        }

        // Touch up method
        private void TouchUp()
        {
            if (!Touched || moveModeEnabled)
                return;

            if (ScrollHandler.AccessByScroll)
                readyToClick = true;
            else
                FinishTransition(false);
        }

        // Touch exit method
        private void TouchExit()
        {
            if (!Touched || moveModeEnabled)
                return;
            
            FinishTransition(false);
        }
        
        // Touch click method
        private void TouchClick()
        {
            if (!readyToClick)
                return;
            
            FinishTransition(true);
        }
        
        public void Start() { }

        // Updates every frame
        public void Update()
        {
            flowTrackButtonLogic.Update();
            
            UpdateButtonScale();
            CatchResetByScroll();
        }

        // Update button scale
        private void UpdateButtonScale()
        {
            if (!Touched)
                return;
            
            trackModeTimer += processSpeed;

            var sleep = Mathf.Clamp((trackModeTimer - processSleep) / (1 - processSleep), 0f, 1f);

            rectTransformSync.SetT(1 - sleep);
            rectTransformSync.SetDefaultT(1 - sleep);

            if (trackModeTimer < 1)
                return;

            if (workFlowItem.Touched)
            {
                FinishTransition(false);
                return;
            }
            
            FinishTransition(true);
            Vibration.Vibrate(25);
        }

        // Reset touch process when scrolled page
        private void CatchResetByScroll()
        {
            if (!Touched)
                return;

            if (ScrollHandler.AccessByScroll)
                return;
            
            FinishTransition(false);
        }

        // Finish touch process
        private void FinishTransition(bool success)
        {
            trackModeTimer = 0;
            Touched = false;
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();

            if (success)
            {
                flowTrackButtonLogic.SetTrackMode();
                TrackArea.StartNewSession(new TrackSession(flowData, FinishedTrackMode, WorkCalendar.CurrentDay));
            }
        }

        // Finished track progress mode
        private void FinishedTrackMode()
        {
            flowTrackButtonLogic.UpdateTrackedDay();
            workFlowItem.UpdateStats();
            flowTrackButtonLogic.SetDefaultMode();
            moveModeEnabled = false;
        }
        
        // Get message when task tracked in other place
        private void TrackedOutOfButton((int flowId, int day) info)
        {
            if (info.day != WorkCalendar.CurrentDay.HomeDayToInt() || flowData.Id != info.flowId)
                return;
            
            flowTrackButtonLogic.UpdateTrackedDay();
            workFlowItem.UpdateStats();
            flowTrackButtonLogic.SetDefaultModeImmediately();
            moveModeEnabled = false;
        }
    }
}
