using System;
using HomeTools.Handling;
using HomeTools.Messenger;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.TrackArea
{
    // Scroll of track task progress area
    public class TrackAreaScroll : IBehaviorSync
    {
        // Scroll states
        private enum ScrollState
        {
            toShow,
            toHide,
            shown,
            hidden,
            handle
        }
        
        // Function for convert delta screen to canvas delta
        private readonly Func<Vector2, Vector2> screenConverter;

        // Move parameters of scroll
        private const float joinSpeed = 0.25f;
        private const float clampDelta = 0.001f;
        private const float hideMultiplierBorder = 0.5f;
        private const float tWhenNeedHide = 0.6f;
        private const float showOutOfBorder = 0.1f;
        /// <summary>
        /// Imaginary scroll position for interpolation for containers for numbers,
        /// which changes to float - tTarget
        /// </summary>
        private float tPosition;
        // Imaginary scroll position for interpolation for containers for numbers
        private float tTarget;

        // Convert interpolation position to delta canvas position
        private readonly float tPerDelta;
        // Current position
        private ScrollState currentState;

        // Dark screen component
        private readonly ScreenBlackout screenBlackout;
        // Main part component
        private readonly MainPart mainPart;
        // Handle component
        private readonly HandleObject trackHandle;
        // Motion detector component
        private readonly HandleMotionSystem swipeDetect;

        // Create nad setup
        public TrackAreaScroll(ScreenBlackout screenBlackout, MainPart mainPart)
        {
            // Save components
            this.screenBlackout = screenBlackout;
            this.mainPart = mainPart;
            
            MainMessenger.AddMember(AutoHideByBlackout, AppMessagesConst.BlackoutTrackClicked);
            // Save params for move
            screenConverter = MainCanvas.ScreenToCanvasDelta;
            tPerDelta = 1f / MainPart.RectOutOfSideDistance;
            
            // Create and setup handle component
            trackHandle = new HandleObject(mainPart.RectTransform.Find("Handle Area").gameObject);
            trackHandle.AddEvent(EventTriggerType.PointerDown, TouchDown);
            trackHandle.AddEvent(EventTriggerType.PointerUp, TouchUp);

            // Create detect swipes component
            swipeDetect = new HandleMotionSystem();
            swipeDetect.SetParameters(0.5f, 40, 50);
            swipeDetect.MotionType = HandleMotionSystem.Motion.swipeDown;
            swipeDetect.ResultAction = AutoHideByHandle;
        }
        
        // Set start state
        public void Start()
        {
            tPosition = 0;
            tTarget = 0;
            currentState = ScrollState.hidden;
        }
        
        // Updates each frame
        public void Update()
        {
            ToShowProcessAuto();
            ToHideProcessAuto();
            MoveByHandleProcess();
        }
        
        // Set auto open state
        public void AutoOpen()
        {
            currentState = ScrollState.toShow;
            tTarget = 1;
        }

        // Set auto open close
        public void AutoClose()
        {
            currentState = ScrollState.toHide;
            tTarget = 0;
        }
        
        // Set auto hide by blackout touched
        private void AutoHideByBlackout()
        {
            if (currentState == ScrollState.handle)
                return;
            
            currentState = ScrollState.toHide;
            tTarget = 0;
        }
        
        // Set auto hide by handle
        private void AutoHideByHandle()
        {
            currentState = ScrollState.toHide;
            tTarget = 0;
        }
        
        // Touch down method
        private void TouchDown()
        {
            if (currentState == ScrollState.hidden)
                return;
            
            currentState = ScrollState.handle;
            swipeDetect.TouchDown();
        }
        
        // Touch up method
        private void TouchUp()
        {
            if (tPosition < tWhenNeedHide) { AutoHideByHandle(); }
            else { AutoOpen(); }
            swipeDetect.TouchUp();
        }

        // Move area by pressed state
        private void MoveByHandleProcess()
        {
            if (currentState != ScrollState.handle)
                return;

            var delta = screenConverter.Invoke(InputHS.DeltaMove);

            swipeDetect.AddMoveDelta(delta);
            
            if (tTarget > 1f && delta.y > 0)
                delta *= 1 - (tTarget - 1f) / showOutOfBorder;

            tTarget += delta.y * tPerDelta;
            tTarget = Mathf.Clamp(tTarget, 0, 1 + showOutOfBorder);
            tPosition = Mathf.Lerp(tPosition, tTarget, joinSpeed * 2.5f);

            screenBlackout.SetState(tPosition);
            mainPart.SetupPositionByT(tPosition);
        }
        
        // Setup movable params by state of scroll - to show auto
        private void ToShowProcessAuto()
        {
            if (currentState != ScrollState.toShow)
                return;
            
            if (Mathf.Abs(tPosition - tTarget) < clampDelta)
                currentState = ScrollState.shown;

            if (currentState == ScrollState.shown)
                tPosition = tTarget;
            
            tPosition = Mathf.Lerp(tPosition, tTarget, joinSpeed);
            
            screenBlackout.SetState(tPosition);
            mainPart.SetupPositionByT(tPosition);
        }
        
        // Setup movable params by state of scroll - to hide auto
        private void ToHideProcessAuto()
        {
            if (currentState != ScrollState.toHide)
                return;

            if (Mathf.Abs(tPosition - tTarget) < clampDelta)
            {
                currentState = ScrollState.hidden;
                MainMessenger.SendMessage(AppMessagesConst.TrackerHidden);
            }

            if (currentState == ScrollState.hidden)
                tPosition = tTarget;

            var localSpeed = joinSpeed;
            if (tPosition < hideMultiplierBorder)
            {
                var multiplier = Mathf.Abs((tPosition - hideMultiplierBorder) / hideMultiplierBorder);
                localSpeed += localSpeed * multiplier * 3;
            }
            
            tPosition = Mathf.Lerp(tPosition, tTarget, localSpeed);
            
            screenBlackout.SetState(tPosition);
            mainPart.SetupPositionByT(tPosition);
        }
    }
}
