using System.Collections.Generic;
using Architecture.Statistics.Interfaces;
using UnityEngine;

namespace Architecture.Statistics
{
    // Component for page item for visualize graphic
    public class GraphItem
    {
        public bool EmptyActivity; // Flag if graphic contains any data for graphic elements
        private readonly RectTransform viewPlace; // Rect object of view

        public int Step; // Page step by order
        private IGraphicInfo currentGraphicInfo; // Data creator
        private IGraphElement exampleGraphElement; // Example graphic element
        public IViewAdditional ViewAdditional { get; } // Additional view for page

        // Active graphic elements
        private readonly List<IGraphElement> inUsing = new List<IGraphElement>();
        // Graphic elements that in pool
        private readonly List<IGraphElement> inPool = new List<IGraphElement>();
        // Width of page
        private readonly float viewPlaceWidth;
        // Flag if no data
        private bool noData;
        
        // Create and save components
        public GraphItem(RectTransform rectTransform, IViewAdditional viewAdditional)
        {
            viewPlace = rectTransform;
            viewPlaceWidth = viewPlace.sizeDelta.x;
            ViewAdditional = viewAdditional;
        }

        // Setup main elements
        public void SetupMainElements(IGraphicInfo graphicInfo, IGraphElement exampleElement)
        {
            currentGraphicInfo = graphicInfo;
            exampleGraphElement = exampleElement;
            exampleGraphElement.SetupViewPlace(viewPlace);
            inUsing.Add(exampleElement);
        }

        // Add steps to page item
        public void SetupStep(int startStep) => Step = startStep + currentGraphicInfo.DefaultStep();

        // Setup step to page item fixed
        public void SetupStepFixed(int startStep) => Step = startStep;

        // Update steps in direction
        public void UpdateStep(int stepDirection)
        {
            Step += stepDirection;

            var dataStruct = currentGraphicInfo.GetInfo(Step);
            if (!dataStruct.Actual())
                return;
            
            SetupElementsToGraphic(dataStruct.GraphElementsDescription.Length, dataStruct);

            var preview = currentGraphicInfo.GetInfo(Step - 1);
            var next = currentGraphicInfo.GetInfo(Step + 1);
            ViewAdditional.FullUpdate(preview, dataStruct, next);

            EmptyActivity = dataStruct.EmptyActivity();
            noData = dataStruct.NoData;
        }

        // Update when motion slept
        public void MotionSlept() => currentGraphicInfo.SendStepToOther(Step);
        
        // Update graphic after data struct updated
        public void UpdateAfterDataChanged()
        {
            var dataStruct = currentGraphicInfo.GetInfo(Step);
            if (!dataStruct.Actual())
                return;
            
            if (noData)
                SetupElementsToGraphic(dataStruct.GraphElementsDescription.Length, dataStruct);
            else
                UpdateGraphicByCorrectedData(inUsing.ToArray(), dataStruct);
            
            var preview = currentGraphicInfo.GetInfo(Step - 1);
            var next = currentGraphicInfo.GetInfo(Step + 1);
            ViewAdditional.Update(preview, dataStruct, next);

            EmptyActivity = dataStruct.EmptyActivity();
            noData = dataStruct.NoData;
        }

        // Setup graph elements to graphic and update
        private void SetupElementsToGraphic(int count, GraphDataStruct graphDataStruct)
        {
            var actual = new IGraphElement[count];
            SetupActualElements(actual);
            PlaceOnGraphic(actual);
            UpdateGraphicByData(actual, graphDataStruct);
        }

        // Setup graph elements to graphic
        private void SetupActualElements(IGraphElement[] actual)
        {
            var fill = 0;

            foreach (var element in inUsing)
            {
                if (fill >= actual.Length)
                {
                    element.SetToPool();
                    inPool.Add(element);
                    continue;
                }

                actual[fill] = element;
                fill++;
            }

            inUsing.Clear();

            for (; fill < actual.Length && inPool.Count > 0; fill++)
            {
                var firstElement = inPool[0];
                inPool.RemoveAt(0);
                actual[fill] = firstElement;
            }

            for (; fill < actual.Length; fill++)
            {
                actual[fill] = exampleGraphElement.CreateClone();
            }

            foreach (var element in actual)
                inUsing.Add(element);
        }

        // Setup positions of graph elements
        private void PlaceOnGraphic(IGraphElement[] actual)
        {
            var widthStep = viewPlaceWidth / ((actual.Length) * 2);
            var firstPosition = -(viewPlaceWidth / 2) + widthStep;
            
            for (var i = 0; i < actual.Length; i++)
            {
                actual[i].SetToView(firstPosition + widthStep * i * 2);
            }
        }
        
        // Update graphic by new data
        private void UpdateGraphicByData(IGraphElement[] actual, GraphDataStruct graphDataStruct)
        {
            for (var i = 0; i < actual.Length; i++)
                actual[i].Update(graphDataStruct.GraphElementsInfo[i], 
                    graphDataStruct.GraphElementsDescription[i], 
                    graphDataStruct.GraphElementActive[i], 
                    graphDataStruct.Highlighted == i);
        }
        
        // Update graphic by corrected data without update of count of graph elements 
        private void UpdateGraphicByCorrectedData(IReadOnlyList<IGraphElement> actual, GraphDataStruct graphDataStruct)
        {
            for (var i = 0; i < actual.Count; i++)
                actual[i].UpdateDynamic(graphDataStruct.GraphElementsInfo[i], 
                    graphDataStruct.GraphElementActive[i], 
                    graphDataStruct.Highlighted == i);
        }
    }
}
