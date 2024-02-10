using UnityEngine;

namespace InternalTheming
{
    // Dark theme colors
    public class ColorsThemeDark : ITheme
    {
        public Color StatusBar { get; } = new Color32(20, 21, 23, 255);
        public Color AppBarShadow { get; } = new Color32(23, 24, 26, 97);
        public Color AppBarElements { get; } = new Color32(217, 217, 217, 255);
        public Color PrimaryColor { get; }  = new Color32(30, 32, 35, 255);
        public Color PrimaryColorP1 { get; } = new Color32(82, 150, 202, 255);
        public Color WorkAreaYearText { get; }  = new Color32(197, 197, 197, 255);
        public Color WorkAreaFlowCircle { get; } = new Color32(42, 45, 53, 255);
        public Color WorkAreaFlowText { get; } = new Color32(197, 197, 197, 255);
        public Color WorkAreaFlowTextStats { get; } = new Color32(157, 157, 157, 255);
        public Color WorkAreaBackground { get; } = new Color32(24, 25, 27, 255);
        public Color WorkAreaBackgroundShadow { get; } = new Color32(0, 0, 0, 177);
        public Color WorkAreaWeeklyBorders { get; } = new Color32(42, 45, 53, 255);
        public Color WorkAreaFlowHandle { get; } = new Color32(28, 29, 34, 0);

        public Color YearStatsOutline { get; } = new Color32(89, 97, 104, 77);
        public Color YearStatsBackground { get; } = new Color32(62, 66, 77, 77);
        public Color WorkAreaLines { get; } = new Color32(29, 30, 32, 255);
        
        public Color SecondaryColor { get; } = new Color32(108, 117, 210, 255);
        public Color SecondaryColorD1 { get; } =  new Color32(90, 94, 180, 255);
        public Color SecondaryColorD2 { get; } = new Color32(78, 84, 128, 255);
        public Color SecondaryColorD3 { get; } = new Color32(47, 52, 65, 255);
        public Color SecondaryColorD4 { get; } = new Color32(32, 34, 37, 255);
        public Color SecondaryColorD4A50 { get; } = new Color32(32, 34, 37, 128);
        public Color SecondaryColorP1 { get; } = new Color32(134, 143, 244, 255);
        public Color SecondaryColorP2 { get; } = new Color32(142, 151, 255, 255);
        public Color TrackAreaBlackout { get; } = new Color32(10, 11, 12, 87);
        public Color TrackAreaMain { get; } = new Color32(36, 37, 39, 255);
        public Color TrackAreaMainSecondary { get; } = new Color32(33, 34, 36, 255);
        public Color TrackAreaSplitLine { get; } = new Color32(39, 42, 44, 255);
        public Color TrackAreaHandleLine { get; } = new Color32(159, 161, 166, 255);
        public Color TrackAreaFlowTitle { get; } = new Color32(213, 213, 213, 255);
        public Color TrackAreaCalendar { get; } = new Color32(182, 184, 186, 255);
        public Color TrackFlowAreaItems { get; } = new Color32(120, 129, 224, 255);
        public Color TrackFlowAreaCalendarIcon { get; } = new Color32(108, 117, 210, 255);

