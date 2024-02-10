using System.Collections.Generic;
using HTools;
using UnityEngine;

namespace HomeTools.Source
{
    // Component of additional observer 
    public class SyncWithBehaviourAnchor : MonoBehaviour
    {
        // List of components for sync updates
        private readonly List<IBehaviorSync> items = new List<IBehaviorSync>();
        private IBehaviorSync[] syncs = new IBehaviorSync[0];
        
        // Add component to sync item
        public T AddObserver<T>(T behavior) where T: IBehaviorSync
        {
            if (!items.Contains(behavior))
                items.Add(behavior);

            behavior.Start();
            syncs = items.ToArray();

            return behavior;
        }

        // Remove component sync item
        public void RemoveObserver(IBehaviorSync behavior)
        {
            if (items.Contains(behavior))
                items.Remove(behavior);

            syncs = items.ToArray();
        }
        
        // Sync updates
        private void Update()
        {
            for (var i = 0; i < syncs.Length; i++) items[i]?.Update();
        }
    }
}
