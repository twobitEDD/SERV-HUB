using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.ColorTheming
{
    // Component for add UI element in scene 
    public class ThemeLink : MonoBehaviour
    {
        // Color marker of app theme
        public ColorTheme color;
        // Item of UI package
        public AppTheming.AppItem item;
        // Colorize item on awake
        public bool awakeColorize;
        // Disable actions of component
        public bool disable;
    
        [Space]
        public bool debug;
        private void Awake()
        {
            if (disable)
                return;
        
            var element = GetComponent<Graphic>();
            DebugMessage($"Element: {gameObject.name} - Awake");
        
            if (element == null) 
                return;
        
            DebugMessage($"Element: {gameObject.name} - Awake found");
            AppTheming.AddElement(element, color, item);
        
            if (awakeColorize)
                AppTheming.ColorizeElement(element, color);
        
            AwakeDestroy();
        }

        private void AwakeDestroy()
        {
            DebugMessage($"Element: {gameObject.name} - Try destroy");

            if (SceneResources.GetGameObjectPath(gameObject).Contains(SceneResources.ResourcesObjectName)) 
                return;
        
            DebugMessage($"Element: {gameObject.name} - Destroy");
            Destroy(this);
        }

        private void DebugMessage(string message)
        {
            if (!debug)
                return;
        
            Debug.Log(message);
        }
    }
}
