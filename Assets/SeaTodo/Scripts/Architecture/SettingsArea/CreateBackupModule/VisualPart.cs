using System;
using Architecture.Pages;
using Architecture.TrackArea;
using HomeTools.Source.Design;
using HTools;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea.CreateBackupModule
{
    // Component for visual of UI for create backup view
    public class VisualPart : IBehaviorSync
    {
        private const float animationSpeed = 0.27f;

        // Position of view in screen
        private readonly Vector2 centeredPosition = new Vector2(0, -100);
        // Animation component of alpha channel of UI
        private readonly UIAlphaSync uiSyncItem;
        // Animation component of rect when open view
        private readonly RectTransformSync rectTransformSync;
        // Animation component of rect when start edit name
        private readonly RectTransformSync rectTransformSyncInput;
        // Rect of view
        private readonly RectTransform rectTransform;
        // Close action
        private readonly Action closeAction;
        // Open action
        private readonly Action openedAction;

        // Dark screen image under view
        private readonly ScreenBlackout screenBlackout;
        // Markers to control of alpha channel of dark screen
        private float screenMarker;
        private float targetScreenMarker;
        private bool activeScreenMarker;

        // Create view
        public VisualPart(RectTransform rectTransform, Action openedAction, int blackoutLayer)
        {
            // Save main components and add view image to Color Theming
            this.rectTransform = rectTransform;
            this.rectTransform = rectTransform;
            this.openedAction = openedAction;
            rectTransform.anchoredPosition = centeredPosition;
            AppTheming.AddElement(rectTransform.GetComponent<Image>(), ColorTheme.TrackAreaMain, AppTheming.AppItem.TimeTrackModule);

            // Create animation component of rect
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.Speed = animationSpeed;
            rectTransformSync.TargetScale = Vector3.zero;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync);
            
            // Create animation component of rect when start edit name
            rectTransformSyncInput = new RectTransformSync();
            rectTransformSyncInput.SetRectTransformSync(rectTransform);
            rectTransformSyncInput.Speed = animationSpeed;
            rectTransformSyncInput.TargetPosition = new Vector3(0, 100, 0);
            rectTransformSyncInput.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSyncInput.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSyncInput);

            // Find title of view
            var title = rectTransform.Find("Info").GetComponent<Text>();

            // Create animation component of alpha channel for view
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.GetComponent<Image>());
            uiSyncItem.AddElement(title);
            uiSyncItem.AddElement(rectTransform.Find("Accept/Circle").GetComponent<SVGImage>());
            uiSyncItem.AddElement(rectTransform.Find("Accept/Icon").GetComponent<SVGImage>());
            uiSyncItem.AddElement(rectTransform.Find("Accept/Load Round").GetComponent<Image>());
            uiSyncItem.AddElement(rectTransform.Find("Accept/Load Circle").GetComponent<SVGImage>());
            uiSyncItem.AddElement(rectTransform.Find("Accept/Load Circle Start").GetComponent<SVGImage>());
            uiSyncItem.AddElement(rectTransform.Find("Edit Name Input/Placeholder").GetComponent<Text>());
            uiSyncItem.AddElement(rectTransform.Find("Edit Name Input/Text").GetComponent<Text>());
            uiSyncItem.AddElement(rectTransform.Find("Description").GetComponent<Text>());
            uiSyncItem.AddElement(rectTransform.Find("Upper background").GetComponent<Image>());
            uiSyncItem.AddElement(rectTransform.Find("Line").GetComponent<Image>());
            uiSyncItem.Speed = animationSpeed * 0.8f;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem);
            
            // Create dark screen under view
            screenBlackout = new ScreenBlackout("EditProject Blackout", blackoutLayer);
            // Setup color of dark screen
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.TrackAreaBlackout, AppTheming.AppItem.TimeTrackModule);
            // Add message that calls when screen clicked
            screenBlackout.MessageBlackoutClicked = AppMessagesConst.BlackoutCreateBackupModuleClicked;
        }

        // Initialize dark screen and animation components
        public void Initialize()
        {
            screenBlackout.PrepareBlackout();
            screenBlackout.SetState(0);

            rectTransformSync.SetDefaultT(0);
            CheckModuleSetActive();
            
            uiSyncItem.PrepareToWork();
            uiSyncItem.SetDefaultAlpha(0);
            uiSyncItem.SetAlpha(0);
        }

        // Start animation component when start edit name
        public void ActivateInput()
        {
            rectTransformSyncInput.SetTByDynamic(0);
            rectTransformSyncInput.Run();
        }

        // Start animation component when finish edit name
        public void DeactivateInput()
        {
            rectTransformSyncInput.SetTByDynamic(1);
            rectTransformSyncInput.Run();
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

            rectTransformSyncInput.SetT(1);
            rectTransformSyncInput.SetDefaultT(1);
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
            
            PageScroll.Instance.Enabled = true;
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
                CheckInvokeOpenedAction();
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
        
        // Check invoke close action once when view closed
        private void CheckInvokeCloseAction()
        {
            if (screenMarker < 0.01f)
                closeAction?.Invoke();
        }
        
        // Check invoke action once when view opened
        private void CheckInvokeOpenedAction()
        {
            if (screenMarker > 0.99f)
                openedAction?.Invoke();
        }
    }
}
