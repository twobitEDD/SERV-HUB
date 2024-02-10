using System;
using Architecture.Data;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Source.Calendar;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.NormalView
{
    // Component of estimate date part in task normal view
    public class PartEstimate
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Current flow for view
        private Flow GetCurrentFlow() => AreasLocator.Instance.FlowViewArea.CurrentFlow;
        
        // Background height
        private const float backgroundHeight = 647;
        // Position of content
        private const float contentPositionFromTop = 170;
        // Background component
        private readonly GroupBackground groupBackground;
        // Animation component for background
        private RectTransformSync groupBackgroundSync;
        
        // UI components
        private Text title;
        private Text description;
        private Text dateStarted;
        private Text dateFinished;
        private Text daysLeft;
        private RectTransform lineDone;
        private Image lineDoneImage;
        private RectTransform lineEstimate;
        // Circles that placed around icon
        private IconColorCircles iconColorCircles;
        // Bottom position of part in page
        public readonly float GetBottomSide;

        // Create and setup
        public PartEstimate(float upperSidePosition)
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
            var content = SceneResources.Get("ViewFlow Estimate Part").GetComponent<RectTransform>();
            groupBackground.AddItemToBackground(content);
            content.anchoredPosition = new Vector2(0, groupBackground.RectTransform.sizeDelta.y / 2 - contentPositionFromTop);

            // Find UI elements
            title = content.Find("Title").GetComponent<Text>();
            description = content.Find("Description").GetComponent<Text>();
            dateStarted = content.Find("Start").GetComponent<Text>();
            dateFinished = content.Find("Estimate").GetComponent<Text>();
            daysLeft = content.Find("Left days").GetComponent<Text>();
            lineDone = content.Find("Line done").GetComponent<RectTransform>();
            lineEstimate = content.Find("Line estimate").GetComponent<RectTransform>();
            lineDoneImage = lineDone.GetComponent<Image>();
            
            // Initialize image circles to background
            var circle1 = content.Find("Image/Circle 0").GetComponent<Image>();
            var circle2 = content.Find("Image/Circle 1").GetComponent<Image>();
            var circle3 = content.Find("Image/Circle 2").GetComponent<Image>();
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);
            
            // Add UI text to localization
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.EstimatedFinishDate);
            TextLocalization.Instance.AddLocalization(description, TextKeysHolder.FlowAreaEstimatedDateDescription);
        }

        // Play move animation of part in new page when open
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, -SetupBasePositions(), 0);
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
            var color = FlowColorLoader.LoadColorById(flow.Color);

            var startedDay = flow.GetStartedDay();
            var startedDayConverted = new DateTime(startedDay.Year, startedDay.Month, startedDay.Day);
            
            var finishedDay = EstimateFinishDate.EstimateByFlow(flow);
            var finishedDayConverted = new DateTime(finishedDay.Year, finishedDay.Month, finishedDay.Day);

            dateStarted.text = $"{startedDayConverted.Day} {CalendarNames.GetMonthShortName(startedDayConverted.Month)} {startedDayConverted.Year}";
            dateFinished.text = $"≈ {finishedDayConverted.Day} {CalendarNames.GetMonthShortName(finishedDayConverted.Month)} {finishedDayConverted.Year}";

            var allTime = finishedDayConverted - startedDayConverted;
            var passedTime = DateTime.Now - startedDayConverted;
            var percentage = (float)(passedTime.TotalHours / allTime.TotalHours);

            var leftDaysInt = (int) (allTime - passedTime).TotalDays;
            if (leftDaysInt < 0)
                leftDaysInt = 0;
            
            daysLeft.text = $"{TextLocalization.GetLocalization(TextKeysHolder.DaysLeft)} {leftDaysInt}";
            SetupLineDone(percentage);
            SetupLineEstimate(percentage);

            dateStarted.color = color;
            lineDoneImage.color = color;
            
            iconColorCircles.UpdateColors(color);
        }

        // Update line of done percentage view 
        private void SetupLineDone(float percentage)
        {
            var maxWidth = 2848;
            var minWidth = lineDone.rect.height;
            
            var width = minWidth + (maxWidth - minWidth) * percentage;
            
            lineDone.sizeDelta = new Vector2(width, minWidth);

            var minPosition = -149f;
            var maxPosition = 0f;
            
            lineDone.anchoredPosition = new Vector2(Mathf.Lerp(minPosition, maxPosition, percentage), 
                lineDone.anchoredPosition.y);
        }
        
        // Update line of estimated percentage view 
        private void SetupLineEstimate(float percentage)
        {
            percentage = Mathf.Clamp(percentage, 0f, 1f);
            
            percentage += 0.04f;
            
            var maxWidth = 2848;
            var minWidth = lineDone.rect.height;
            
            var width = minWidth + (maxWidth - minWidth) * (1 - percentage);
            
            lineEstimate.sizeDelta = new Vector2(width, minWidth);

            var minPosition = 0f;
            var maxPosition = 149f;
            
            lineEstimate.anchoredPosition = new Vector2(Mathf.Lerp(minPosition, maxPosition, percentage), 
                lineEstimate.anchoredPosition.y);
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
