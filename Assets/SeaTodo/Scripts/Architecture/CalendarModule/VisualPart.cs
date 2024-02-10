using System;
using Architecture.TrackArea;
using HomeTools.Source.Design;
using HTools;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.CalendarModule
{
    // Responsible for visual behaviour of UI
    public class VisualPart : IBehaviorSync
    {
        // Speed of animation of calendar
        private const float animationSpeed = 0.3f;
        
        public UIAlphaSync UiSyncElements;
        private readonly Action closeAction;
        
        private readonly Vector2 centeredPosition = new Vector2(0, 0);
        private readonly UIAlphaSync uiSyncItem;
        private readonly RectTransformSync rectTransformSync;
        private readonly RectTransform rectTransform;

        private readonly ScreenBlackout screenBlackout;
        private float screenMarker;
        private float targetScreenMarker;
        private bool activeScreenMarker;

        // Create visual part
        public VisualPart(RectTransform rectTransform, Action closeAction, int blackoutLayer)
        {
            // Setup main components and colorize view image
            this.closeAction = closeAction;
            this.rectTransform = rectTransform;
            rectTransform.anchoredPosition = centeredPosition;
            AppTheming.AddElement(rectTransform.GetComponent<Image>(), ColorTheme.TrackAreaMain, AppTheming.AppItem.TimeTrackModule);

            //Create transform animation component for scale of view when opens
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.Speed = animationSpeed;
            rectTransformSync.TargetScale = Vector3.zero;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.CalendarObject);
            
            // Create alpha color animation for view
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.GetComponent<Image>());
            uiSyncItem.Speed = animationSpeed * 0.8f;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem, AppSyncAnchors.CalendarObject);

            // Create background screen
            screenBlackout = new ScreenBlackout("CalendarTrack Blackout", blackoutLayer);
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.TrackAreaBlackout, AppTheming.AppItem.TimeTrackModule);
            screenBlackout.MessageBlackoutClicked = AppMessagesConst.BlackoutCalendarTrackClicked;
        }

        // Setup and initialize animation elements to default state
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

        // Play animation elements for open animation
        public void Open(Vector2 startPosition)
        {
            activeScreenMarker = true;
            targetScreenMarker = 1;

            rectTransformSync.TargetPosition = startPosition;
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();
            
            uiSyncItem.SetAlphaByDynamic(1);
            uiSyncItem.Run();

            if (UiSyncElements != null)
            {
                UiSyncElements.Speed = animationSpeed * 0.8f;
                UiSyncElements.SetAlpha(0);
                UiSyncElements.SetDefaultAlpha(0);
                UiSyncElements.SetAlphaByDynamic(1);
                UiSyncElements.Run();
            }
        }

        // Play animation elements for close animation
        public void Close()
        {
            activeScreenMarker = true;
            targetScreenMarker = 0;
            
            rectTransformSync.SetTByDynamic(0);
            rectTransformSync.Run();
            
            uiSyncItem.SetAlphaByDynamic(0);
            uiSyncItem.Run();

            if (UiSyncElements != null)
            {
                UiSyncElements.SetAlpha(1);
                UiSyncElements.SetDefaultAlpha(1);
                UiSyncElements.SetAlphaByDynamic(0);
                UiSyncElements.Run();
            }
        }

        public void Start() { }

        public void Update()
        {
            UpdateBlackoutScreen();
        }

        // Update alpha of black background
        private void UpdateBlackoutScreen()
        {
            if (!activeScreenMarker)
                return;

            if (Mathf.Abs(targetScreenMarker - screenMarker) < animationSpeed * 0.1f)
            {
                screenMarker = targetScreenMarker;
                screenBlackout.SetState(targetScreenMarker);
                CheckModuleSetActive();
                CheckInvokeCloseAction();
                activeScreenMarker = false;
            }

            screenMarker = Mathf.Lerp(screenMarker, targetScreenMarker, animationSpeed);
            screenBlackout.SetState(screenMarker);

            CheckModuleSetActive();
        }

        // Update active state of view by activity
        private void CheckModuleSetActive()
        {
            if (!rectTransform.gameObject.activeSelf && screenMarker > 0.01f)
                rectTransform.gameObject.SetActive(true);
            
            if (rectTransform.gameObject.activeSelf && screenMarker < 0.01f)
                rectTransform.gameObject.SetActive(false);
        }

        // Invoke action when closed
        private void CheckInvokeCloseAction()
        {
            if (screenMarker < 0.01f)
                closeAction.Invoke();
        }
    }
}
