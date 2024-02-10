using Architecture.Elements;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;

namespace Architecture.TutorialArea.TutorialElements
{
    // Page with tutorial
    public class ViewAdditionalElements : IViewAdditional
    {
        // Link to component with tutorial steps
        private StepsPackage StepsPackage => AreasLocator.Instance.Tutorial.StepsPackage;
        public UIAlphaSync UIAlphaSyncLocal => tutorialItem?.UIAlphaSync();

        // Rect object
        private RectTransform rectTransform;
        // Current tutorial item
        private ITutorialItem tutorialItem;

        public void Setup(RectTransform view)
        {
            rectTransform = view;
        }

        public void SetupScroll(StatisticsScroll scroll)
        {
            
        }

        // Update page
        public void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next)
        {
            UpdateItems(current);
        }

        // Update page
        public void Update(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            FullUpdate(preview, current, next);
        
        // Update items by new data
        private void UpdateItems(GraphDataStruct graphDataStruct)
        {
            if (graphDataStruct.EmptyActivity())
            {
                tutorialItem?.SetActive(false);
                return;
            }
            
            if (tutorialItem != null)
                StepsPackage.SetItemToPool(tutorialItem);

            tutorialItem = StepsPackage.GetItemByStep((int)graphDataStruct.GraphElementsInfo[0]);
            tutorialItem.RectTransform().SetParent(rectTransform);
            tutorialItem.RectTransform().anchoredPosition = Vector2.zero;
            tutorialItem?.SetActive(true);
        }
    }
}