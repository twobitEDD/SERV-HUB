using Architecture.DaysMarkerArea.DaysColors;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TaskViewArea.NormalView;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea
{
    // Part of Sea Calendar with characteristics of days
    public class ColorsPart
    {
        // Link to page system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;

        private RectTransform rectTransform; // Rect of this part 
        private const float partAnchoredPosition = 0; // Position of UI in background
        private const float backgroundHeight = 1065; // Height of background
        
        private readonly GroupBackground groupBackground; // Background component with UI
        private RectTransformSync groupBackgroundSync; // Animation component of background
        private DaysPalette daysPalette; // Component with list of characteristics
        private IconColorCircles iconColorCircles; // Icon of part

        // Bottom position in page of this part
        public readonly float GetBottomSide;

        // Create main
        public ColorsPart(float upperSidePosition)
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add background to page pool object
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background rect params: height and position in parent
            groupBackground.UpdateVisible(backgroundHeight, upperSidePosition + AppElementsInfo.DistanceBetweenBackgrounds);
            // Calculate bottom side of page
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Create animation component for background moving in page
        public void SetupMovableParameters()
        {
            groupBackgroundSync = new RectTransformSync();
            groupBackgroundSync.SetRectTransformSync(groupBackground.RectTransform);
            groupBackgroundSync.TargetPosition = new Vector3(0, 0, 0);
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SpeedMode = RectTransformSync.Mode.Lerp;
            groupBackgroundSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(groupBackgroundSync);
        }
        
        // Initialize main elements of part
        public void InitializeContent()
        {
            // Find part object in resources on scene
            rectTransform = SceneResources.Get("DaysMarker Colors Part").GetComponent<RectTransform>();
            // Add to synchronization with new Behaviour Component
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.MarkersAreaColors);
            // Add part to background
            groupBackground.AddItemToBackground(rectTransform);
            // Setup position of part in background
            rectTransform.anchoredPosition = new Vector2(0, partAnchoredPosition);
            // Create component on year view in this part
            daysPalette = new DaysPalette(rectTransform);
            
            // Find and localize title of part
            var title = rectTransform.Find("Title").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.MarkersAreaColorTitle);
            
            // Find and localize description of part
            var description = rectTransform.Find("Description").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(description, TextKeysHolder.MarkersAreaColorDescription);
            
            // Find part of icons
            var circle1 = rectTransform.Find("Content/Circle 0").GetComponent<Image>();
            var circle2 = rectTransform.Find("Content/Circle 1").GetComponent<Image>();
            var circle3 = rectTransform.Find("Content/Circle 2").GetComponent<Image>();
            // Create component for icon parts
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);
        }
        
        // Start animation of background when Open Sea Calendar
        public void SetPartToNewPage()
        {
            // Setup animation params and start animation
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, -SetupBasePositions(), 0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            // Update characteristics of days by data
            daysPalette.Update();
            // Update icon of part
            UpdateCircles();
        }
        
        // Start animation of background when close Sea Calendar 
        public void SetPartFromPage()
        {
            // Calculate direction of part by position on screen
            var centeredPosition = PageTransition().CurrentPage().Page.anchoredPosition.y +
                                   groupBackground.RectTransform.anchoredPosition.y;
            var direction = centeredPosition > 0 ? -1 : 1;

            // Start animation
            groupBackgroundSync.Speed = 0.37f;
            groupBackgroundSync.TargetPosition = new Vector3(0, groupBackground.RectTransform.anchoredPosition.y - SetupBasePositions() * direction, 0);
            groupBackgroundSync.SetTByDynamic(0);
            groupBackgroundSync.Run();
        }

        // Sea background with part in new page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(groupBackground.RectTransform);
        
        // Setup activity of part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);
        
        // Calculate main position of part background in page
        private float SetupBasePositions() =>  MainCanvas.RectTransform.sizeDelta.y - 
                                               AppElementsInfo.DistanceContentToBottom + backgroundHeight / 2;
        
        // Update circles of icon in part
        private void UpdateCircles() => iconColorCircles.UpdateColors(ThemeLoader.GetCurrentTheme().CreateFlowAreaContentCircles);
    }
}
