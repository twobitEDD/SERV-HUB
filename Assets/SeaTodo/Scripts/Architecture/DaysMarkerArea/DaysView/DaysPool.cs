using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView
{
    // Pool of day items for month view in Sea Calendar page
    public class DaysPool
    {
        private readonly RectTransform poolParent; // Object of pool
        private readonly Image example; // Example of day circle 
        private readonly Queue<DayItem> items = new Queue<DayItem>(); // Set of items in pool

        // Create pool
        public DaysPool(RectTransform poolParent)
        {
            this.poolParent = poolParent;
            // Find example of day circle   
            example = poolParent.Find("Day").GetComponent<Image>();
            // Create package of items for year
            GenerateStartPool();
            // Set pool object to scene root
            poolParent.SetParent(null);
        }

        // Get day item
        public DayItem GetItem(RectTransform place)
        {
            var result = items.Count == 0 ? CreateNew() : items.Dequeue();
            result.SetParent(place);

            return result;
        }

        // Setup day item to pool 
        public void SetToPool(DayItem dayItem, bool setTransformInPool = true)
        {
            if (setTransformInPool)
                dayItem.SetParent(poolParent);
            
            items.Enqueue(dayItem);
        }

        // Create new day item
        private DayItem CreateNew() => new DayItem(Object.Instantiate(example, poolParent));

        // Create day items
        private void GenerateStartPool()
        {
            for (var i = 0; i < 365; i++)
            {
                var item = CreateNew();
                items.Enqueue(item);
            }
        }
    }
}
