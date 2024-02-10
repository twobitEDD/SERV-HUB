using System.Collections.Generic;
using System.Linq;
using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Elements;
using Architecture.Pages;
using HomeTools.Messenger;
using HomeTools.Other;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.WorkArea
{
    // Component of view tasks
    public class WorkFlowsList
    {
        // Get bottom border of last task view
        public float BottomFlowsBorder { get; private set; }
        //  Current global tasks package
        private FlowGroup currentGroup;
        // Pool of task view items
        private readonly Queue<WorkFlowItem> flowsPool = new Queue<WorkFlowItem>();
        // Additional layer with move tools for tasks view
        private readonly WorkFlowsListSet workFlowsListSet;
        // Position params
        private const float firstFlowPosition = -688F;
        private const float flowHeight = 165f;
        // Lines between tasks
        private readonly Queue<(RectTransform rectTransform, Image image)> linesPool = new Queue<(RectTransform, Image)>();
        private readonly List<(RectTransform rectTransform, Image image)> linesUsed = new List<(RectTransform, Image)>();
        // Calendar
        private Calendar WorkCalendar => AreasLocator.Instance.WorkArea.WorkCalendar;
        
        public bool Empty { get; private set; } // Empty tasks list flag

        // Create and setup
        public WorkFlowsList()
        {
            workFlowsListSet = new WorkFlowsListSet();
            SyncWithBehaviour.Instance.AddObserver(workFlowsListSet, AppSyncAnchors.WorkAreaYear);
            MainMessenger<int>.AddMember(FlowUpdatedAction, AppMessagesConst.FlowUpdated);
        }
        
        // Initialize list of tasks by current global tasks package
        public void InitializeByCurrentGroup()
        {
            CleanUsedFlows();

            var flows = GetActualFlows();
            
            for (var i = 0; i < flows.Count; i++)
            {
                var currentData = flows[i];
                var newFlow = flowsPool.Count == 0 ? CreateNewFlow() : flowsPool.Dequeue();
                newFlow.Id = currentData.Id;
                newFlow.SetupByFlowData(currentData);
                newFlow.SetActive(true);
                workFlowsListSet.Add(newFlow);
            }

            Empty = flows.Count == 0;
            InitializeLinesByCurrentGroup();
        }

        // Setup tasks to page
        public void SetupFlowsToPage(PageItem pageItem)
        {
            BottomFlowsBorder = firstFlowPosition - flowHeight;
            
            if (workFlowsListSet.Count == 0)
                return;

            BottomFlowsBorder -= flowHeight * (workFlowsListSet.Count - 1);
            
            for (var i = 0; i < workFlowsListSet.Count; i++)
            {
                pageItem.AddContent(workFlowsListSet[i].RectTransform, Vector2.zero);
                workFlowsListSet[i].RectTransform.SetRectTransformAnchorHorizontal(0, 0);
            }

            UpdateFlowPositions();
            SetupLinesToPage(pageItem);
        }

        // Update all tasks whe first launch
        public void FirstUpdateAfterAll()
        {
            workFlowsListSet.UnhighlightAllFlows();
        }

        // Get active tasks list
        private List<Flow> GetActualFlows()
        {
            currentGroup = AppData.GetCurrentGroup();
            var currentDay = WorkCalendar.CurrentDay.HomeDayToInt();
            var flows = new List<Flow>();

            foreach (var flow in currentGroup.Flows)
            {
                if (flow.DateStarted > currentDay)
                    continue;
                
                flows.Add(flow);
            }

            var orderedFlows = flows.OrderBy(e => e.Order).ToList();

            for (var i = 0; i < orderedFlows.Count; i++)
                orderedFlows[i].Order = (byte) i;

            return orderedFlows;
        }
        
        // Update positions of task items
        private void UpdateFlowPositions()
        {
            if (workFlowsListSet.Count == 0)
                return;
            
            workFlowsListSet[0].SetPosition(new Vector2(workFlowsListSet[0].RectTransform.anchoredPosition.x, firstFlowPosition));

            for (var i = 1; i < workFlowsListSet.Count; i++)
                workFlowsListSet[i].SetPosition(new Vector2(workFlowsListSet[i].RectTransform.anchoredPosition.x, firstFlowPosition - flowHeight * i));
        }
        
        // Move all split lines to pool
        private void CleanUsedFlows()
        {
            for (var i = 0; i < workFlowsListSet.Count; i++)
            {
                workFlowsListSet[i].SetActive(false);
                flowsPool.Enqueue(workFlowsListSet[i]);
            }

            workFlowsListSet.Clear();
        }

        // Initialize split lines for put between tasks
        private void InitializeLinesByCurrentGroup()
        {
            CleanUsedLines();
            var flows = GetActualFlows();
            for (var i = 0; i < flows.Count; i++)
            {
                var newLine = linesPool.Count == 0 ? GetNewLine() : linesPool.Dequeue();
                newLine.rectTransform.gameObject.SetActive(true);
                linesUsed.Add(newLine);
            }
        }

        // Create new line object
        private (RectTransform rectTransform, Image image) GetNewLine()
        {
            var line = SceneResources.GetPreparedCopy("Main Split Line");
            var image = line.GetComponent<Image>();
            return (line, image);
        }

        // Setup split tasks lines to page
        private void SetupLinesToPage(PageItem pageItem)
        {
            if (linesUsed.Count == 0)
                return;
            
            for (var i = 0; i < linesUsed.Count; i++)
            {
                pageItem.AddContent(linesUsed[i].rectTransform);
                linesUsed[i].rectTransform.SetRectTransformAnchorHorizontal(35, 35);
            }
            
            linesUsed[0].rectTransform.anchoredPosition 
                = new Vector2(linesUsed[0].rectTransform.anchoredPosition.x, firstFlowPosition + flowHeight / 2);
            
            for (var i = 1; i < linesUsed.Count; i++)
            {
                linesUsed[i].rectTransform.anchoredPosition 
                    = new Vector2(linesUsed[i].rectTransform.anchoredPosition.x, 
                        firstFlowPosition - flowHeight / 2 - flowHeight * (i - 1));
            }

            workFlowsListSet.LinesAlpha.Clear();
            for (var i = 0; i < linesUsed.Count; i++)
                workFlowsListSet.LinesAlpha.AddElement(linesUsed[i].image);
        }

        // Put all lines to pool
        private void CleanUsedLines()
        {
            foreach (var line in linesUsed)
            {
                line.rectTransform.gameObject.SetActive(false);
                linesPool.Enqueue(line);
            }

            linesUsed.Clear();
        }

        // Update task view
        private void FlowUpdatedAction(int flow)
        {
            for (var i = 0; i < workFlowsListSet.Count; i++)
            {
                if (workFlowsListSet[i].FlowData.Id == flow)
                    workFlowsListSet[i].UpdateMainViewInfo();
            }
        }

        // Create new task item
        private WorkFlowItem CreateNewFlow()
        {
            var flow = new WorkFlowItem(workFlowsListSet);
            SyncWithBehaviour.Instance.AddObserver(flow, AppSyncAnchors.WorkAreaYear);
            return flow;
        }
    }
}
