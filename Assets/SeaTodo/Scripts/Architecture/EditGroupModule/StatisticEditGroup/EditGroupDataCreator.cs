using System;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using UnityEngine;

namespace Architecture.EditGroupModule.StatisticEditGroup
{
    // Additional layer for EditGroupDataGenerator
    public class EditGroupDataCreator : IGraphicInfo
    {
        // Data generator for icons
        private readonly EditGroupDataGenerator editGroupDataGenerator;
        // Call actions when get new step
        private Action<int> getNewStepAction;

        // Create data generator
        public EditGroupDataCreator() => editGroupDataGenerator = new EditGroupDataGenerator();
        // Get default step for pages
        public int DefaultStep() => EditGroupDataGenerator.DefaultStep;
        public void ReloadData(HomeDay homeDay) { }
        
        // Get step of pages by selected icon item
        public int GetStepById(int id) => Mathf.Abs(id - 1) / 4;
        
        // Add close actions
        public void SetActionGetStep(Action<int> action) => getNewStepAction = action;
       
        // Get info struct of page by id
        public GraphDataStruct GetInfo(int step) => editGroupDataGenerator.CreateStruct(step);
        
        // Invoke actions that has been added
        public void SendStepToOther(int current) => getNewStepAction?.Invoke(current);
    }
}
