using System;

namespace Architecture.DaysMarkerArea.DaysView.DayMarkerPicker
{
    // Component for choose day characteristic session
    public class ChooseDaySession
    {
        private readonly Action<int> closeAction; // Call actions when finished
        public int MarkerId; // Selected id of characteristic

        // Create session
        public ChooseDaySession(Action<int> closeAction, int markerId)
        {
            this.closeAction = closeAction;
            MarkerId = markerId;
        }

        // Finish session
        public void Closed()
        {
            closeAction?.Invoke(MarkerId - 1);
        }
    }
}
