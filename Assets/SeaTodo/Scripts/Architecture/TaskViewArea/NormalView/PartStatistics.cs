using Architecture.Data;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Pages;
using Architecture.TaskViewArea.NormalView.Statistics;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.NormalView
{
    // Component of statistics part in task normal view
    public class PartStatistics
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Current flow for view
        private Flow GetCurrentFlow() => AreasLocator.Instance.FlowViewArea.CurrentFlow;
        
        // Background height
        private const float backgroundHeight = 747;
        // Position of content
        private const float contentPositionFromTop = 170;
        // Background component
        private readonly GroupBackground groupBackground;
        // Animation component for background
        private RectTransformSync groupBackgroundSync;

        // Component of statistics view
        private MainStats mainStats;
        // Component of calendar icon view
        private MonthCalendarIcon monthCalendarIcon;
        
        // UI components
        private Text title;
        private Text description;
        // Circles that placed around icon
        private IconColorCircles iconColorCircles;
        // Bottom position of part in page
        public readonly float GetBottomSide;

        // Create and setup
        public PartStatistics(float upperSidePosition)
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add part to pool
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background height and Y position
            groupBackground.UpdateVisible(backgroundHeight, upperSidePosition + AppElementsInfo.DistanceBetweenBackgrounds);
            // Calculate bottom side
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Create animation component for background part
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
        
        // Initialize base elements to background
        public void InitializeContent()
        {
            // Find and setup part to background
            var content = SceneResources.Get("ViewFlow Statistics Part").GetComponent<RectTransform>();
            groupBackground.AddItemToBackground(content);
            content.anchoredPosition = new Vector2(0, groupBackground.RectTransform.sizeDelta.y / 2 - contentPositionFromTop);

            // Find UI elements
            title = content.Find("Title").GetComponent<Text>();
            description = content.Find("Description").GetComponent<Text>();
            
            // Create component of statistics view
            mainStats = new MainStats(content.Find("Graphic").GetComponent<RectTransform>());

            // Create calendar icon component
            var calendarIcon = content.Find("Calendar Icon").GetComponent<RectTransform>();
            monthCalendarIcon = new MonthCalendarIcon(calendarIcon, mainStats.FlowDataCreator, 
                                mainStats.StatisticsScroll, mainStats.FlowStatistics);
            
            // Initialize image circles to background
            var circle1 = content.Find("Image/Circle 0").GetComponent<Image>();
            var circle2 = content.Find("Image/Circle 1").GetComponent<Image>();
            var circle3 = content.Find("Image/Circle 2").GetComponent<Image>();
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);
            
            // Add UI text to localization
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.MonthlyView);
            TextLocalization.Instance.AddLocalization(description, TextKeysHolder.FlowAreaStatisticsDescription);
        }

        // Play move animation of part in new page when open
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, -SetupBasePositions() * 3, 0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
        }
        
        // Play move animation of part in new page when close
        public void SetPartFromPage()
        {
            var centeredPosition = PageTransition().CurrentPage().Page.anchoredPosition.y +
                                   groupBackground.RectTransform.anchoredPosition.y;
            var direction = centeredPosition > 0 ? -1 : 1;

            groupBackgroundSync.Speed = 0.37f;
            groupBackgroundSync.TargetPosition = new Vector3(0, groupBackground.RectTransform.anchoredPosition.y - SetupBasePositions() * direction, 0);
            groupBackgroundSync.SetTByDynamic(0);
            groupBackgroundSync.Run();
        }

        // Update view of part
        public void UpdateInfo()
        {
            var flow = GetCurrentFlow();
            mainStats.FlowDataCreator.SetupFlowToGenerator(flow);
            mainStats.Update();
            UpdateStatistics();
            
            var color = FlowColorLoader.LoadColorById(flow.Color);
            iconColorCircles.UpdateColors(color);
        }
        
        // Update statistics view
        private void UpdateStatistics()
        {
            var deltaImmediately = mainStats.FlowDataCreator.DefaultStep();

            var animate = deltaImmediately > 0;
            if (animate)
            {
                deltaImmediately--;
                mainStats.FlowStatistics.ViewsLine.UpdatePositions();
                mainStats.FlowStatistics.AddStepDelta(deltaImmediately);
                monthCalendarIcon.StartMoveToDefaultStep(1);
            }
            else
            {
                mainStats.FlowStatistics.ViewsLine.UpdatePositions();
            }
        }

        // Set part to page
        public void SetToPage(PageItem pageItem)
        {
            pageItem.AddContent(groupBackground.RectTransform);
            groupBackground.RectTransform.anchoredPosition = new Vector2(-10000, 0);
        }
        
        // Set active state of part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);
        
        // Setup main position of part in page
        private float SetupBasePositions() =>  MainCanvas.RectTransform.sizeDelta.y - 
                                               AppElementsInfo.DistanceContentToBottom;
    }
}
