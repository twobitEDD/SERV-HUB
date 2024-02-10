using Architecture.AboutSeaCalendar;
using Architecture.CalendarModule;
using Architecture.DaysMarkerArea.DaysView.DayMarkerPicker;
using Architecture.DaysMarkerArea.DaysView.MonthMarkersModule;
using Architecture.EditGroupModule;
using Architecture.EditTaskArea;
using Architecture.ModuleTrackFlow;
using Architecture.ModuleTrackFlow.ModuleTrackGoal;
using Architecture.Pages;
using Architecture.SettingsArea.LoadBackupModule;
using Architecture.SettingsArea.SinglePageView;
using Architecture.TaskViewArea;
using Architecture.TutorialArea;
using MainActivity.AppBar;
using Modules.Notifications;

namespace Architecture.Elements
{
    // Component for storing main app modules
    public class AreasLocator
    {
        private static AreasLocator instance;
        public static AreasLocator Instance => instance ?? (instance = new AreasLocator());

        public PageTransition PageTransition; // Pages system

        public Tutorial Tutorial; // Tutorial of app
        public ViewAboutSeaCalendar ViewAboutSeaCalendar; // Tutorial of Sea Calendar
        public WorkArea.WorkArea WorkArea; // Main page
        public TrackArea.TrackArea TrackArea; // Track task area
        public MenuArea.MenuArea MenuArea; // Menu view
        public TrackFlowModule TrackFlowModule; // Module for track task progress
        public CreateTaskArea.CreateTaskArea CreateTaskArea; // Create task page
        public ModuleReminders.RemindersModule RemindersModule; // Setup notification module
        public ModuleTrackTime.ModuleTrackTime ModuleTrackTime; // Track time view
        public FlowViewArea FlowViewArea; // View task page
        public EditFlowArea EditFlowArea; // Edit task page
        public ModuleTrackGoal ModuleTrackGoal; // View for track goal of task
        public StatisticsArea.StatisticsArea StatisticsArea; // Activity page
        public CalendarTrackModule CalendarTrackModule; // Calendar view
        public ChooseIconModule.ChooseIconModule ChooseIconModule; // Setup task icon view
        public UpdateTitleModule UpdateTitleModule; // Update app main page title view
        public DaysMarkerArea.DaysMarkerArea DaysMarkerArea; // Sea Calendar page
        public AcceptModule.AcceptModule AcceptModule; // Accept view
        public TipModule.TipModule TipModule; // Tip view
        public MonthMarkersModule MonthMarkersModule; // Choose day of month view for Sea Calendar
        public ChooseDayMarkerModule ChooseDayMarkerModule; // Choose characteristic view for day in Sea Calendar
        public SettingsArea.SettingsArea SettingsArea; // Setting page
        public LoadBackupModule LoadBackupModule; // Load backup view
        public SinglePage SinglePage; // Page for view other UI

        public AppBar AppBar; // Component for app bar 
        public IosRequestAuthorization IosRequestAuthorization; // Module for notification requests on iOS
    }
}
