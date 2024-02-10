using System.Collections.Generic;
using System.Linq;
using Architecture.Data;
using Architecture.Data.Structs;
using HomeTools.Other;
using HTools;
using MainActivity.MainComponents;
using Modules.Notifications;
using Theming;
using UnityEngine;

namespace Architecture.ModuleReminders
{
    // The class that is responsible for the reminders window
    public class RemindersModule
    {
        private readonly RectTransform rectTransform; // Rect object of view
        private readonly MainPart mainPart; // Main view component

        // Notification editing session
        public ReminderSession CurrentSession { private set; get; }
        
        // Setup and create
        public RemindersModule()
        {
            // Find object in scene resources
            rectTransform = SceneResources.Get("Reminders Module").GetComponent<RectTransform>();
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.RemindersModule);
            // Create main view component
            mainPart = new MainPart(rectTransform, this);
            // Colorize parts that marked as RemindersModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.RemindersModule);
        }

        // Setup reminders view to place
        public void SetRemindersToPlace(RectTransform parent, Vector2 position)
        {
            rectTransform.SetParent(parent);
            rectTransform.anchoredPosition = position;
            rectTransform.SetRectTransformAnchorHorizontal(0, 0);
        }

        // Setup session to reminders view
        public void SetupSession(ReminderSession newSession)
        {
            if (newSession == null)
                return;
            
            CurrentSession = newSession;
            mainPart.SetupSession();
        }
        
        // Apply edited reminders to task data
        public void SetupRemindersToFlow(Flow flow)
        {
            // Get reminders data from session component
            var remindersInfo = GetPackagedFlowData();

            // Check for update notifications
            if (flow.Reminders != null)
            {
                if (remindersInfo.Count != flow.Reminders.Count 
                    || remindersInfo.Except(flow.Reminders).Any())
                    AppNotifications.ShouldUpdate = true;
            }
            else if (remindersInfo.Count > 0)
            {
                AppNotifications.ShouldUpdate = true;
            }

            // Set reminders data to flow data
            flow.Reminders = remindersInfo;
        }

        // Get reminders data from session component
        private Dictionary<int, HomeTime> GetPackagedFlowData()
        {
            if (CurrentSession == null)
                return null;
            
            // Refresh session data
            mainPart.UpdateSession();

            // Create reminders data
            var data = new Dictionary<int, HomeTime>();

            // Setup reminders data
            var sessionInfo = CurrentSession.SessionInfo;
            for (var i = 0; i < sessionInfo.Count; i++)
            {
                var day = sessionInfo.ElementAt(i);
                if (day.Value.active)
                    data.Add(i, day.Value.time);
            }

            return data;
        }
    }
}
