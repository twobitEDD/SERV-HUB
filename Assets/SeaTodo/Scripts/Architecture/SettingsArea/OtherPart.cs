using Architecture.Elements;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Part of settings page with buttons for other 
    public class OtherPart
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;

        private RectTransform rectTransform; // Rect object of part
        private const float partAnchoredPosition = 0; // Position of part
        private const float backgroundHeight = 425; // Height of background
        
        // Background component
        private readonly GroupBackground groupBackground;
        // Animation components for background
        private RectTransformSync groupBackgroundSync;

        // Bottom position of part in page
        public float GetBottomSide;

        // Components of buttons
        private LineReviewApp lineReviewApp;
        private LineConnections lineConnections;

        // Create and setup
        public OtherPart(float upperSidePosition)
        {
            // Create background
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
            // Initialize rect object of part and setup to background
            rectTransform = SceneResources.Get("Settings Other Part").GetComponent<RectTransform>();
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.SettingsOther);
            groupBackground.AddItemToBackground(rectTransform);
            rectTransform.anchoredPosition = new Vector2(0, partAnchoredPosition);

            // Find and localize title
            var title = rectTransform.Find("Title").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.SettingsOtherTitle);

            // Initialize settings buttons
            lineReviewApp = new LineReviewApp(rectTransform.Find("Review App").GetComponent<RectTransform>());
            lineConnections = new LineConnections(rectTransform.Find("Connections").GetComponent<RectTransform>());
        }
        
        // Play move animation of part in new page
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, -SetupBasePositions(), 0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            lineReviewApp.Reactivate();
            lineConnections.Reactivate();
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
        
        // Update visible of buy button
        public void UpdateAnchorPosition(float anchor)
        {
            groupBackground.UpdateVisible(backgroundHeight, anchor + AppElementsInfo.DistanceBetweenBackgrounds);
            groupBackgroundSync.PrepareToWork(true);
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Setup main position of part in page
        private float SetupBasePositions() =>  MainCanvas.RectTransform.sizeDelta.y - 
                                               AppElementsInfo.DistanceContentToBottom + backgroundHeight / 2;
    }
}
