using System;
using Architecture.Data.Structs;
using Architecture.TextHolder;
#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine;

namespace Modules.Notifications
{
    // Class for create notifications for Android (works with Unity Notifications)
    public static class AndroidNotificationsBuilder
    {
        // Channel
        private const string channel = "sea_do";
        
        // Rebuild notifications
        public static void RebuildNotifications()
        {
            ClearNotifications();

            CreateChanel();
            AddNotifications();
        }

        // Create chanel for notifications
        private static void CreateChanel()
        {
            var c = new AndroidNotificationChannel()
            {
                Id = channel,
                Name = "Sea Channel",
                Importance = Importance.High,
                Description = "Sea Todo Notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(c);
        }
        
        // Add notifications by tasks
        private static void AddNotifications()
        {
            var remindersList = NotificationsDataCreator.GetData();

            foreach (var valueTuple in remindersList)
                SendNotification(valueTuple.title, valueTuple.description, valueTuple.day, valueTuple.time);
        }
        
        // Plan notification
        private static void SendNotification(string title, string name, int day, HomeTime homeTime)
        {
            var time = NotificationsDataCreator.CreateFireTime(day, homeTime);

            var notification = new AndroidNotification();
            notification.Title = name;
            notification.Text = TextLocalization.GetLocalization(TextKeysHolder.NotificationUpdateProgress);
            notification.FireTime = time;
            notification.RepeatInterval = new TimeSpan(7, 0, 0, 0);
            notification.SmallIcon = "icon_0";

            Debug.Log($"notification planned: {title}; {name}; {time};");
            AndroidNotificationCenter.SendNotification(notification, channel);
        }

        // Remove all planed notifications
        private static void ClearNotifications()
        {
            AndroidNotificationCenter.CancelAllNotifications();
            AndroidNotificationCenter.DeleteNotificationChannel(channel);
            AndroidNotificationCenter.CancelAllDisplayedNotifications();
        }
    }
}
#endif
