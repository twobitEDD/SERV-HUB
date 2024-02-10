using Architecture.Statistics;
using HomeTools.Handling;
using HTools;
using UnityEngine;

namespace Architecture.TutorialArea.TutorialElements
{
    // Tutorial pages view object
    public class MainStats
    {
        private readonly RectTransform rectTransform;
        private readonly StatisticsScroll statisticsScroll;
        private readonly HandleObject handleObject;
        public readonly TutorialStatistics TutorialStatistics;

        public TutorialDataCreator TutorialDataCreator => TutorialStatistics.TutorialDataCreator;
        
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
            // Create main view of tutorial pages
            TutorialStatistics = new TutorialStatistics(statsContainer);
            // Create scroll component for view
            statisticsScroll = new StatisticsScroll(handleObject, TutorialStatistics, scrollParameters);
            // Setup scroll component to view
            TutorialStatistics.SetupStatisticsScroll(statisticsScroll);
            // Join view to update calls
            SyncWithBehaviour.Instance.AddObserver(statisticsScroll, AppSyncAnchors.TutorialArea);
        }

        // Move tutorial pages forward
        public void AddStepForward()
        {
            statisticsScroll.SetScrollBySteps(-1);
        }
    }
}
