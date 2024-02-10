using System;
using System.Collections.Generic;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;

namespace Architecture.TutorialArea.TutorialElements
{
    // Additional layer for TutorialDataGenerator
    public class TutorialDataCreator : IGraphicInfo
    {

        // Data generator for pages
        private readonly TutorialDataGenerator tutorialDataGenerator;
        // Call actions when get new step
        private readonly List<Action<int>> getNewStepActions = new List<Action<int>>();

        // Create data generator
        public TutorialDataCreator() => tutorialDataGenerator = new TutorialDataGenerator();
        // Get default step for pages
        public int DefaultStep() => TutorialDataGenerator.DefaultStep;
        
        public void ReloadData(HomeDay homeDay) { }
        
        // Add close actions
        public void SetActionGetStep(Action<int> action) => getNewStepActions.Add(action);
        // Get info struct of page by id
        public GraphDataStruct GetInfo(int step) => tutorialDataGenerator.CreateStruct(step);
        // Invoke actions that has been added
        public void SendStepToOther(int current)
        {
            foreach (var action in getNewStepActions)
                action.Invoke(current);
        }
    }
}
