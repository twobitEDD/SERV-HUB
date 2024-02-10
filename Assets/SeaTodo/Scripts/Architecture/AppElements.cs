using Theming;

namespace Architecture
{
    public enum FlowType
    {
        count = 1, // int
        done = 2, // bool
        symbol = 3, // bool
        stars = 4, // byte
        timeS = 5, // time
        timeM = 6, // time
        timeH = 7, // time
    }

    public static class AppElementsInfo
    {
        public const float BackgroundShadowCorrection = 12;
        public const float DistanceBetweenBackgrounds = 27;
        public const float DistanceContentToBottom = 170f;
    }

    public static class AppMessagesConst
    {
        public const string BlackoutTrackClicked = "BlackoutTrackClicked";
        public const string BlackoutMenuClicked = "BlackoutMenuClicked";
        public const string BlackoutTimeTrackClicked = "BlackoutTimeTrackClicked";
        public const string BlackoutGoalTrackClicked = "BlackoutGoalTrackClicked";
        public const string BlackoutCalendarTrackClicked = "BlackoutCalendarTrackClicked";
        public const string BlackoutChooseIconClicked = "BlackoutChooseIconClicked";
        public const string BlackoutEditGroupClicked = "BlackoutEditProjectClicked";
        public const string BlackoutAcceptModuleClicked = "BlackoutAcceptModuleClicked";
        public const string BlackoutTipModuleClicked = "BlackoutTipModuleClicked";
        public const string BlackoutUpdateArchivedModuleClicked = "BlackoutUpdateArchivedModuleClicked";
        public const string BlackoutUpdateMarkerModuleClicked = "BlackoutUpdateMarkerModuleClicked";
        public const string BlackoutMonthMarkersModuleClicked = "BlackoutMonthMarkersModuleClicked";
        public const string BlackoutChooseDayMarkerModuleClicked = "BlackoutChooseDayMarkerModuleClicked";
        public const string BlackoutCreateBackupModuleClicked = "BlackoutCreateBackupModuleClicked";
        public const string BlackoutCreateBackupModuleResultClicked = "BlackoutCreateBackupModuleResultClicked";
        public const string BlackoutLoadBackupModuleClicked = "BlackoutLoadBackupModuleClicked";
        public const string BlackoutLanguagePanel = "BlackoutLanguagePanel";
        public const string TrackerHidden = "TrackerHidden";
        public const string MenuStartedShow = "MenuStartedShow";
        public const string WorkCalendarUpdated = "WorkCalendarUpdated";
        public const string TrackCalendarUpdated = "TrackCalendarUpdated";
        public const string UpdateWorkAreaGraphic = "UpdateWorkAreaGraphic";
        public const string UpdateWorkAreaGraphicDays = "UpdateWorkAreaGraphicDays";
        public const string TrackedInStatisticArea = "TrackedInStatisticArea";
        public const string UpdateWorkAreaByNewDayImmediately = "UpdateWorkAreaByNewDayImmediately";
        public const string UpdateWorkAreaImmediately = "UpdateWorkAreaByNewDayImmediately";
        public const string UpdateWorkAreaFlowsViewTrack = "UpdateWorkAreaFlowsViewTrack";
        public const string FlowUpdated = "FlowUpdated";
        public const string ShouldUpdateColorsCountInCalendar = "ShouldUpdateColorsCountInCalendar";
        public const string UpdateSelectiveInCalendar = "UpdateSelectiveInCalendar";
        public const string UpdateLoadBackupContent = "UpdateLoadBackupContent";
        public const string OpenPrivacyPolicy = "OpenPrivacyPolicy";
        public const string OpenTermsAndConditions = "OpenTermsAndConditions";

        public const string MenuButtonToClose = "MenuButtonToClose";
        public const string MenuButtonFromClose = "MenuButtonFromClose";
        public const string CloseButtonToMainClose = "CloseButtonToMainClose";
        public const string MenuButtonToEdit = "MenuButtonToEdit";
        public const string MenuButtonFromEdit = "MenuButtonFromEdit";
        public const string CloseToEdit = "CloseButtonToEdit";
        public const string EditToClose = "EditToCloseButton";
        public const string MenuButtonToEmpty = "MenuButtonToEmpty";
        public const string MenuButtonFromEmpty = "MenuButtonFromEmpty";
        public const string CloseButtonToEmpty = "CloseButtonToEmpty";
        public const string MenuButtonFromSea = "MenuButtonFromSea";
        public const string CloseButtonToSea = "CloseButtonToSea";
        
        public const string Empty = "Empty";
        
        public static string ColorizedArea = "Colorized{0}";
        
        public const string LanguageUpdated = "LanguageUpdated";
        public const string PurchaseUpdated = "PurchaseUpdated";
    }

    public static class AppFontCustomization
    {
        public const string Done = "ʿ";
        public const string Cross = "ʻ";
        public const string Up = "ʾ";
        public const string Down = "ˀ";
        public const string Star = "ʽ";
        public const string Rating = "ˁ";
    }
    
    public static class AppTimeCustomization
    {
        public const string Am = "AM";
        public const string Pm = "PM";
    }
    
    public static class AppSyncAnchors
    {
        public const int CalendarObject = 0;
        public const int MonthMarkersModule = 1;
        public const int ChooseDayMarkerModule = 2;
        public const int MarkersAreaDays = 3;
        public const int MarkersAreaColors = 4;
        public const int UpdateCalendarMarkerModule = 5;
        public const int EditTitleModule = 6;
        public const int ChooseIconModule = 7;
        public const int TrackTimeModule = 8;
        public const int RemindersModule = 9;
        public const int TrackFlowModule = 10;
        public const int FlowViewFinished = 11;
        public const int CreateFlowAreaChooseType = 12;
        public const int WorkAreaYear = 13;
        public const int WorkAreaWeeklyActivity = 14;
        public const int ArchivedUpdateModule = 15;
        public const int AcceptModule = 16;
        public const int TipModule = 17;
        public const int StatisticsArea = 18;
        public const int TutorialArea = 19;
        public const int SettingsMain = 20;
        public const int SettingsData = 21;
        public const int SettingsOther = 22;
        public const int ChooseItemsPanel = 23;
        public const int AboutSeaCalendarArea = 24;
        public const int CreateBackupModule = 25;
        public const int CreateBackupResultModule = 26;
        public const int LoadBackupModule = 27;
        public const int SinglePageView = 28;
        public const int PurchasePageView = 29;
    }
}
