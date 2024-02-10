using Architecture.TrackArea;
using HomeTools.Source.Design;
using HTools;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView.DayMarkerPicker
{
    // Component for visual of UI for choose day characteristic view
    public class VisualPart : IBehaviorSync
    {
        private const float animationSpeed = 0.27f;

        // Position of view in screen
        private readonly Vector2 centeredPosition = new Vector2(0, -100);
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
        
        // Additional animation component of characteristic items
        public UIAlphaSync AlphaSyncOut;

        // Create view
        public VisualPart(RectTransform rectTransform, int blackoutLayer)
        {
            // Save main components and add view image to Color Theming
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
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.ChooseDayMarkerModule);
            
            // Find title of view
            var title = rectTransform.Find("Info").GetComponent<Text>();

            // Create animation component of alpha channel for view
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.GetComponent<Image>());
            uiSyncItem.AddElement(title);
            uiSyncItem.AddElement(rectTransform.Find("Upper background").GetComponent<Image>());
            uiSyncItem.AddElement(rectTransform.Find("Line").GetComponent<Image>());
            uiSyncItem.Speed = animationSpeed * 0.8f;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem, AppSyncAnchors.ChooseDayMarkerModule);
            
            // Create dark screen under view
            screenBlackout = new ScreenBlackout("EditProject Blackout", blackoutLayer);
            // Setup color of dark screen
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.TrackAreaBlackout, AppTheming.AppItem.TimeTrackModule);
            // Add message that calls when screen clicked
            screenBlackout.MessageBlackoutClicked = AppMessagesConst.BlackoutChooseDayMarkerModuleClicked;
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

            if (AlphaSyncOut == null)
                return;
            
            AlphaSyncOut.SetAlpha(0);
            AlphaSyncOut.SetDefaultAlpha(0);
            AlphaSyncOut.Speed = animationSpeed;
            AlphaSyncOut.SetAlphaByDynamic(1);
            AlphaSyncOut.Run();
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
            
            if (AlphaSyncOut == null)
                return;
            
            AlphaSyncOut.Speed = animationSpeed * 1.5f;
            AlphaSyncOut.SetAlphaByDynamic(0);
            AlphaSyncOut.Run();
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
    }
}
