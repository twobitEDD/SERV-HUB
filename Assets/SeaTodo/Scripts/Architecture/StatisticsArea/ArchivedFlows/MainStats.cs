using Architecture.Statistics;
using HomeTools.Handling;
using HTools;
using UnityEngine;

namespace Architecture.StatisticsArea.ArchivedFlows
{
    // Archived tasks view object
    public class MainStats
    {
        private readonly StatisticsScroll statisticsScroll;
        private readonly ArchivedFlowsStatistics archivedFlowsStatistics;
        private readonly HandleObject handleObject;
        
        private ArchivedFlowsDataCreator ArchivedFlowsDataCreator => archivedFlowsStatistics.ArchivedFlowsDataCreator;

        // Create in rect transform
        public MainStats(RectTransform statsContainer)
        {
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
            // Create main view of colors pages
            archivedFlowsStatistics = new ArchivedFlowsStatistics(statsContainer);
            // Create scroll component for view
            statisticsScroll = new StatisticsScroll(handleObject, archivedFlowsStatistics, scrollParameters);
            // Setup scroll component to view
            archivedFlowsStatistics.SetupStatisticsScroll(statisticsScroll);
            // Join view to update calls
            SyncWithBehaviour.Instance.AddObserver(statisticsScroll, AppSyncAnchors.StatisticsArea);
        }

        // Update tasks
        public void UpdateElementsWhenOpenedPage()
        {
            ArchivedFlowsDataCreator.ReloadData();
            archivedFlowsStatistics.UpdateElements();
            archivedFlowsStatistics.ResetArrows();
        }
        
        // Update tasks when other task has been removed from archived tasks list
        public void UpdateElementsWhenRemovedFlow()
        {
            var currentStep = archivedFlowsStatistics.ViewLine().GetCenteredView().GraphItem.Step;
            ArchivedFlowsDataCreator.ReloadData();
            archivedFlowsStatistics.UpdateElementsToStep(currentStep);
            archivedFlowsStatistics.ResetArrows();
        }
    }
}
