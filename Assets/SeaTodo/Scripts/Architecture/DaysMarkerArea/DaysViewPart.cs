using Architecture.AboutSeaCalendar;
using Architecture.Components;
using Architecture.DaysMarkerArea.DaysView;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TextHolder;
using Architecture.TutorialArea;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea
{
    // Part of Sea Calendar with days list
    public class DaysViewPart : IBehaviorSync
    {
        // Link to page About Sea Calendar
        private ViewAboutSeaCalendar ViewAboutSeaCalendar => AreasLocator.Instance.ViewAboutSeaCalendar;
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;

        private RectTransform rectTransform; // Rect of part
        private const float partAnchoredPosition = -327; // Position of part in background
        private const float backgroundHeight = 2060; // Height of background image

        private readonly GroupBackground groupBackground; // Background of part
        // Animation component of rect for background
        private RectTransformSync groupBackgroundSync; 
        private MainButtonJob tipButton; // Button to About Sea Calendar
        private RectTransform aboutParent; // Parent for tip button
        
        private MarkersYear markersYear; // Component with days lines view
        private bool autoOpenTip; // Info about auto open of tutorial
        public bool WaitToTryOpenTip; // Lock auto open of tutorial
        
        // Bottom position in page of this part
        public readonly float GetBottomSide;

        // Create main
        public DaysViewPart()
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add background to page pool object
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background rect params: height and position in parent
            groupBackground.UpdateVisible(backgroundHeight, 77);
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
            rectTransform = SceneResources.Get("DaysMarker Days Part").GetComponent<RectTransform>();
            // Add to synchronization with new Behaviour Component
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.MarkersAreaDays);
            SyncWithBehaviour.Instance.AddObserver(this, AppSyncAnchors.MarkersAreaDays);
            // Add part to background
            groupBackground.AddItemToBackground(rectTransform);
            // Setup position of part in background
            rectTransform.anchoredPosition = new Vector2(0, partAnchoredPosition);
            // Create component on year view in this part
            markersYear = new MarkersYear(rectTransform);
            
            // Find tip button object
            aboutParent = rectTransform.Find("About").GetComponent<RectTransform>();
            
            // Create button component of tip
            var tipHandler = aboutParent.Find("About Handler").gameObject;
            tipButton = new MainButtonJob(aboutParent, SeaCalendarOpenTip, tipHandler);
            tipButton.AttachToSyncWithBehaviour(AppSyncAnchors.MarkersAreaDays);
            tipButton.Reactivate();
            
            // Find and localize text of tip button
            var aboutText = aboutParent.Find("Text").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(aboutText, TextKeysHolder.SeaCalendarAbout);
            
            // Find and localize title of part
            var title = rectTransform.Find("Title").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.MarkersAreaDaysTitle);
            
            // Find and localize description of part
            var description = rectTransform.Find("Description").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(description, TextKeysHolder.MarkersAreaDaysDescription);

            // Setup flag about auto open About Sea Calendar
            autoOpenTip = TutorialInfo.AutoCalendarTip();
        }

        // Start animation of background when Open Sea Calendar
        public void SetPartToNewPage()
        {
            // Prepare and start of animation component
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, SetupBasePositions() * 3, 0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            WaitToTryOpenTip = true; // Begin wait for auto open About Sea Calendar
            markersYear.DefaultUpdate(); // Update days view by data and year
        }

        // Start animation of background when close Sea Calendar 
        public void SetPartFromPage()
        {
            groupBackgroundSync.Speed = 0.37f;
            groupBackgroundSync.TargetPosition = new Vector3(0, groupBackground.RectTransform.anchoredPosition.y + SetupBasePositions(), 0);
            groupBackgroundSync.SetTByDynamic(0);
            groupBackgroundSync.Run();
        }
        
        // Sea background with part in new page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(groupBackground.RectTransform);
        
        // Setup activity of part
        public void SetActive(bool active) =>  groupBackground.RectTransform.gameObject.SetActive(active);

        public void Start() { }

        // Try auto open About Sea Calendar
        public void Update()
        {
            if (!WaitToTryOpenTip)
                return;

            if (groupBackgroundSync.LocalT < 0.998f)
                return;

            TryOpenTip();
            WaitToTryOpenTip = false;
        }

        // Try open About Sea Calendar view
        private void TryOpenTip()
        {
            if (!autoOpenTip) 
                return;
            
            autoOpenTip = false;
            SeaCalendarOpenTip();
        }
        
        // Calculate main position of part background in page
        private float SetupBasePositions() =>  MainCanvas.RectTransform.sizeDelta.y - 
                                               AppElementsInfo.DistanceContentToBottom + backgroundHeight / 2;

        // Open view about Sea Calendar
        private void SeaCalendarOpenTip()
        {
            tipButton.Reactivate(); // Reset tip button state
            ViewAboutSeaCalendar.Open(); // Open view About Sea Calendar
        }
    }
}
