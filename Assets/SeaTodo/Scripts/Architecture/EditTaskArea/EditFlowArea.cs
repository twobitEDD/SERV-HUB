using Architecture.Data;
using Architecture.Elements;
using Architecture.ModuleReminders;
using Architecture.Pages;
using HomeTools.Messenger;
using HTools;
using MainActivity.AppBar;
using Modules.Notifications;
using Theming;
using UnityEngine;

namespace Architecture.EditTaskArea
{
    // The class that is responsible for the work of the task edit page
    public class EditFlowArea : IBehaviorSync
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Link to view of setup reminders 
        private RemindersModule RemindersModule() => AreasLocator.Instance.RemindersModule;
        // Link to app bar component
        private AppBar AppBar() => AreasLocator.Instance.AppBar;
        // Link to main page
        private WorkArea.WorkArea WorkArea() => AreasLocator.Instance.WorkArea;
        // Component that contains view of edit task page
        private readonly UpdateFlowPart updateFlowPart;
        // The task being edited
        public Flow CurrentFlow;
        // Created page flag
        private bool started;

        // Create main component of page
        public EditFlowArea()
        {
            updateFlowPart = new UpdateFlowPart();
        }
        
        // Setup this page to page transitions system
        public void SetupToPage()
        {
            // Prepare page position and page scroll borders
            var newPage = PageTransition().CurrentPage();
            var max = -updateFlowPart.GetBottomSide - MainCanvas.RectTransform.sizeDelta.y + AppElementsInfo.DistanceContentToBottom;
            newPage.MaxAnchor = max;
            newPage.MinAnchor = 0;
            newPage.Page.anchoredPosition = Vector2.zero;
            
            // Update page view by task
            updateFlowPart.SetPartToNewPage();
            updateFlowPart.SetupTrackFlow(CurrentFlow);
            updateFlowPart.CreateRemindersSession(CurrentFlow);

            // Set activity of view of page
            updateFlowPart.SetActive(true);
            // Enable scroll of page
            PageScroll.Instance.Enabled = true;
        }

        // Setup parts to page object
        public void SetupParents()
        {
            // Get page object
            var newPage = PageTransition().CurrentPage();
            // Set parts to new page
            updateFlowPart.SetToPage(newPage);
            // Deactivate parts of page
            updateFlowPart.SetActive(false);
        }

        // Refresh task data after editing
        public void UpdateFlow()
        {
            // Update task goal
            CurrentFlow.GoalInt = updateFlowPart.GetGoal();
            // Update task name
            CurrentFlow.Name = AppBar().BarFlowInputName.GetName();
            // Update task icon
            CurrentFlow.Icon = (byte) AppBar().BarFlowIcon.GetIconId();
            // Update task icon color
            CurrentFlow.Color = (byte) AppBar().BarFlowIcon.GetColorId();
            // Setup updated reminders
            RemindersModule().SetupRemindersToFlow(CurrentFlow);
            // Update title name of task in page
            AppBar().AppBarViewMode.FlowName.text = CurrentFlow.Name;
            // Setup update notifications
            AppNotifications.UpdateNotifications();
            // Send message that flow updated
            MainMessenger<int>.SendMessage(AppMessagesConst.FlowUpdated, CurrentFlow.Id);
        }
        
        // Setup parts from page 
        public void SetupFromPage()
        {
            // Update scroll borders of main page
            WorkArea().UpdateAnchorsForScroll();
            // Play animation - move from screen
            updateFlowPart.SetPartFromPage();
        }

        // Setup parts of page to page pool
        public void SetupToPool()
        {
            var newPage = PageTransition().PagePool;
            updateFlowPart.SetToPage(newPage);
        }

        // Setup task to page
        public void SetupCurrentFlow(Flow flow) => CurrentFlow = flow;

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

            updateFlowPart.SetupMovableParameters();   
            updateFlowPart.SetupBaseElements();

            // Setup colors of UI in this page
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.CreateFlowArea);

            started = true;
        }
    }
}
