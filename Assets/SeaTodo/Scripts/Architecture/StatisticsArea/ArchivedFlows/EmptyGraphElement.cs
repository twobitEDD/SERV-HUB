using System.Collections.Generic;
using Architecture.Statistics.Interfaces;
using UnityEngine;

namespace Architecture.StatisticsArea.ArchivedFlows
{
    // Empty component for statistics scroll module
    public class EmptyGraphElement : IGraphElement
    {
        public void SetupViewPlace(RectTransform viewPlace) { }
        public void SetToView(float position) { }
        public void SetToPool() { }
        public void Update(float infoGraph, List<int> infoDescription, bool infoActive, bool highlighted) { }
        public void UpdateDynamic(float infoGraph, bool infoActive, bool highlighted) { }
        public IGraphElement CreateClone() => new EmptyGraphElement();
    }
}
