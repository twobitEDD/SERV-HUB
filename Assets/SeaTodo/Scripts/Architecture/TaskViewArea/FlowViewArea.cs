using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TaskViewArea.FinishedView;
using Architecture.TaskViewArea.NormalView;
using HomeTools.Messenger;
using HTools;
using MainActivity.AppBar;
using Theming;
using UnityEngine;

namespace Architecture.TaskViewArea
{
    // Main class of Task view page
    public class FlowViewArea : IBehaviorSync
    {
        // Link to main App Bar component
        private AppBar AppBar() => AreasLocator.Instance.AppBar;
        // Link to main page
        private WorkArea.WorkArea WorkArea() => AreasLocator.Instance.WorkArea;
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;

        // Page modes
        // Normal task view
        private readonly ViewNormal viewNormal;
        // Finished task view
        private readonly ViewFinished viewFinished;
        
        // Active task view
        private IFlowView activeView;
        private bool completedProgressActive; // Finished task flag
        public bool ProgressCompleted { private set; get; } // Finished task progress flag

        public Flow CurrentFlow { get; private set; } // Current task in view
        
        private bool started; // Flag Initialized view
        
        // Create and setup view modes
        public FlowViewArea()
        {
            viewNormal = new ViewNormal(this);
            viewFinished = new ViewFinished(this);
        }

        // Setup parts to page object
        public void SetupParents()
        {
            // Get page object
            var newPage = PageTransition().CurrentPage();
            // Set current part to new page
            activeView.SetupParents(newPage);
        }
        
        // Setup this page to page transitions system
        public void SetupToPage()
        {
            // Prepare page position and page scroll borders
            var newPage = PageTransition().CurrentPage();
            var max = -activeView.GetBottomSide() - MainCanvas.RectTransform.sizeDelta.y + AppElementsInfo.DistanceContentToBottom;
            newPage.MaxAnchor = max;
            newPage.MinAnchor = 0;
            newPage.Page.anchoredPosition = Vector2.zero;
            
            // Setup current mode to page
            activeView.SetupToPage();
        }
        
        // Setup parts from page
        public void SetupFromPage()
        {
            // Update scroll borders of main page
            WorkArea().UpdateAnchorsForScroll();
            // Play animation - move from screen
            activeView.SetupFromPage();
        }
        
        // Setup parts of page to page pool
        public void SetupToPool()
        {
            var newPage = PageTransition().PagePool;
            activeView.SetupToPool(newPage);

            ActionsAfterClose();
        }
        
        // Send messages to messenger when page closed
        private void ActionsAfterClose()
        {
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaByNewDayImmediately);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
        }

        // Setup task and update view
        public void SetupCurrentFlow(Flow flow)
        {
            CurrentFlow = flow;
            UpdateByCurrentFlow();
        }

        // Update active mode by task
        public void UpdateByCurrentFlow()
        {
            completedProgressActive = EstimateFinishDate.CompletedProgress(CurrentFlow);
            ProgressCompleted = completedProgressActive;
            if (!completedProgressActive)
                completedProgressActive = CurrentFlow.Finished;
            
            activeView = completedProgressActive ? (IFlowView)viewFinished : (IFlowView)viewNormal;
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
            
            viewNormal.LateStart();
            viewFinished.LateStart();

            // Setup colors of UI in this page
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.FlowViewArea);

            started = true;
        }

        // Mark task as completed in normal view mode
        public void MarkedAsCompleted()
        {
            SetupToPool();
            
            CurrentFlow.Finished = true;
            CurrentFlow.DateFinished = Calendar.Today.HomeDayToInt();

            PageTransition().CurrentPage().Page.anchoredPosition = Vector2.zero;
            
            activeView = viewFinished;
            SetupParents();
            SetupToPage();
        }
        
        // Setup task to progress when task in finished view mode
        public void MarkedAsInProgress()
        {
            SetupToPool();
            
            CurrentFlow.Finished = false;
            CurrentFlow.DateFinished = 0;

            PageTransition().CurrentPage().Page.anchoredPosition = Vector2.zero;
            
            activeView = viewNormal;
            SetupParents();
            SetupToPage();
        }
        
        // Archive task in finished view mode
        public void ArchiveTask()
        {
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonFromEdit);
            PageTransitionTemplates.ClosePageToWorkArea(SetupFromPage, 5, SetupToPool);
            
            ArchiveFlowData();
            SetupFlows();
            AppBar().CloseViewModeState();
        }
        
        // Remove task in normal view mode
        public void RemoveTask()
        {
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonFromEdit);
            PageTransitionTemplates.ClosePageToWorkArea(SetupFromPage, 5, SetupToPool);

            RemoveFlowData();
            SetupFlows();
            AppBar().CloseViewModeState();
        }
        
        // Update tasks when task moved to archived tasks or removed
        private void SetupFlows()
        {
            WorkArea().UpdateFlows();
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
        }

        // Move task to archived tasks
        private void ArchiveFlowData()
        {
            FlowCreator.ArchiveFlowInCurrentGroup(CurrentFlow);
            FlowCreator.UpdateRemindersWhenStatusChanged(CurrentFlow);
        }
        
        // Remove task
        private void RemoveFlowData()
        {
            FlowCreator.RemoveFlowInCurrentGroup(CurrentFlow);
            FlowCreator.UpdateRemindersWhenStatusChanged(CurrentFlow);
        }
    }
}
