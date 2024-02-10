using Architecture.DaysMarkerArea.DaysColors;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView
{
    // The day component to display the circle in the month line in Sea Calendar page
    public class DayItem
    {
        private readonly RectTransform rectTransform; // Rect of circle
        private readonly Image image; // Image of circle
        private readonly UIColorSync uiColorSync; // Animation component of color for circle
        private readonly RectTransformSync rectTransformSync; // Animation component for rect
        public int DayStatus; // Day characteristic id

        private const float joinSpeed = 0.07f; // Move speed
        public float TargetPosition; // Target position for move
        
        // Create
        public DayItem(Image image)
        {
            // Save components
            this.image = image;
            rectTransform = image.rectTransform;

            // Create animation component of circle color
            uiColorSync = new UIColorSync();
            uiColorSync.AddElement(image, ThemeLoader.GetCurrentTheme().SecondaryColorD4);
            uiColorSync.Speed = 0.1f;
            uiColorSync.SpeedMode = UIColorSync.Mode.Lerp;
            uiColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiColorSync, AppSyncAnchors.MarkersAreaDays);
            
            // Create animation component of circle rect 
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.TargetScale = Vector3.one * 1.17f;
            rectTransformSync.Speed = 0.1f;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.MarkersAreaDays);
        }

        // Set circle to parent and update position
        public void SetParent(Transform transform)
        {
            rectTransform.SetParent(transform);
            rectTransform.anchoredPosition = new Vector2(0, DaysSet.DayHeightPosition);
        }

        // Set position for circle without animation
        public void SetPosition(float position)
        {
            TargetPosition = position;
            rectTransform.anchoredPosition = new Vector2(position, rectTransform.anchoredPosition.y);
        }

        // Set position of circle with animation
        public void SetDynamicPosition(float position) => TargetPosition = position;

        // Update circle without animation
        public void UpdateByDefault()
        {
            // Reset state of animation component
            uiColorSync.SetDefaultMarker(0);
            uiColorSync.SetColor(0);
            uiColorSync.Stop();

            // Create new color for circle and change by characteristic id
            var newColor = ThemeLoader.GetCurrentTheme().SecondaryColorD4A50;
            
            if (DayStatus == -1)
                newColor = ThemeLoader.GetCurrentTheme().SecondaryColorD4;

            if (DayStatus >= 0)
                newColor = ColorMarkersDescriptor.GetColor(DayStatus);

            // Reset state of rect animation component 
            rectTransformSync.SetDefaultT(DayStatus >= 0 ? 0 : 1);
            rectTransformSync.SetT(DayStatus >= 0 ? 0 : 1);
            rectTransformSync.Stop();
            
            // Update circle color to new color
            image.color = newColor;
            // Update color animation component
            uiColorSync.AddElement(image, newColor);
        }

        // Set color of circle to color "hidden"
        public void ColorizeToHiddenColor()
        {
            // Reset color animation component
            uiColorSync.SetDefaultMarker(0);
            uiColorSync.SetColor(0);
            uiColorSync.Stop();
            
            // Set color "hidden" (background color)
            image.color = ThemeLoader.GetCurrentTheme().WorkAreaBackground;
            uiColorSync.AddElement(image, image.color);
            // Reset animation component
            rectTransformSync.SetDefaultT(1);
            rectTransformSync.SetT(1);
            rectTransformSync.Stop();
        }
        
        // Set "hidden" color to circle with color animation
        public void ColorizeToHiddenColorByDynamic()
        {
            // Save current color
            var color = image.color;
            
            // Reset state of color animation component
            uiColorSync.SetDefaultMarker(0);
            uiColorSync.SetColor(0);
            uiColorSync.Stop();
            
            // Update to animation component to "hidden" color
            uiColorSync.AddElement(image, ThemeLoader.GetCurrentTheme().WorkAreaBackground);
            // Update circle color to saved color
            image.color = color;
            
            // Start color animation
            uiColorSync.Speed = 0.2f;
            uiColorSync.SetTargetByDynamic(1);
            uiColorSync.Run();
            
            // Set order in hierarchy
            image.rectTransform.SetSiblingIndex(0);
            
            // Start rect animation
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();
        }

        // Start color animation from "hidden" color to target color
        public void ColorizeFromHiddenColor()
        {
            // Get color by status (-2 means that is future day)
            var color = DayStatus == -2
                ? ThemeLoader.GetCurrentTheme().SecondaryColorD4A50
                : ThemeLoader.GetCurrentTheme().SecondaryColorD4;
            
            // Setup animation component and start
            uiColorSync.AddElement(image, color);
            uiColorSync.Speed = 0.2f;
            uiColorSync.SetTargetByDynamic(1);
            uiColorSync.Run();
            
            // Start rect animation component
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();
        }
        
        // Update circle with animation
        public void UpdateByDynamic()
        {
            // Save color of circle
            var oldColor = image.color;
            
            // Reset color animation component
            uiColorSync.SetDefaultMarker(0);
            uiColorSync.SetColor(0);
            uiColorSync.Stop();

            // Create color for circle
            
            var newColor = ThemeLoader.GetCurrentTheme().SecondaryColorD4A50;
            
            if (DayStatus == -1)
                newColor = ThemeLoader.GetCurrentTheme().SecondaryColorD4;

            if (DayStatus >= 0)
                newColor = ColorMarkersDescriptor.GetColor(DayStatus);

            // Start rect animation component
            rectTransformSync.SetTByDynamic(DayStatus >= 0 ? 0 : 1);
            rectTransformSync.Run();
            
            // Colorize circle after reset color animation component
            image.color = oldColor;
            // Setup and start color animation component
            uiColorSync.AddElement(image, newColor);
            uiColorSync.Speed = 0.27f;
            uiColorSync.SetTargetByDynamic(1);
            uiColorSync.Run();
        }

        // Calls every frame
        public void Update()
        {
            JoinToTargetPosition();
        }

        // Move process of move circle to target position
        private void JoinToTargetPosition()
        {
            // Save old position
            var oldPosition = rectTransform.anchoredPosition;
            
            if (Mathf.Abs(TargetPosition - oldPosition.x) < 0.05f) 
                return;
            
            // Move smoothly to new position
            oldPosition.x = Mathf.Lerp(oldPosition.x, TargetPosition, joinSpeed);
            rectTransform.anchoredPosition = oldPosition;
        }
    }
}
