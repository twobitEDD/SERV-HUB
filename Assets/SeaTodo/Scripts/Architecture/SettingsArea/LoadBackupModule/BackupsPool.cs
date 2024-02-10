using System.Collections.Generic;
using UnityEngine;

namespace Architecture.SettingsArea.LoadBackupModule
{
    // Backup items pool
    public class BackupsPool
    {
        private readonly RectTransform poolParent; // Pool object
        private readonly RectTransform example; // Example object of backup UI
        private readonly Queue<BackupItem> items = new Queue<BackupItem>(); // Set of items

        // Create and setup
        public BackupsPool(RectTransform poolParent)
        {
            this.poolParent = poolParent;
            // Find example backup UI
            example = poolParent.Find("Day").GetComponent<RectTransform>();
        }

        // Get backup item
        public BackupItem GetItem(RectTransform place)
        {
            var result = items.Count == 0 ? CreateNew() : items.Dequeue();
            result.SetParent(place);

            return result;
        }

        // Set backup item to pool
        public void SetToPool(BackupItem dayItem, bool setTransformInPool = true)
        {
            if (setTransformInPool)
                dayItem.SetParent(poolParent);
            
            items.Enqueue(dayItem);
        }

        // Create new backup item
        private BackupItem CreateNew() => new BackupItem(Object.Instantiate(example, poolParent));
    }
}
