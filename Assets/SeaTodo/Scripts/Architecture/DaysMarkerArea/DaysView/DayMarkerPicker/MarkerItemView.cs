using Architecture.Components;
using Architecture.Data;
using Architecture.DaysMarkerArea.DaysColors;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using UnityEngine;
using Unity.VectorGraphics;

namespace Architecture.DaysMarkerArea.DaysView.DayMarkerPicker
{
    // Item of day characteristic in view of choose characteristic
    public class MarkerItemView
    {
        public readonly int Order; // Id of item
        
        private readonly MainButtonJob mainButtonJob; // Button component of item
        private readonly SVGImage circle; // Image component of item
        private readonly RectTransformSync rectTransformSync; // Animation component of rect of item
        private readonly UIAlphaSync uiAlphaSync; // Animation component of alpha channel
        
        // Return color of circle with opaque alpha channel
        public Color ItemColor
        {
            get
            {
                var color = circle.color;
                color.a = 1f;
                return color;
            }
        }

        // Create and setup item
        public MarkerItemView(SVGImage circle, GameObject handler, DayPicker dayPicker, int order)
        {
            // Save image and id
            this.circle = circle;
            Order = order;
            
            // Create button component of item
            mainButtonJob = new MainButtonJob(circle.rectTransform, () => dayPicker.MarkerTouched(order), handler);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.ChooseDayMarkerModule);

            // Create animation component of size of circle
            rectTransformSync = new RectTransformSync {Speed = 0.17f, SpeedMode = RectTransformSync.Mode.Lerp};
            rectTransformSync.SetRectTransformSync(circle.rectTransform);
            rectTransformSync.TargetSizeDelta = circle.rectTransform.sizeDelta * 1.57f;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.ChooseDayMarkerModule);
            
            // Create animation component of alpha channel of item
            uiAlphaSync = new UIAlphaSync() { Speed = 0.1f, SpeedMode = UIAlphaSync.Mode.Lerp};
            uiAlphaSync.AddElement(circle);
            uiAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiAlphaSync, AppSyncAnchors.ChooseDayMarkerModule);
        }

        // Reset selective state of item
        public void Prepare()
        {
            // Reset state of button
            mainButtonJob.Reactivate();
            // Reset color to default
            SetupColor();
            // Deactivate state
            Deactivate(true);
        }
        
        // Setup item to selective state
        public void Activate(bool immediately)
        {
            // Start animation component of alpha channel of circle
            if (immediately)
            {
                uiAlphaSync.SetAlpha(1);
                uiAlphaSync.SetDefaultAlpha(1);
                uiAlphaSync.Stop();
                rectTransformSync.SetT(0);
                rectTransformSync.SetDefaultT(0);
                rectTransformSync.Stop();
            }
            else
            {
                uiAlphaSync.SetAlphaByDynamic(1f);
                uiAlphaSync.Run();
                rectTransformSync.SetTByDynamic(0);
                rectTransformSync.Run();
            }
        }

        // Setup item to default state
        public void Deactivate(bool immediately)
        {
            // Start animation component of alpha channel of circle
            if (immediately)
            {
                uiAlphaSync.SetAlpha(0.5f);
                uiAlphaSync.SetDefaultAlpha(0.5f);
                uiAlphaSync.Stop();
                rectTransformSync.SetT(1);
                rectTransformSync.SetDefaultT(1);
                rectTransformSync.Stop();
            }
            else
            {
                uiAlphaSync.SetAlphaByDynamic(0.5f);
                uiAlphaSync.Run();
                rectTransformSync.SetTByDynamic(1);
                rectTransformSync.Run();
            }
        }
        
        // Return name of item
        public string GetItemName()
        {
            // Return empty if exception
            if (Order > AppData.ColorMarkers.Count) 
                return TextLocalization.GetLocalization(TextKeysHolder.Empty);

            return Order > 0 ? AppData.ColorMarkers[Order - 1] : TextLocalization.GetLocalization(TextKeysHolder.Empty);
        }

        // Reset color to default
        private void SetupColor()
        {
            if (Order == 0)
                circle.color = ThemeLoader.GetCurrentTheme().DaysMarkerAreaPickerEmptyColor;
            else if (Order < AppData.ColorMarkers.Count + 1)
                circle.color = ColorMarkersDescriptor.GetColor(Order - 1);
        }
    }
}
