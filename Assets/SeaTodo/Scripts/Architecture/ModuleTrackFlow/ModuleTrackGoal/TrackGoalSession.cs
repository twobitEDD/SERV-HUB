using System;
using Architecture.Data;
using Architecture.Elements;

namespace Architecture.ModuleTrackFlow.ModuleTrackGoal
{
    // Component of track goal session
    public class TrackGoalSession
    {
        // Link to track module
        private TrackFlowModule TrackFlowModule() => AreasLocator.Instance.TrackFlowModule;
        
        public readonly Flow Flow; // Task that in editing
        private readonly Action<int> finishSession; // Action of close action
        
        // Create
        public TrackGoalSession(Flow flow, Action<int> finishSession)
        {
            Flow = flow;
            this.finishSession = finishSession;
        }

        // Finish session
        public void FinishSession()
        {
            finishSession.Invoke(TrackFlowModule().GetOriginPosition());
        }
    }
}
