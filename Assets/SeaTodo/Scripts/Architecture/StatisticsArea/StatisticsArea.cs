using Architecture.Data;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.StatisticsArea.ArchivedFlowUpdate;
using HomeTools.Messenger;
using HTools;
using Theming;
using UnityEngine;

namespace Architecture.StatisticsArea
{
    // Main class of Activity page
    public class StatisticsArea : IBehaviorSync
    {
        // Link to pages system
        private static PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Link to main page
        private static WorkArea.WorkArea WorkArea() => AreasLocator.Instance.WorkArea;
        
        // Parts of page
        private readonly InProgressPart inProgressPart;
        private readonly FrequencyPart frequencyPart;
        private readonly WeeksPassedPart weeksPassedPart;
        private readonly ArchivedPart archivedPart;

        // View for restore archived tasks
        public readonly ArchivedUpdateModule ArchivedUpdateModule;

        private bool started; // Initialized components flag
        
        // Create and setup
        public StatisticsArea()
        {
            // Create parts of page
            inProgressPart = new InProgressPart();
            frequencyPart = new FrequencyPart(inProgressPart.GetBottomSide);
            weeksPassedPart = new WeeksPassedPart(frequencyPart.GetBottomSide);
            archivedPart = new ArchivedPart(weeksPassedPart.GetBottomSide);
            
            // Create view for restore archived tasks
            ArchivedUpdateModule = new ArchivedUpdateModule();
        }
        
        // Setup this page to page transitions system
        public void SetupToPage()
        {
            // Update parts of page
            inProgressPart.UpdateView();
            frequencyPart.UpdateHeightAnchor(inProgressPart.BottomSidePosition);
            weeksPassedPart.UpdateHeightAnchor(frequencyPart.GetBottomSide);
            archivedPart.UpdateHeightAnchor(weeksPassedPart.GetBottomSide);
            
            // Prepare page position and page scroll borders
            var newPage = PageTransition().CurrentPage();
            var max = -archivedPart.GetBottomSide - MainCanvas.RectTransform.sizeDelta.y + AppElementsInfo.DistanceContentToBottom;
            newPage.MaxAnchor = max;
            newPage.MinAnchor = 0;
            newPage.Page.anchoredPosition = Vector2.zero;
            
            // Start animation of parts of page
            inProgressPart.SetPartToNewPage();
            frequencyPart.SetPartToNewPage();
            weeksPassedPart.SetPartToNewPage();
            archivedPart.SetPartToNewPage();

            // Activate parts of page
            inProgressPart.SetActive(true);
            frequencyPart.SetActive(true);
            weeksPassedPart.SetActive(true);
            archivedPart.SetActive(true);

            // Activate pages scroll
            PageScroll.Instance.Enabled = true;
        }

        // Setup parts to page object
        public void SetupParents()
        {
            // Get page object
            var newPage = PageTransition().CurrentPage();
            // Set parts to new page
            inProgressPart.SetToPage(newPage);
            frequencyPart.SetToPage(newPage);
            weeksPassedPart.SetToPage(newPage);
            archivedPart.SetToPage(newPage);
            // Deactivate parts of page 
            inProgressPart.SetActive(false);
            frequencyPart.SetActive(false);
            weeksPassedPart.SetActive(false);
            archivedPart.SetActive(false);
        }

        // Setup parts from page 
        public void SetupFromPage()
        {
            // Update scroll borders of main page
            WorkArea().UpdateAnchorsForScroll();
            // Play animation - move from screen
            inProgressPart.SetPartFromPage();
            frequencyPart.SetPartFromPage();
            weeksPassedPart.SetPartFromPage();
            archivedPart.SetPartFromPage();
        }

        // Setup parts of page to page pool
        public void SetupToPool()
        {
            var newPage = PageTransition().PagePool;
            inProgressPart.SetToPage(newPage);
            frequencyPart.SetToPage(newPage);
            weeksPassedPart.SetToPage(newPage);
            archivedPart.SetToPage(newPage);

            ActionsAfterClose();
        }

        // Send messages to messenger when page closed
        private void ActionsAfterClose()
        {
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaByNewDayImmediately);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
        }

        // Update part of archived tasks
        public void UpdateArchivedPart()
        {
            archivedPart.UpdateAfterRemove();
        }

        public void Start() { }

        public void Update()
        {
            LateStart();
        }

        // Initialize parts of page
        private void LateStart()
        {
            if (started)
                return;

            inProgressPart.SetupMovableParameters();   
            inProgressPart.InitializeContent();
            frequencyPart.SetupMovableParameters();
            frequencyPart.InitializeContent();
            weeksPassedPart.SetupMovableParameters();
            weeksPassedPart.InitializeContent();
            archivedPart.SetupMovableParameters();
            archivedPart.InitializeContent();

            // Setup colors of UI in this page
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.StatisticsArea);

            started = true;
        }
    }
}
