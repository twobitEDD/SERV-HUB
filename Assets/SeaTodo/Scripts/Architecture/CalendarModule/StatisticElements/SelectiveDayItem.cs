using HomeTools.Source.Design;
using HTools;
using Packages.HomeTools.Source.Design;
using UnityEngine;
using Unity.VectorGraphics;

namespace Architecture.CalendarModule.StatisticElements
{
    // Selective day circle
    public class SelectiveDayItem
    {
        private readonly SVGImage image; // Image of circle
        private UIColorSync lastDayItem; // Animation component of preview selected day

        private readonly RectTransformSync rectTransformSync; // Animation component
        private readonly UIAlphaSync uiAlphaSync; // Animation component

        // Create by svg image
        public SelectiveDayItem(SVGImage image)
        {
            this.image = image;
            
            // Create animation component of rect
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(image.rectTransform);
            rectTransformSync.TargetScale = Vector3.one * 1.5f;
            rectTransformSync.Speed = 0.27f;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.CalendarObject);
            
            // Create animation component of alpha channel for image
            uiAlphaSync = new UIAlphaSync();
            uiAlphaSync.AddElement(image);
            uiAlphaSync.Speed = 0.11f;
            uiAlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            uiAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiAlphaSync, AppSyncAnchors.CalendarObject);
        }

        // Setup circle to place
        public void SetupToPlace(RectTransform place, UIColorSync calendarDayItem)
        {
            SetActive(true);
            
            // Setup circle to new place
            image.rectTransform.anchoredPosition = place.anchoredPosition;
            
            // Play color animation of last selected day
            lastDayItem?.SetColor(0);
            lastDayItem?.SetDefaultMarker(0);

            // Update last day as current
            lastDayItem = calendarDayItem;
            
            // Play rect animation of this circle
            rectTransformSync.SetT(0);
            rectTransformSync.SetDefaultT(0);
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();
            
            // Play alpha channel animation of circle
            uiAlphaSync.SetAlpha(0);
            uiAlphaSync.SetDefaultAlpha(0);
            uiAlphaSync.SetAlphaByDynamic(1);
            uiAlphaSync.Run();
        }

        // Set active state of circle item
        public void SetActive(bool active)
        {
            image.enabled = active;
            
            if (active)
                return;
            
            lastDayItem?.SetColor(0);
            lastDayItem?.SetDefaultMarker(0);
        }
    }
}
