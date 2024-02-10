using System.Collections.Generic;
using UnityEngine;

namespace Architecture.Statistics.Interfaces
{
    // Component for graph item
    public interface IGraphElement
    {
        void SetupViewPlace(RectTransform viewPlace); // Setup graph item to place
        
        void SetToView(float position); // Setup position in place
        void SetToPool(); // Setup graph item to pool
        // Update graph item
        void Update(float infoGraph, List<int> infoDescription, bool infoActive, bool highlighted);
        // Update graph item with animation
        void UpdateDynamic(float infoGraph, bool infoActive, bool highlighted);
        // Create clone of graph item
        IGraphElement CreateClone();
    }
}
