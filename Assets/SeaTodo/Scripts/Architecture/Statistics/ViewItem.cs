using System;
using Architecture.Statistics.Interfaces;
using UnityEngine;

namespace Architecture.Statistics
{
    // Component for motion of statistics page item
    public class ViewItem : IComparable<ViewItem>
    {
        private readonly RectTransform rectTransform; // Rect of statistics page
        private float heightPosition; // Y position of page item

        // Anchored position of page
        public Vector2 AnchoredPosition => rectTransform.anchoredPosition;
        // Component for setup graphic
        public readonly GraphItem GraphItem;

        // X position of page
        public float Position => rectTransform.anchoredPosition.x;
        // Width of page
        public float Width => rectTransform.sizeDelta.x;
        
        // Create and setup
        public ViewItem(RectTransform rectTransform, IViewAdditional viewAdditional)
        {
            this.rectTransform = rectTransform;
            viewAdditional.Setup(rectTransform);
            GraphItem = new GraphItem(rectTransform, viewAdditional);
        }

        // Setup Y position of page
        public void SetupHeightPosition(float position)
        {
            heightPosition = position;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, heightPosition);
        }

        // Add delta for motion by X axis
        public void AddDeltaWidth(float delta)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + delta, heightPosition);
        }
        
        // Setup X axis ofr page
        public void AddDeltaWidthFixed(float delta)
        {
            rectTransform.anchoredPosition = new Vector2(delta, heightPosition);
        }

        // Update statistics graphic in page to new data
        public void MoveDataForward() => GraphItem.UpdateStep(2);
        
        // Update statistics graphic in page to new data
        public void MoveDataBack() => GraphItem.UpdateStep(-2);

        // Compare order of page items
        public int CompareTo(ViewItem other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            
            return rectTransform.anchoredPosition.x.CompareTo(other.rectTransform.anchoredPosition.x);
        }
    }
}
