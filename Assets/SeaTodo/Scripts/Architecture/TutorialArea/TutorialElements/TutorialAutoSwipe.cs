using System;
using HTools;

namespace Architecture.TutorialArea.TutorialElements
{
    // Component for auto swipes of pages
    public class TutorialAutoSwipe : IBehaviorSync
    {
        private const int delay = 60 * 17; // Delay (updates count)
        private readonly Action swipeAction; // Action when swipe
        private int currentDelay; // Current delay to next swipe
        private bool active; // Active flag
        
        // Setup action
        public TutorialAutoSwipe(Action swipeAction)
        {
            this.swipeAction = swipeAction;
        }
        
        public void Start() { }

        public void Update()
        {
            if (!active)
                return;
            
            // Check if touched - reset timer
            if (InputHS.Touched)
                currentDelay = delay;
                
            // Decrease timer
            currentDelay--;
            
            // Check finished timer
            if (currentDelay > 0)
                return;
            
            // Invoke action and reset timer
            swipeAction.Invoke();
            currentDelay = delay;
        }

        // Activate component
        public void Open()
        {
            currentDelay = delay;
            active = true;
        }

        // Deactivate component
        public void Close()
        {
            active = false;
        }
    }
}
