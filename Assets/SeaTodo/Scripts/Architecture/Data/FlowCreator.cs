using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.CalendarModule;
using HomeTools.Other;
using Modules.Notifications;
using UnityEngine;

namespace Architecture.Data
{
    // Create tasks class
    public static class FlowCreator
    {
        // Create new empty task
        public static Flow CreateFlow() => new Flow { Id = AppData.GenerateNewId() };

        // Add task to list
        public static void AddFlowToCurrentGroup(Flow flow)
        {
            var flowList = AppData.GetCurrentGroup().Flows.ToList();
            flowList.Add(flow);
            AppData.GetCurrentGroup().Flows = flowList.ToArray();
        }
        
        // Move task tpo archived state
        public static void ArchiveFlowInCurrentGroup(Flow flow)
        {
            RemoveFlowInCurrentGroup(flow);

            CheckArchivedFlowFinishDate(flow);
            var flowListArchived = AppData.GetCurrentGroup().ArchivedFlows.ToList();
            flowListArchived.Add(flow);
            AppData.GetCurrentGroup().ArchivedFlows = flowListArchived.ToArray();
        }
        
        // Return task to active state
        public static void BackToProgressFromArchive(Flow flow)
        {
            RemoveFlowFromArchive(flow);
            AddFlowToCurrentGroup(flow);
            flow.Finished = false;
            flow.Order = (byte) AppData.GetCurrentGroup().Flows.Length;
        }
        
        // Remove task
        public static void RemoveFlowInCurrentGroup(Flow flow)
        {
            var flowList = AppData.GetCurrentGroup().Flows.ToList();
            flowList.Remove(flow);
            AppData.GetCurrentGroup().Flows = flowList.ToArray();
        }
        
        // Remove archived task
        public static void RemoveFlowFromArchive(Flow flow)
        {
            var flowList = AppData.GetCurrentGroup().ArchivedFlows.ToList();
            flowList.Remove(flow);
            AppData.GetCurrentGroup().ArchivedFlows = flowList.ToArray();
        }

        // Check the task for a finished state
        public static void CheckArchivedFlowFinishDate(Flow flow)
        {
            if (flow.DateFinished == 0)
                flow.DateFinished = Calendar.Today.HomeDayToInt();

            flow.Finished = true;
        }

        // Recreate notifications if reminders have been updated in the current task
        public static void UpdateRemindersWhenStatusChanged(Flow flow)
        {
            if (flow?.Reminders != null && flow.Reminders.Count != 0)
            {
                AppNotifications.ShouldUpdate = true;
                AppNotifications.UpdateNotifications();
            }
        }
    }
}
