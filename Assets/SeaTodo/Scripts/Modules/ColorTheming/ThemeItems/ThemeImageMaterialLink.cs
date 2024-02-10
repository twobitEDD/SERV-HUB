using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ColorTheming.ThemeItems
{
    // Component for create additional graphics component to App theming module
    public class ThemeImageMaterialLink : MonoBehaviour
    {
        public Material whiteSprite; // Material of image component for white theme
        public Material darkSprite;  // Material of image component for dark theme
        
        private void Start()
        {
            var image = gameObject.GetComponent<Image>();
            
            if (image == null)
                return;
            
            AppTheming.AddGraphics(new GraphicImageMaterialItem(image, whiteSprite, darkSprite));
            Destroy(this);
        }
    }
}