        public Color MenuAreaButtons { get; } = new Color32(36, 37, 39, 255);
        public Color MenuAreaSplitLine { get; } = new Color32(49, 51, 53, 255);
        public Color MenuAreaButtonName { get; } = new Color32(197, 197, 197, 255);
        public Color MenuAreaButtonIcon { get; } = new Color32(117, 117, 117, 255);
        public Color MenuAreaButtonIconAdditional { get; } = new Color32(157, 157, 157, 255);
        public Color DefaultBackgroundColor { get; } = new Color32(20, 21, 23, 255);
        public Color MenuAreaFlowNameLine { get; } = new Color32(37, 40, 46, 0);
        public Color MenuAreaFlowNamePlaceholder { get; } = new Color32(179, 180, 184, 0);
        public Color MenuAreaFlowNameText { get; } = new Color32(241, 242, 246, 0);
        public Color MenuAreaFlowIconBackground { get; } = new Color32(42, 45, 53, 0);
        public Color CreateFlowAreaTitle { get; } = new Color32(217, 217, 217, 255);
        public Color CreateFlowAreaDescription { get; } = new Color32(197, 197, 197, 255);
        public Color CreateFlowAreaTypeIcons { get; } = new Color32(255, 255, 255, 255);
        public Color CreateFlowAreaButtonTextElements { get; } = new Color32(227, 231, 243, 255);
        public Color RemindersModuleSplitLine { get; } = new Color32(33, 35, 38, 255);
        public Color RemindersModuleDays { get; } = new Color32(97, 97, 97, 255);
        public Color RemindersModuleDaysRest { get; } = new Color32(97, 97, 97, 255);
        public Color RemindersModuleButtonBackground { get; } = new Color32(36, 38, 43, 255);
        public Color CreateFlowAreaTypeText { get; } = new Color32(227, 227, 227, 255);
        public Color CreateFlowAreaTypeDescription { get; } = new Color32(177, 177, 177, 255);
        public Color ViewFlowAreaTitle { get; } = new Color32(217, 217, 217, 255);
        public Color ViewFlowAreaDescription { get; } = new Color32(177, 177, 177, 255);
        public Color ViewFlowAreaInProgressGoal { get; } = new Color32(134, 143, 217, 255);
        public Color ViewFlowAreaInProgressPercentage { get; } = new Color32(148, 148, 148, 255);
        public Color ViewFlowAreaRemindersCirclePassive { get; } = new Color32(32, 34, 37, 255);
        public Color ViewFlowAreaRemindersIconPassive { get; } = new Color32(107, 107, 114, 255);
        public Color ViewFlowAreaRemindersDayPassive { get; } = new Color32(137, 137, 137, 255);
        public Color ViewFlowAreaRemoveButton { get; } = new Color32(137, 137, 137, 255);
        
        public Color EditFlowChosenType { get; } = new Color32(108, 117, 210, 255);
        public Color CalendarItemActive { get; } = new Color32(217, 218, 229, 255);
        public Color CalendarItemActiveOut { get; } = new Color32(141, 142, 152, 255);
        public Color CalendarItemPassive { get; } = new Color32(85, 87, 92, 255);
        public Color CalendarMonthPassive { get; } = new Color32(101, 109, 193, 255);
        public Color CalendarMonthActive { get; } = new Color32(137, 146, 250, 255);
        public Color ActivityFrequencyDays { get; } = new Color32(114, 114, 116, 255);
        public Color ActivityWeekSplitLine { get; } = new Color32(42, 45, 53, 255);
        public Color ActivityWeekDayDefault { get; } = new Color32(187, 187, 187, 255);
        public Color ActivityYear { get; } = new Color32(181, 188, 198, 157);
        public Color ActivityWeekDayActive { get; } = new Color32(134, 143, 219, 255);
        public Color ActivityDayPreviewDonePassive { get; } = new Color32(72, 81, 106, 255);
        public Color ActivityResultDonePassive { get; } = new Color32(109, 109, 169, 255);
        public Color ActivityFlowInProgressGoal { get; } = new Color32(158, 158, 158, 255);


