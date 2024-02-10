using System.Collections.Generic;
using Architecture.Data;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Statistics.Interfaces;
using InternalTheming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.NormalView.Statistics
{
    // Component for element of graphic in statistics view
    public class FlowGraphElement : IGraphElement
    {
        // Current task
        private Flow GetCurrentFlow() => AreasLocator.Instance.FlowViewArea.CurrentFlow;
        
        // Pool object
        private readonly RectTransform pool;
        // Name of example graph object
        private readonly string nameOfExample;
        // Rect of page view
        private RectTransform viewPlace;
        // Example of graph element
        private readonly Transform originalElement;

        // Rect of graph element
        private RectTransform elementView;
        // UI component of graph element
        private Image graphLine;

        // Create and setup
        public FlowGraphElement(RectTransform pool, string nameOfExample)
        {
            // Save main components
            this.nameOfExample = nameOfExample;
            this.pool = pool;
            originalElement = pool.Find(nameOfExample);
            // Create graph element object
            CreateElement(this);
        }
        
        // Create and setup
        private FlowGraphElement(RectTransform pool, string nameOfExample, RectTransform viewPlace, Transform originalElement)
        {
            // Save main components
            this.nameOfExample = nameOfExample;
            this.pool = pool;
            this.viewPlace = viewPlace;
            this.originalElement = originalElement;
            // Create graph element object
            CreateElement(this);
        }

        // Setup place for element
        public void SetupViewPlace(RectTransform viewPlace) => this.viewPlace = viewPlace;

        // Setup X position of graph element
        public void SetToView(float position)
        {
            elementView.SetParent(viewPlace);
            elementView.anchoredPosition = new Vector2(position, 0);
        }

        // Set element to pool object
        public void SetToPool() => elementView.SetParent(pool);

        // Update element immediately
        public void Update(float infoGraph, List<int> infoDescription, bool infoActive, bool highlighted)
        {
            UpdateLine(infoGraph, infoActive);
        }

        // Update element with animation
        public void UpdateDynamic(float infoGraph, bool infoActive, bool highlighted)
        {
            UpdateLine(infoGraph, infoActive);
        }
        
        // Update height of graph element
        private void UpdateLine(float percentage, bool active)
        {
            percentage = Mathf.Clamp(percentage,0 ,1);

            var rectTransform = graphLine.rectTransform;
            var maxWidth = 930f;
            var minWidth = 65f;
            var width = maxWidth * percentage;

            if (width < minWidth)
                width = minWidth;
            
            rectTransform.sizeDelta = new Vector2(65f, width);

            var minPosition = -62.02f;

            var position = minPosition + (width / 2) * rectTransform.localScale.y;

            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, position);

            var color = ThemeLoader.GetCurrentTheme().SecondaryColorD1;
            color.a = 1;
            if (!active) color.a = 0.127f;
            if (active && percentage < 0.01f) color.a = 0.47f;
            if (active && percentage >= 0.01f) color = FlowColorLoader.LoadColorById(GetCurrentFlow().Color); 
            
            graphLine.color = color;
        }

        // Clone graph element with components
        public IGraphElement CreateClone() => 
            new FlowGraphElement(pool, nameOfExample, viewPlace, originalElement);

        // Create same graph element
        private void CreateElement(FlowGraphElement flowGraphElement)
        {
            var instance = Object.Instantiate(flowGraphElement.originalElement, flowGraphElement.pool);
            instance.name = $"{nameOfExample} -D";
            
            flowGraphElement.elementView = instance.GetComponent<RectTransform>();
            flowGraphElement.graphLine = instance.Find("Line").GetComponent<Image>();
        }
    }
}
