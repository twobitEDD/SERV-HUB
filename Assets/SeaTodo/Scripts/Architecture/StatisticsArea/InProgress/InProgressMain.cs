using System.Collections.Generic;
using Architecture.Data;
using HTools;
using UnityEngine;

namespace Architecture.StatisticsArea.InProgress
{
    // Class that contains tasks progress view
    public class InProgressMain
    {
        // Position of first task
        private const float upperYAnchor = -433f;
        // Distance between tasks
        public const float DistanceBetweenFlows = 127f;
        
        // Rect that contains tasks
        private readonly RectTransform rectTransform;
        // Rect of pool object
        private readonly RectTransform pool;
        // Task example object
        private readonly RectTransform exampleFlow;
        
        // Active task in view
        private readonly List<FlowInProgress> usedFlows = new List<FlowInProgress>();
        // Tasks in pool
        private readonly List<FlowInProgress> flowsInPool = new List<FlowInProgress>();
        
        // Create and setup
        public InProgressMain(RectTransform rectTransform)
        {
            this.rectTransform = rectTransform;
            pool = rectTransform.Find("Pool").GetComponent<RectTransform>();
            exampleFlow = pool.Find("Flow In Progress").GetComponent<RectTransform>();
        }

        // Update view by tasks
        public void UpdateView(Flow[] flows)
        {
            // Set all tasks in pool
            SetFlowsToPool();

            for (var i = 0; i < flows.Length; i++)
            {
                var currentFlowView = GetNewFlowInProgress();
                currentFlowView.UpdateByFlow(flows[i]);
                currentFlowView.RectTransform.SetParent(rectTransform);
                currentFlowView.RectTransform.anchoredPosition = new Vector2(0, upperYAnchor - DistanceBetweenFlows * i);
                
                usedFlows.Add(currentFlowView);
            }
        }
        
        // Set all tasks in pool
        private void SetFlowsToPool()
        {
            foreach (var flowInProgress in usedFlows)
            {
                flowsInPool.Add(flowInProgress);
                flowInProgress.RectTransform.SetParent(pool);
            }
            
            usedFlows.Clear();
        }

        // Get new task to active state
        private FlowInProgress GetNewFlowInProgress() => flowsInPool.Count > 0 ? 
                GetFlowInProgressFromPool() : 
                SyncWithBehaviour.Instance.AddObserver(
                    new FlowInProgress(Object.Instantiate(exampleFlow, pool)), AppSyncAnchors.StatisticsArea);
        
        // Get task from pool
        private FlowInProgress GetFlowInProgressFromPool()
        {
            var result = flowsInPool[0];
            flowsInPool.RemoveAt(0);

            return result;
        }
    }
}
