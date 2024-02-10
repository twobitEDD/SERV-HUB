using System;
using Architecture.TextHolder;
#if UNITY_IOS || UNITY_IPHONE
using Unity.Notifications.iOS;

namespace Modules.Notifications
{
    // Class for create notifications for iOS (works with Unity Notifications)
    public static class IosNotificationsBuilder
    {
        // Channel
        private const string channel = "sea_do";
        
        // Rebuild notifications
        public static void RebuildNotifications()
        {
            ClearNotifications();
            
            AddNotifications();
            OnForeground();
        }

        // Add notifications by tasks
        private static void AddNotifications()
        {
            var remindersList = NotificationsDataCreator.GetData();

            for (var i = 0; i < remindersList.Count; i++)
            {
                var (title, description, day, time) = remindersList[i];

                var dateTime = NotificationsDataCreator.CreateFireTime(day, time);
                for (var repeat = 0; repeat < 4; repeat++)
                {
                    var id = (i * 100) + repeat;
                    SendNotification(title, description, dateTime, id);
                    dateTime += new TimeSpan((repeat + 1) * 7, 0 ,0,0);
                }
            }
        }

        // Plan notification
        private static void SendNotification(string title, string name, DateTime time, int id)
        {
            var calendarTrigger = new iOSNotificationCalendarTrigger()
            {
                Year = time.Year,
                Month = time.Month,
                Day = time.Day,
                Hour = time.Hour,
                Minute = time.Minute,
                Second = 0,
                Repeats = false
            };

            var notification = new iOSNotification()
            {
                Identifier = $"{channel}_{id}",
                Title = name,
                Body = TextLocalization.GetLocalization(TextKeysHolder.NotificationUpdateProgress),
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = channel,
                ThreadIdentifier = channel,
                Trigger = calendarTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
        }

        // Remove all planed notifications
        private static void ClearNotifications()
        {
            iOSNotificationCenter.RemoveAllScheduledNotifications();
            iOSNotificationCenter.RemoveAllDeliveredNotifications();
        }
        
        // Setup Foreground
        private static void OnForeground()
        {
            iOSNotificationCenter.ApplicationBadge = 0;
        }
    }
}
#endif
