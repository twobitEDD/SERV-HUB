using System;

namespace Architecture.DaysMarkerArea.DaysColors.UpdateModule
{
    // Component for rename day characteristic session
    public class UpdateMarkerSession
    {
        private readonly Action updateAction; // Call action when finished
        public readonly int ColorMarker; // Id of day characteristic item in list

        // Create session
        public UpdateMarkerSession(int colorMarker, Action updateAction)
        {
            this.updateAction = updateAction; // Save action
            ColorMarker = colorMarker; // Save id of characteristic item
        }
        
        // Finish session
        public void Finish() => updateAction?.Invoke();
    }
}
