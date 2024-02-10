using Architecture.Statistics;
using HomeTools.Handling;
using HTools;
using UnityEngine;

namespace Architecture.WorkArea.WeeklyActivity
{
    // Weekly activity view object
    public class MainStats
    {
        public readonly RectTransform RectTransform;
        private readonly StatisticsScroll statisticsScroll;
        private readonly WeeklyStatistics weeklyStatistics;
        private readonly HandleObject handleObject;

        // Create in rect transform
        public MainStats(RectTransform statsContainer)
        {
            RectTransform = statsContainer;
            
            // Setup scroll parameters
            var scrollParameters = new ScrollProperties()
            {
                JointSpeedTouched = 0.2f,
                JointSpeedFree = 0.23f,
                InertiaMultiplier = 1f,
                InertiaMultiplierDelta = 1f,
            };
            
            // Create handle of view
            handleObject = new HandleObject(statsContainer.Find("Handler").gameObject);
            // Create main view of calendar pages
            weeklyStatistics = new WeeklyStatistics(statsContainer);
            // Create scroll component for calendar view
            statisticsScroll = new StatisticsScroll(handleObject, weeklyStatistics, scrollParameters);
            // Setup scroll component to main view
            weeklyStatistics.SetupStatisticsScroll(statisticsScroll);
            // Join view to update calls
            SyncWithBehaviour.Instance.AddObserver(statisticsScroll, AppSyncAnchors.WorkAreaWeeklyActivity);
        }
    }
}
