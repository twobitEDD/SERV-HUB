using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.NormalView
{
    // Component of progress part in task normal view
    public class PartProgress
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Current flow for view
        private Flow GetCurrentFlow() => AreasLocator.Instance.FlowViewArea.CurrentFlow;
        
        // Background height
        private const float backgroundHeight = 967;
        // Position of content
        private const float contentPositionFromTop = 450;
        // Background component
        private readonly GroupBackground groupBackground;
        // Animation component for background
        private RectTransformSync groupBackgroundSync;
        
        // UI components
        private Text title;
        private Text description;
        private Text progressPercentage;
        private Text goal;
        private Text completed;
        private Rect line;
        private RectTransform lineProgress;
        private Image lineProgressImage;

        // Circles that placed around icon
        private IconColorCircles iconColorCircles;
        // Bottom position of part in page
        public readonly float GetBottomSide;

        // Create and setup
        public PartProgress()
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add part to pool
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background height and Y position
            groupBackground.UpdateVisible(backgroundHeight, 207);
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
            var content = SceneResources.Get("ViewFlow Progress Part").GetComponent<RectTransform>();
            groupBackground.AddItemToBackground(content);
            content.anchoredPosition = new Vector2(0, groupBackground.RectTransform.sizeDelta.y / 2 - contentPositionFromTop);

            // Find UI elements
            title = content.Find("Title").GetComponent<Text>();
            description = content.Find("Description").GetComponent<Text>();
            progressPercentage = content.Find("Percentage").GetComponent<Text>();
            goal = content.Find("Goal").GetComponent<Text>();
            completed = content.Find("Completed").GetComponent<Text>();
            line = content.Find("Line Back").GetComponent<RectTransform>().rect;
            lineProgress = content.Find("Line Progress").GetComponent<RectTransform>();
            lineProgressImage = lineProgress.GetComponent<Image>();

            // Initialize image circles to background
            var circle1 = content.Find("Image/Circle 0").GetComponent<Image>();
            var circle2 = content.Find("Image/Circle 1").GetComponent<Image>();
            var circle3 = content.Find("Image/Circle 2").GetComponent<Image>();
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);
            
            // Add UI text to localization
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.TaskProgress);
            TextLocalization.Instance.AddLocalization(description, TextKeysHolder.FlowAreaInProgressDescription);
        }

        // Play move animation of part in new page when open
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, SetupBasePositions(), 0);
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

            var progressInt = flow.GetIntProgress();
            
            var inGoalInt = FlowInfoAll.GetGoalByOriginFlowInt(flow.Type, flow.GoalInt);
            
            var floatPercentage = progressInt / (float)inGoalInt;
            
            var percentage = $"{(floatPercentage * 100):0.##}";
            
            goal.text = FlowInfoAll.GetViewGoalByOriginFlow(flow.Type, inGoalInt, goal.fontSize);
            var completedText = FlowInfoAll.GetWorkProgressFlow(flow.Type, progressInt, completed.fontSize, false);
            completed.text = $"{completedText}";
            progressPercentage.text = $"{percentage}%";
            
            SetupLineProgress(floatPercentage);

            lineProgressImage.color = color;
            
            iconColorCircles.UpdateColors(color);
        }

        // Setup progress of task line view 
        private void SetupLineProgress(float percentage)
        {
            percentage = Mathf.Clamp(percentage, 0f, 1f);
            
            var maxWidth = line.width;
            var minWidth = line.height;
            var width = minWidth + (maxWidth - minWidth) * percentage;
            
            lineProgress.sizeDelta = new Vector2(width, minWidth);

            var minPosition = -331f;
            var maxPosition = 0f;
            
            lineProgress.anchoredPosition = new Vector2(Mathf.Lerp(minPosition, maxPosition, percentage), 
                lineProgress.anchoredPosition.y);
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
        private float SetupBasePositions() => MainCanvas.RectTransform.sizeDelta.y - 
                                              AppElementsInfo.DistanceContentToBottom + backgroundHeight / 2;
    }
}
