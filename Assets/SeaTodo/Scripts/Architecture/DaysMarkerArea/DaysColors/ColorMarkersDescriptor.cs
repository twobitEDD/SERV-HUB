using System.Linq;
using Architecture.Data;
using UnityEngine;

namespace Architecture.DaysMarkerArea.DaysColors
{
    // Class that contains colors of days characteristics
    public class ColorMarkersDescriptor
    {
        public static int MarkersCount => markerColors.Length; // Return count of colors

        // Return array of colors by app theme (light or dark)
        private static Color[] MarkerColorsByTheme => AppCurrentSettings.DarkTheme ? markerColorsDark : markerColors;

        // Color of days for light theme
        private static readonly Color[] markerColors = {
            new Color(0f, 0.77f, 1f),
            new Color(0.1f, 0.62f, 0.95f),
            new Color(0.24f, 0.4f, 0.87f),
            new Color(0.39f, 0.16f, 0.8f),
            new Color(0.49f, 0f, 0.75f),
        };
        
        // Color of days for dark theme
        private static readonly Color[] markerColorsDark = {
            new Color(0.27f, 0.83f, 1f),
            new Color(0.34f, 0.73f, 0.96f),
            new Color(0.44f, 0.56f, 0.91f),
            new Color(0.55f, 0.39f, 0.86f),
            new Color(0.62f, 0.27f, 0.82f),
        };

        // Return characteristic name by order in list
        public static string GetColorName(int queue)
        {
            // Checking for an array out of bounds
            if (queue < 0)
                queue = 0;

            // Checking for an array out of bounds
            if (queue >= MarkersCount)
                queue = MarkersCount - 1;
            
            // Return characteristics day
            return AppData.ColorMarkers[queue];
        }

        // Return color by order in list
        public static Color GetColor(int id)
        {
            // Checking for an array out of bounds
            if (id < 0)
                id = 0;

            // Checking for an array out of bounds
            if (id >= MarkersCount)
                id = MarkersCount - 1;
            
            // Return color by order in list
            return MarkerColorsByTheme[id];
        }

        // Get count of days that marked with characteristic by order in list
        public int GetDaysCountByMarkerId(int markerId) => 
            AppData.PaletteDays.Count(e => e.Value == markerId);
    }
}
