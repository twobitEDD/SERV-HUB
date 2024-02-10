using UnityEngine;

namespace Architecture.ModuleTrackTime
{
    // Looped line calculation to display minutes
    public static class MinutesPositionLoop
    {
        public static int GetMinutes(int position)
        {
            while (position < 0)
            {
                position += 60;
            }
            
            while (position > 60)
            {
                position -= 60;
            }
            
            position = Mathf.Abs(position) - 61;

            if (Mathf.Abs(position) == 61)
                position = 1;
            
            if (Mathf.Abs(position) == 60)
                position = 0;

            return Mathf.Abs(position);
        }

        public static int GetPositionByMinutes(int minutes)
        {
            if (minutes == 1)
                return 0;
            
            return 61 - minutes;
        }
    }
}
