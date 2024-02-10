using System;
using Architecture.Pages;
using HomeTools.Handling;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.Components
{
    // General implementation of button
    public class MainButtonJob : IBehaviorSync
    {
        private readonly RectTransform rectTransform; // button rect
        public readonly HandleObject HandleObject; // handle of button
        private readonly Action touchedAction; // touched action
        private readonly RectTransformSync rectTransformSync; // Animation component of rect
        
        // Parameters of button animation
        public float SimulateWaveScale = 1.1f;
        private const float scaleTransformSpeed = 0.4f;
        private const float processSpeed = 0.025f;
        private const float processSleep = 0.7f;

        private bool touched; // Touched flag
        private bool readyToClick; // Ready to click flag
        private float pressedTimer; // Wait timer when pressed
        private bool active; // Active flag

        // Create
        public MainButtonJob(RectTransform rectTransform, Action touchedAction, GameObject handle)
        {
            // Save main
            this.rectTransform = rectTransform;
            this.touchedAction = touchedAction;

            // Create handle object with events
            HandleObject = new HandleObject(handle);
            HandleObject.AddEvent(EventTriggerType.PointerDown, TouchDown);
            HandleObject.AddEvent(EventTriggerType.PointerUp, TouchUp);
            HandleObject.AddEvent(EventTriggerType.PointerExit, TouchExit);
            HandleObject.AddEvent(EventTriggerType.PointerClick, TouchClick);
            
            // Create animation component for button rect
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.Speed = scaleTransformSpeed;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
        }

        // Attach object to update calls by key to local MonoBehaviour
        public void AttachToSyncWithBehaviour(int key)
        {
            SyncWithBehaviour.Instance.AddObserver(this, key);
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, key);
        }
        
        // Attach object to update calls to general MonoBehaviour
        public void AttachToSyncWithBehaviour()
        {
            SyncWithBehaviour.Instance.AddObserver(this);
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync);
        }

        // Reactivate button state (Need call before using)
        public void Reactivate()
        {
            if (active)
                return;
            
            active = true;
            // Set animation component to default state
            rectTransformSync.TargetScale = Vector3.one * SimulateWaveScale;
            rectTransformSync.SetDefaultT(1);
            rectTransformSync.SetT(1);
            rectTransformSync.Stop();
        }
        
        // Deactivate button
        public void Deactivate()
        {
            active = false;
            readyToClick = false;
        }

        public void Start() { }

        public void Update()
        {
            UpdateButtonScale();
            CatchResetByScroll();
        }

        // Run animation component to simulate button scale effect
        public void SimulateWave()
        {
            rectTransformSync.SetDefaultT(0);
            rectTransformSync.TargetScale = Vector3.one * SimulateWaveScale;
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();
        }

        // Update activity of button
        public void SetActive(bool state) => rectTransform.gameObject.SetActive(state);

        // Calls when touched down
        private void TouchDown()
        {
            if (!active)
                return;
            
            pressedTimer = 0;
            touched = true;
            readyToClick = false;
        }

        // Calls when touched up
        private void TouchUp()
        {
            if (!touched)
                return;

            if (ScrollHandler.AccessByScroll)
                readyToClick = true;
            else
                FinishTransition(false);
            
            PageScroll.Instance.ResetInertia();
        }

        // Calls when finger exited from button area
        private void TouchExit()
        {
            if (!touched)
                return;
            
            FinishTransition(false);
        }
        
        // Calls when clicked
        private void TouchClick()
        {
            if (!readyToClick)
                return;
            
            FinishTransition(true);
        }
        
        // Update button scale each update when pressed
        private void UpdateButtonScale()
        {
            if (!touched)
                return;
            
            pressedTimer += processSpeed;

            var sleep = Mathf.Clamp((pressedTimer - processSleep) / (1 - processSleep), 0f, 1f);

            rectTransformSync.SetT(1 - sleep);
            rectTransformSync.SetDefaultT(1 - sleep);

            if (pressedTimer < 1)
                return;
            
            FinishTransition(true);
        }
        
        // Finish if the finger has moved a lot
        private void CatchResetByScroll()
        {
            if (!touched)
                return;

            if (ScrollHandler.AccessByScroll)
                return;
            
            FinishTransition(false);
        }
        
        // Process the result of clicking
        private void FinishTransition(bool success)
        {
            pressedTimer = 0;
            touched = false;
            
            rectTransformSync.TargetScale = Vector3.one * SimulateWaveScale;
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();

            if (success)
                touchedAction.Invoke();
        }
    }
}
