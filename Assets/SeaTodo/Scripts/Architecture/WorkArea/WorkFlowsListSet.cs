using System;
using System.Collections.Generic;
using Architecture.Data;
using Architecture.Pages;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;

namespace Architecture.WorkArea
{
    // Move systems for task items
    public class WorkFlowsListSet : IBehaviorSync
    {
        // Move params
        private const float speedJoinToAnchors = 0.15f;
        private const float minDelta = 0.7f;
        
        // Animation component of alpha
        public readonly UIAlphaSync LinesAlpha;
        // Task items
        private readonly List<WorkFlowItem> activeFlows = new List<WorkFlowItem>();
        // Add task item to active
        public void Add(WorkFlowItem flow) => activeFlows.Add(flow);
        // Clear active task items list
        public void Clear() => activeFlows.Clear();
        public int Count => activeFlows.Count; // Get count of active tasks
        public WorkFlowItem this[int id] => activeFlows[id]; // Return task item by id
        
        // Edit tasks order move
        private bool editMode;
        public bool EditMode
        {
            get => editMode;
            set 
            {
                editMode = value;
                if (editMode)
                    EnterInEditMode();
                else
                    ExitFromEditMode();
            }
        }

        private List<float> flowAnchors; // Saved positions of tasks

        // Current task that moves
        private WorkFlowItem currentFlow;
        public WorkFlowItem CurrentFlow
        {
            get => currentFlow;
            set
            {
                if (currentFlow != null && currentFlow != value)
                    currentFlow.UnHighlight();
                
                currentFlow = value;
                idOfCurrent = activeFlows.IndexOf(currentFlow);
            }
        }

        private int idOfCurrent; // id of current task item

        // Join task to place flag
        private bool sleepAnchorJoin = true;
        // Get state of task position mode
        public bool JoinToAnchorsState { private set; get; } 
        public bool HasActivity; // Has move activity
        public bool DisabledByScroll = true; // Disable by scroll
        
        // Create and setup
        public WorkFlowsListSet()
        {
            LinesAlpha = new UIAlphaSync {SpeedMode = UIAlphaSync.Mode.Lerp, Speed = 0.1f};
            SyncWithBehaviour.Instance.AddObserver(LinesAlpha, AppSyncAnchors.WorkAreaYear);
        }

        // Setup fields
        public void Start()
        {
            flowAnchors = new List<float>();
            sleepAnchorJoin = true;
        }

        // Unhighlight all tasks
        public void UnhighlightAllFlows()
        {
            foreach (var activeFlow in activeFlows)
                activeFlow.UnHighlight();
        }

        public void Update()
        {
            UpdateJoinToAnchors();
            PositionExchanger();
        }

        // Enter in edit tasks order mode method
        private void EnterInEditMode()
        {
            UpdateAnchors();

            LinesAlpha.Speed = 0.1f;
            LinesAlpha.PrepareToWork();
            LinesAlpha.SetAlphaByDynamic(0);
            LinesAlpha.Run();
           
            if (sleepAnchorJoin)
                Vibration.Vibrate(30);
            
            sleepAnchorJoin = false;
            JoinToAnchorsState = true;
        }

        // Update default positions of tasks
        private void UpdateAnchors()
        {
            if (CurrentFlow != null)
                return;

            flowAnchors.Clear();
            for (var i = 0; i < activeFlows.Count; i++)
                flowAnchors.Add(activeFlows[i].RectTransform.anchoredPosition.y);
        }

        // Exit from edit tasks order mode
        private void ExitFromEditMode()
        {
            sleepAnchorJoin = false;
        }

        // Update join to achors state
        private void UpdateJoinToAnchors()
        {
            if (activeFlows.Count != flowAnchors.Count)
                return;
            
            if (sleepAnchorJoin)
                return;
            
            sleepAnchorJoin = true;

            for (var i = 0; i < activeFlows.Count; i++)
            {
                if (activeFlows[i] == CurrentFlow && EditMode)
                    continue;

                if (Math.Abs(activeFlows[i].RectTransform.anchoredPosition.y - flowAnchors[i]) < minDelta * 0.1f)
                    continue;
                
                if (Mathf.Abs(activeFlows[i].RectTransform.anchoredPosition.y - flowAnchors[i]) < minDelta)
                    activeFlows[i].SetPosition(new Vector2(0, flowAnchors[i]));
                
                activeFlows[i].SetPosition(Vector2.Lerp(activeFlows[i].RectTransform.anchoredPosition, new Vector2(0, flowAnchors[i]),
                    speedJoinToAnchors));
                
                sleepAnchorJoin = false;
            }

            if (!EditMode && sleepAnchorJoin)
            {
                LinesAlpha.Speed = 0.05f;
                LinesAlpha.PrepareToWork(1);
                LinesAlpha.SetAlpha(0);
                LinesAlpha.SetDefaultAlpha(0);
                LinesAlpha.SetAlphaByDynamic(1);
                LinesAlpha.Run();
                CurrentFlow?.UnHighlight();
                PageScroll.Instance.Enabled = true;
                currentFlow = null;
                DisabledByScroll = false;
                JoinToAnchorsState = false;
            }
        }

        // Check for tasks order changed
        private void PositionExchanger()
        {
            if (activeFlows.Count != flowAnchors.Count)
                return;

            var exchanged = UpperExchanger();
            
            if (!exchanged)
                BottomExchanger();
        }

        // Check if task moved to upper direction
        private bool UpperExchanger()
        {
            if (idOfCurrent == 0)
                return false;

            if (currentFlow == null)
                return false;
            
            if (flowAnchors[idOfCurrent - 1] > currentFlow.RectTransform.anchoredPosition.y)
                return false;
            
            var upperFlow = activeFlows[idOfCurrent - 1];
            activeFlows[idOfCurrent - 1] = currentFlow;
            activeFlows[idOfCurrent] = upperFlow;
            idOfCurrent--;
            sleepAnchorJoin = false;
            SavePositions();
            
            return true;
        }

        // Check if task moved to bottom direction
        private void BottomExchanger()
        {
            if (idOfCurrent == flowAnchors.Count - 1)
                return;

            if (currentFlow == null)
                return;

            if (flowAnchors[idOfCurrent + 1] < currentFlow.RectTransform.anchoredPosition.y)
                return;

            var bottomFlow = activeFlows[idOfCurrent + 1];
            activeFlows[idOfCurrent + 1] = currentFlow;
            activeFlows[idOfCurrent] = bottomFlow;
            idOfCurrent++; 
            sleepAnchorJoin = false;
            SavePositions();
        }

        // Save order of tasks
        private void SavePositions()
        {
            for (var i = 0; i < activeFlows.Count; i++)
            {
                var flow = Array.Find(AppData.GetCurrentGroup().Flows, e => e.Id == activeFlows[i].Id);
                flow.Order = (byte) i;
            }
        }
    }
}
