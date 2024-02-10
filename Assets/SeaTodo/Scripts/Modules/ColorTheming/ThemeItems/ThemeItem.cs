using System.Collections.Generic;
using Theming;
using UnityEngine.UI;

namespace InternalTheming
{
    // Component of UI elements package
    public class ThemeItem : IThemeItem
    {
        // Graphics list
        private readonly List<(Graphic, ColorTheme)> graphics = new List<(Graphic, ColorTheme)>();
        // Add UI elements
        public void AddItem(Graphic item, ColorTheme colorTheme) => graphics.Add((item, colorTheme));
        // Remove UI elements
        public void RemoveItem(Graphic item, ColorTheme colorTheme) => graphics.Remove((item, colorTheme));
        // Get UI elements
        public List<(Graphic, ColorTheme)> GetGraphicsItems() => graphics;
    }
}
