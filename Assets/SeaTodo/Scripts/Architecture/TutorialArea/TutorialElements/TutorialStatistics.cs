using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Source.Design;
using UnityEngine;

namespace Architecture.TutorialArea.TutorialElements
{
    // Main component for tutorial pages
    public class TutorialStatistics : IStatistic
    {
        // Data generator
        public readonly TutorialDataCreator TutorialDataCreator;
        
        private const float heightPosition = 0;
        private readonly ViewsLine viewsLine; // Component of statistics module
        private readonly GraphItem[] graphItems; // Pages of statistics module

        private readonly RectTransform pool; // Pool object for pages
        private readonly RectTransform graphicRect; // Rect of view
        public float StatisticsViewWidth { get; }  // Width of page

        public TutorialStatistics(RectTransform container)
        {
            // Find and setup graphic objects
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
            TutorialDataCreator = new TutorialDataCreator();
            foreach (var graphItem in graphItems)
            {
                graphItem.SetupMainElements(TutorialDataCreator, new TutorialGraphElement());
            }

            // Setup default step
            TutorialDataCreator.DefaultStep();
            // Set positions of pages of view
            viewsLine.SetupPositions();
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
        
        // Update pages by new step
        public void UpdateToDefaultStep(int step)
        {
            viewsLine.UpdatePositions();
            foreach (var item in graphItems) item.UpdateStep(step);
        }

        // Return animation component of alpha channel of page
        public UIAlphaSync CurrentAlphaSync() =>
            ((ViewAdditionalElements) viewsLine.GetCenteredView().GraphItem.ViewAdditional).UIAlphaSyncLocal;
    }
}
