using Architecture.Statistics;
using HomeTools.Handling;
using HTools;
using UnityEngine;

namespace Architecture.TaskViewArea.NormalView.Statistics
{
    // Task statistics view object
    public class MainStats
    {
        private readonly RectTransform rectTransform;
        public readonly StatisticsScroll StatisticsScroll;
        private readonly HandleObject handleObject;
        public readonly FlowStatistics FlowStatistics;

        public FlowDataCreator FlowDataCreator => FlowStatistics.FlowDataCreator;

        // Create in rect transform
        public MainStats(RectTransform statsContainer)
        {
            rectTransform = statsContainer;
            
            // Setup scroll parameters
            var scrollParameters = new ScrollProperties()
            {
                JointSpeedTouched = 0.23f,
                JointSpeedFree = 0.14f,
                InertiaMultiplier = 0.5f,
                InertiaMultiplierDelta = 1f,
            };
            
            // Create handle of view
            handleObject = new HandleObject(statsContainer.Find("Handler").gameObject);
            // Create main view of statistic pages
            FlowStatistics = new FlowStatistics(statsContainer);
            // Create scroll component for view
            StatisticsScroll = new StatisticsScroll(handleObject, FlowStatistics, scrollParameters);
            // Setup scroll component to view
            FlowStatistics.SetupStatisticsScroll(StatisticsScroll);
            // Join view to update calls
            SyncWithBehaviour.Instance.AddObserver(StatisticsScroll);
        }

        // Update statistics view
        public void Update()
        {
            FlowStatistics.UpdateElements();
        }
    }
}
