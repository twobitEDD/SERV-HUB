using System;
using Architecture.TextHolder;
using Architecture.TrackArea;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ChooseIconModule
{
    // Responsible for visual behaviour of UI
    public class VisualPart : IBehaviorSync
    {
        private const float animationSpeed = 0.27f;
        public UIAlphaSyncGroup UiSyncElements { get; } // Animation of alpha channel component
        private readonly Action closeAction; // Action that calls when view closed
        
        // Position of view where should start
        private readonly Vector2 centeredPosition = new Vector2(0, -100);
        private readonly UIAlphaSync uiSyncItem; // Animation component of alpha channel for view
        private readonly RectTransformSync rectTransformSync; //Animation component of view rect
        private readonly RectTransform rectTransform; // Rect of view

        // Dark screen image under view
        private readonly ScreenBlackout screenBlackout;
        // Markers to control of alpha channel of dark screen
        private float screenMarker;
        private float targetScreenMarker;
        private bool activeScreenMarker;

        // Create view
        public VisualPart(RectTransform rectTransform, Action closeAction, int blackoutLayer)
        {
            // Save main components and add view image to Color Theming
            this.closeAction = closeAction;
            this.rectTransform = rectTransform;
            rectTransform.anchoredPosition = centeredPosition;
            AppTheming.AddElement(rectTransform.GetComponent<Image>(), ColorTheme.TrackAreaMain, AppTheming.AppItem.TimeTrackModule);

            // Create animation component of rect
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.Speed = animationSpeed;
            rectTransformSync.TargetScale = Vector3.zero;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.ChooseIconModule);

            // Find and add to localize title of view
            var title = rectTransform.Find("Info").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.ChooseIcon);
            
            // Create animation component of alpha channel for view
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.GetComponent<Image>());
            uiSyncItem.AddElement(title);
            uiSyncItem.AddElement(rectTransform.Find("Line").GetComponent<Image>());
            uiSyncItem.AddElement(rectTransform.Find("Upper background").GetComponent<Image>());
            uiSyncItem.Speed = animationSpeed * 0.8f;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem, AppSyncAnchors.ChooseIconModule);
            
            // Add to animation component points of navigation for colors list
            for (var i = 0; i < 3; i++)
                uiSyncItem.AddElement(rectTransform.Find($"NavItem {i + 1} Colors").GetComponent<SVGImage>());
            
            // Add to animation component points of navigation for icons list
            for (var i = 0; i < 2; i++)
                uiSyncItem.AddElement(rectTransform.Find($"NavItem {i + 1} Icons").GetComponent<SVGImage>());
            
            // Create dark screen under view
            screenBlackout = new ScreenBlackout("CalendarTrack Blackout", blackoutLayer);
            // Setup color of dark screen
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.TrackAreaBlackout, AppTheming.AppItem.TimeTrackModule);
            // Add message that calls when screen clicked
            screenBlackout.MessageBlackoutClicked = AppMessagesConst.BlackoutChooseIconClicked;
            
            // Create additional animation component of alpha channel for icons and colors
            UiSyncElements = new UIAlphaSyncGroup();
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

            UiSyncElements.Speed = animationSpeed * 0.8f;
            UiSyncElements.SetAlpha(0);
            UiSyncElements.SetDefaultAlpha(0);
            UiSyncElements.SetAlphaByDynamic(1);
            UiSyncElements.Run();
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

            UiSyncElements.SetAlpha(1);
            UiSyncElements.SetDefaultAlpha(1);
            UiSyncElements.SetAlphaByDynamic(0);
            UiSyncElements.Run();
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

            if (Mathf.Abs(targetScreenMarker - screenMarker) < animationSpeed * 0.1f)
            {
                screenMarker = targetScreenMarker;
                screenBlackout.SetState(targetScreenMarker); // Setup alpha channel
                CheckModuleSetActive();
                CheckInvokeCloseAction();
                activeScreenMarker = false;
            }

            screenMarker = Mathf.Lerp(screenMarker, targetScreenMarker, animationSpeed);
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

        // Call once close action when dark screen hidden
        private void CheckInvokeCloseAction()
        {
            if (screenMarker < 0.01f)
                closeAction.Invoke();
        }
    }
}
