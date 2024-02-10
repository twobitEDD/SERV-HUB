using Architecture.Other;
using Architecture.TutorialArea;
using HomeTools.HPrefs;
using UnityEngine;

namespace Architecture.Data
{
    // Layer for saving and using settings via PlayerPrefs
    public static class AppCurrentSettings
    {
        // First day of week is Sunday or Monday
        public static bool DaysFromSunday 
        {
            get => PlayerPrefs.GetInt("DaysFromSunday") == 1;
            set => PlayerPrefs.SetInt("DaysFromSunday", value ? 1 : 0);
        }
        
        // Is dark theme enabled
        public static bool DarkTheme
        {
            get => PlayerPrefs.GetInt("DarkTheme") == 1;
            set => PlayerPrefs.SetInt("DarkTheme", value ? 1 : 0);
        }

        // Is automatic data saving enabled
        public static bool AutoBackupOn
        {
            get => PlayerPrefs.GetInt("AutoBackupOn") == 1;
            set => PlayerPrefs.SetInt("AutoBackupOn", value ? 1 : 0);
        }
        
        // What time display format is used when setting up notifications
        public static bool EnglishFormat
        {
            get => PlayerPrefs.GetInt("EnglishFormat") == 1;
            set => PlayerPrefs.SetInt("EnglishFormat", value ? 1 : 0);
        }
        
        // File name with last saved data
        public static string LastAutoBackup
        {
            get => PlayerPrefs.GetString("LastAutoBackup");
            set => PlayerPrefs.SetString("LastAutoBackup", value);
        }

        // What day of the week is defined as a day of rest
        public static WeekInfo.DayOfWeek DayOff
        {
            get => (WeekInfo.DayOfWeek) PlayerPrefs.GetInt("DayOff");
            set => PlayerPrefs.SetInt("DayOff", ((int) value));
        }
        
        // Day of the week when data is automatically saved
        public static WeekInfo.DayOfWeek AutoBackup
        {
            get => (WeekInfo.DayOfWeek) PlayerPrefs.GetInt("AutoBackup");
            set => PlayerPrefs.SetInt("AutoBackup", ((int) value));
        }
        
        // Date of the first launch of the application
        public static int AppInstalledDate
        {
            get => PlayerPrefs.GetInt("AppInstalledDate");
            set => PlayerPrefs.SetInt("AppInstalledDate", value);
        }
        
        // Counter for the logic of showing the guide button in the menu
        public static int TutorialCounter
        {
            get => PlayerPrefs.GetInt("TutorialCounter");
            set => PlayerPrefs.SetInt("TutorialCounter", value);
        }
        
        // Counter for the logic of showing the Sea Calendar guide
        public static int AboutCalendarCounter
        {
            get => PlayerPrefs.GetInt("AboutCalendarCounter");
            set => PlayerPrefs.SetInt("AboutCalendarCounter", value);
        }

        // Called when the app is first launched to set default settings
        public static void SetDefaultSettings()
        {
            DaysFromSunday = false;
            DarkTheme = true;
            AutoBackupOn = false;
            EnglishFormat = true;
            LastAutoBackup = string.Empty;
            DayOff = WeekInfo.DayOfWeek.Saturday;
            AutoBackup = WeekInfo.DayOfWeek.Saturday;
            TutorialCounter = TutorialInfo.TutorialCounterOnStart;
            AboutCalendarCounter = TutorialInfo.AboutCalendarCounterOnStart;
        }
    }
}
