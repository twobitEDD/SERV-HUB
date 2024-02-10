using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Messenger;
using HomeTools.Source.Design;
using UnityEngine;

namespace Architecture.CalendarModule.StatisticElements
{
    // Statistics view that used as calendar view
    public class CalendarStatistics : IStatistic
    {
        // Calendar data generator
        public readonly CalendarDataCreator CalendarDataCreator;
        
        private const float heightPosition = 0;
        private readonly ViewsLine viewsLine; // Component of statistics module
        private readonly GraphItem[] graphItems; // Pages of statistics module

        private readonly RectTransform pool; // Pool object for pages
        private readonly RectTransform graphicRect; // Rect of calendar view
        public float StatisticsViewWidth { get; } // Width of page
        private int defaultStep; // Default step when open view

        public CalendarStatistics(RectTransform container)
        {
            // Find and setup calendar objects
            pool = container.Find("Pool").GetComponent<RectTransform>();
            var viewExample = pool.Find("View").GetComponent<RectTransform>();
            graphicRect = container.Find("Graphic").GetComponent<RectTransform>();
            var mask = graphicRect.Find("Mask");
            viewsLine = new ViewsLine(viewExample, mask, typeof(ViewAdditionalElements));
            
            // Setup width of calendar page
            var clampShift = viewExample.sizeDelta.x;
            StatisticsViewWidth = clampShift;
            viewsLine.SetupHeight(heightPosition);

            // Setup calendar pages
            graphItems = viewsLine.GetGraphItems();
            CalendarDataCreator = new CalendarDataCreator();
            foreach (var graphItem in graphItems)
            {
                graphItem.SetupMainElements(CalendarDataCreator, new CalendarGraphElement());
            }

            // Setup default step
            defaultStep = CalendarDataCreator.DefaultStep();
            // Set positions of pages of calendar
            viewsLine.SetupPositions();
            // Add member to messenger
            MainMessenger<HomeDay>.AddMember(UpdateGraphicByClosedTracker, AppMessagesConst.UpdateWorkAreaGraphic);
        }

        public ViewsLine ViewLine() => viewsLine;

        // Setup statistics scroll to pages
        public void SetupStatisticsScroll(StatisticsScroll statisticsScroll)
        {
            foreach (var graphItem in graphItems)
            {
                graphItem.ViewAdditional.SetupScroll(statisticsScroll);
                graphItem.UpdateStep(0);
            }
        }

        // Update calendar from other places by messenger
        private void UpdateGraphicByClosedTracker(HomeDay homeDay)
        {
            CalendarDataCreator.ReloadData(homeDay);
            CheckUpdateDefaultStep();
            foreach (var item in graphItems) item.UpdateAfterDataChanged();
        }

        // Setup pages to default state 
        private void CheckUpdateDefaultStep()
        {
            if (defaultStep == CalendarDataCreator.DefaultStep()) 
                return;
            
            for (var i = 0; i < graphItems.Length; i++)
                graphItems[i].SetupStep(i);

            defaultStep = CalendarDataCreator.DefaultStep();
        }
        
        // Update calendar pages by new day
        public void UpdateToDefaultStep(HomeDay currentDay)
        {
            viewsLine.UpdatePositions();
            var deltaStep = CalendarDataCreator.GetStepByHomeDay(currentDay);

            foreach (var graph in graphItems)
                graph.UpdateStep(-deltaStep);
        }

        // Return animation component of alpha channel of page
        public UIAlphaSync CurrentAlphaSync() =>
            ((ViewAdditionalElements) viewsLine.GetCenteredView().GraphItem.ViewAdditional).UIAlphaSyncLocal;
        
    }
}
