using System;
using System.Collections.Generic;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;

namespace Architecture.TaskViewArea.NormalView.Statistics
{
    // Additional layer for FlowDataGenerator
    public class FlowDataCreator : IGraphicInfo
    {
        private readonly FlowDataGenerator flowDataGenerator;
        // Actions that invokes when statistic updates
        private readonly List<Action<int>> getNewStepAction = new List<Action<int>>();

        // Create task data generator
        public FlowDataCreator() => flowDataGenerator = new FlowDataGenerator();
        // Get default step of pages of calendar
        public int DefaultStep() => flowDataGenerator.DefaultStep;

        public void ReloadData(HomeDay homeDay)
        {
        }

        // Set actions to invoke when statistic updates
        public void SetActionGetStep(Action<int> action) => getNewStepAction.Add(action);
        // Setup task to generator
        public void SetupFlowToGenerator(Flow flow) => flowDataGenerator.Update(flow);

        // Get info about page
        public GraphDataStruct GetInfo(int step) => flowDataGenerator.CreateStruct(step);

        // Invoke actions when statistic updates
        public void SendStepToOther(int current)
        {
            foreach (var action in getNewStepAction)
                action.Invoke(current);
        }
    }
}
