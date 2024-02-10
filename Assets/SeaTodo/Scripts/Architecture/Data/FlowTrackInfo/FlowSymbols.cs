using Architecture.SettingsArea;
using Architecture.TextHolder;

namespace Architecture.Data.FlowTrackInfo
{
    // Symbols for types of tasks
    public static class FlowSymbols
    {
        public static string Seconds { get; private set; }
        public static string Minutes { get; private set; }
        public static string Hours { get; private set; }
        public static string Days { get; private set; }
        
        public static string SecondsFull { get; private set; }
        public static string MinutesFull { get; private set; }
        public static string HoursFull { get; private set; }
        public static string DaysFull { get; private set; }
        
        // Get Name of task type
        public static string GetNameFullByFlowType(FlowType flowType)
        {
            switch (flowType)
            {
                case FlowType.count:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeCount);
                case FlowType.done:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeDone);
                case FlowType.stars:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeStars);
                case FlowType.symbol:
                    return string.Empty;
                case FlowType.timeS:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeMinutes);
                case FlowType.timeM:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeHours);
                case FlowType.timeH:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeDays);
            }
            
            return string.Empty;
        }

        // Update task symbols by language
        public static void UpdateByLanguage()
        {
            LanguageSettings.TryInitialize();
            
            Seconds = TextLocalization.GetLocalization(TextKeysHolder.UnitsSeconds);//"s";
            Minutes = TextLocalization.GetLocalization(TextKeysHolder.UnitsMinutes);//"m";
            Hours = TextLocalization.GetLocalization(TextKeysHolder.UnitsHours);//"h";
            Days = TextLocalization.GetLocalization(TextKeysHolder.UnitsDays);//"d";
        
            SecondsFull = TextLocalization.GetLocalization(TextKeysHolder.UnitsSecondsFull);//"seconds";
            MinutesFull = TextLocalization.GetLocalization(TextKeysHolder.UnitsMinutesFull);//"minutes";
            HoursFull = TextLocalization.GetLocalization(TextKeysHolder.UnitsHoursFull);//"hours";
            DaysFull = TextLocalization.GetLocalization(TextKeysHolder.UnitsDaysFull);//"days";
        }
    }
}
