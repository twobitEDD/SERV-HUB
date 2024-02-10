using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using UnityEngine;

namespace Architecture.TaskViewArea.NormalView.Statistics
{
    // Statistics view that used as task statistics view
    public class FlowStatistics : IStatistic
    {
        // Task statistics data generator
        public readonly FlowDataCreator FlowDataCreator;
        
        private const float heightPosition = 21.5f;
        public readonly ViewsLine ViewsLine; // Component of statistics module
        private readonly GraphItem[] graphItems;  // Pages of statistics module

        // Pool object for pages
        private RectTransform pool;
        // Width of page
        public float StatisticsViewWidth { get; }

        public FlowStatistics(RectTransform container)
        {
            // Find and setup task statistics objects
            pool = container.Find("Pool").GetComponent<RectTransform>();
            var viewExample = pool.Find("View").GetComponent<RectTransform>();
            var mask = container.Find("Graphic Week").Find("Mask");
            ViewsLine = new ViewsLine(viewExample, mask, typeof(ViewAdditionalElements));
            
            // Setup width of task statistics page
            var clampShift = viewExample.sizeDelta.x;
            StatisticsViewWidth = clampShift;
            ViewsLine.SetupHeight(heightPosition);

            // Setup statistics pages
            graphItems = ViewsLine.GetGraphItems();
            FlowDataCreator = new FlowDataCreator();
            foreach (var graphItem in graphItems)
            {
                var flowGraphElement = new FlowGraphElement(pool, "WeekElement");
                graphItem.SetupMainElements(FlowDataCreator, flowGraphElement);
            }

            // Setup default step
            FlowDataCreator.DefaultStep();
            // Set positions of pages
            ViewsLine.SetupPositions();
        }

        public ViewsLine ViewLine() => ViewsLine;

        // Setup statistics scroll to pages
        public void SetupStatisticsScroll(StatisticsScroll statisticsScroll)
        {
            foreach (var graphItem in graphItems)
            {
                graphItem.UpdateStep(0);
            }
        }
        
        // Move pages by step number 
        public void AddStepDelta(int stepDelta)
        {
            foreach (var graphItem in graphItems)
                graphItem.UpdateStep(stepDelta);
        }
        
        // Update statistics pages
        public void UpdateElements()
        {
            foreach (var graphItem in graphItems)
                graphItem.UpdateStep(0);
        }
    }
}
