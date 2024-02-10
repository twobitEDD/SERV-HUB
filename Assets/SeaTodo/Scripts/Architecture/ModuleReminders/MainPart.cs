using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.Other;
using HomeTools.Other;
using MainActivity.MainComponents;
using UnityEngine;

namespace Architecture.ModuleReminders
{
    // Main view component of reminders view
    public class MainPart
    {
        // Link to track time view
        private ModuleTrackTime.ModuleTrackTime ModuleTrackTime => AreasLocator.Instance.ModuleTrackTime;
        
        private const float dayHeight = 117f; // Height of day
        // Link to class that is responsible for the reminders window
        private readonly RemindersModule remindersModule;
        
        // Array of day items
        private DayView[] days;
        // Array of week days
        private WeekInfo.DayOfWeek[] daysOfWeek;

        // Create and setup
        public MainPart(RectTransform rectTransform, RemindersModule remindersModule)
        {
            // Save components
            this.remindersModule = remindersModule;
            // Initialize lines between day items
            InitializeSplitLines(rectTransform);
            // Initialize day items
            InitializeDays(rectTransform);
        }

        // Setup by current session
        public void SetupSession()
        {
            // Setup order of days of week
            daysOfWeek = WeekInfo.DaysOrder();
            // Update names of days of week
            UpdateDays();

            // Update time
            var defaultTime = new HomeTime();
            var hasTime = false;
            
            // Update reminder time for each day by session 
            for (var i = 0; i < daysOfWeek.Length; i++)
            {
                var (active, time) = remindersModule.CurrentSession.SessionInfo[daysOfWeek[i]];
                days[i].SetupState(active, time);
                
                if (!hasTime && active)
                {
                    defaultTime = time;
                    hasTime = true;
                }
            }
            
            // Setup default time for track time view
            ModuleTrackTime.SetupConstructedTime(hasTime, defaultTime);
        }

        // Update session by day items info 
        public void UpdateSession()
        {
            for (var i = 0; i < daysOfWeek.Length; i++)
                remindersModule.CurrentSession.SessionInfo[daysOfWeek[i]] = days[i].GetNotification();
        }

        // Create set of lines between day items
        private void InitializeSplitLines(RectTransform rectTransform)
        {
            var upperPosition = dayHeight * 2.5f;
            for (var i = 0; i < 6; i++)
            {
                var line = SceneResources.GetPreparedCopy("Reminders Line");
                line.SetParent(rectTransform);
                line.SetRectTransformAnchorHorizontal(55, 55);
                line.anchoredPosition = new Vector2(0, upperPosition - dayHeight * i);
            }
        }
        
        // Initialize day items
        private void InitializeDays(RectTransform rectTransform)
        {
            days = new DayView[7];
            var upperPosition = dayHeight * 3f;
            for (var i = 0; i < 7; i++)
            {
                var day = new DayView(rectTransform);
                day.SetPosition(upperPosition - dayHeight * i);
                days[i] = day;
            }
        }

        // Update names of days of week
        private void UpdateDays()
        {
            for (var i = 0; i < days.Length; i++)
                days[i].SetupDay(daysOfWeek[i]);
        }
    }
}
