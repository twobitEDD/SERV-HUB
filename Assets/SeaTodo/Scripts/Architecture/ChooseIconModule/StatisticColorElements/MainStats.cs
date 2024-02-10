using Architecture.Statistics;
using HomeTools.Handling;
using HTools;
using UnityEngine;

namespace Architecture.ChooseIconModule.StatisticColorElements
{
    // Colors view object
    public class MainStats
    {
        private readonly RectTransform rectTransform;
        public readonly StatisticsScroll StatisticsScroll;
        private readonly HandleObject handleObject;
        public readonly ChooseColorStatistics ChooseColorStatistics;

        public ChooseColorDataCreator ChooseColorDataCreator => ChooseColorStatistics.ChooseColorDataCreator;
        
        // Create in rect transform
        public MainStats(RectTransform statsContainer)
        {
            rectTransform = statsContainer;
            
            // Setup scroll parameters
            var scrollParameters = new ScrollProperties()
            {
                JointSpeedTouched = 0.25f,
                JointSpeedFree = 0.23f,
                InertiaMultiplier = 0.5f,
                InertiaMultiplierDelta = 1f,
            };
            
            // Create handle of view
            handleObject = new HandleObject(statsContainer.Find("Handler").gameObject);
            // Create main view of colors pages
            ChooseColorStatistics = new ChooseColorStatistics(statsContainer);
            // Create scroll component for view
            StatisticsScroll = new StatisticsScroll(handleObject, ChooseColorStatistics, scrollParameters);
            // Setup scroll component to view
            ChooseColorStatistics.SetupStatisticsScroll(StatisticsScroll);
            // Join view to update calls
            SyncWithBehaviour.Instance.AddObserver(StatisticsScroll, AppSyncAnchors.ChooseIconModule);
        }
    }
}
