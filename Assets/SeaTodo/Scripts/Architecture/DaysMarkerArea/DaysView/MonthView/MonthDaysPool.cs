using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView.MonthView
{
    // Pool component for day items in month view
    public class MonthDaysPool
    {
        private readonly RectTransform poolParent; // Pool object
        private readonly RectTransform example; // Day item example
        private readonly Queue<MonthDay> items = new Queue<MonthDay>(); // Set of items

        // Create
        public MonthDaysPool(RectTransform poolParent)
        {
            this.poolParent = poolParent;
            // Find in scene resources example of day item
            example = poolParent.Find("Day").GetComponent<RectTransform>();
        }

        // Return item from pool
        public MonthDay GetItem(RectTransform place)
        {
            var result = items.Count == 0 ? CreateNew() : items.Dequeue();
            result.SetParent(place);

            return result;
        }

        // Set item to pool
        public void SetToPool(MonthDay dayItem, bool setTransformInPool = true)
        {
            if (setTransformInPool)
                dayItem.SetParent(poolParent);
            
            items.Enqueue(dayItem);
        }

        // Create new day item
        private MonthDay CreateNew() => new MonthDay(Object.Instantiate(example, poolParent));
    }
}
