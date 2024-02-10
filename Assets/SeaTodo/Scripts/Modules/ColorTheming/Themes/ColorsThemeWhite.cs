using UnityEngine;

namespace InternalTheming
{
    // White theme colors
    public class ColorsThemeWhite : ITheme
    {
        public Color StatusBar { get; } = new Color32(75, 135, 189, 255);
        public Color AppBarShadow { get; } = new Color32(70, 97, 137, 67);
        public Color AppBarElements { get; } = new Color32(255, 255, 255, 255);
        public Color PrimaryColor { get; }  = new Color32(84, 148, 201, 255);
        public Color PrimaryColorP1 { get; } = new Color32(108, 168, 210, 255);
        public Color WorkAreaYearText { get; }  = new Color32(67, 69, 78, 255);
        public Color WorkAreaFlowCircle { get; } = new Color32(255, 255, 255, 255);
        public Color WorkAreaFlowText { get; } = new Color32(90, 90, 90, 255);
        public Color WorkAreaFlowTextStats { get; } = new Color32(157, 157, 157, 255);
        public Color WorkAreaBackground { get; } = new Color32(249, 249, 250, 255);
        public Color WorkAreaBackgroundShadow { get; } = new Color32(247, 247, 248, 255);
        public Color WorkAreaWeeklyBorders { get; } = new Color32(214, 218, 251, 255);
        public Color WorkAreaFlowHandle { get; } = new Color32(251, 251, 252, 0);

        public Color YearStatsOutline { get; } = new Color32(255, 255, 255, 77);
        public Color YearStatsBackground { get; } = new Color32(229, 241, 248, 77);
        public Color WorkAreaLines { get; } = new Color32(232, 234, 238, 255);
        
        public Color SecondaryColor { get; } = new Color32(108, 117, 210, 255);
        public Color SecondaryColorD1 { get; } = new Color32(134, 143, 217, 255);
        public Color SecondaryColorD2 { get; } = new Color32(168, 175, 229, 255);
        public Color SecondaryColorD3 { get; } = new Color32(202, 206, 239, 255);
        public Color SecondaryColorD4 { get; } = new Color32(234, 236, 249, 255);
        public Color SecondaryColorD4A50 { get; } = new Color32(234, 236, 249, 128);
        public Color SecondaryColorP1 { get; } = new Color32(84, 91, 201, 255);
        public Color SecondaryColorP2 { get; } = new Color32(78, 82, 190, 255);
        public Color TrackAreaBlackout { get; } = new Color32(54, 58, 61, 87);
        public Color TrackAreaMain { get; } = new Color32(251, 251, 252, 255);
        public Color TrackAreaMainSecondary { get; } = new Color32(249, 249, 250, 255);
        public Color TrackAreaSplitLine { get; } = new Color32(238, 240, 243, 255);
        public Color TrackAreaHandleLine { get; } = new Color32(178, 180, 185, 255);
        public Color TrackAreaFlowTitle { get; } = new Color32(102, 102, 102, 255);
        public Color TrackAreaCalendar { get; } = new Color32(137, 137, 137, 255);
        public Color TrackFlowAreaItems { get; } = new Color32(84, 90, 201, 255);
        public Color TrackFlowAreaCalendarIcon { get; } = new Color32(168, 175, 229, 255);

