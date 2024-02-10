using Architecture.DaysMarkerArea.DaysColors.UpdateModule;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using Packages.HomeTools.Source.Design;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView.DayMarkerPicker
{
    // The component that is responsible for choosing the characteristics of the day
    public class DayPicker
    {
        // Day characteristics
        private readonly MarkerItemView[] markerItemView;
        
        private readonly Text markerTitle; // View title
        private readonly UIColorSync titleSync; // Animation component of color
        private readonly UIAlphaSync titleAlphaSync; // Animation component of alpha channel of title
        private readonly RectTransformSync titleRectSync; // Animation component of rect
        private readonly Text description; // View description
        
        private ChooseDaySession currentSession; // Session of choose day
        private MarkerItemView currentItem; // Selected day characteristic
        
        public readonly UIAlphaSync UIAlphaSync; // Animation component of alpha channel

        public DayPicker(RectTransform rectTransform)
        {
            // Find name and description texts
            markerTitle = rectTransform.Find("Default Name").GetComponent<Text>();
            description = rectTransform.Find("Description").GetComponent<Text>();

            // Create array and animation components
            markerItemView = new MarkerItemView[6];
            UIAlphaSync = new UIAlphaSync();

            // Find and setup day characteristic items
            for (var i = 0; i < markerItemView.Length; i++)
            {
                // Find circle
                var circle = rectTransform.Find($"Marker {i}").GetComponent<SVGImage>();
                // Find handler of circle
                var handler = rectTransform.Find($"Handler Marker {i}").gameObject;
                // Create item component
                markerItemView[i] = new MarkerItemView(circle, handler, this, i);
                // Add circle to animation component of alpha channel
                UIAlphaSync.AddElement(circle);
            }

            // Setup animation component of alpha channel of elements
            UIAlphaSync.AddElement(markerTitle);
            UIAlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            UIAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(UIAlphaSync, AppSyncAnchors.ChooseDayMarkerModule);
            
            // Create color animation component for title of characteristic
            titleSync = new UIColorSync() { Speed = 0.27f, SpeedMode = UIColorSync.Mode.Lerp};
            titleSync.AddElement(markerTitle, Color.white);
            titleSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(titleSync, AppSyncAnchors.ChooseDayMarkerModule);
            
            // Create alpha channel animation component for title and description
            titleAlphaSync = new UIAlphaSync() { Speed = 0.27f, SpeedMode = UIAlphaSync.Mode.Lerp};
            titleAlphaSync.AddElement(markerTitle);
            titleAlphaSync.AddElement(description);
            titleAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(titleAlphaSync, AppSyncAnchors.ChooseDayMarkerModule);
            
            // Create scale animation component for title
            titleRectSync = new RectTransformSync() { Speed = 0.27f, SpeedMode = RectTransformSync.Mode.Lerp};
            titleRectSync.SetRectTransformSync(markerTitle.rectTransform);
            titleRectSync.TargetScale = Vector3.one * 1.07f;
            titleRectSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(titleRectSync, AppSyncAnchors.ChooseDayMarkerModule);
        }

        // Update view by new session
        public void PrepareView(ChooseDaySession chooseDaySession)
        {
            // Save session
            currentSession = chooseDaySession;

            // Reset state for items
            foreach (var item in markerItemView)
                item.Prepare();

            // Get and check characteristic item id
            
            var id = currentSession.MarkerId;
            
            if (id < 0)
                id = 0;
            
            if (id >= markerItemView.Length)
                id = markerItemView.Length - 1;
            
            // Select and set activity for current item
            currentItem = markerItemView[id];
            currentItem.Activate(true);

            // Update title
            UpdateTitleText();
            markerTitle.color = currentItem.ItemColor;
            titleSync.Stop();
            titleAlphaSync.Stop();
            titleRectSync.SetDefaultT(1);
            titleRectSync.SetT(1);
            titleRectSync.Stop();
        }

        // The method is called when the user clicked on item
        public void MarkerTouched(int id)
        {
            if (currentSession.MarkerId == id)
                return;
            
            // Deactivate old selected item
            currentItem?.Deactivate(false);
            // Update new selected item
            currentItem = markerItemView[id];
            currentItem.Activate(false);
            currentSession.MarkerId = id;
            // Update title of characteristic
            UpdateMarkerTitle();
        }

        // Update title of characteristic
        private void UpdateMarkerTitle()
        {
            // Update title and description of characteristic
            UpdateTitleText();
            // Save color of title
            var color = markerTitle.color;
            // Reset animation component
            titleSync.SetColor(0);
            titleSync.SetDefaultMarker(0);
            // Restore color of title
            markerTitle.color = color;
            // Prepare and run animation component
            titleSync.AddElement(markerTitle, currentItem.ItemColor);
            titleSync.SetTargetByDynamic(1);
            titleSync.Run();
            // Prepare and run alpha channel animation component
            titleAlphaSync.SetAlpha(0.37f);
            titleAlphaSync.SetDefaultAlpha(0.37f);
            titleAlphaSync.SetAlphaByDynamic(1);
            titleAlphaSync.Run();
            // Prepare and run rect animation component
            titleRectSync.SetDefaultT(0);
            titleRectSync.SetT(0);
            titleRectSync.SetTByDynamic(1);
            titleRectSync.Run();
        }

        // Update title and description of characteristic
        private void UpdateTitleText()
        {
            // Update title
            markerTitle.text = currentItem.GetItemName();

            //Update description
            if (currentItem.Order == 0)
            {
                description.text = TextLocalization.GetLocalization(TextKeysHolder.MarkerDescription0);
            }
            else
            {
                description.text = TextLocalization.GetLocalization(UpdateMarkerModule.GetMarkerDescriptionKey(currentItem.Order - 1));
            }
        }
    }
}
