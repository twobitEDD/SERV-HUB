using System.Collections.Generic;
using Theming;
using UnityEngine.UI;

namespace InternalTheming
{
    // Interface for components of UI packages
    public interface IThemeItem
    {
        // Add UI element
        void AddItem(Graphic item, ColorTheme colorTheme);
        
        // Remove UI element
        void RemoveItem(Graphic item, ColorTheme colorTheme);

        // Get UI elements
        List<(Graphic text, ColorTheme themeColor)> GetGraphicsItems();
    }
}
