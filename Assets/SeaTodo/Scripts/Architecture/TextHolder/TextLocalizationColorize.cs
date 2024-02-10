using System;
using Architecture.Other;
using InternalTheming;
using UnityEngine.UI;

namespace Architecture.TextHolder
{
    // Component for colorize text parts
    public static class TextLocalizationColorize
    {
        // Key in localization text
        private const string keyText = "<color=#>";

        public static void ColorizeFilter(Text localizedText)
        {
            // Try find key
            if (!localizedText.text.Contains(keyText))
                return;

            // Try get component with color
            var colorsSet = localizedText.GetComponent<TextColorsSet>();

            if (colorsSet == null)
                return;
            
            var colorsId = colorsSet.colors.Length;
            var i = 0;
            
            // Setup color to text from component
            while (i < colorsId)
            {
                if (i >= colorsSet.colors.Length)
                    return;
                
                var id = localizedText.text.IndexOf(keyText, StringComparison.Ordinal) + keyText.Length - 1;
                var newText = localizedText.text.Insert(id, FlowColorLoader.HexConverter(ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), colorsSet.colors[i])));
                localizedText.text = newText;
                i++;
            }
        }
    }
}
