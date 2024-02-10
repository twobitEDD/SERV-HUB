using System;
using System.Collections.Generic;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using Architecture.WorkArea.Activity;

namespace Architecture.StatisticsArea.ArchivedFlows
{
    // Additional layer for ArchivedFlowsDataGenerator
    public class ArchivedFlowsDataCreator : IGraphicInfo
    {
        private readonly ArchivedFlowsDataGenerator weekDataGenerator;
        private readonly List<Action<int>> getNewStepAction = new List<Action<int>>();

        // Create week data generator generator
        public ArchivedFlowsDataCreator()
        {
            weekDataGenerator = new ArchivedFlowsDataGenerator();
            weekDataGenerator.UpdateData();
        }

        // Get default step of pages
        public int DefaultStep() => AppData.GetCurrentGroup().ArchivedFlows.Length - 1;
        // Reload data of archived tasks
        public void ReloadData(HomeDay homeDay) => weekDataGenerator.UpdateData();
        // Reload data of archived tasks
        public void ReloadData() => weekDataGenerator.UpdateData();
        // Set action for invoke when updated
        public void SetActionGetStep(Action<int> action) => getNewStepAction.Add(action);
        // Get info about page
        public GraphDataStruct GetInfo(int step) => weekDataGenerator.CreateStruct(step);

        // Invoke actions when view updated
        public void SendStepToOther(int current)
        {
            foreach (var action in getNewStepAction)
                action.Invoke(current);
        }
    }
}
