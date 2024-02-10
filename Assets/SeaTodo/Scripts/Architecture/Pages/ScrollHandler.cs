using UnityEngine;

namespace Architecture.Pages
{
    // Class for determining access to clicks when using page scrolling
    public static class ScrollHandler
    {
        public static bool AccessByScroll => accessByScroll;

        private static bool accessByScroll = true;
        private static float deltaCollector;

        private const float moveLimit = 10f;

        public static void Unlock()
        {
            deltaCollector = 0;
            accessByScroll = true;
        }

        public static void AddDeltaMove(Vector2 delta)
        {
            if (!accessByScroll)
                return;
            
            deltaCollector += Mathf.Abs(delta.x) + Mathf.Abs(delta.y);
            SetScrolledState();
        }
        
        private static void SetScrolledState()
        {
            if (deltaCollector > moveLimit)
                accessByScroll = false;
        }
    }
}
