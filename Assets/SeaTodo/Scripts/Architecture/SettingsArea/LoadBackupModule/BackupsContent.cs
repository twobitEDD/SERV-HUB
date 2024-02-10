using System.Collections.Generic;
using UnityEngine;

namespace Architecture.SettingsArea.LoadBackupModule
{
    // Component for backups view in window
    public class BackupsContent
    {
        public readonly RectTransform RectTransform; // Rect object with backups list
        private readonly BackupsPool backupsPool; // Pool with items
        private readonly List<BackupItem> activeItems = new List<BackupItem>(); // Active items
        private List<string> backupNamesLocal = new List<string>(); // Last of backup names
        
        public const float ShiftBetweenItems = 137f; // Distance between items
        
        // Create and setup
        public BackupsContent(RectTransform rectTransform)
        {
            RectTransform = rectTransform;
            // Create items pool component
            backupsPool = new BackupsPool(rectTransform.Find("Pool").GetComponent<RectTransform>());
        }

        // Update backups list
        public void Setup(List<string> itemsList)
        {
            // Save names of backups
            backupNamesLocal = itemsList;
            
            // Calculate how many items need to be added or removed
            var delta = backupNamesLocal.Count - activeItems.Count;    
            
            // Add additional items
            if (delta > 0)
                AddItems(delta);

            // Remove additional items
            if (delta < 0)
                RemoveItems(delta);
            
            if (activeItems.Count == 0)
                return;
            
            // Update backups rect by backups count
            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, 
                Mathf.Abs(activeItems[activeItems.Count - 1].RectTransform.anchoredPosition.y) + ShiftBetweenItems * 0.37f);

            // Update items with new data
            UpdateItemsByNewInfoDefault();
        }

        // Check backups
        public bool HasBackups() => backupNamesLocal.Count > 0;

        // Add backup items to list
        private void AddItems(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var item = backupsPool.GetItem(RectTransform);
                activeItems.Add(item);
                item.RectTransform.anchoredPosition = new Vector2(0, -(activeItems.Count - 1) * ShiftBetweenItems - ShiftBetweenItems * 0.5f);
            }
        }

        // Remove backup items from list
        private void RemoveItems(int count)
        {
            count = Mathf.Abs(count);

            for (var i = 0; i < count; i++)
            {
                var item = activeItems[activeItems.Count - 1];
                activeItems.Remove(item);
                backupsPool.SetToPool(item);
            }
        }

        // Update items with new data
        private void UpdateItemsByNewInfoDefault()
        {
            for (var i = 0; i < activeItems.Count; i++)
            {
                if (i >= backupNamesLocal.Count)
                    continue;
                
                activeItems[i].ResetByName(backupNamesLocal[i]);
                activeItems[i].SetActivateLineBorder(i != activeItems.Count - 1);
            }
        }
    }
}
