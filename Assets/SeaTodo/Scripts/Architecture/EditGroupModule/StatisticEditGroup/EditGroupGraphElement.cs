using System.Collections.Generic;
using Architecture.Statistics.Interfaces;
using UnityEngine;

namespace Architecture.EditGroupModule.StatisticEditGroup
{
    // Empty part from statistics module
    public class EditGroupGraphElement : IGraphElement
    {
        
        public void SetupViewPlace(RectTransform viewPlace)
        {
        }

        public void SetToView(float position)
        {
        }

        public void SetToPool()
        {
        }

        public void Start()
        {
        }

        public void Update(float infoGraph, List<int> infoDescription, bool infoActive, bool highlighted)
        {
        }

        public void UpdateDynamic(float infoGraph, bool infoActive, bool highlighted)
        {
        }
        
        public IGraphElement CreateClone() => new EditGroupGraphElement();
    }
}
