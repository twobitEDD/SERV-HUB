using Architecture.CalendarModule;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.Pages;
using HomeTools.Messenger;
using HTools;
using MainActivity.AppBar;
using Theming;

namespace Architecture.WorkArea
{
    // Main class of Main page
    public class WorkArea : IBehaviorSync
    {
        // Link to current page
        private static PageItem CurrentPage() => AreasLocator.Instance.PageTransition.CurrentPage();
        private PageItem currentPageSaved; // Saved current page
        // Link to app bar main component
        private AppBar AppBarElements() => AreasLocator.Instance.AppBar;
        
        // First part of page (with tasks list)
        private MainPart mainPart;
        // Second part of page (with weeks stats)
        private SecondaryPart secondaryPart;
        
        private bool started; // Initialized flag

        // Link to calendar area
        public Calendar WorkCalendar => mainPart.WorkCalendar;
        private HomeDay lastCurrentDay; // Saved day for tasks list
        
        public void Start()
        {
            // Create and setup parts of page
            mainPart = new MainPart();
            secondaryPart = new SecondaryPart();
            mainPart.SpawnElements();
            // Save day from calendar
            lastCurrentDay = WorkCalendar.CurrentDay;

            // Add methods for update tasks list
            MainMessenger.AddMember(UpdateFlowsByCalendar, AppMessagesConst.UpdateWorkAreaByNewDayImmediately);
            MainMessenger.AddMember(UpdateFlows, AppMessagesConst.UpdateWorkAreaImmediately);
        }

        public void Update()
        {
            SetupMainElements();
        }

        // Update borders of page scroll
        public void UpdateAnchorsForScroll()
        {
            currentPageSaved.MinAnchor = 0;
            currentPageSaved.MaxAnchorByPosition = secondaryPart.BottomSidePosition;

            if (currentPageSaved.MaxAnchor < 0)
                currentPageSaved.MaxAnchor = 0;
        }
        
        // Setup main elements of page
        private void SetupMainElements()
        {
            if (started)
                return;
            
            // Update link to current page
            currentPageSaved = CurrentPage();
            // Setup parts of page
            mainPart.SetupElementsToPage(currentPageSaved);
            secondaryPart.AnchoredPosition = mainPart.BottomSidePosition + AppElementsInfo.DistanceBetweenBackgrounds;
            secondaryPart.SetupElementsToPage(currentPageSaved);
            AppBarElements().UpdateProjectTitleImmediately();

            UpdateAnchorsForScroll();
            
            // Setup message when calendar updates
            mainPart.WorkCalendar.SetMessageForUpdates(AppMessagesConst.WorkCalendarUpdated);
            // Colorize UI
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.WorkArea);

            // Update main part of page
            mainPart.FirstUpdateAfterAll();
            
            started = true;
        }

        // Update tasks when calendar day changed
        private void UpdateFlowsByCalendar()
        {
            if (lastCurrentDay == WorkCalendar.CurrentDay)
                return;

            UpdateFlows();
            lastCurrentDay = WorkCalendar.CurrentDay;
        }
        
        // Update tasks list
        public void UpdateFlows()
        {
            mainPart.UpdateFlows(currentPageSaved);
            secondaryPart.AnchoredPosition = mainPart.BottomSidePosition + AppElementsInfo.DistanceBetweenBackgrounds;
            secondaryPart.SetupElementsToPage(currentPageSaved);
            UpdateAnchorsForScroll();
        }
    }
}
