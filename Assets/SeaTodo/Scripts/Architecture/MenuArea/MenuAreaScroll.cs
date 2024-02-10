using System;
using Architecture.TrackArea;
using HomeTools.Handling;
using HomeTools.Messenger;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.MenuArea
{
    // Scroll component for dark screen under menu
    public class MenuAreaScroll : IBehaviorSync
    {
        // Scroll states
        public enum ScrollState
        {
            toShow,
            toHide,
            shown,
            hidden,
            handle
        }
        
        // Function of convert screen delta to canvas delta
        private readonly Func<Vector2, Vector2> screenConverter;

        // Move params
        private const float joinSpeed = 0.2f;
        private const float clampDelta = 0.001f;
        private const float hideMultiplierBorder = 0.5f;
        private const float tWhenNeedHide = 0.6f;
        private const float showOutOfBorder = 0.1f;
        
        private float tPosition; // Current interpolation position of menu
        private float tTarget; // Target interpolation position

        private readonly float tPerDelta; // Interpolation amount of one rect delta unit
        
        public ScrollState CurrentState { get; private set; } // Current state

        // Link to dark screen
        private readonly ScreenBlackout screenBlackout;
        // Link to component with buttons
        private readonly MainPart mainPart;
        // Array of button handles
        private HandleObject[] handles;
        // User action detectors
        private readonly HandleMotionSystem swipeDetect;
        private readonly HandleMotionSystem touchDetect;
        // Send message when closed menu
        private string messageWhenClosed = AppMessagesConst.MenuButtonFromClose;

        // Create and setup
        public MenuAreaScroll(ScreenBlackout screenBlackout, MainPart mainPart)
        {
            // Save main components
            this.screenBlackout = screenBlackout;
            this.mainPart = mainPart;

            // Setup main params
            MainMessenger.AddMember(AutoHideByBlackout, AppMessagesConst.BlackoutMenuClicked);
            screenConverter = MainCanvas.ScreenToCanvasDelta;
            tPerDelta = 1f / MainPart.RectOutOfScreenDistance;
            
            // Create user actions detectors
            
            swipeDetect = new HandleMotionSystem();
            swipeDetect.SetParameters(0.5f, 40, 50);
            swipeDetect.MotionType = HandleMotionSystem.Motion.swipeRight;
            swipeDetect.ResultAction = AutoHideByHandle;
            
            touchDetect = new HandleMotionSystem();
            touchDetect.SetParameters(0.5f, 40, 50);
            touchDetect.MotionType = HandleMotionSystem.Motion.touch;
            touchDetect.ResultAction = AutoHideByHandle;
        }

        // Setup buttons handles
        public void SetHandleObjects(HandleObject[] handleObjects)
        {
            handles = handleObjects;
            
            foreach (var handleObject in handleObjects)
            {
                handleObject.AddEvent(EventTriggerType.PointerDown, TouchDown);
                handleObject.AddEvent(EventTriggerType.PointerUp, TouchUp);
            }
        }
        
        // Setup handle of dark screen
        public void SetHandleBlackout(HandleObject handleObjects)
        {
            var blackoutHandle = handleObjects;

            blackoutHandle.AddEvent(EventTriggerType.PointerDown, TouchDown);
            blackoutHandle.AddEvent(EventTriggerType.PointerUp, TouchUp);
            
            blackoutHandle.AddEvent(EventTriggerType.PointerDown, touchDetect.TouchDown);
            blackoutHandle.AddEvent(EventTriggerType.PointerUp, touchDetect.TouchUp);
        }
        
        // Start component
        public void Start()
        {
            tPosition = 0;
            tTarget = 0;
            CurrentState = ScrollState.hidden;
        }
        
        // Calls each frame
        public void Update()
        {
            ToShowProcessAuto(); // Show menu process
            ToHideProcessAuto(); // Close menu process
            MoveByHandleProcess(); // Calculate interpolation motions when user moves menu
        }
        
        // Setup auto open menu state
        public void AutoOpen()
        {
            CurrentState = ScrollState.toShow;
            tTarget = 1;
        }
        
        // Setup auto close menu state
        public void AutoClose()
        {
            messageWhenClosed = AppMessagesConst.MenuButtonFromClose;
            CurrentState = ScrollState.toHide;
            tTarget = 0;
        }
        
        // Setup auto close menu without animation state 
        public void AutoCloseWithoutAnimation()
        {
            messageWhenClosed = string.Empty;
            CurrentState = ScrollState.toHide;
            tTarget = 0;
        }
        
        // Setup auto close menu
        public void AutoCloseWithoutButton()
        {
            messageWhenClosed = AppMessagesConst.CloseButtonToMainClose;
            CurrentState = ScrollState.toHide;
            tTarget = 0;
        }

        // Called when touch down background
        private void TouchDown()
        {
            if (CurrentState == ScrollState.hidden)
                return;
            
            CurrentState = ScrollState.handle;
            swipeDetect.TouchDown();
        }
        
        // Called when touch up background
        private void TouchUp()
        {
            if (tPosition < tWhenNeedHide) { AutoHideByHandle(); }
            else { AutoOpen(); }
            
            swipeDetect.TouchUp();
        }
        
        // Calculate interpolation motions when user moves menu
        private void MoveByHandleProcess()
        {
            // Check current state
            if (CurrentState != ScrollState.handle)
                return;
            
            // Calculate delta of screen to canvas delta
            var delta = screenConverter.Invoke(InputHS.DeltaMove);

            // Add delta motion to user action detectors
            swipeDetect.AddMoveDelta(delta);
            touchDetect.AddMoveDelta(delta);

            // Calculate delta by curve
            if (tTarget > 1f && delta.x < 0)
                delta *= 1 - (tTarget - 1f) / showOutOfBorder;

            // Update target interpolation position
            tTarget += -delta.x * tPerDelta;
            tTarget = Mathf.Clamp(tTarget, 0, 1 + showOutOfBorder);
            
            // Update current interpolation position
            tPosition = Mathf.Lerp(tPosition, tTarget, joinSpeed * 2.5f);

            // Setup current interpolation position to dark screen and blackout
            screenBlackout.SetState(tPosition);
            mainPart.SetupPositionByT(tPosition);
        }
        
        // Show menu process
        private void ToShowProcessAuto()
        {
            if (CurrentState != ScrollState.toShow)
                return;
            
            if (Mathf.Abs(tPosition - tTarget) < clampDelta)
                CurrentState = ScrollState.shown;

            if (CurrentState == ScrollState.shown)
                tPosition = tTarget;
            
            // Update current interpolation position
            tPosition = Mathf.Lerp(tPosition, tTarget, joinSpeed);
            
            // Setup current interpolation position to dark screen and blackout
            screenBlackout.SetState(tPosition);
            mainPart.SetupPositionByT(tPosition);
        }
        
        // Close menu process
        private void ToHideProcessAuto()
        {
            if (CurrentState != ScrollState.toHide)
                return;

            if (Mathf.Abs(tPosition - tTarget) < clampDelta)
            {
                CurrentState = ScrollState.hidden;
                MainMessenger.SendMessage(messageWhenClosed);
            }

            if (CurrentState == ScrollState.hidden)
                tPosition = tTarget;

            var localSpeed = joinSpeed;
            if (tPosition < hideMultiplierBorder)
            {
                var multiplier = Mathf.Abs((tPosition - hideMultiplierBorder) / hideMultiplierBorder);
                localSpeed += localSpeed * multiplier * 3;
            }
            
            // Update current interpolation position
            tPosition = Mathf.Lerp(tPosition, tTarget, localSpeed);
            
            // Setup current interpolation position to dark screen and blackout
            screenBlackout.SetState(tPosition);
            mainPart.SetupPositionByT(tPosition);
        }

        // Calling automatic close when clicking on the background
        private void AutoHideByBlackout()
        {
            if (CurrentState == ScrollState.handle)
                return;
            
            AutoClose();
        }
        
        // Calling automatic close
        private void AutoHideByHandle() => AutoClose();
    }
}
