using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Messenger;
using UnityEngine;

namespace Architecture.WorkArea.WeeklyActivity
{
    // Statistics view that used as weekly activity view
    public class WeeklyStatistics : IStatistic
    {
        // Data generator
        private readonly WeeklyDataCreator weeklyDataCreator;

        private const float heightPosition = -27f;
        private readonly ViewsLine viewsLine; // Component of statistics module
        private readonly GraphItem[] graphItems; // Pages of statistics module

        private readonly RectTransform pool; // Pool object for pages
        public float StatisticsViewWidth { get; } // Width of page

        public WeeklyStatistics(RectTransform container)
        {
            // Find and setup graphic objects
            pool = container.Find("Pool").GetComponent<RectTransform>();
            var viewExample = pool.Find("View").GetComponent<RectTransform>();
            var mask = container.Find("Graphic").Find("Mask");
            viewsLine = new ViewsLine(viewExample, mask, typeof(ViewAdditionalElements));
            
            // Setup width of calendar page
            var clampShift = viewExample.sizeDelta.x;
            StatisticsViewWidth = clampShift;
            viewsLine.SetupHeight(heightPosition);

            // Setup calendar pages
            graphItems = viewsLine.GetGraphItems();
            weeklyDataCreator = new WeeklyDataCreator();
            foreach (var graphItem in graphItems)
                graphItem.SetupMainElements(weeklyDataCreator, new WeeklyGraphElement());

            // Add members to messenger
            MainMessenger<HomeDay>.AddMember(UpdateGraphicByClosedTracker, AppMessagesConst.UpdateWorkAreaGraphic);
            MainMessenger.AddMember(UpdateGraphicAfterDataChanged, AppMessagesConst.UpdateWorkAreaGraphicDays);
        }

        public ViewsLine ViewLine() => viewsLine;

        // Setup statistics scroll to pages
        public void SetupStatisticsScroll(StatisticsScroll scroll)
        {
            foreach (var graphItem in graphItems)
            {
                graphItem.ViewAdditional.SetupScroll(scroll);
                graphItem.UpdateStep(0);
            }
            
            viewsLine.SetupPositions();
        }

        // Move pages by added steps count
        public void AddStepDelta(int stepDelta)
        {
            foreach (var graphItem in graphItems)
                graphItem.UpdateStep(stepDelta);
        }
        
        // Update graphic from other places by messenger
        private void UpdateGraphicByClosedTracker(HomeDay homeDay)
        {
            weeklyDataCreator.ReloadData(homeDay);
            foreach (var item in graphItems) item.UpdateAfterDataChanged();
        }
        
        // Update graphic after data changed
        private void UpdateGraphicAfterDataChanged()
        {
            foreach (var graphItem in graphItems)
                graphItem.UpdateAfterDataChanged();
        }
    }
}
