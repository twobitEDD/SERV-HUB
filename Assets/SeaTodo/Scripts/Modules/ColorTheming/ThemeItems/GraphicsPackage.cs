using System.Collections.Generic;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ColorTheming.ThemeItems
{
    // Components for additional UI graphics
    
    // Package of additional UI graphics
    public class GraphicsPackage
    {
        // Elements
        private readonly List<GraphicImageItem> items = new List<GraphicImageItem>();
        private readonly List<GraphicImageMaterialItem> itemsMaterial = new List<GraphicImageMaterialItem>();

        // Add element
        public void AddImage(GraphicImageItem item)
        {
            if (items.Contains(item))
                return;
            
            items.Add(item);
            item.UpdateSprite(!AppTheming.DarkTheme);
        }
        
        // Add element
        public void AddImage(GraphicImageMaterialItem item)
        {
            if (itemsMaterial.Contains(item))
                return;
            
            itemsMaterial.Add(item);
            item.UpdateSprite(!AppTheming.DarkTheme);
        }

        // Update UI elements
        public void UpdateGraphics()
        {
            foreach (var item in items)
                item.UpdateSprite(!AppTheming.DarkTheme);
            
            foreach (var item in itemsMaterial)
                item.UpdateSprite(!AppTheming.DarkTheme);
        }
    }

    // Component with images for different app themes
    public class GraphicImageItem
    {
        // Image component
        private readonly Image image;
        // Sprite for white theme
        private readonly Sprite whiteSprite;
        // Sprite for dark theme
        private readonly Sprite darkSprite;

        // Create component
        public GraphicImageItem(Image image, Sprite white, Sprite dark)
        {
            this.image = image;
            whiteSprite = white;
            darkSprite = dark;
        }

        // Update
        public void UpdateSprite(bool white) => image.sprite = white ? whiteSprite : darkSprite;
    }
    
    // Component with materials for different app themes
    public class GraphicImageMaterialItem
    {
        // Image component
        private readonly Image image;
        // Image material for white theme
        private readonly Material whiteSprite;
        // Image material for dark theme
        private readonly Material darkSprite;

        // Create component
        public GraphicImageMaterialItem(Image image, Material white, Material dark)
        {
            this.image = image;
            whiteSprite = white;
            darkSprite = dark;
        }

        // Update
        public void UpdateSprite(bool white) => image.material = white ? whiteSprite : darkSprite;
    }
}