        public Color MenuAreaButtons { get; } = new Color32(251, 251, 252, 255);
        public Color MenuAreaSplitLine { get; } = new Color32(239, 241, 244, 255);
        public Color MenuAreaButtonName { get; } = new Color32(108, 117, 210, 255);
        public Color MenuAreaButtonIcon { get; } = new Color32(134, 143, 217, 255);
        public Color MenuAreaButtonIconAdditional { get; } = new Color32(255, 255, 255, 255);
        public Color DefaultBackgroundColor { get; } = new Color32(241, 242, 246, 255);
        public Color MenuAreaFlowNameLine { get; } = new Color32(101, 163, 208, 0);
        public Color MenuAreaFlowNamePlaceholder { get; } = new Color32(241, 242, 246, 0);
        public Color MenuAreaFlowNameText { get; }  = new Color32(241, 242, 246, 0);
        public Color MenuAreaFlowIconBackground { get; } = new Color32(255, 255, 255, 0);
        public Color CreateFlowAreaTitle { get; } = new Color32(63, 64, 72, 204);
        public Color CreateFlowAreaDescription { get; } = new Color32(128, 128, 128, 255);
        public Color CreateFlowAreaTypeIcons { get; } = new Color32(255, 255, 255, 255);
        public Color CreateFlowAreaButtonTextElements { get; } = new Color32(255, 255, 255, 255);
        public Color RemindersModuleSplitLine { get; } = new Color32(237, 238, 243, 255);
        public Color RemindersModuleDays { get; } = new Color32(97, 97, 97, 255);
        public Color RemindersModuleDaysRest { get; } = new Color32(157, 157, 157, 255);
        public Color RemindersModuleButtonBackground { get; } = new Color32(234, 236, 249, 255);
        public Color CreateFlowAreaTypeText { get; } = new Color32(67, 67, 67, 255);
        public Color CreateFlowAreaTypeDescription { get; } = new Color32(127, 127, 127, 255);
        public Color ViewFlowAreaTitle { get; } = new Color32(90, 90, 90, 255);
        public Color ViewFlowAreaDescription { get; } = new Color32(147, 147, 147, 255);
        public Color ViewFlowAreaInProgressGoal { get; } = new Color32(134, 143, 217, 255);
        public Color ViewFlowAreaInProgressPercentage { get; } = new Color32(102, 102, 102, 255);
        public Color ViewFlowAreaRemindersCirclePassive { get; } = new Color32(168, 175, 229, 255);
        public Color ViewFlowAreaRemindersIconPassive { get; } = new Color32(255, 255, 255, 255);
        public Color ViewFlowAreaRemindersDayPassive { get; } = new Color32(128, 128, 128, 255);
        public Color ViewFlowAreaRemoveButton { get; } = new Color32(168, 175, 229, 255);
        
        public Color EditFlowChosenType { get; } = new Color32(108, 117, 210, 255);
        public Color CalendarItemActive { get; } = new Color32(124, 126, 143, 255);
        public Color CalendarItemActiveOut { get; } = new Color32(206, 207, 214, 255);
        public Color CalendarItemPassive { get; } = new Color32(232, 233, 243, 255);
        public Color CalendarMonthPassive { get; } = new Color32(168, 175, 229, 255);
        public Color CalendarMonthActive { get; } = new Color32(108, 117, 210, 255);
        
        public Color ActivityFrequencyDays { get; } = new Color32(126, 127, 133, 255);
        public Color ActivityWeekSplitLine { get; } = new Color32(225, 228, 245, 255);
        public Color ActivityWeekDayDefault { get; } = new Color32(123, 123, 123, 255);
        public Color ActivityYear { get; } = new Color32(47, 47, 47, 157);
        public Color ActivityWeekDayActive { get; } = new Color32(108, 117, 210, 255);
        public Color ActivityDayPreviewDonePassive { get; } = new Color(255,255,255,255);
        public Color ActivityResultDonePassive { get; } = new Color(255,255,255,255);
        public Color ActivityFlowInProgressGoal { get; } = new Color32(128, 128, 128, 255);


