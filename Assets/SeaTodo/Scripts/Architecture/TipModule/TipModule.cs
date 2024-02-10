using Architecture.Components;
using Architecture.Elements;
using HomeTools.Messenger;
using HomeTools.Other;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TipModule
{
    // Class of tips view
    public class TipModule : IBehaviorSync
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;

        private RectTransform rectTransform; // Rect object of view
        private VisualPart visualPart; // Class that contains visual part of view
        private Text textInfo; // Title of view
        // Container of tips
        private TipsContentContainer tipsContentContainer;
        // Rect for tip text objects
        private RectTransform contentContainer;

        // Rect of scroll
        private RectTransform scrollVerticalRect;
        private float defaultRectHeight;  // Default scroll rect height
        private float defaultModuleHeight; // Default view height
        private const float sizePerUnit = 5.57f; // Convert unit size to height

        private TipSession tipSession; // Keep current session
        private RectTransform currentContent; // Rect of current tip
        private bool started; // Initialized flag

        public void Start() { }

        public void Update()
        {
            if (started)
                return;

            Setup();

            started = true;
        }

        // Setup
        private void Setup()
        {
            // Find view and set parent of this view
            rectTransform = SceneResources.Get("Tip Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.TipModule);
            
            // Setup order in hierarchy 
            var layerIndex = MainCanvas.RectTransform.childCount - 4;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            
            // Create visual component for UI
            visualPart = new VisualPart(rectTransform, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            // Colorize parts that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            // Initialize visual component
            visualPart.Initialize();
            // Add close method for clicks on dark background under view
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutTipModuleClicked);
            
            // Find title text
            textInfo = rectTransform.Find("Info").GetComponent<Text>();

            // Create component with tips container
            tipsContentContainer = new TipsContentContainer();
            // Find additional scroll components
            contentContainer = rectTransform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            scrollVerticalRect = rectTransform.Find("Scroll View/Scrollbar Vertical").GetComponent<RectTransform>();

            // Save default height of scroll and view
            defaultModuleHeight = rectTransform.sizeDelta.y;
            defaultRectHeight = scrollVerticalRect.sizeDelta.y;
        }
        
        // The method which is responsible for open view
        public void Open(Vector2 startPosition, TipSession session, float height)
        {
            // Save session
            tipSession = session;
            // Update tip content
            PrepareView();
            // Open view
            visualPart.Open(startPosition);
            // Add close action to list for device home button
            AppBarActions().AddAction(Close);
            // Update title of view
            textInfo.text = session.Title;
            // Update height of view
            UpdateModuleHeight(height);
        }

        // Update height of view
        private void UpdateModuleHeight(float height)
        {
            // Update height of view rect
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
            // Update height of scroll rect
            var newHeight = defaultRectHeight - (defaultModuleHeight - height) * sizePerUnit;
            scrollVerticalRect.sizeDelta = new Vector2(scrollVerticalRect.sizeDelta.x, newHeight);
        }
        
        // The method which is responsible for close of view
        private void Close()
        {
            // Start close process of view
            visualPart.Close();
            // Remove action of close from list (device home button)
            AppBarActions().RemoveAction(Close);
            // Finish session
            tipSession.Closed();
        }

        // Update content position in view
        private void PrepareView()
        {
            // Move back old tip object
            if (currentContent != null)
                currentContent.SetParent(SceneResources.Container);

            // Get new tip content
            currentContent = tipsContentContainer.GetTipContent(tipSession.Type);
            
            // Update height of view
            UpdateHeight(currentContent);
            
            // Update current tip content
            currentContent.SetParent(contentContainer);
            contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, currentContent.sizeDelta.y);
            currentContent.anchoredPosition = Vector2.zero;
            currentContent.localScale = Vector3.one;
            contentContainer.anchoredPosition = Vector2.zero;

            // Move alpha component of tip to visual component
            visualPart.AlphaSyncOut = tipsContentContainer.GetTipContentAlphaSync(tipSession.Type);
        }

        // Update view height by content
        private void UpdateHeight(RectTransform contentRect)
        {
            if (contentRect.childCount == 0)
                return;
            
            var text = contentRect.GetChild(0).GetComponent<Text>();
            
            if (text == null)
                return;
            
            OtherHTools.UpdateHeightOfText(text);
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, text.rectTransform.sizeDelta.y);
        }
    }
}
