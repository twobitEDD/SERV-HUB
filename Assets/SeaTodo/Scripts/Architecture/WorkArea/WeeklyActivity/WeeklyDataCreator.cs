using System;
using System.Collections.Generic;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using Architecture.WorkArea.Activity;

namespace Architecture.WorkArea.WeeklyActivity
{
    // Additional layer for WeeklyDataGenerator
    public class WeeklyDataCreator : IGraphicInfo
    {
        private readonly WeeklyDataGenerator weekDataGenerator;
        private readonly List<Action<int>> getNewStepAction = new List<Action<int>>();

        // Create data generator
        public WeeklyDataCreator() => weekDataGenerator = new WeeklyDataGenerator();
        
        // Get default step of pages
        public int DefaultStep() => WeekDataGenerator.DefaultStep;
        // Reload data by chosen day
        public void ReloadData(HomeDay homeDay) => weekDataGenerator.Update(homeDay);
        // Add close actions
        public void SetActionGetStep(Action<int> action) => getNewStepAction.Add(action);
        // Get info about page
        public GraphDataStruct GetInfo(int step) => weekDataGenerator.CreateStruct(step);
        // Invoke actions that has been added
        public void SendStepToOther(int current)
        {
            foreach (var action in getNewStepAction)
                action.Invoke(current);
        }
    }
}
