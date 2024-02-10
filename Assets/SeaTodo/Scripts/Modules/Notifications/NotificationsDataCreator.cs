using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.Data;
using Architecture.Data.Structs;

namespace Modules.Notifications
{
    // Class for analyze tasks and create list of notifications
    public static class NotificationsDataCreator
    {
        // Get planned notifications of days for each active tasks
        public static List<(string title, string description, int day, HomeTime time)> GetData()
        {
            var result = new List<(string title, string description, int day, HomeTime time)>();

            var activeGroups = GetActiveGroups();

            for (var i = 0; i < activeGroups.Length; i++)
            {
                var activeFlows = GetActiveFlows(activeGroups[i]);

                foreach (var activeFlow in activeFlows)
                    foreach (var reminders in activeFlow.Reminders)
                    {
                        result.Add((activeGroups[i].Name, activeFlow.Name, reminders.Key,
                                reminders.Value));
                    }
            }

            return result;
        }

        // Create fire time by day of week and time
        public static DateTime CreateFireTime(int day, HomeTime time)
        {
            var timeNow = DateTime.Now;
            
            var currentDay = day - (int) timeNow.DayOfWeek;

            var (hours, minutes, _) = time.GetTime();
            
            if (currentDay < 0)
            {
                currentDay = 7 + currentDay;
            }
            else if (currentDay == 0 && (hours < timeNow.Hour || (hours == timeNow.Hour && minutes < timeNow.Minute)))
            {
                currentDay = 7;
            }
            
            var result = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day) + new TimeSpan(currentDay, 0, 0, 0);

            return new DateTime(result.Year, result.Month, result.Day, hours, minutes, 0);
        }
        
        // Get active tasks
        private static Flow[] GetActiveFlows(FlowGroup group) => group.Flows.Where(e => e.Reminders != null && e.Reminders.Count > 0).ToArray();
        // Get tasks global package
        private static FlowGroup[] GetActiveGroups() => AppData.GetGroups.Where(e=>e.Reminders).ToArray();
    }
}
