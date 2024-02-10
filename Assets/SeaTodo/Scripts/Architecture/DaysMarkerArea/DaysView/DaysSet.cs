using System.Collections.Generic;
using System.Linq;
using HTools;
using UnityEngine;

namespace Architecture.DaysMarkerArea.DaysView
{
    // The component that is responsible for displaying day circles
    public class DaysSet : IBehaviorSync
    {
        private readonly RectTransform place; // Place for circles
        private readonly DaysPool daysPool; // Pool component of day circles

        // Keep current month data packages
        private MonthMarkersPackage currentMarkersPackage;
        // List of day items
        public readonly List<DayItem> Days = new List<DayItem>();
        
        // Left border for day circles line
        private const float leftBorder = -247f;
        // Right border for day circles line
        private const float rightBorder = 333f;
        // Y position of day circles
        public const float DayHeightPosition = 0f;

        // Save components
        public DaysSet(RectTransform place, DaysPool daysPool)
        {
            this.place = place;
            this.daysPool = daysPool;
        }

        public void Start() { }

        // Calls every frame using the interface
        public void Update()
        {
            foreach (var dayItem in Days)
                dayItem?.Update();
        }
        
        // Update days view by new month data package
        public void SetupByDefault(MonthMarkersPackage markersPackage)
        {
            // Save new data package
            currentMarkersPackage = markersPackage;
            // List with day items
            UpdateDaysList();
            // Update anchor position of day items
            UpdateDaysPositions();
            // Update day characteristics
            UpdateDaysStatuses();
            // Update colors of day items immediately
            UpdateDaysColorsByDefault();
        }

        // Update days view by new month data package with animation
        public void SetupByDynamic(MonthMarkersPackage markersPackage)
        {
            // Save new data package
            currentMarkersPackage = markersPackage;
            // Calculate day items count that should add or remove in list
            var daysCount = currentMarkersPackage.Days.Count;
            var delta = daysCount - Days.Count;

            // If should add days
            if (delta > 0)
                DynamicAddDays(delta);
            
            // If should remove days
            if (delta < 0)
                DynamicRemoveDays(delta);

            // Start animation of move days
            DynamicMoveDays();
            // Update day characteristics
            UpdateDaysStatuses();
            // Start animation of update colors of days
            DynamicColorizeDays();
        }
        
        // Add days count
        private void DynamicAddDays(int count)
        {
            // Colorize days to color that means no characteristic
            ColorizeCurrentDaysToEmptyColor();

            // Add days
            for (var i = 0; i < count; i++)
            {
                // Get new day item
                var addedDay = daysPool.GetItem(place);
                // Add item to list
                Days.Add(addedDay);
                // Set position of new day in line
                addedDay.SetPosition(GetDayPosition(Days.Count - 1));
                // Colorize day to hidden color
                addedDay.ColorizeToHiddenColor();
                
                // Check for exceptions
                if (Days.Count > currentMarkersPackage.Days.Count)
                    continue;
                
                // Setup characteristic id to day item
                addedDay.DayStatus = currentMarkersPackage.Days.ElementAt(Days.Count - 1).Value;
                // Start animation for new day item
                addedDay.ColorizeFromHiddenColor();
            }
        }

        // Remove days count
        private void DynamicRemoveDays(int count)
        {
            // Count of days that need to remove
            count = Mathf.Abs(count);
            
            // Remove days
            for (var i = 0; i < count; i++)
            {
                // Get day at the end of line
                var day = Days[Days.Count - 1];
                // Remove day from list
                Days.RemoveAt(Days.Count - 1);
                // Setup day to pool without parenting
                daysPool.SetToPool(day, false);
                // Start hide animation
                day.ColorizeToHiddenColorByDynamic();
            }
            
            // Colorize days to color that means no characteristic
            ColorizeCurrentDaysToEmptyColor();
        }
        
        // Colorize days to color that means no characteristic
        private void ColorizeCurrentDaysToEmptyColor()
        {
            for (var i = 0; i < Days.Count; i++)
            {
                // Check for exceptions
                if (Days.Count > currentMarkersPackage.Days.Count)
                    continue;

                // Update characteristic status
                Days[i].DayStatus = currentMarkersPackage.Days.ElementAt(Days.Count - 1).Value;
                // Start animation
                Days[i].ColorizeFromHiddenColor();
            }
        }

        // Start move animation of days to target positions
        private void DynamicMoveDays()
        {
            for (var i = 0; i < Days.Count; i++)
            {
                Days[i].SetDynamicPosition(GetDayPosition(i));
            }
        }

        // Start colorize days with animation 
        private void DynamicColorizeDays()
        {
            for (var i = 0; i < Days.Count; i++)
                Days[i].UpdateByDynamic();
        }

        // Update days list
        private void UpdateDaysList()
        {
            // Calculate day items count that should add or remove in list
            var daysCount = currentMarkersPackage.Days.Count;
            var delta = daysCount - Days.Count;

            // Add days
            while (delta > 0)
            {
                delta--;
                Days.Add(daysPool.GetItem(place));
            }
            
            // Remove days
            while (delta < 0)
            {
                delta++;
                var lastDay = Days[Days.Count - 1];
                Days.Remove(lastDay);
                daysPool.SetToPool(lastDay);
            }
        }

        // Update day items position without animation
        private void UpdateDaysPositions()
        {
            for (var i = 0; i < Days.Count; i++)
                Days[i].SetPosition(GetDayPosition(i));
        }

        // Calculate day item position by position in order
        private float GetDayPosition(int dayOrder)
        {
            // Count of days in month
            var daysCount = currentMarkersPackage.Days.Count;
            // Length for days line
            const float length = rightBorder - leftBorder;
            // Calculate delta position of day by percentage
            var position = ((float)dayOrder/(daysCount - 1)) * length;
            // Add delta position to left side
            return leftBorder + position;
        }

        // Update days characteristics
        private void UpdateDaysStatuses()
        {
            for (var i = 0; i < Days.Count; i++)
            {
                // Check for exceptions
                if (i >= currentMarkersPackage.Days.Count)
                    continue;
                
                Days[i].DayStatus = currentMarkersPackage.Days.ElementAt(i).Value;
            }
        }
        
        // Update colors of day items by default
        private void UpdateDaysColorsByDefault()
        {
            for (var i = 0; i < Days.Count; i++)
                Days[i].UpdateByDefault();
        }
    }
}
