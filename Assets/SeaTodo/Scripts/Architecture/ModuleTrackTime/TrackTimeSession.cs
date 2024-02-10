using System;
using Architecture.Data.Structs;

namespace Architecture.ModuleTrackTime
{
    // Component of edit time session
    public class TrackTimeSession
    {
        public HomeTime HomeTime; // Current time in tracker
        private readonly Action<HomeTime> closedAction; // Call action when close

        // Create session
        public TrackTimeSession(HomeTime homeTime, Action<HomeTime> closedAction)
        {
            HomeTime = homeTime;
            this.closedAction = closedAction;
        }

        // Finish session
        public void FinishSession(HomeTime newTime)
        {
            closedAction.Invoke(newTime);
        }
    }
}
