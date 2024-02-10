using Architecture.Components;
using Architecture.DaysMarkerArea.DaysView.MonthView;
using Architecture.Elements;
using HomeTools.Messenger;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView.MonthMarkersModule
{
    // View for choose day of month in Sea Calendar page
    public class MonthMarkersModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;

        private readonly RectTransform rectTransform; // Rect of view
        private readonly VisualPart visualPart; // Visual component for UI
        private readonly Text textInfo; // Title of view
        private readonly RectTransform contentContainer; // Object for day items
        private readonly RectTransform viewPort; // Rect of viewport

        private readonly RectTransform scrollVerticalRect; // Rect with scroll
        private readonly float defaultRectHeight; // Default scroll rect height
        private readonly float defaultModuleHeight; // Default view height
        private const float sizePerUnit = 5.57f; // Convert unit size to height

        private MonthMarkersSession monthMarkersSession; // Keep current session
        private readonly MonthViewContent monthViewContent; // Content with days list

        // Create and setup
        public MonthMarkersModule()
        {
            // Find view and set parent of this view
            rectTransform = SceneResources.Get("MonthMarkers Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            
            // Setup order in hierarchy 
            var layerIndex = MainCanvas.RectTransform.childCount - 5;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.MonthMarkersModule);
            
            // Create visual component for UI
            visualPart = new VisualPart(rectTransform, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            // Colorize parts that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            // Initialize visual component
            visualPart.Initialize();
            // Add close method for clicks on dark background under view
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutMonthMarkersModuleClicked);
            
            // Find title text
            textInfo = rectTransform.Find("Info").GetComponent<Text>();
            
            // Find additional scroll components
            viewPort = rectTransform.Find("Scroll View/Viewport").GetComponent<RectTransform>();
            contentContainer = rectTransform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            scrollVerticalRect = rectTransform.Find("Scroll View/Scrollbar Vertical").GetComponent<RectTransform>();

            // Save default height of scroll and view
            defaultModuleHeight = rectTransform.sizeDelta.y;
            defaultRectHeight = scrollVerticalRect.sizeDelta.y;
            
            // Create component of days list
            monthViewContent = new MonthViewContent(
                                contentContainer.Find("Month Marker Content").GetComponent<RectTransform>());
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, MonthMarkersSession session)
        {
            // Save session
            monthMarkersSession = session;
            // Update days list
            monthViewContent.SetupMonth(monthMarkersSession.MonthMarkersPackage);
            // Update content position in view
            PrepareView();
            // Open view
            visualPart.Open(startPosition);
            // Add close action to list for device home button
            AppBarActions().AddAction(Close);
            // Update title of view
            textInfo.text = session.Title;
            textInfo.color = monthMarkersSession.MonthMarkersPackage.Current
                ? ThemeLoader.GetCurrentTheme().SecondaryColor
                : ThemeLoader.GetCurrentTheme().DaysMarkerAreaMarkerMonthTitle;
            // Update height of view
            UpdateModuleHeight(GetModuleHeightByAspect());
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
            monthMarkersSession.Closed();
        }

        // Update content position in view
        private void PrepareView()
        {
            // Setup container size by days list
            contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, monthViewContent.RectTransform.sizeDelta.y);

            // Setup position of scroll that is on selected day
            
            var currentDayId = monthMarkersSession.MonthMarkersPackage.CurrentDay;
            var anchorPosition = Vector3.zero;

            if (currentDayId < 0)
                currentDayId = 0;
            
            if (currentDayId >= 0)
            {
                var percentage = (float) currentDayId / monthMarkersSession.MonthMarkersPackage.Days.Count;
                percentage = Mathf.Clamp(percentage, 0, 1f);
                anchorPosition.y += percentage * monthViewContent.RectTransform.sizeDelta.y - MonthViewContent.ShiftBetweenDays;
                
                var maxContentPosition = contentContainer.sizeDelta.y - viewPort.rect.height;
                if (anchorPosition.y > maxContentPosition)
                    anchorPosition.y = maxContentPosition;
            }
            
            contentContainer.anchoredPosition = anchorPosition;
        }

        // Get height for module by screen aspect ratio
        private float GetModuleHeightByAspect()
        {
            const float min = 1.5f;
            const float heightPerOne = 400f;
            return 670 + (MainCanvas.AspectRatio - min) * heightPerOne;
        }
    }
}
