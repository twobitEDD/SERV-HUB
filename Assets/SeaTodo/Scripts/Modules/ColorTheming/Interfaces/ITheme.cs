using UnityEngine;

namespace InternalTheming
{
    // Interface for colors of themes
    public interface ITheme
    {
        Color StatusBar { get; }
        Color AppBarShadow { get; }
        Color AppBarElements { get; }
        Color PrimaryColor { get; }
        Color PrimaryColorP1 { get; }
        Color WorkAreaYearText { get; }
        Color WorkAreaFlowCircle { get; }
        Color WorkAreaFlowText { get; }
        Color WorkAreaFlowTextStats { get; }
        Color WorkAreaBackground { get; }
        Color WorkAreaBackgroundShadow { get; }
        Color WorkAreaWeeklyBorders { get; }
        
        Color WorkAreaFlowHandle { get; }
        
        Color YearStatsOutline { get; }
        Color YearStatsBackground { get; }
        Color WorkAreaLines { get; }
        
        Color SecondaryColor { get; }
        Color SecondaryColorD1 { get; }
        Color SecondaryColorD2 { get; }
        Color SecondaryColorD3 { get; }
        Color SecondaryColorD4 { get; }
        Color SecondaryColorD4A50 { get; }
        Color SecondaryColorP1 { get; }
        Color SecondaryColorP2 { get; }
        
        Color TrackAreaBlackout { get; }
        Color TrackAreaMain { get; }
        Color TrackAreaMainSecondary { get; }
        Color TrackAreaSplitLine { get; }
        Color TrackAreaHandleLine { get; }
        Color TrackAreaFlowTitle { get; }
        Color TrackAreaCalendar { get; }
        Color TrackFlowAreaItems { get; }
        Color TrackFlowAreaCalendarIcon { get; }
        
        Color MenuAreaButtons { get; }
        Color MenuAreaSplitLine { get; }
        Color MenuAreaButtonName { get; }
        Color MenuAreaButtonIcon { get; }
        Color MenuAreaButtonIconAdditional { get; }
        Color DefaultBackgroundColor { get; }
        Color MenuAreaFlowNameLine { get; }
        Color MenuAreaFlowNamePlaceholder { get; }
        Color MenuAreaFlowNameText { get; }
        Color MenuAreaFlowIconBackground { get; }
        
        Color CreateFlowAreaTitle { get; }
        Color CreateFlowAreaDescription { get; }
        Color CreateFlowAreaTypeIcons { get; }
        Color CreateFlowAreaButtonTextElements { get; }
        Color RemindersModuleSplitLine { get; }
        Color RemindersModuleDays { get; }
        Color RemindersModuleDaysRest { get; }
        Color RemindersModuleButtonBackground { get; }
        Color CreateFlowAreaTypeText { get; }
        Color CreateFlowAreaTypeDescription { get; }
        
        Color ViewFlowAreaTitle { get; }
        Color ViewFlowAreaDescription { get; }
        Color ViewFlowAreaInProgressGoal { get; }
        Color ViewFlowAreaInProgressPercentage { get; }
        Color ViewFlowAreaRemindersCirclePassive { get; }
        Color ViewFlowAreaRemindersIconPassive { get; }
        Color ViewFlowAreaRemindersDayPassive { get; }
        Color ViewFlowAreaRemoveButton { get; }
        
        Color EditFlowChosenType { get; }
        Color CalendarItemActive { get; }
        Color CalendarItemActiveOut { get; }
        Color CalendarItemPassive { get; }
        Color CalendarMonthPassive { get; }
        Color CalendarMonthActive { get; }
        
        Color ActivityFrequencyDays { get; }
        Color ActivityWeekSplitLine { get; }
        Color ActivityWeekDayDefault { get; }
        Color ActivityYear { get; }
        Color ActivityWeekDayActive { get; }
        Color ActivityDayPreviewDonePassive { get; }
        Color ActivityResultDonePassive { get; }
        Color ActivityFlowInProgressGoal { get; }

        Color CalendarModuleWeeks { get; }
        Color CalendarCapBackground { get; }
        Color EditGroupInputBackground { get; }
        Color UpdateIconModuleTitle { get; }
        Color EditGroupTextInput { get; }
        Color EditGroupNavigationPassive { get; }
        Color EditGroupIconCirclePassive { get; }
        Color EditGroupPlaceholder { get; }
        Color EditGroupSelection { get; }

        Color DaysMarkerAreaDaysColor { get; }
        Color DaysMarkerAreaYear { get; }
        Color DaysMarkerAreaYearArrow { get; }
        Color DaysMarkerAreaMarkerMonthTitle { get; }
        Color DaysMarkerAreaMarkerMonthSliderBackground { get; }
        Color DaysMarkerAreaModuleSplitLine { get; }
        Color DaysMarkerAreaModuleMarkerNameEmpty { get; }
        Color DaysMarkerAreaModuleMarkerNameDisabled { get; }
        Color DaysMarkerAreaModuleDayNameEmpty { get; }
        Color DaysMarkerAreaModuleDayNameDisabled { get; }
        Color DaysMarkerAreaTipText { get; }
        Color DaysMarkerAreaMonthDayCirclePassive { get; }
        Color DaysMarkerAreaMonthDayCircleDisabled { get; }
        Color DaysMarkerAreaPickerEmptyColor { get; }
        Color DaysMarkerAreaUpdateMarkerInput { get; }
        Color DaysMarkerAreaUpdateMarkerPlaceholder { get; }
        
        Color TutorialText { get; }
        Color SettingsUnlock { get; }
        Color SettingsUnlockThanks { get; }
        Color SettingsUnlockArrow { get; }
        Color SettingsLockIcon { get; }
        Color ChooseItemPanelCirclePassive { get; }
        Color ChooseItemPanelDonePassive { get; }
        Color LoadBackupModuleDescription { get; }
        
        Color EmptyDescriptionText { get; }
        
        Color ImagesColor { get; }
        Color WorkAreaBackgroundImage { get; }
        Color WorkAreaBackgroundImageCircle { get; }
        Color StatisticsAreaBackgroundImage { get; }
        Color FlowViewAreaIconImage { get; }
        Color WorkAreaFlowHandleShadow { get; }
        Color TipsText { get; }
        Color TipsModuleSplitLine { get; }
        Color UpdateArchivedFlowModuleRemoveLoad { get; }
        Color UpdateArchivedFlowModuleRemoveCircle { get; }
        
        Color CreateFlowAreaContentCircles { get; }
        Color CreateFlowAreaContentIcon { get; }
        
        Color PurchasePageBottomText { get; }
        Color PurchaseRestoreText { get; }
        Color ThanksItemName { get; }
    }
}
