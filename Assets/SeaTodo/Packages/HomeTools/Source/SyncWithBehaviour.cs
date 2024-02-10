using System.Collections.Generic;
using HomeTools.Source;
using UnityEngine;

namespace HTools
{
    // Observer for sync Unity updates with updates of components
    public class SyncWithBehaviour : MonoBehaviour
    {
        // Component with interface
        private readonly List<IBehaviorSync> items = new List<IBehaviorSync>();
        private IBehaviorSync[] syncs; // Array of components

        // Singleton object
        private static SyncWithBehaviour instance;

        // Additional observers for sync only part of items
        private readonly Dictionary<int, SyncWithBehaviourAnchor> syncAnchors = new Dictionary<int, SyncWithBehaviourAnchor>();
        
        public static SyncWithBehaviour Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = SingletonCenter.Instance.gameObject;
                    instance = go.AddComponent<SyncWithBehaviour>();
                }

                return instance;
            }
        }

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

        // Add additional observer to gameObject
        public void AddAnchor(GameObject syncObject, int anchorKey)
        {
            if (syncAnchors.ContainsKey(anchorKey))
                return;

            var sync = syncObject.AddComponent<SyncWithBehaviourAnchor>();
            syncAnchors.Add(anchorKey, sync);
        }
        
        // Add component sync item by key to additional observer
        public T AddObserver<T>(T behavior, int key) where T: IBehaviorSync
        {
            if (syncAnchors.ContainsKey(key)) 
                return syncAnchors[key].AddObserver(behavior);
            
            Debug.Log("<color=red>No Sync Anchor</color>");
            AddObserver(behavior);
            
            return behavior;
        }

        // Sync updates
        private void Update()
        {
            for (var i = 0; i < syncs.Length; i++) items[i]?.Update();
        }
    }
}