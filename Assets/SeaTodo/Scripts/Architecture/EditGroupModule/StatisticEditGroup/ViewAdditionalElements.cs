using Architecture.Elements;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;

namespace Architecture.EditGroupModule.StatisticEditGroup
{
    // Page with title icons
    public class ViewAdditionalElements : IViewAdditional
    {
        // Link to view
        private UpdateTitleModule EditGroupModule() => AreasLocator.Instance.UpdateTitleModule;
        // Animation component of alpha channel for title icons
        public UIAlphaSync UIAlphaSyncLocal;
        // Rect of page
        private RectTransform rectTransform;
        // Array of title icon items
        private EditGroupItem[] iconPlaceItem = new EditGroupItem[0];

        // Initialize page
        public void Setup(RectTransform view)
        {
            rectTransform = view;
            
            UIAlphaSyncLocal = new UIAlphaSync();
            UIAlphaSyncLocal.Speed = 0.2f;
            UIAlphaSyncLocal.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(UIAlphaSyncLocal, AppSyncAnchors.EditTitleModule);

            SetupIconsItems();
            
            UIAlphaSyncLocal.PrepareToWork();
        }

        // Setup scroll component for each color item
        public void SetupScroll(StatisticsScroll statisticsScroll)
        {
            foreach (var dayItem in iconPlaceItem) dayItem.SetupScrollEvents(statisticsScroll);
        }

        // Update page
        public void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next)
        {
            UpdateIcons(current);
            EditGroupModule()?.MainStatsIcons.EditGroupDataCreator.SendStepToOther((int) current.GraphElementsInfo[0]);
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

            // Get title icons info
            var infoIcons = graphDataStruct.GraphElementsDescription[0];

            // Update each items
            var currentId = EditGroupModule()?.EditGroupSession.SelectedIconLocal;
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
            iconPlaceItem = new EditGroupItem[4];
            for (var i = 0; i < iconPlaceItem.Length; i++)
            {
                // Find object in scene for item and prepare it
                var place = rectTransform.Find($"Place {i + 1}").gameObject;
                iconPlaceItem[i] = new EditGroupItem(place);
                UIAlphaSyncLocal.AddElement(iconPlaceItem[i].Circle);
                UIAlphaSyncLocal.AddElement(iconPlaceItem[i].Icon);
            }
        }
    }
}