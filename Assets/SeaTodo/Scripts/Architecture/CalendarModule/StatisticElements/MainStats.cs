using Architecture.Statistics;
using HomeTools.Handling;
using HTools;
using UnityEngine;

namespace Architecture.CalendarModule.StatisticElements
{
    // Calendar view object
    public class MainStats
    {
        private readonly RectTransform rectTransform;
        private readonly StatisticsScroll statisticsScroll;
        private readonly HandleObject handleObject;
        public readonly CalendarStatistics CalendarStatistics;

        // Create in rect transform
        public MainStats(RectTransform statsContainer)
        {
            rectTransform = statsContainer;
            
            // Setup scroll parameters
            var scrollParameters = new ScrollProperties()
            {
                JointSpeedTouched = 0.23f,
                JointSpeedFree = 0.25f,
                InertiaMultiplier = 0.5f,
                InertiaMultiplierDelta = 1f,
            };
            
            // Create handle of view
            handleObject = new HandleObject(statsContainer.Find("Handler").gameObject);
            // Create main view of calendar pages
            CalendarStatistics = new CalendarStatistics(statsContainer);
            // Create scroll component for calendar view
            statisticsScroll = new StatisticsScroll(handleObject, CalendarStatistics, scrollParameters);
            // Setup scroll component to main view
            CalendarStatistics.SetupStatisticsScroll(statisticsScroll);
            // Join view to update calls
            SyncWithBehaviour.Instance.AddObserver(statisticsScroll, AppSyncAnchors.CalendarObject);
        }
    }
}
