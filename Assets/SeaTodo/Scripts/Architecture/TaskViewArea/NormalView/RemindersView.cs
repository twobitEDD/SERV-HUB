using System.Linq;
using Architecture.Data;
using Architecture.Other;
using UnityEngine;

namespace Architecture.TaskViewArea.NormalView
{
    // Component of reminders view
    public class RemindersView
    {
        private readonly ReminderItem[] reminderItems; // Reminder item views

        // Create
        public RemindersView(RectTransform rectTransform)
        {
            // Create reminder icon views
            reminderItems = new ReminderItem[7];
            for (var i = 0; i < 7; i++)
                reminderItems[i] = new ReminderItem(rectTransform.Find($"Reminder {i + 1}").GetComponent<RectTransform>());
        }

        // Setup reminders view by task
        public void SetupViewByFlow(Flow flow)
        {
            // Update week days order
            InitializeDaysOrder();
            
            // Set reminder items as no reminders
            foreach (var reminderItem in reminderItems)
                reminderItem.Update(false);
            
            // Get info about task reminders
            var info = flow.Reminders;
            if (info == null)
                return;
            
            // Set reminders activity
            for (var i = 0; i < flow.Reminders.Count; i++)
            {
                var elementReminders = flow.Reminders.ElementAt(i);
                var activeList = reminderItems.Where(e => e.DayOfWeek == (WeekInfo.DayOfWeek) (elementReminders.Key));
               
                foreach (var item in activeList) item.Update(true);
            }
        }
        
        // Update day of week order
        private void InitializeDaysOrder()
        {
            var weekDaysOrdered = WeekInfo.DaysOrder();

            for (var i = 0; i < reminderItems.Length; i++)
                reminderItems[i].DayOfWeek = weekDaysOrdered[i];
        }
    }
}
