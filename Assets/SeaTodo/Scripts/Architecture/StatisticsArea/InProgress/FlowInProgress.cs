using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Other;
using HTools;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.StatisticsArea.InProgress
{
    // Task item
    public class FlowInProgress : IBehaviorSync
    {
        public readonly RectTransform RectTransform; // Rect of task
        // UI elements of task
        private readonly SVGImage icon;
        private readonly Text goal;
        private readonly Text name;
        private readonly Image lineBack;
        private readonly Image lineProgress;
        private Rect line;
        
        // Position params of task progress line
        private const float linePositionFrom = -243.5f;
        private const float linePositionTo = 37f;

        // Params for animation of task percentage when open activity page
        private float percentageMain;
        private float percentageCurrent;
        private const float percentageSpeed = 0.07f;

        // Create and setup
        public FlowInProgress(RectTransform rectTransform)
        {
            // Save rect
            RectTransform = rectTransform;
            // Get UI elements
            icon = rectTransform.Find("Icon").GetComponent<SVGImage>();
            goal = rectTransform.Find("Goal").GetComponent<Text>();
            name = rectTransform.Find("Name").GetComponent<Text>();
            lineBack = rectTransform.Find("Line Back").GetComponent<Image>();
            lineProgress = rectTransform.Find("Line Progress").GetComponent<Image>();
            line = lineBack.rectTransform.rect;
        }

        // Update view by task
        public void UpdateByFlow(Flow flow)
        {
            icon.sprite = FlowIconLoader.LoadIconById(flow.Icon);

            var flowColor = FlowColorLoader.LoadColorById(flow.Color);
            icon.color = flowColor;
            
            var progressInt = flow.GetIntProgress();
            var inGoalInt = FlowInfoAll.GetGoalByOriginFlowInt(flow.Type, flow.GoalInt);
            var floatPercentage = progressInt / (float)inGoalInt;
            
            goal.text = FlowInfoAll.GetViewGoalByOriginFlow(flow.Type, inGoalInt, goal.fontSize);

            name.text = flow.Name;

            lineProgress.color = flowColor;
            
            flowColor.a = 0.1f;
            lineBack.color = flowColor;

            percentageMain = floatPercentage;
            percentageCurrent = 0;
        }
        
        public void Start() { }

        // Animate moving of task progress line
        public void Update()
        {
            if (Mathf.Abs(percentageCurrent - percentageMain) < 0.01f)
                return;
            
            percentageCurrent = Mathf.Lerp(percentageCurrent, percentageMain, percentageSpeed);
            SetupLineProgress(percentageCurrent);
        }
        
        // Setup task line progress by percentage
        private void SetupLineProgress(float percentage)
        {
            if (percentage > 1)
                percentage = 1f;
            
            var maxWidth = line.width;
            var minWidth = line.height;
            var width = minWidth + (maxWidth - minWidth) * percentage;
            
            var lineRectTransform = lineProgress.rectTransform;
            lineRectTransform.sizeDelta = new Vector2(width, minWidth);

            lineRectTransform.anchoredPosition = new Vector2(Mathf.Lerp(linePositionFrom, linePositionTo, percentage), 
                lineRectTransform.anchoredPosition.y);
        }
    }
}
