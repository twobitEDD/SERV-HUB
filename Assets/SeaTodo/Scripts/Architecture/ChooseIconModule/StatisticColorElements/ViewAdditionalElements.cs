using Architecture.Elements;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;

namespace Architecture.ChooseIconModule.StatisticColorElements
{
    // Page with color circles
    public class ViewAdditionalElements : IViewAdditional
    {
        // Link to view
        private ChooseIconModule ChooseIconModule() => AreasLocator.Instance.ChooseIconModule;
        // Animation component of alpha channel for circles
        public UIAlphaSyncGroup UIAlphaSyncLocalGroup;
        // Rect of page
        private RectTransform rectTransform;
        // Array of circle items
        private ChooseColorItem[] iconPlaceItem = new ChooseColorItem[0];
        
        // Initialize page
        public void Setup(RectTransform view)
        {
            rectTransform = view;
            UIAlphaSyncLocalGroup = new UIAlphaSyncGroup();
            SetupIconsItems();
        }

        // Setup scroll component for each color item
        public void SetupScroll(StatisticsScroll statisticsScroll)
        {
            foreach (var colorItem in iconPlaceItem) colorItem.SetupScrollEvents(statisticsScroll);
        }

        // Update page
        public void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next)
        {
            UpdateItems(current);
            ChooseIconModule()?.MainStatsColors.ChooseColorDataCreator.SendStepToOther((int) current.GraphElementsInfo[0]);
        }

        // Update page
        public void Update(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            FullUpdate(preview, current, next);
        
        // Update items by new data
        private void UpdateItems(GraphDataStruct graphDataStruct)
        {
            // Check if page is empty
            if (graphDataStruct.EmptyActivity())
            {
                foreach (var item in iconPlaceItem)
                    item.Disable();
                
                return;
            }
            // Get color info
            var infoIcons = graphDataStruct.GraphElementsDescription[0];

            // Update each items
            var currentId = ChooseIconModule()?.ChooseIconSession.SelectedColorLocal;
            for (var i = 0; i < infoIcons.Count; i++)
            {
                // Update item
                iconPlaceItem[i].UpdateView(infoIcons[i]);
                
                // Setup to selected state if need it
                if (currentId != null && currentId == infoIcons[i])
                    iconPlaceItem[i].CheckToSelective();
            }
        }
        
        // Prepare items for use
        private void SetupIconsItems()
        {
            // Create array of items
            iconPlaceItem = new ChooseColorItem[5];
            for (var i = 0; i < iconPlaceItem.Length; i++)
            {
                // Find object in scene for item and prepare it
                var place = rectTransform.Find($"Place {i + 1}").gameObject;
                iconPlaceItem[i] = new ChooseColorItem(place);
                UIAlphaSyncLocalGroup.AddSync(iconPlaceItem[i].UIAlphaSync);
            }
        }
    }
}