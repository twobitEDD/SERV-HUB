using Architecture.Pages;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.Elements
{
    // Component for working with background UI
    public class GroupBackground
    {
        // Name of background example in scene resources
        private const string assetName = "GroupBackground";
        public RectTransform RectTransform { get; } // Rect of background
        public readonly Image Image; // Image pf background
        public readonly Image Shadow; // Image of background shadow
        private const float shadowSize = 24f; // Shadow default size (const by UI size)
        
        // Create new background
        public GroupBackground()
        {
            // Initialize components
            RectTransform = Object.Instantiate(SceneResources.Get(assetName)).GetComponent<RectTransform>();
            Image = RectTransform.GetComponent<Image>();
            Shadow = RectTransform.Find("Shadow").GetComponent<Image>();
            
            // Setup anchors of background in screen 
            RectTransform.anchorMax = new Vector2(1, 1f);
            RectTransform.anchorMin = new Vector2(0, 1f);
            // Add scroll events to background object
            RectTransform.gameObject.AddComponent<AddScrollEvents>();
        }

        // Update visible params of background
        public void UpdateVisible(float height, float position)
        {
            // Setup scale and width of background
            RectTransform.transform.localScale = Vector3.one;
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 
                MainCanvas.RectTransform.sizeDelta.x + shadowSize * 2);
            // Setup custom pivot of background rect
            RectTransform.pivot = new Vector2(0.5f, 1 - shadowSize / height * 0.8f);
            
            // Setup background height
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
                height);

            // Update background position
            var oldPosition = RectTransform.anchoredPosition;
            RectTransform.anchoredPosition3D = Vector3.zero;
            RectTransform.anchoredPosition = new Vector2(oldPosition.x, position);
        }

        // Add UI objects in background object
        public void AddItemToBackground(RectTransform rectItem)
        {
            rectItem.SetParent(RectTransform);
        }
    }
}
