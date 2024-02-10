using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using Architecture.TextHolder;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.StatisticsArea.ArchivedFlows
{
    // Main component for archived tasks line
    public class ArchivedFlowsStatistics : IStatistic
    {
        // Data generator
        public readonly ArchivedFlowsDataCreator ArchivedFlowsDataCreator;

        private const float heightPosition = -17f; // Height position of tasks
        private readonly ViewsLine viewsLine; // Component of statistics module
        private readonly GraphItem[] graphItems; // Pages of statistics module

        // Components of arrows
        private readonly ArrowNavigation arrowNavigationLeft;
        private readonly ArrowNavigation arrowNavigationRight;
        // Steps count view
        private readonly Text stepsCounter;
        
        // UI elements that shows when no archived tasks
        private readonly SVGImage emptyIcon1;
        private readonly SVGImage emptyIcon2;
        private readonly Image emptyFloor;
        private readonly Text emptyDescription;

        // Pool object for pages
        private RectTransform pool;
        // Rect of calendar view
        private StatisticsScroll statisticsScroll;
        public float StatisticsViewWidth { get; } // Width of page
        private int currentStep; // Current page 
        private int allSteps; // Count of pages in general

        // Create and setup
        public ArchivedFlowsStatistics(RectTransform container)
        {
            // Setup objects and create graphic objects
            pool = container.Find("Pool").GetComponent<RectTransform>();
            var viewExample = pool.Find("View").GetComponent<RectTransform>();
            var mask = container.Find("Graphic").Find("Mask");
            viewsLine = new ViewsLine(viewExample, mask, typeof(ViewAdditionalElements));
            
            // Setup left arrow component
            var leftArrow = container.Find("Graphic/Arrow Left").GetComponent<RectTransform>();
            arrowNavigationLeft = new ArrowNavigation(leftArrow, TouchedLeftArrow);
            
            // Setup right arrow component
            var rightArrow = container.Find("Graphic/Arrow Right").GetComponent<RectTransform>();
            arrowNavigationRight = new ArrowNavigation(rightArrow, TouchedRightArrow);
            
            // Find text that shows steps counter
            stepsCounter = container.Find("Graphic/Counter").GetComponent<Text>();
            
            // Find and setup UI elements for view when no archived tasks
            emptyIcon1 = container.Find("Graphic/Empty Icon 1").GetComponent<SVGImage>();
            emptyIcon2 = container.Find("Graphic/Empty Icon 2").GetComponent<SVGImage>();
            emptyDescription = container.Find("Graphic/Empty Description").GetComponent<Text>();
            emptyFloor = container.Find("Graphic/Empty Floor").GetComponent<Image>();
            TextLocalization.Instance.AddLocalization(emptyDescription, TextKeysHolder.Empty);
            
            // Setup width of page
            var clampShift = viewExample.sizeDelta.x;
            StatisticsViewWidth = clampShift;
            viewsLine.SetupHeight(heightPosition);

            // Setup statistics pages
            graphItems = viewsLine.GetGraphItems();
            ArchivedFlowsDataCreator = new ArchivedFlowsDataCreator();
            foreach (var graphItem in graphItems)
                graphItem.SetupMainElements(ArchivedFlowsDataCreator, new EmptyGraphElement());
            
            // Set action for generator for update arrow and counter
            // when archived task list updated
            ArchivedFlowsDataCreator.SetActionGetStep(UpdateArrowsActivityByStep);
        }

        public ViewsLine ViewLine() => viewsLine;

        // Setup statistics scroll to pages
        public void SetupStatisticsScroll(StatisticsScroll scroll)
        {
            statisticsScroll = scroll;

            foreach (var graphItem in graphItems)
            {
                graphItem.ViewAdditional.SetupScroll(scroll);
                graphItem.UpdateStep(0);
            }
            
            viewsLine.SetupPositions();
        }
        
        // Reset arrows immediately
        public void ResetArrows()
        {
            arrowNavigationLeft.UpdateColors();
            arrowNavigationRight.UpdateColors();
            
            arrowNavigationLeft.UpdateActivityImmediately();
            arrowNavigationRight.UpdateActivityImmediately();
        }
        
        // Update view of archived tasks to default state
        public void UpdateElements()
        {
            viewsLine.UpdatePositions();
            var defaultStep = ArchivedFlowsDataCreator.DefaultStep();
            
            foreach (var graphElement in graphItems)
                graphElement.UpdateStep(defaultStep);

            UpdateEmptyVisible(defaultStep);

            allSteps = defaultStep + 1;
            currentStep = defaultStep;
            UpdateArrowsActivityByStep(defaultStep);
        }
        
        // Update view of archived tasks to target step
        public void UpdateElementsToStep(int step)
        {
            viewsLine.UpdatePositions();

            if (step > ArchivedFlowsDataCreator.DefaultStep())
                step = ArchivedFlowsDataCreator.DefaultStep();
            
            var defaultStep = step;
            
            foreach (var graphElement in graphItems)
                graphElement.UpdateStep(defaultStep);

            UpdateEmptyVisible(defaultStep);

            allSteps = ArchivedFlowsDataCreator.DefaultStep() + 1;
            currentStep = defaultStep;
            UpdateArrowsActivityByStep(defaultStep);
        }

        // Update visible of UI for view when no archived tasks
        private void UpdateEmptyVisible(int defaultStep)
        {
            var enabledEmpty = defaultStep == -1;
            emptyIcon1.enabled = enabledEmpty;
            emptyIcon2.enabled = enabledEmpty;
            emptyDescription.enabled = enabledEmpty;
            emptyFloor.enabled = enabledEmpty;
        }

        // Touched left arrow method
        private void TouchedLeftArrow()
        {
            currentStep -= 1;
            statisticsScroll.SetScrollBySteps(1);
            UpdateArrowsActivityByStep(currentStep);
        }

        // Touched right arrow method
        private void TouchedRightArrow()
        {
            currentStep += 1;
            statisticsScroll.SetScrollBySteps(-1);
            UpdateArrowsActivityByStep(currentStep);
        }

        // Update activity of arrows
        private void UpdateArrowsActivityByStep(int step)
        {
            var leftActivity = currentStep > 0;
            var rightActivity = currentStep < ArchivedFlowsDataCreator.DefaultStep();
            
            arrowNavigationLeft.UpdateActivityStatus(leftActivity);
            arrowNavigationRight.UpdateActivityStatus(rightActivity);

            arrowNavigationLeft.UpdateActivity();
            arrowNavigationRight.UpdateActivity();

            currentStep = step;
            UpdateStepsCounter();
        }

        // Update view of steps counter
        private void UpdateStepsCounter() => stepsCounter.text = $"{currentStep + 1}/{allSteps}";
    }
}
