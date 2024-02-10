using System.Collections.Generic;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Messenger;
using HomeTools.Source.Design;
using UnityEngine;

namespace Architecture.ChooseIconModule.StatisticColorElements
{
    // Main component for circle with color line
    public class ChooseColorStatistics : IStatistic
    {
        // Data generator
        public readonly ChooseColorDataCreator ChooseColorDataCreator;
        
        private const float heightPosition = 0;
        private readonly ViewsLine viewsLine; // Component of statistics module
        private readonly GraphItem[] graphItems; // Pages of statistics module

        private readonly RectTransform pool; // Pool object for pages
        private readonly RectTransform graphicRect; // Rect of calendar view
        public float StatisticsViewWidth { get; } // Width of page
        private int defaultStep; // Default step when open view

        public ChooseColorStatistics(RectTransform container)
        {
            // Find and setup color graphic objects
            pool = container.Find("Pool").GetComponent<RectTransform>();
            var viewExample = pool.Find("View").GetComponent<RectTransform>();
            graphicRect = container.Find("Graphic").GetComponent<RectTransform>();
            var mask = graphicRect.Find("Mask");
            viewsLine = new ViewsLine(viewExample, mask, typeof(ViewAdditionalElements));
            
            // Setup width of page
            var clampShift = viewExample.sizeDelta.x;
            StatisticsViewWidth = clampShift;
            viewsLine.SetupHeight(heightPosition);

            // Setup statistics pages
            graphItems = viewsLine.GetGraphItems();
            ChooseColorDataCreator = new ChooseColorDataCreator();
            foreach (var graphItem in graphItems)
            {
                graphItem.SetupMainElements(ChooseColorDataCreator, new ChooseColorGraphElement());
            }

            // Setup default step
            defaultStep = ChooseColorDataCreator.DefaultStep();
            // Set positions of pages of view
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

        // Update view from other places by messenger
        private void UpdateGraphicByClosedTracker(HomeDay homeDay)
        {
            ChooseColorDataCreator.ReloadData(homeDay);
            CheckUpdateDefaultStep();
            foreach (var item in graphItems) item.UpdateAfterDataChanged();
        }

        // Setup pages to default state 
        private void CheckUpdateDefaultStep()
        {
            if (defaultStep == ChooseColorDataCreator.DefaultStep()) 
                return;
            
            for (var i = 0; i < graphItems.Length; i++)
                graphItems[i].SetupStep(i);

            defaultStep = ChooseColorDataCreator.DefaultStep();
        }
        
        // Update pages by new step
        public void UpdateToDefaultStep(int step)
        {
            viewsLine.UpdatePositions();
            foreach (var item in graphItems) item.UpdateStep(step);
        }

        // Return animation component of alpha channel of page
        public IEnumerable<UIAlphaSyncGroup> GetAlphaSyncs()
        {
            var items = viewsLine.GetGraphItems();
            var result = new UIAlphaSyncGroup[items.Length];

            for (var i = 0; i < result.Length; i++)
                result[i] = ((ViewAdditionalElements) items[i].ViewAdditional).UIAlphaSyncLocalGroup;

            return result;
        }
    }
}
