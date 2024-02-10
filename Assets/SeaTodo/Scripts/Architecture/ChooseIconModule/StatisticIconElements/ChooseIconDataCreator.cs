using System;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;

namespace Architecture.ChooseIconModule.StatisticIconElements
{
    // Additional layer for ChooseIconDataGenerator
    public class ChooseIconDataCreator : IGraphicInfo
    {
        // Data generator for icons
        private readonly ChooseIconDataGenerator chooseIconDataGenerator;
        // Call actions when get new step
        private Action<int> getNewStepAction;
        // Create data generator
        public ChooseIconDataCreator() => chooseIconDataGenerator = new ChooseIconDataGenerator();
        // Get default step for pages
        public int DefaultStep() => ChooseIconDataGenerator.DefaultStep;
        public void ReloadData(HomeDay homeDay) { }
        // Get step of pages by selected color item
        public int GetStepById(int id) => id / 8;
        // Add close actions
        public void SetActionGetStep(Action<int> action) => getNewStepAction = action;
        // Get info struct of page by id
        public GraphDataStruct GetInfo(int step) => chooseIconDataGenerator.CreateStruct(step);
        // Call to actions that has been added
        public void SendStepToOther(int current) => getNewStepAction?.Invoke(current);
    }
}
