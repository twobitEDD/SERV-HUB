using System.Collections.Generic;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.DaysMarkerArea.DaysColors;
using UnityEngine;

namespace Architecture.DaysMarkerArea.DaysView
{
    // Component with days characteristics data for month
    public class MonthMarkersPackage
    {
        public Dictionary<HomeDay, int> Days; // Days and characteristics ids
        public bool ShouldUpdate; // Update month days UI flag
        public bool Current; // Is current month day flag
        public int CurrentDay; // If today = day of month, if other days = -1
        public bool Passed; // If a month has already passed

        // Update day characteristic method
        public void UpdateDay(HomeDay homeDay, int marker)
        {
            // Does the day belong to this month
            if (!Days.ContainsKey(homeDay))
                return;

            var oldMarker = Days[homeDay]; // Save old day characteristic
            Days[homeDay] = marker; // Setup new day characteristic

            var dayInt = homeDay.HomeDayToInt(); // Convert date to int view
            // Check if days without characteristic
            if (marker < 0 && oldMarker >= 0)
            {
                if (AppData.PaletteDays.ContainsKey(dayInt))
                    AppData.PaletteDays.Remove(dayInt);

                return;
            }

            // Save day characteristic to data
            if (marker >= 0 && marker < ColorMarkersDescriptor.MarkersCount)
            {
                if (AppData.PaletteDays.ContainsKey(dayInt))
                    AppData.PaletteDays[dayInt] = marker;
                else
                    AppData.PaletteDays.Add(dayInt, marker);
            }
        }
    }
}
