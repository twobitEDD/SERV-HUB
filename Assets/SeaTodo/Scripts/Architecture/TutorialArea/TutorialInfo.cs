using Architecture.Data;

namespace Architecture.TutorialArea
{
    // Component for tutorial info
    public static class TutorialInfo
    {
        // Score of tutorial activity
        public const int TutorialCounterOnStart = 25;
        // Score of About Sea Calendar page
        public const int AboutCalendarCounterOnStart = 2;

        // Check if should open tutorial on startup
        public static bool OpenImmediately()
        {
            return AppCurrentSettings.TutorialCounter >= TutorialCounterOnStart;
        }

        // Show tutorial button in menu flag
        public static bool ShowInMenu()
        {
            if (AppCurrentSettings.TutorialCounter <= -1) 
                return false;
            
            if (!OpenImmediately())
                TutorialCounterIncrease(1);
            
            return true;
        }

        public static void SkipTutorial() => TutorialCounterIncrease(1);

        public static void ApplyTutorial() => TutorialCounterIncrease(2);
        
        public static bool AutoCalendarTip() => AppCurrentSettings.AboutCalendarCounter > 0;

        public static void SkipCalendar() => CalendarCounterIncrease(1);

        public static void ApplyCalendar() => CalendarCounterIncrease(2);

        // Update scores for tutorial
        private static void TutorialCounterIncrease(int increase)
        {
            AppCurrentSettings.TutorialCounter -= increase;
            
            if (AppCurrentSettings.TutorialCounter == 0)
                AppCurrentSettings.TutorialCounter = -1;
        }
        
        // Update scores for About Sea Calendar page
        private static void CalendarCounterIncrease(int increase)
        {
            AppCurrentSettings.AboutCalendarCounter -= increase;
            
            if (AppCurrentSettings.AboutCalendarCounter == 0)
                AppCurrentSettings.AboutCalendarCounter = -1;
        }
    }
}
