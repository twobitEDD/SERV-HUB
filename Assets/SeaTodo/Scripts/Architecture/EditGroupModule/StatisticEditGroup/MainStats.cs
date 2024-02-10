using Architecture.Statistics;
using HomeTools.Handling;
using HTools;
using UnityEngine;

namespace Architecture.EditGroupModule.StatisticEditGroup
{
    // Title icons line view
    public class MainStats
    {
        private readonly RectTransform rectTransform;
        private readonly StatisticsScroll statisticsScroll;
        private readonly HandleObject handleObject;
        public readonly EditGroupStatistics EditGroupStatistics;

        public EditGroupDataCreator EditGroupDataCreator => EditGroupStatistics.EditGroupDataCreator;
        
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
            // Create main view of title icons pages
            EditGroupStatistics = new EditGroupStatistics(statsContainer);
            // Create scroll component for view
            statisticsScroll = new StatisticsScroll(handleObject, EditGroupStatistics, scrollParameters);
            // Setup scroll component to view
            EditGroupStatistics.SetupStatisticsScroll(statisticsScroll);
            // Join view to update calls
            SyncWithBehaviour.Instance.AddObserver(statisticsScroll, AppSyncAnchors.EditTitleModule);
        }
    }
}
