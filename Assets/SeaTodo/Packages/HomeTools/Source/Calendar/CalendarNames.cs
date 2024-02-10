using Architecture.SettingsArea;
using Architecture.TextHolder;

namespace HomeTools.Source.Calendar
{
    // Components for save and colorize month names
    public static class CalendarNames
    {
        private static string[] monthsShort;
        private static string[] monthsFull;
        
        // Get short month name by month number
        public static string GetMonthShortName(int month)
        {
            month--;
            
            if (month < 0)
                month = 0;

            if (month >= 12)
                month = 11;

            return monthsShort[month];
        }
        
        // Get full month name by month number
        public static string GetMonthFullName(int month)
        {
            month--;
            
            if (month < 0)
                month = 0;

            if (month >= 12)
                month = 11;

            return monthsFull[month];
        }

        // Localize month names
        public static void UpdateByLanguage()
        {
            LanguageSettings.TryInitialize();
            
            monthsShort = new[]
            {
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortJanuary),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortFebruary),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortMarch),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortApril),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortMay),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortJune),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortJuly),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortAugust),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortSeptember),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortOctober),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortNovember),
                TextLocalization.GetLocalization(TextKeysHolder.MonthShortDecember),
            };
            
            monthsFull = new[]
            {
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullJanuary),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullFebruary),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullMarch),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullApril),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullMay),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullJune),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullJuly),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullAugust),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullSeptember),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullOctober),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullNovember),
                TextLocalization.GetLocalization(TextKeysHolder.MonthFullDecember),
            };
        }
    }
}
