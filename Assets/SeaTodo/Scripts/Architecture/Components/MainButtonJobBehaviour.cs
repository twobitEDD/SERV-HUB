using System;
using System.Collections.Generic;
using Architecture.Pages;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.Components
{
    // General MonoBehaviour implementation of button
    public class MainButtonJobBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, 
                                                            IPointerClickHandler, IPointerExitHandler
    {
        private Action touchedAction; // touched action
        private RectTransformSync rectTransformSync; // Animation component of rect

        // Parameters of button animation
        public float SimulateWaveScale = 1.1f;
        private const float scaleTransformSpeed = 0.4f;
        private const float processSpeed = 0.025f;
        private const float processSleep = 0.7f;

        private bool touched; // Touched flag
        private bool readyToClick; // Ready to click flag
        private float pressedTimer; // Wait timer when pressed
        private bool active; // Active flag

        // List of actions - TouchDown / TouchUp
        private List<Action> touchDownActions;
        private List<Action> touchUpActions;

        // Setup
        public void Setup(RectTransform rectTransform, Action touchedAction, GameObject handle)
        {
            // Save main
            this.touchedAction = touchedAction;
            
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
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, key);
        }
        
        // Attach object to update calls to general MonoBehaviour
        public void AttachToSyncWithBehaviour()
        {
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync);
        }

        // Reactivate button state (Need call before using)
        public void Reactivate()
        {
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

        // Add touch down action to list
        public void AddTouchDownAction(Action action)
        { 
            if (touchDownActions == null)
                touchDownActions = new List<Action>();
            
            if (!touchDownActions.Contains(action))
                touchDownActions.Add(action);
        }
        
        // Remove touch down actions from list
        public void RemoveTouchDownAction(Action action)
        {
            if (touchDownActions.Contains(action))
                touchDownActions.Remove(action);
        }
        
        // Add touch up action to list
        public void AddTouchUpAction(Action action)
        { 
            if (touchUpActions == null)
                touchUpActions = new List<Action>();
            
            if (!touchUpActions.Contains(action))
                touchUpActions.Add(action);
        }
        
        // Remove touch up actions from list
        public void RemoveTouchUpAction(Action action)
        {
            if (touchUpActions.Contains(action))
                touchUpActions.Remove(action);
        }
        
        // Calls when touched down
        private void TouchDown()
        {
            if (!active)
                return;
            
            pressedTimer = 0;
            touched = true;
            readyToClick = false;

            if (touchDownActions == null)
                return;
            
            foreach (var action in touchDownActions)
                action.Invoke();
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
            
            if (touchUpActions == null)
                return;
            
            foreach (var action in touchUpActions)
                action.Invoke();
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

        public void OnPointerDown(PointerEventData eventData) => TouchDown();

        public void OnPointerUp(PointerEventData eventData) => TouchUp();

        public void OnPointerClick(PointerEventData eventData) => TouchClick();

        public void OnPointerExit(PointerEventData eventData) => TouchExit();
    }
}
