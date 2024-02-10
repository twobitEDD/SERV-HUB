using HomeTools.Source.Design;
using UnityEngine;

namespace Architecture.TutorialArea.TutorialElements
{
    // Component with pages of tutorial
    public class StepsPackage
    {
        private readonly ITutorialItem[] tutorialItems; // Array of pages
        private readonly RectTransform pool; // Pool object
        
        // Create pages
        public StepsPackage(RectTransform pool)
        {
            // Save pool object component
            this.pool = pool;
            // Create array of pages
            tutorialItems = new ITutorialItem[4];
            
            // Create pages
            tutorialItems[0] = new StepOnlyView(pool.Find("Step 1").GetComponent<RectTransform>());
            tutorialItems[1] = new StepOnlyView(pool.Find("Step 2").GetComponent<RectTransform>());
            tutorialItems[2] = new StepOnlyView(pool.Find("Step 3").GetComponent<RectTransform>());
            tutorialItems[3] = new StepStart(pool.Find("Step 4").GetComponent<RectTransform>());
        }

        // Get page by step
        public ITutorialItem GetItemByStep(int step)
        {
            if (step < 0 || step >= tutorialItems.Length)
                return null;
            
            tutorialItems[step].ResetToDefault();
            return tutorialItems[step];
        }

        // Setup page to pool
        public void SetItemToPool(ITutorialItem item)
        {
            item.RectTransform().SetParent(pool);
        }

        // Get animation components of alpha channel of pages
        public UIAlphaSync[] UIAlphaSyncs()
        {
            var result = new UIAlphaSync[tutorialItems.Length];
            
            for (var i = 0; i < tutorialItems.Length; i++)
                result[i] = tutorialItems[i].UIAlphaSync();

            return result;
        }
    }
}
