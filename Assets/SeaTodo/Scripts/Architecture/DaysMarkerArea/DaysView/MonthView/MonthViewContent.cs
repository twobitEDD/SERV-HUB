using System.Collections.Generic;
using System.Linq;
using MainActivity.MainComponents;
using UnityEngine;

namespace Architecture.DaysMarkerArea.DaysView.MonthView
{
    // List of day items for month view
    public class MonthViewContent
    {
        public readonly RectTransform RectTransform; // rect parent for days list
        private readonly MonthDaysPool monthDaysPool; // Pool component with day items
        private readonly List<MonthDay> monthDays = new List<MonthDay>(); // List with active items
        
        private MonthMarkersPackage markersPackage; // Month data package
        public const float ShiftBetweenDays = 117f; // Distance between day items
        
        // Create and setup
        public MonthViewContent(RectTransform rectTransform)
        {
            RectTransform = rectTransform;
            // Create pool component
            monthDaysPool = new MonthDaysPool(rectTransform.Find("Pool").GetComponent<RectTransform>());
            
            // Generate 30 items for month
            GenerateBaseDays();
        }

        // Update list by new month data package
        public void SetupMonth(MonthMarkersPackage monthMarkersPackage)
        {
            // Save month data package
            markersPackage = monthMarkersPackage;

            if (markersPackage == null)
                return;

            // Calculate day items count that should add or remove in list
            var delta = markersPackage.Days.Count - monthDays.Count;    
            
            // If should add days
            if (delta > 0)
                AddDays(delta);

            // If should remove days
            if (delta < 0)
                RemoveDays(delta);
            
            // Update rect size by day items
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, 
                Mathf.Abs(monthDays[monthDays.Count - 1].RectTransform.anchoredPosition.y) + ShiftBetweenDays * 0.37f);

            // Update day items by new month data package
            UpdateDaysByNewInfoDefault();
        }

        // Add days count
        private void AddDays(int count)
        {
            for (var i = 0; i < count; i++)
            {
                // Get new day item
                var day = monthDaysPool.GetItem(RectTransform);
                // Add item to list
                monthDays.Add(day);
                // Set position of new item
                day.RectTransform.anchoredPosition = new Vector2(0, -(monthDays.Count - 1) * ShiftBetweenDays - ShiftBetweenDays * 0.5f);
            }
        }

        // Remove days count
        private void RemoveDays(int count)
        {
            // Count of days that need to remove
            count = Mathf.Abs(count);

            for (var i = 0; i < count; i++)
            {
                // Get day at the end of list
                var day = monthDays[monthDays.Count - 1];
                // Remove day from list
                monthDays.Remove(day);
                // Setup day to pool
                monthDaysPool.SetToPool(day);
            }
        }

        // Generate 30 items for month
        private void GenerateBaseDays()
        {
            for (var i = 0; i < 30; i++)
            {
                // Get new day item
                var day = monthDaysPool.GetItem(RectTransform);
                // Add item to list
                monthDays.Add(day);
                // Set position of new item
                day.RectTransform.anchoredPosition = new Vector2(0, -ShiftBetweenDays * i - ShiftBetweenDays * 0.5f);
            }
        }

        // Update day items by new month data package
        private void UpdateDaysByNewInfoDefault()
        {
            for (var i = 0; i < monthDays.Count; i++)
            {
                // Checking for errors
                if (i >= markersPackage.Days.Count)
                    continue;
                
                // Get data for day item
                var dayElement = markersPackage.Days.ElementAt(i);
                // Reset item by data
                monthDays[i].ResetByDefault(dayElement.Key, dayElement.Value, i == markersPackage.CurrentDay);
                // Update activity of border of item
                monthDays[i].SetActivateLineBorder(i != monthDays.Count - 1);
                // Send month data package to item
                monthDays[i].SetMonthPackage(markersPackage);
            }
        }
    }
}
