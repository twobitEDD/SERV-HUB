using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ColorTheming.ThemeItems
{
    // Component for create additional graphics component to App theming module
    public class ThemeGraphicsLink : MonoBehaviour
    {
        public Sprite whiteSprite; // Sprite for white theme
        public Sprite darkSprite; // Sprite for dark theme
        
        private void Start()
        {
            var image = gameObject.GetComponent<Image>();
            
            if (image == null)
                return;
            
            AppTheming.AddGraphics(new GraphicImageItem(image, whiteSprite, darkSprite));
            Destroy(this);
        }
    }
}