        public Color CalendarModuleWeeks { get; } = new Color32(134, 143, 217, 255);
        public Color CalendarCapBackground { get; } = new Color32(249, 249, 250, 255);
        public Color EditGroupInputBackground { get; } = new Color32(234, 236, 249, 128);
        public Color UpdateIconModuleTitle { get; } = new Color32(117, 117, 117, 255);
        public Color EditGroupTextInput { get; } = new Color32(134, 143, 217, 255);
        public Color EditGroupNavigationPassive { get; } = new Color32(234, 236, 249, 255);
        public Color EditGroupIconCirclePassive { get; } = new Color32(202, 206, 239, 255);
        public Color EditGroupPlaceholder { get; } = new Color32(168, 175, 229, 255);
        public Color EditGroupSelection { get; } = new Color32(168, 206, 255, 187);
        public Color DaysMarkerAreaDaysColor { get; } = new Color32(126, 126, 126, 255);
        public Color DaysMarkerAreaYear { get; } = new Color32(47, 47, 47, 137);
        public Color DaysMarkerAreaYearArrow { get; } = new Color32(47, 47, 47, 255);
        public Color DaysMarkerAreaMarkerMonthTitle { get; } = new Color32(134, 134, 134, 255);
        public Color DaysMarkerAreaMarkerMonthSliderBackground { get; } = new Color32(234, 236, 249, 255);
        public Color DaysMarkerAreaModuleSplitLine { get; } = new Color32(238, 238, 238, 255);
        public Color DaysMarkerAreaModuleMarkerNameEmpty { get; } = new Color32(177, 177, 177, 255);
        public Color DaysMarkerAreaModuleMarkerNameDisabled { get; } = new Color32(207, 207, 207, 255);
        public Color DaysMarkerAreaModuleDayNameEmpty { get; } = new Color32(167, 167, 167, 255);
        public Color DaysMarkerAreaModuleDayNameDisabled { get; } = new Color32(197, 197, 197, 255);
        public Color DaysMarkerAreaTipText { get; } = new Color32(117, 117, 117, 255);
        public Color DaysMarkerAreaMonthDayCirclePassive { get; } = new Color32(234, 236, 249, 255);
        public Color DaysMarkerAreaMonthDayCircleDisabled { get; } = new Color32(234, 236, 249, 255);
        public Color DaysMarkerAreaPickerEmptyColor { get; } = new Color32(202, 206, 239, 255);
        public Color DaysMarkerAreaUpdateMarkerInput { get; } = new Color32(137, 137, 137, 255);
        public Color DaysMarkerAreaUpdateMarkerPlaceholder { get; } = new Color32(202, 206, 239, 255);
        public Color TutorialText { get; } = new Color32(157, 157, 157, 255);
        public Color SettingsUnlock { get; } = new Color32(233, 160, 20, 255);
        public Color SettingsUnlockThanks { get; } = new Color32(233, 160, 20, 137);
        public Color SettingsUnlockArrow { get; } = new Color32(233, 160, 20, 255);
        public Color SettingsLockIcon { get; } = new Color32(233, 160, 20, 255);
        public Color ChooseItemPanelCirclePassive { get; } = new Color32(202, 206, 239, 255);
        public Color ChooseItemPanelDonePassive { get; } = new Color32(255,255,255,255);
        public Color LoadBackupModuleDescription { get; } = new Color32(217, 217, 217, 255);

        public Color EmptyDescriptionText { get; } = new Color32(227, 227, 227, 255);

        public Color ImagesColor { get; } = new Color32(255, 255, 255, 255);
        public Color WorkAreaBackgroundImage { get; } = new Color32(241, 242, 246, 255);
        public Color WorkAreaBackgroundImageCircle { get; } = new Color32(255, 255, 255, 255);
        public Color StatisticsAreaBackgroundImage { get; } = new Color32(249, 249, 250, 255);
        public Color FlowViewAreaIconImage { get; } = new Color32(219, 238, 255, 255);
        public Color WorkAreaFlowHandleShadow { get; } = new Color32(251, 251, 252, 0);
        public Color TipsText { get; }  = new Color32(90, 90, 90, 255);
        public Color TipsModuleSplitLine { get; } = new Color32(237, 238, 243, 255);
        public Color UpdateArchivedFlowModuleRemoveLoad { get; } = new Color32(202, 206, 239, 255);
        public Color UpdateArchivedFlowModuleRemoveCircle { get; } = new Color32(168, 175, 229, 255);
        public Color CreateFlowAreaContentCircles { get; }  = new Color32(106, 148, 255, 255);
        public Color CreateFlowAreaContentIcon { get; } = new Color32(233, 237, 255, 255);
        public Color PurchasePageBottomText { get; } = new Color32(177, 177, 177, 255);
        public Color PurchaseRestoreText { get; } = new Color32(108, 117, 210, 255);
        public Color ThanksItemName { get; } = new Color32(117, 117, 117, 255);
    }
}