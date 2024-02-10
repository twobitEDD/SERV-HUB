using System.Collections.Generic;
using UnityEngine;

namespace Architecture.Other
{
    // Class for loading task icon colors
    public static class FlowColorLoader
    {
        private static readonly List<Color> Colors = new List<Color>()
        {
            new Color(0.96f, 0.32f, 0.12f, 1f),
            new Color(0.98f, 0.55f, 0f, 1f),
            new Color(1f, 0.7f, 0f, 1f),
            new Color(0.79f, 0.83f, 0.22f),
            new Color(0.58f, 0.83f, 0.3f),
            
            new Color(0.16f, 0.63f, 0.2f),
            new Color(0f, 0.71f, 0.64f),
            new Color(0f, 0.73f, 0.82f),
            new Color(0.01f, 0.61f, 0.9f, 1f),
            new Color(0.12f, 0.53f, 0.9f, 1f),
            
            new Color(0.22f, 0.29f, 0.67f, 1f),
            new Color(0.37f, 0.21f, 0.69f, 1f),
            new Color(0.56f, 0.14f, 0.67f, 1f),
            new Color(0.85f, 0.11f, 0.38f, 1f),
            new Color(0.9f, 0.22f, 0.21f, 1f),
        };

        public static Color LoadColorById(int id)
        {
            if (id < 0)
                id = 0;
            
            if (id >= Colors.Count)
                id = Colors.Count - 1;

            return Colors[id];
        }

        public static int GetPaletteCount() => Colors.Count;
        
        // Converting color object to Hex format
        public static string HexConverter(Color color)
        { 
            var r = (int)(color.r * 256);
            var g = (int)(color.g * 256);
            var b = (int)(color.b * 256);
            return  $"{r:X2}{g:X2}{b:X2}";
        }
    }
}
