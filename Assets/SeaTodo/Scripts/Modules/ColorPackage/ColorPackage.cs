using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ColorPackage
{
    // Component for color control of UI group
    public class ColorPackage
    {
        // UI elements
        private readonly List<Graphic> elements;

        // Create component
        public ColorPackage() => elements = new List<Graphic>();

        // Add UI element
        public void AddItems(IEnumerable<Graphic> items) => elements.AddRange(items);

        // Setup color to UI elements
        public void ColorizeItems(Color color)
        {
            foreach (var element in elements)
                element.color = new Color(color.r, color.g, color.b, element.color.a);
        }
    }
}
