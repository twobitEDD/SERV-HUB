using System;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;

namespace Architecture.TaskViewArea
{
    // Components with tools for estimate task finish date
    public static class EstimateFinishDate
    {
        // Check if task completed
        public static bool CompletedProgress(Flow flow)
        {
            var inProgress = flow.GetIntProgress();
            var inGoalInt = FlowInfoAll.GetGoalByOriginFlowInt(flow.Type, flow.GoalInt);
            
            return inProgress >= inGoalInt;
        }
        
        // Estimate finish date of task
        public static DateTime EstimateByFlow(Flow flow)
        {
            // Get number of task progress
            var inProgress = flow.GetIntProgress();
            // Get number of task goal
            var inGoalInt = FlowInfoAll.GetGoalByOriginFlowInt(flow.Type, flow.GoalInt);

            // Calculate progress per day
            var progressPerDay = (float) inProgress / flow.GoalData.Count;
            // Calculate how mush progress need for finish
            var needToDo = inGoalInt - inProgress;

            // Check for exceptions
            if (float.IsNaN(progressPerDay) || progressPerDay < 1)
                progressPerDay = 1f;
            
            // Calculate days count
            var days = (int)(needToDo / progressPerDay);

            // Create estimated finish date
            var trackedAll = DateTime.Now + new TimeSpan(days, 0, 0, 0);

            return trackedAll;
        }
    }
}