        public Color CalendarModuleWeeks { get; } = new Color32(107, 115, 198, 255);
        public Color CalendarCapBackground { get; } = new Color32(38, 39, 41, 255);
        public Color EditGroupInputBackground { get; } = new Color32(48, 48, 51, 128);
        public Color UpdateIconModuleTitle { get; } = new Color32(200, 200, 200, 255);
        public Color EditGroupTextInput { get; } = new Color32(212, 214, 229, 255);
        public Color EditGroupNavigationPassive { get; } = new Color32(79, 80, 89, 255);
        public Color EditGroupIconCirclePassive { get; } = new Color32(61, 63, 71, 255);
        public Color EditGroupPlaceholder { get; } = new Color32(99, 102, 123, 255);
        public Color EditGroupSelection { get; } = new Color32(68, 76, 87, 187);
        public Color DaysMarkerAreaDaysColor { get; } = new Color32(157, 157, 157, 255);
        public Color DaysMarkerAreaYear { get; } = new Color32(181, 188, 198, 137);
        public Color DaysMarkerAreaYearArrow { get; } = new Color32(114, 114, 114, 255);
        public Color DaysMarkerAreaMarkerMonthTitle { get; } = new Color32(157, 157, 157, 255);
        public Color DaysMarkerAreaMarkerMonthSliderBackground { get; } = new Color32(32, 34, 37, 255);
        public Color DaysMarkerAreaModuleSplitLine { get; } = new Color32(49, 51, 53, 255);
        public Color DaysMarkerAreaModuleMarkerNameEmpty { get; } = new Color32(177, 177, 177, 255);
        public Color DaysMarkerAreaModuleMarkerNameDisabled { get; } = new Color32(207, 207, 207, 255);
        public Color DaysMarkerAreaModuleDayNameEmpty { get; } = new Color32(146, 146, 146, 255);
        public Color DaysMarkerAreaModuleDayNameDisabled { get; } = new Color32(85, 85, 85, 255);
        public Color DaysMarkerAreaTipText { get; } = new Color32(178, 178, 178, 255);
        public Color DaysMarkerAreaMonthDayCirclePassive { get; } = new Color32(100, 100, 100, 255);
        public Color DaysMarkerAreaMonthDayCircleDisabled { get; } = new Color32(53, 53, 53, 255);
        public Color DaysMarkerAreaPickerEmptyColor { get; } = new Color32(85, 85, 85, 255);
        public Color DaysMarkerAreaUpdateMarkerInput { get; } = new Color32(241, 242, 246, 0);
        public Color DaysMarkerAreaUpdateMarkerPlaceholder { get; } = new Color32(179, 180, 184, 0);
        public Color TutorialText { get; } = new Color32(157, 157, 157, 255);
        public Color SettingsUnlock { get; } = new Color32(233, 160, 20, 255);
        public Color SettingsUnlockThanks { get; } = new Color32(233, 160, 20, 137);
        public Color SettingsUnlockArrow { get; } = new Color32(233, 160, 20, 255);
        public Color SettingsLockIcon { get; } = new Color32(233, 160, 20, 255);
        public Color ChooseItemPanelCirclePassive { get; } = new Color32(62, 67, 77, 255);
        public Color ChooseItemPanelDonePassive { get; } = new Color32(102, 111, 121, 255);
        public Color LoadBackupModuleDescription { get; } = new Color32(107, 107, 107, 255);

        public Color EmptyDescriptionText { get; } = new Color32(81, 84, 90, 255);

        public Color ImagesColor { get; } = new Color32(253, 253, 253, 255);
        public Color WorkAreaBackgroundImage { get; } = new Color32(255, 255, 255, 255);
        public Color WorkAreaBackgroundImageCircle { get; } = new Color32(67, 118, 171, 255);
        public Color StatisticsAreaBackgroundImage { get; } = new Color32(255, 255, 255, 255);
        public Color FlowViewAreaIconImage { get; } = new Color32(191, 202, 227, 255);
        public Color WorkAreaFlowHandleShadow { get; } = new Color32(17, 17, 17, 0);
        public Color TipsText { get; } = new Color32(178, 178, 178, 255);
        public Color TipsModuleSplitLine { get; } = new Color32(48, 50, 53, 255);
        public Color UpdateArchivedFlowModuleRemoveLoad { get; } = new Color32(69, 70, 80, 255);
        public Color UpdateArchivedFlowModuleRemoveCircle { get; } = new Color32(105, 108, 119, 255);
        public Color CreateFlowAreaContentCircles { get; } = new Color32(46, 65, 114, 255);
        public Color CreateFlowAreaContentIcon { get; } = new Color32(190, 206, 249, 255);
        public Color PurchasePageBottomText { get; } = new Color32(111, 111, 111, 255);
        public Color PurchaseRestoreText { get; } = new Color32(157, 157, 157, 255); 
        public Color ThanksItemName { get; } = new Color32(197, 197, 197, 255);
    }
}