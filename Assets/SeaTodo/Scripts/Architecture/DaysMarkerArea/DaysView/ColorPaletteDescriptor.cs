using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Data.Structs;

namespace Architecture.DaysMarkerArea.DaysView
{
    // Generator of months data packages
    public class ColorPaletteDescriptor
    {
        // Get data packages of month by year
        public MonthMarkersPackage[] GetYearDaysPalette(int year)
        {
            // Get today day
            var today = Calendar.Today;
            // Convert day to int
            var todayInt = today.HomeDayToInt();

            // Create array of month packages
            var result = new MonthMarkersPackage[12];
            // Setup each month package
            for (var i = 0; i < result.Length; i++)
            {
                // Create month package component
                var month = new MonthMarkersPackage { Days = new Dictionary<HomeDay, int>() };
                // Create first day of months
                var firstDay = new HomeDay(year, i + 1, 1);
                // Calculate days count in month
                var daysInMonth = DateTime.DaysInMonth(year, i + 1);
                // Setup each day to package
                for (var d = 0; d < daysInMonth; d++)
                {
                    // Get day in int format
                    var key = firstDay.HomeDayToInt();
                    // Create default day characteristic (without characteristic)
                    var markerId = -1;

                    // Set value if these are future days
                    if (key > todayInt)
                        markerId = -2;

                    // Try to get day characteristic id and set to id
                    if (AppData.PaletteDays.ContainsKey(key))
                        markerId = AppData.PaletteDays[key];

                    // Add day info to list
                    month.Days.Add(firstDay, markerId);
                    // Add day for next iteration
                    firstDay.AddDays(1);
                }
                // Check if this month is current
                month.Current = today.Year == year && today.Month == i + 1;
                // Check today day in package
                month.CurrentDay = month.Current ? today.Day - 1 : -1;
                // Check if a month has already passed
                var lastMonthDay = month.Days.ElementAt(month.Days.Count - 1).Key.HomeDayToInt();
                month.Passed = lastMonthDay < todayInt;

                // Save month package that has been created
                result[i] = month;
            }
            
            return result;
        }
    }
}
