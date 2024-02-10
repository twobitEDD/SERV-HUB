using System.Collections.Generic;
using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Elements;
using Architecture.ModuleTrackFlow;
using Architecture.Pages;
using HomeTools.Messenger;
using HTools;
using InternalTheming;
using MainActivity.AppBar;
using Modules.Notifications;
using Theming;

namespace Architecture.CreateTaskArea
{
    // Main class of create task page
    public class CreateTaskArea : IBehaviorSync
    {
        // Link to track flow module
        private static TrackFlowModule TrackFlowModule() => AreasLocator.Instance.TrackFlowModule;
        // Link to reminders module
        private ModuleReminders.RemindersModule RemindersModule() => AreasLocator.Instance.RemindersModule;
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Link to app bar systems
        private AppBar AppBar() => AreasLocator.Instance.AppBar;
        // Link to main page
        private WorkArea.WorkArea WorkArea() => AreasLocator.Instance.WorkArea;

        // First part - choose task type
        private readonly ChooseTaskTypePart chooseTaskTypePart;
        // Second part - choose reminders
        private readonly ChooseRemindersPart chooseRemindersPart;
        // Create task button
        private readonly CreateTaskDoneButton createTaskDoneButton;
        // Created page flag
        private bool started; 

        // Create page
        public CreateTaskArea()
        {
            // Create first page part
            chooseTaskTypePart = new ChooseTaskTypePart();
            // Create second page part
            chooseRemindersPart = new ChooseRemindersPart(chooseTaskTypePart.GetBottomSide);
            // Create button and attach to update calls
            createTaskDoneButton = new CreateTaskDoneButton(chooseRemindersPart.GetBottomSide, SetupNewFlow);
            SyncWithBehaviour.Instance.AddObserver(createTaskDoneButton);
        }
        
        // Setup this page to page transitions system
        public void SetupToPage()
        {
            // Prepare page position and page scroll borders
            var newPage = PageTransition().CurrentPage();
            var max = -createTaskDoneButton.GetBottomSide - MainCanvas.RectTransform.sizeDelta.y + AppElementsInfo.DistanceContentToBottom;
            newPage.MaxAnchor = max;
            newPage.MinAnchor = 0;
            
            // Prepare parts in page object and start animation
            chooseTaskTypePart.SetPartToNewPage();
            chooseRemindersPart.SetPartToNewPage();
            createTaskDoneButton.SetPartToNewPage();
            chooseTaskTypePart.SetupCurrentType();
            
            // Activate parts
            chooseTaskTypePart.SetActive(true);
            chooseRemindersPart.SetActive(true);
            
            // Update color of items in Track module
            TrackFlowModule().SetColorToArea(ThemeLoader.GetCurrentTheme().TrackFlowAreaItems);
            // Enable scroll
            PageScroll.Instance.Enabled = true;
        }

        // Setup parts to page object
        public void SetupParents()
        {
            // Get page object
            var newPage = PageTransition().CurrentPage();
            // Set parts to new page
            chooseTaskTypePart.SetToPage(newPage);
            chooseRemindersPart.SetToPage(newPage);
            createTaskDoneButton.SetToPage(newPage);
            // Deactivate parts of page 
            chooseTaskTypePart.SetActive(false);
            chooseRemindersPart.SetActive(false);
        }

        // Setup parts from page 
        public void SetupFromPage()
        {
            // Update scroll borders of main page
            WorkArea().UpdateAnchorsForScroll();
            
            // Play animation - move from screen
            chooseTaskTypePart.SetPartFromPage();
            chooseRemindersPart.SetPartFromPage();
            createTaskDoneButton.SetPartFromPage();
        }

        // Setup parts of page to page pool
        public void SetupToPool()
        {
            // Get page pool
            var newPage = PageTransition().PagePool;
            // Setup part to pool
            chooseTaskTypePart.SetToPage(newPage);
            chooseRemindersPart.SetToPage(newPage);
            createTaskDoneButton.SetToPage(newPage);

            // Send messages when closed
            ActionsAfterClose();
        }

        // Send messages when closed page
        private void ActionsAfterClose()
        {
            // Update work area with new task
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaByNewDayImmediately);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
        }
        
        
        public void Start()
        {
        }

        public void Update()
        {
            LateStart();
        }

        // Initialize parts of page
        private void LateStart()
        {
            if (started)
                return;
            
            chooseTaskTypePart.SetupMovableParameters();
            chooseTaskTypePart.SetupBaseElements();
            chooseRemindersPart.SetupMovableParameters();
            chooseRemindersPart.SetupBaseElements();
            createTaskDoneButton.SetupMovableParameters();
            
            // Setup colors of UI in this page
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.CreateFlowArea);

            started = true;
        }

        // Create new task
        private void SetupNewFlow()
        {
            // Create new task
            BuildNewFlow();
            // Update tasks view in main page
            WorkArea().UpdateFlows();
            // Send message about task created
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
        }

        // Build new task
        private void BuildNewFlow()
        {
            var flow = FlowCreator.CreateFlow(); // Create new task object
            flow.Name = AppBar().BarFlowInputName.GetName(); // Setup name
            flow.Icon = (byte) AppBar().BarFlowIcon.GetIconId(); // Setup icon
            flow.Type = chooseTaskTypePart.GetFlowType(); // Setup type of task
            flow.Color = (byte) AppBar().BarFlowIcon.GetColorId(); // Setup color
            flow.Order = (byte) (AppData.GetCurrentGroup().Flows.Length - 1); // Setup order in tasks list
            flow.GoalInt = chooseTaskTypePart.GetFlowGoal(); // Setup goal of task
            flow.DateStarted = Calendar.Today.HomeDayToInt(); // Save started date
            flow.GoalData = new Dictionary<int, int>(); // Create dictionary for tracks
            RemindersModule().SetupRemindersToFlow(flow); // Create reminders for new task

            // Add flow to creator
            FlowCreator.AddFlowToCurrentGroup(flow);
            // Update notifications with new task
            AppNotifications.UpdateNotifications();
        }
    }
}
