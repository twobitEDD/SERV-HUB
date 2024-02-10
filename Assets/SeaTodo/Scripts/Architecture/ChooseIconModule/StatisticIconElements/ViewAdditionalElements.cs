using Architecture.Elements;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;

namespace Architecture.ChooseIconModule.StatisticIconElements
{
    // Page with icon items
    public class ViewAdditionalElements : IViewAdditional
    {
        // Link to view
        private ChooseIconModule ChooseIconModule() => AreasLocator.Instance.ChooseIconModule;
        // Animation component of alpha channel for icons
        public UIAlphaSync UIAlphaSyncLocal;
        // Rect of page
        private RectTransform rectTransform;
        // Array of icon items
        private ChooseIconItem[] iconPlaceItem = new ChooseIconItem[0];
        
        // Initialize page
        public void Setup(RectTransform view)
        {
            rectTransform = view;
            
            // Create animation component of alpha channel
            UIAlphaSyncLocal = new UIAlphaSync();
            UIAlphaSyncLocal.Speed = 0.2f;
            UIAlphaSyncLocal.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(UIAlphaSyncLocal, AppSyncAnchors.ChooseIconModule);

            SetupIconsItems();
            
            UIAlphaSyncLocal.PrepareToWork();
        }

        // Setup scroll component for each icon item
        public void SetupScroll(StatisticsScroll statisticsScroll)
        {
            foreach (var dayItem in iconPlaceItem) dayItem.SetupScrollEvents(statisticsScroll);
        }

        // Update page
        public void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next)
        {
            UpdateIcons(current);
            ChooseIconModule()?.MainStatsIcons.ChooseIconDataCreator.SendStepToOther((int) current.GraphElementsInfo[0]);
        }

        // Update page
        public void Update(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            FullUpdate(preview, current, next);
        
        // Update items by new data
        private void UpdateIcons(GraphDataStruct graphDataStruct)
        {
            // Check if page is empty
            if (graphDataStruct.EmptyActivity())
            {
                foreach (var day in iconPlaceItem)
                    day.Disable();

                return;
            }
            // Get icons info
            var infoIcons = graphDataStruct.GraphElementsDescription[0];

            // Update each items
            var currentId = ChooseIconModule()?.ChooseIconSession.SelectedIconLocal;
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
            iconPlaceItem = new ChooseIconItem[8];
            for (var i = 0; i < iconPlaceItem.Length; i++)
            {
                // Find object in scene for item and prepare it
                var place = rectTransform.Find($"Place {i + 1}").gameObject;
                iconPlaceItem[i] = new ChooseIconItem(place);
                // Add elements of item to animation component
                UIAlphaSyncLocal.AddElement(iconPlaceItem[i].Circle);
                UIAlphaSyncLocal.AddElement(iconPlaceItem[i].Icon);
            }
        }
    }
}