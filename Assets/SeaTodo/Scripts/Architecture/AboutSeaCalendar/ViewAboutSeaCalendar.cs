using AppSettings.StatusBarSettings;
using Architecture.Components;
using Architecture.Elements;
using Architecture.TextHolder;
using Architecture.TutorialArea;
using HomeTools.Other;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.AboutSeaCalendar
{
    // Class that contains page "About Sea Calendar"
    public class ViewAboutSeaCalendar : IBehaviorSync
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private bool started;
        public RectTransform RectTransform;
        private VisualPart visualPart; // Class that contains visual part of this page
        private MainButtonJob closeButton; // Class of close button
        private RectTransform contentContainer;
        
        private RectTransform scrollVerticalRect;
        private RectTransform scrollView;
        
        private const float verticalRectMultiplier = 0.85f;

        public void Start() => started = false;

        // Initialize on the second call of update
        public void Update()
        {
            if (started)
                return;

            Initialize();
            Setup();

            started = true;
        }

        // Initialize main ui components
        private void Initialize()
        {
            // Find and setup view "About Sea Calendar"
            RectTransform = SceneResources.Get("About Sea Calendar").GetComponent<RectTransform>();
            RectTransform.SetParent(MainCanvas.RectTransform);
            RectTransform.anchoredPosition = Vector2.zero;
            RectTransform.SetRectTransformAnchorHorizontal(0, 0);
            RectTransform.SetRectTransformAnchorVertical(0, 0);
            SyncWithBehaviour.Instance.AddAnchor(RectTransform.gameObject, AppSyncAnchors.AboutSeaCalendarArea);
            
            // Setup links to components
            contentContainer = RectTransform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            scrollVerticalRect = RectTransform.Find("Scroll View/Scrollbar Vertical").GetComponent<RectTransform>();
            scrollView = RectTransform.Find("Scroll View").GetComponent<RectTransform>();

            // Find and localize title of view "About Sea Calendar"
            var title = RectTransform.Find("Title").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.SeaCalendar);
        }
        
        // The method which is responsible for open view
        public void Open()
        {
            visualPart.Open(); // call open for visual elements
            AppBarActions().AddAction(CloseTouched); // add action of close this view

            UpdateHeight(contentContainer); // update height by localized text
        }

        // Setup main part of view
        private void Setup()
        {
            // Create class for control of visual elements
            visualPart = new VisualPart(RectTransform);
            SyncWithBehaviour.Instance.AddObserver(visualPart, AppSyncAnchors.AboutSeaCalendarArea);
            visualPart.Initialize();

            // Setup and localize close button 
            var close = RectTransform.Find("Close").GetComponent<Text>();
            SetupClosePosition(close.rectTransform);
            closeButton = new MainButtonJob(close.rectTransform, CloseTouched, close.gameObject);
            closeButton.Reactivate();
            closeButton.AttachToSyncWithBehaviour(AppSyncAnchors.AboutSeaCalendarArea);
            TextLocalization.Instance.AddLocalization(close, TextKeysHolder.CloseAboutSeaCalendar);

            // Calculate main params for scroll
            var height = scrollView.rect.height;
            var newHeight = height / scrollVerticalRect.localScale.y * verticalRectMultiplier;
            scrollVerticalRect.sizeDelta = new Vector2(scrollVerticalRect.sizeDelta.x, newHeight);
            
            // Colorize view and call immediately visual close
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.AboutSeaCalendar);
            visualPart.CloseImmediately();
        }
        
        // Setup position of close button
        private static void SetupClosePosition(RectTransform rectTransform)
        {
            var closeHeightPosition = -77 - StatusBarSettings.Height;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 
                closeHeightPosition);
        }
        
        // The method which is responsible for close of view
        private void CloseTouched()
        {
            closeButton.Reactivate();
            CalendarTipClosedAnalyze();
            visualPart.Close();
            AppBarActions().RemoveAction(CloseTouched);
        }
        
        // Analyze whether the user has read to the end
        private void CalendarTipClosedAnalyze()
        {
            if (contentContainer.anchoredPosition.y < 700)
                TutorialInfo.SkipCalendar();
            else
                TutorialInfo.ApplyCalendar();
        }
        
        // Update height of object with localized text 
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
