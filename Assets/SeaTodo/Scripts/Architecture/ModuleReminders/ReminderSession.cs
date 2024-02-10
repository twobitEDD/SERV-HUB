using System.Collections.Generic;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.Other;
using Modules.Notifications;

namespace Architecture.ModuleReminders
{
    // Component of reminders session
    public class ReminderSession
    {
        // Reminders data for session
        public readonly Dictionary<WeekInfo.DayOfWeek, (bool active, HomeTime time)> SessionInfo;
        private bool requestAsked;

        // Create session
        public ReminderSession()
        {
            SessionInfo = new Dictionary<WeekInfo.DayOfWeek, (bool, HomeTime)>()
            {
                {WeekInfo.DayOfWeek.Sunday, (false, new HomeTime())},
                {WeekInfo.DayOfWeek.Monday, (false, new HomeTime())},
                {WeekInfo.DayOfWeek.Tuesday, (false, new HomeTime())},
                {WeekInfo.DayOfWeek.Wednesday, (false, new HomeTime())},
                {WeekInfo.DayOfWeek.Thursday, (false, new HomeTime())},
                {WeekInfo.DayOfWeek.Friday, (false, new HomeTime())},
                {WeekInfo.DayOfWeek.Saturday, (false, new HomeTime())},
            };
        }

        // Check permission to receive notifications
        public void CheckToRequest()
        {
            if (requestAsked)
                return;

#if UNITY_ANDROID
            
#elif UNITY_IOS || UNITY_IPHONE
            AreasLocator.Instance.IosRequestAuthorization.Ask();
#endif
            
            requestAsked = true;
        }
    }
}
