using System.Collections.Generic;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;

namespace Architecture.SettingsArea.ChooseItems
{
    // Pool of text items to view
    public class ItemsPool
    {
        public float ItemHeight => example.sizeDelta.y; // Get height of view
        public readonly UIAlphaSync UIAlphaSync; // Animation component of all items
        
        private readonly RectTransform pool; // Pool object
        private readonly ChooseItemsPanel chooseItemsPanel; // Link to panel view
        private readonly RectTransform example; // Item object example

        // Pool of items
        private readonly Queue<ChooseItem> items = new Queue<ChooseItem>();

        // Create and setup
        public ItemsPool(RectTransform pool, ChooseItemsPanel chooseItemsPanel)
        {
            // Save components 
            this.pool = pool;
            this.chooseItemsPanel = chooseItemsPanel;
            // Find example object
            example = pool.Find("Language Item").GetComponent<RectTransform>();

            // Create animation component of alpha channel for items
            UIAlphaSync = new UIAlphaSync {SpeedMode = UIAlphaSync.Mode.Lerp, Speed = 0.1f};
            SyncWithBehaviour.Instance.AddObserver(UIAlphaSync, AppSyncAnchors.ChooseItemsPanel);
        }

        // Set to pool item method
        public void SetToPool(ChooseItem chooseItem)
        {
            chooseItem.RectTransform.SetParent(pool);
            items.Enqueue(chooseItem);
        }

        // Get item from pool
        public ChooseItem GetFromPool() => items.Count == 0 ? CreateItem() : items.Dequeue();

        // Create new item
        private ChooseItem CreateItem()
        {
            var newObject = Object.Instantiate(example, pool);
            newObject.name = "Item";
            var result = new ChooseItem(newObject, chooseItemsPanel);
            UIAlphaSync.AddElements(result.Circle, result.Name, result.DoneIcon);
            UIAlphaSync.PrepareToWork();
            return result;
        }
        
    }
}
