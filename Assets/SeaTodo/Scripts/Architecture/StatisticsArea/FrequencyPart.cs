using Architecture.Elements;
using Architecture.Pages;
using Architecture.StatisticsArea.Frequency;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace Architecture.StatisticsArea
{
    // Class of frequency of user activity part
    public class FrequencyPart
    {
        // Link to pages system
        private static PageTransition PageTransition() => AreasLocator.Instance.PageTransition;

        private RectTransform rectTransform; // Rect of part
        private const float partAnchoredPosition = -190; // Position of part in page
        private const float backgroundHeight = 617; // Height of background

        private Text title; // Title of part
        private Text description; // Description of part

        // Background of part
        private readonly GroupBackground groupBackground;
        // Animation component for background
        private RectTransformSync groupBackgroundSync;

        // View of frequency of user activity
        private FrequencyView frequencyView;
        
        // Additional UI elements of part
        private Text dateStarted;
        private Text dateFinished;
        private Text daysLeft;
        private SVGImage speedIcon;
        private RectTransform lineDone;
        private RectTransform lineEstimate;

        // Bottom position of part in page
        public float GetBottomSide;

        // Create and setup
        public FrequencyPart(float upperSidePosition)
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add part to pool
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background height and Y position
            groupBackground.UpdateVisible(backgroundHeight, upperSidePosition + AppElementsInfo.DistanceBetweenBackgrounds);
            // Calculate bottom side
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
            // Add action to messenger when frequency updated
            MainMessenger<(int,int)>.AddMember(DynamicUpdate, AppMessagesConst.TrackedInStatisticArea);
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
        
        // Initialize part
        public void InitializeContent()
        {
            // Find part rect
            rectTransform = SceneResources.Get("Statistics Frequency Part").GetComponent<RectTransform>();
            // Setup part to background
            groupBackground.AddItemToBackground(rectTransform);
            // Setup position of part in background
            rectTransform.anchoredPosition = new Vector2(0, partAnchoredPosition);
            // Find UI components
            title = rectTransform.Find("Title").GetComponent<Text>();
            description = rectTransform.Find("Description").GetComponent<Text>();
            // Create frequency view
            frequencyView = new FrequencyView(rectTransform);
            // Setup content
            SetupContent();
        }

        // Update position in page
        public void UpdateHeightAnchor(float position)
        {
            groupBackground.UpdateVisible(backgroundHeight, position + AppElementsInfo.DistanceBetweenBackgrounds);
            groupBackgroundSync.PrepareToWork();
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Setup content
        private void SetupContent()
        {
            // Localize UI text
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.Frequency);
            TextLocalization.Instance.AddLocalization(description, TextKeysHolder.ActivityAreaFrequencyDescription);
        }

        // Update frequency
        private void DynamicUpdate((int i, int j) empty)
        {
            frequencyView.UpdateDynamic();
        }

        // Play move animation of part in new page when open
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, -SetupBasePositions(), 0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            frequencyView.SetupViewByTasks();
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
        
        // Set part to page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(groupBackground.RectTransform);
        // Set active state of part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);
        // Setup main position of part in page
        private float SetupBasePositions() =>  MainCanvas.RectTransform.sizeDelta.y - 
                                               AppElementsInfo.DistanceContentToBottom;
    }
}
