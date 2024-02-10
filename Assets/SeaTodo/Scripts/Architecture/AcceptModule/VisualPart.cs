using Architecture.TrackArea;
using HomeTools.Source.Design;
using HTools;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.AcceptModule
{
    // Responsible for visual behaviour of UI
    public class VisualPart : IBehaviorSync
    {
        private const float animationSpeed = 0.27f;

        private readonly Vector2 centeredPosition = new Vector2(0, -100);
        private readonly UIAlphaSync uiSyncItem;
        private readonly RectTransformSync rectTransformSync;
        private readonly RectTransformSync rectTransformSyncInput;
        private readonly RectTransform rectTransform;

        private readonly ScreenBlackout screenBlackout;
        private float screenMarker;
        private float targetScreenMarker;
        private bool activeScreenMarker;

        public VisualPart(RectTransform rectTransform, int blackoutLayer)
        {
            // Setup main rect
            this.rectTransform = rectTransform;
            rectTransform.anchoredPosition = centeredPosition;
            AppTheming.AddElement(rectTransform.GetComponent<Image>(), ColorTheme.TrackAreaMain, AppTheming.AppItem.TimeTrackModule);

            // Create animation component for rect
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.Speed = animationSpeed;
            rectTransformSync.TargetScale = Vector3.zero;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.AcceptModule);
            
            // Create animation component of upward movement for rect (when input)
            rectTransformSyncInput = new RectTransformSync();
            rectTransformSyncInput.SetRectTransformSync(rectTransform);
            rectTransformSyncInput.Speed = animationSpeed;
            rectTransformSyncInput.TargetPosition = new Vector3(0, 100, 0);
            rectTransformSyncInput.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSyncInput.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSyncInput, AppSyncAnchors.AcceptModule);

            // Find title of view
            var title = rectTransform.Find("Info").GetComponent<Text>();

            // Create animation component of alpha channel for UI elements
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.GetComponent<Image>());
            uiSyncItem.AddElement(title);
            uiSyncItem.AddElement(rectTransform.Find("Accept/Circle").GetComponent<SVGImage>());
            uiSyncItem.AddElement(rectTransform.Find("Accept/Icon").GetComponent<SVGImage>());
            uiSyncItem.AddElement(rectTransform.Find("Upper background").GetComponent<Image>());
            uiSyncItem.AddElement(rectTransform.Find("Line").GetComponent<Image>());
            uiSyncItem.Speed = animationSpeed * 0.8f;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem, AppSyncAnchors.AcceptModule);

            // Setup background screen blackout
            screenBlackout = new ScreenBlackout("EditProject Blackout", blackoutLayer);
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.TrackAreaBlackout, AppTheming.AppItem.TimeTrackModule);
            screenBlackout.MessageBlackoutClicked = AppMessagesConst.BlackoutAcceptModuleClicked;
        }

        // Setup animation elements to default state
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
            
            rectTransformSyncInput.SetT(1);
            rectTransformSyncInput.SetDefaultT(1);
        }

        // Update target position for upward movement of view
        public void UpdateTargetPosition(Vector2 targetPosition)
        {
            rectTransformSync.TargetPosition = targetPosition;
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
        }
        
        public void Start() { }

        public void Update()
        {
            UpdateBlackoutScreen();
        }

        // Update blackout screen view when open or close view
        private void UpdateBlackoutScreen()
        {
            if (!activeScreenMarker)
                return;

            if (Mathf.Abs(targetScreenMarker - screenMarker) < animationSpeed * 0.1f)
            {
                screenMarker = targetScreenMarker;
                screenBlackout.SetState(targetScreenMarker);
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
