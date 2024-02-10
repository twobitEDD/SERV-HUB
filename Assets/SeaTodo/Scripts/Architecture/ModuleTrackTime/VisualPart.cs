using System;
using Architecture.TrackArea;
using HomeTools.Source.Design;
using HTools;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleTrackTime
{
    // Component for visual of UI for track time view
    public class VisualPart : IBehaviorSync
    {
        private const float AnimationSpeed = 0.3f;
        
        // Animation component of alpha channel of UI
        public readonly UIAlphaSync UiSyncItemPoints;
        // Close action
        private readonly Action closeAction;
        // Position of view in screen
        private readonly Vector2 centeredPosition = new Vector2(0, 0);
        // Animation component of alpha channel of UI
        private readonly UIAlphaSync uiSyncItem;
        // Animation component of rect when open view
        private readonly RectTransformSync rectTransformSync;
        // Rect of view
        private readonly RectTransform rectTransform;
        
        // Dark screen image under view
        private readonly ScreenBlackout screenBlackout;
        // Markers to control of alpha channel of dark screen
        private float screenMarker;
        private float targetScreenMarker;
        private bool activeScreenMarker;

        // Create view
        public VisualPart(RectTransform rectTransform, Action closeAction)
        {
            // Save main components and add view image to Color Theming
            this.closeAction = closeAction;
            this.rectTransform = rectTransform;
            rectTransform.anchoredPosition = centeredPosition;
            AppTheming.AddElement(rectTransform.GetComponent<Image>(), ColorTheme.TrackAreaMain, AppTheming.AppItem.TimeTrackModule);

            // Create animation component of rect
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.Speed = AnimationSpeed;
            rectTransformSync.TargetScale = Vector3.zero;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.TrackTimeModule);
            
            // Create animation component of alpha channel for view
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.GetComponent<Image>());
            uiSyncItem.Speed = AnimationSpeed * 0.8f;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem, AppSyncAnchors.TrackTimeModule);

            // Create animation component of alpha channel for additional elements
            UiSyncItemPoints = new UIAlphaSync {Speed = AnimationSpeed * 0.8f, SpeedMode = UIAlphaSync.Mode.Lerp};
            SyncWithBehaviour.Instance.AddObserver(UiSyncItemPoints, AppSyncAnchors.TrackTimeModule);
            UiSyncItemPoints.PrepareToWork();

            // Setup color object in view
            var colonText = rectTransform.Find("Colon").GetComponent<Text>();
            AppTheming.AddElement(colonText, ColorTheme.SecondaryColor, AppTheming.AppItem.TimeTrackModule);
            uiSyncItem.AddElement(colonText);

            // Create dark screen under view
            screenBlackout = new ScreenBlackout("TimeTrack Blackout", 4);
            // Setup color of dark screen
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.TrackAreaBlackout, AppTheming.AppItem.TimeTrackModule);
            // Add message that calls when screen clicked
            screenBlackout.MessageBlackoutClicked = AppMessagesConst.BlackoutTimeTrackClicked;
        }

        // Initialize dark screen and animation components
        public void Initialize()
        {
            screenBlackout.PrepareBlackout();
            screenBlackout.SetState(0);

            rectTransformSync.SetDefaultT(0);
            rectTransformSync.SetT(0);
            CheckModuleSetActive();
            
            uiSyncItem.PrepareToWork();
            uiSyncItem.SetDefaultAlpha(0);
            uiSyncItem.SetAlpha(0);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition)
        {
            // Setup for animation of dark screen (use in Update method)
            activeScreenMarker = true;
            targetScreenMarker = 1;

            // Start play animation components
            
            rectTransformSync.TargetPosition = startPosition;
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();
            
            uiSyncItem.SetAlphaByDynamic(1);
            uiSyncItem.Run();
        }

        // Start animation components for close animation
        public void Close()
        {
            // Setup for animation of dark screen (use in Update method)
            activeScreenMarker = true;
            targetScreenMarker = 0;
            
            // Start play animation components
            
            rectTransformSync.SetTByDynamic(0);
            rectTransformSync.Run();
            
            uiSyncItem.SetAlphaByDynamic(0);
            uiSyncItem.Run();
            
            UiSyncItemPoints.PrepareToWork();
            UiSyncItemPoints.SetAlphaByDynamic(0);
            UiSyncItemPoints.Run();
        }

        public void Start() { }

        public void Update()
        {
            UpdateBlackoutScreen();
        }

        // Change dark screen process
        private void UpdateBlackoutScreen()
        {
            if (!activeScreenMarker)
                return;

            if (Mathf.Abs(targetScreenMarker - screenMarker) < AnimationSpeed * 0.1f)
            {
                screenMarker = targetScreenMarker;
                screenBlackout.SetState(targetScreenMarker); // Setup alpha channel
                CheckModuleSetActive();
                CheckInvokeCloseAction();
                activeScreenMarker = false;
            }

            screenMarker = Mathf.Lerp(screenMarker, targetScreenMarker, AnimationSpeed);
            screenBlackout.SetState(screenMarker);

            CheckModuleSetActive();
        }

        // Update activity by state
        private void CheckModuleSetActive()
        {
            if (!rectTransform.gameObject.activeSelf && screenMarker > 0.01f)
                rectTransform.gameObject.SetActive(true);
            
            if (rectTransform.gameObject.activeSelf && screenMarker < 0.01f)
                rectTransform.gameObject.SetActive(false);
        }

        // Check invoke close action once when view closed
        private void CheckInvokeCloseAction()
        {
            if (screenMarker < 0.01f)
                closeAction.Invoke();
        }
    }
}
