using System.Collections.Generic;
using Architecture.Other;
using Architecture.SettingsArea;

namespace Architecture.TextHolder
{
    // Component for hold localized names of week days
    public static class TextHolderTime
    {
        public static Dictionary<WeekInfo.DayOfWeek, string> WeekDaysFull { get; private set; }
        private static Dictionary<WeekInfo.DayOfWeek, string> LoadWeekDaysShort { get; set; }
        
        public static string DaysOfWeekFull(WeekInfo.DayOfWeek weekInfo) => WeekDaysFull[weekInfo];
        public static string DaysOfWeekShort(WeekInfo.DayOfWeek weekInfo) => LoadWeekDaysShort[weekInfo];

        // Update localization of days
        public static void UpdateByLanguage()
        {
            LanguageSettings.TryInitialize();
            
            WeekDaysFull = new Dictionary<WeekInfo.DayOfWeek, string>()
            {
                {WeekInfo.DayOfWeek.Sunday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayFullSunday)},
                {WeekInfo.DayOfWeek.Monday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayFullMonday)},
                {WeekInfo.DayOfWeek.Tuesday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayFullTuesday)},
                {WeekInfo.DayOfWeek.Wednesday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayFullWednesday)},
                {WeekInfo.DayOfWeek.Thursday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayFullThursday)},
                {WeekInfo.DayOfWeek.Friday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayFullFriday)},
                {WeekInfo.DayOfWeek.Saturday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayFullSaturday)},
            };
            
            LoadWeekDaysShort = new Dictionary<WeekInfo.DayOfWeek, string>()
            {
                {WeekInfo.DayOfWeek.Sunday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayShortSunday)},
                {WeekInfo.DayOfWeek.Monday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayShortMonday)},
                {WeekInfo.DayOfWeek.Tuesday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayShortTuesday)},
                {WeekInfo.DayOfWeek.Wednesday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayShortWednesday)},
                {WeekInfo.DayOfWeek.Thursday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayShortThursday)},
                {WeekInfo.DayOfWeek.Friday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayShortFriday)},
                {WeekInfo.DayOfWeek.Saturday, TextLocalization.GetLocalization(TextKeysHolder.WeekDayShortSaturday)},
            };
        }
    }
}