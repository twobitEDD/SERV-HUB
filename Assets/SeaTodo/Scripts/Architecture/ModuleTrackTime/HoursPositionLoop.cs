using Architecture.Data;
using UnityEngine;

namespace Architecture.ModuleTrackTime
{
    // Looped line calculation to display hours
    public static class HoursPositionLoop
    {
        public static int GetActualHour(int position)
        {
            var english = AppCurrentSettings.EnglishFormat;
            return english ? GetHourEnglish(position) : GetHourFull(position);
        }

        public static int GetHourEnglish(int position)
        {
            while (position < 0)
            {
                position += 12;
            }
            
            while (position > 12)
            {
                position -= 12;
            }
            
            position = Mathf.Abs(position) - 13;

            if (Mathf.Abs(position) == 13)
                position = 1;

            return Mathf.Abs(position);
        }
        
        public static int GetHourFull(int position)
        {
            while (position < 0)
            {
                position += 24;
            }
            
            while (position > 24)
            {
                position -= 24;
            }
            
            position = Mathf.Abs(position) - 24;

            if (Mathf.Abs(position) == 24)
                position = 0;

            return Mathf.Abs(position);
        }

        public static int GetPositionByHour(int hour, bool englishFormat)
        {
            if (!englishFormat)
                return 24 - hour;
            
            if (hour == 1)
                return 0;
            
            return 13 - hour;
        }
    }
}
