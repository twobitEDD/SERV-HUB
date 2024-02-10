using System.Linq;
using Architecture.Data;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.StatisticsArea.InProgress;
using Architecture.TextHolder;
using HomeTools.Source.Calendar;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.StatisticsArea
{
    // Class of tasks in progress part
    public class InProgressPart
    {
        private RectTransform rectTransform; // Rect of part
        private const float partAnchoredPosition = -430; // Position of part in page
        // Height of background
        private const float backgroundHeight = 877;
        // Component that contains tasks line views
        private InProgressMain inProgressMain;

        private Text title; // Title of part
        private Text description; // Description of part
        private Text year; // Year text
        private Text date; // Date text

        private GameObject emptyObject; // Object with UI that shows when no active tasks
        private Text emptyDescription; // Text that shows when no active tasks

        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Background of part
        private readonly GroupBackground groupBackground;
        // Animation component for background
        private RectTransformSync groupBackgroundSync;
        // Bottom position of part in page without tasks in part
        public readonly float GetBottomSide;
        // Bottom position of part in page
        public float BottomSidePosition { private set; get; }
        
        // Create and setup
        public InProgressPart()
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add part to pool
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background height and Y position
            groupBackground.UpdateVisible(backgroundHeight, 170);
            // Calculate bottom side
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Initialize part
        public void InitializeContent()
        {
            // Find part rect
            rectTransform = SceneResources.Get("Statistics InProgress Part").GetComponent<RectTransform>();
            // Setup part to background
            groupBackground.AddItemToBackground(rectTransform);
            // Setup position of part in background
            rectTransform.anchoredPosition = new Vector2(0, partAnchoredPosition);
            // Find UI components
            title = rectTransform.Find("Title").GetComponent<Text>();
            description = rectTransform.Find("Description").GetComponent<Text>();
            year = rectTransform.Find("Year").GetComponent<Text>();
            date = rectTransform.Find("Date").GetComponent<Text>();
            emptyObject = rectTransform.Find("Empty Flow").gameObject;
            emptyDescription = emptyObject.transform.Find("Name").GetComponent<Text>();
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.StatisticsArea);
            // Setup content
            SetupContent();
        }
        
        // Setup content
        private void SetupContent()
        {
            // Localize UI text
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.InProgress);
            TextLocalization.Instance.AddLocalization(description, TextKeysHolder.ActivityAreaInProgressDescription);
            TextLocalization.Instance.AddLocalization(emptyDescription, TextKeysHolder.Empty);
            // Create component of tasks progress view
            inProgressMain = new InProgressMain(rectTransform);
        }

        // Create animation component for background part
        public void SetupMovableParameters()
        {
            groupBackgroundSync = new RectTransformSync();
            groupBackgroundSync.SetRectTransformSync(groupBackground.RectTransform);
            groupBackgroundSync.TargetPosition = new Vector3(0, SetupBasePositions(), 0);
            groupBackgroundSync.Speed = 0.17f;
            groupBackgroundSync.SpeedMode = RectTransformSync.Mode.Lerp;
            groupBackgroundSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(groupBackgroundSync);
        }
        
        // Setup main position of part in page
        private float SetupBasePositions() => -MainCanvas.RectTransform.sizeDelta.y - AppElementsInfo.DistanceContentToBottom;

        // Update part view
        public void UpdateView()
        {
            // Get active tasks
            var flows = AppData.GetCurrentGroup().Flows.OrderBy(e=>e.Order).ToArray();
            // Update tasks view
            inProgressMain.UpdateView(flows);
            // Update date 
            UpdateViewWhenOpened();

            // Update background by count of tasks
            var additionalHeight = flows.Length * InProgressMain.DistanceBetweenFlows;
            // Try to enable UI of empty tasks list
            UpdateNoInProgressView(additionalHeight < InProgressMain.DistanceBetweenFlows);

            if (additionalHeight < InProgressMain.DistanceBetweenFlows)
                additionalHeight = 107f;
            
            BottomSidePosition = GetBottomSide - additionalHeight;
            groupBackground.UpdateVisible(backgroundHeight + additionalHeight, 170);
        }
        
        // Play move animation of part in new page when open
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.TargetPosition = new Vector3(0, -SetupBasePositions() * 2, 0);
            groupBackgroundSync.SetDefaultT(0);
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
            groupBackgroundSync.TargetPosition = new Vector3(0, groupBackground.RectTransform.anchoredPosition.y - MainCanvas.RectTransform.sizeDelta.y * direction, 0);
            groupBackgroundSync.SetTByDynamic(0);
            groupBackgroundSync.Run();
        }

        // Set part to page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(groupBackground.RectTransform);
        
        // Set active state of  part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);

        // Update date
        private void UpdateViewWhenOpened()
        {
            var dateTime = System.DateTime.Now;
            year.text = dateTime.Year.ToString();
            date.text = $"{dateTime.Day} {CalendarNames.GetMonthShortName(dateTime.Month)}";
        }

        // Set activity of UI of empty tasks list
        private void UpdateNoInProgressView(bool active)
        {
            emptyObject.SetActive(active);
            emptyDescription.enabled = active;
        }
    }
}
