using Architecture.CalendarModule;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.ModuleTrackFlow;
using Architecture.Pages;
using Architecture.TrackArea.TrackModule;
using HomeTools.Messenger;
using HTools;
using Theming;

namespace Architecture.TrackArea
{
    // Main component of track progress area in main page
    public class TrackArea : IBehaviorSync
    {
        // Link to track progress view
        private TrackFlowModule TrackFlowArea() => AreasLocator.Instance.TrackFlowModule;
        
        private MainPart mainPart; // Part of view with UI
        private ScreenBlackout screenBlackout; // Component of dark screen under area
        private TrackAreaJob trackAreaJob; // Component with main work of area
        private TrackAreaScroll trackAreaScroll; // Component of track area scroll
        private TrackResultArea trackResultArea; // Component of track area results

        // Link to calendar view
        public Calendar TrackCalendar => trackAreaJob.TrackCalendar;
        
        // Current track session
        private TrackSession currentSession;

        // Has session flag
        public bool HasSession => currentSession != null;
        
        private bool started; // Initialized flag

        // Setup
        public void Start()
        {
            // Setup dark screen under area
            screenBlackout = new ScreenBlackout("Track blackout");
            screenBlackout.MessageBlackoutClicked = AppMessagesConst.BlackoutTrackClicked;
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.TrackAreaBlackout, AppTheming.AppItem.TrackArea);

            // Create main component of visual UI setup
            mainPart = new MainPart(screenBlackout);
            SyncWithBehaviour.Instance.AddObserver(mainPart);
            
            // Create component with main work of area
            trackAreaJob = new TrackAreaJob(mainPart.RectTransform.gameObject);
            
            // Add Finish session action to messenger
            MainMessenger.AddMember(FinishSession, AppMessagesConst.TrackerHidden);
            // Colorize UI of track progress area
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TrackArea);
            // Prepare dark screen for work
            screenBlackout.PrepareBlackout();
            // Create track area scroll component
            trackAreaScroll = new TrackAreaScroll(screenBlackout, mainPart);
            SyncWithBehaviour.Instance.AddObserver(trackAreaScroll);
            // Create track area result component
            trackResultArea = new TrackResultArea(mainPart.RectTransform);
        }

        public void Update()
        {
        }

        // Start new session
        public void StartNewSession(TrackSession trackSession)
        {
            // Save new session
            currentSession = trackSession;
            // Update results part of area
            trackResultArea.PrepareByNewFlow(trackSession.GetFlow());
            // Update UI by session
            trackAreaJob.PrepareToSession(trackSession, trackResultArea.Update);
            // Start auto open area animation
            trackAreaScroll.AutoOpen();
            // Disable scroll
            PageScroll.Instance.Enabled = false;
        }

        // Close area
        public void Close()
        {
            // Finish session
            FinishSession();
            // Start auto close animation of area
            trackAreaScroll.AutoClose();
        }

        // Finish track progress session
        private void FinishSession()
        {
            currentSession?.Finish(TrackFlowArea().GetOriginPosition());
            PageScroll.Instance.Enabled = true;
            
            if (currentSession == null)
                return;
            
            MainMessenger<HomeDay>.SendMessage(AppMessagesConst.UpdateWorkAreaGraphic, currentSession.CalendarDay());
            currentSession = null;
        }
    }
}
