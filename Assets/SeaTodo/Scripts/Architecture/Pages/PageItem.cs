using UnityEngine;

namespace Architecture.Pages
{
    // Page for scrolling with content
    public class PageItem
    {
        // Setup max border of page moving
        public float MaxAnchorByPosition
        {
            set => MaxAnchor = Mathf.Abs(value) - MainCanvas.RectTransform.rect.height + 
                               AppElementsInfo.DistanceContentToBottom;
        }
        
        // Max border of page moving
        public float MaxAnchor { set; get; }
        // Min border of page moving
        public float MinAnchor { set; get; }

        // Rect of page
        public readonly RectTransform Page;
        
        // Set page activity state
        public bool Enable
        {
            set => Page.gameObject.SetActive(value);
        }
        
        // Create
        public PageItem(RectTransform page) => Page = page;

        // Update page position by moving canvas delta
        public void ScrollUpdate(float delta)
        {
            var position = PageScrollTools.MoveCalculation(delta, Page.anchoredPosition.y, MinAnchor, MaxAnchor);
            PageScrollTools.ResetInertiaWhenFree(position, MinAnchor, MaxAnchor);
            
            Page.anchoredPosition = new Vector2(Page.anchoredPosition.x, position);
        }

        // Add UI to page with position
        public void AddContent(RectTransform content, Vector2 position)
        {
            content.SetParent(Page.transform);
            content.anchoredPosition = position;
        }
    
        // Add UI to page
        public void AddContent(RectTransform content)
        {
            content.SetParent(Page.transform);
        }
    }
}
