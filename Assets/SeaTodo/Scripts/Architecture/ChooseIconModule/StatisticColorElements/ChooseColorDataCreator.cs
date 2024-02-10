using System;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;

namespace Architecture.ChooseIconModule.StatisticColorElements
{
    // Additional layer for ChooseColorDataGenerator
    public class ChooseColorDataCreator : IGraphicInfo
    {
        // Data generator for colors
        private readonly ChooseColorDataGenerator chooseColorDataGenerator;
        // Call actions when get new step
        private Action<int> getNewStepAction;

        // Create data generator
        public ChooseColorDataCreator() => chooseColorDataGenerator = new ChooseColorDataGenerator();
       
        // Get default step for pages
        public int DefaultStep() => ChooseColorDataGenerator.DefaultStep;

        // Get step of pages by selected color item
        public int GetStepById(int id) => id / 5;
        public void ReloadData(HomeDay homeDay) { }
        
        // Add close actions
        public void SetActionGetStep(Action<int> action) => getNewStepAction = action;
        
        // Get info struct of page by id
        public GraphDataStruct GetInfo(int step) => chooseColorDataGenerator.CreateStruct(step);
        
        // Invoke actions that has been added
        public void SendStepToOther(int current) => getNewStepAction?.Invoke(current);
    }
}
