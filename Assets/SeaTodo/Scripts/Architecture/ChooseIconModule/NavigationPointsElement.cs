using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ChooseIconModule
{
    // Class for navigation (points list)
    public class NavigationPointsElement
    {
        private readonly Graphic[] pointsGraphics; // Array of points
        // Array of animation component of alpha channel of this points
        private readonly UIColorSync[] points;
        // Array of animation component of rect of this points
        private readonly RectTransformSync[] pointsScale;
        private readonly int anchorsCount;

        // Colors for selected and passive states
        private ColorTheme colorSelected;
        private ColorTheme colorPassive;
        
        // Current active point in order
        private int currentAnchor;

        public NavigationPointsElement(params Graphic[] graphic)
        {
            // Create and save components
            points = new UIColorSync[graphic.Length];
            pointsScale = new RectTransformSync[graphic.Length];
            pointsGraphics = graphic;
            anchorsCount = pointsGraphics.Length;
        }

        // Setup main components before using navigation
        public void SetupComponents(int syncAnchor, ColorTheme colorSelected, ColorTheme colorPassive, float selectedSize = 1f)
        {
            // Save color items
            this.colorSelected = colorSelected;
            this.colorPassive = colorPassive;
            
            // For each point
            for (var i = 0; i < pointsGraphics.Length; i++)
            {
                // Create and setup animation component of alpha channel
                var newSync = new UIColorSync();
                newSync.AddElement(pointsGraphics[i], ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), colorSelected));
                newSync.Speed = 0.1f;
                newSync.SpeedMode = UIColorSync.Mode.Lerp;
                newSync.PrepareToWork();
                SyncWithBehaviour.Instance.AddObserver(newSync, syncAnchor);
                points[i] = newSync;

                // Create and setup animation component of rect
                var newScaleSync = new RectTransformSync();
                newScaleSync.SetRectTransformSync(pointsGraphics[i].rectTransform);
                newScaleSync.TargetScale = Vector3.one * selectedSize;
                newScaleSync.PrepareToWork();
                SyncWithBehaviour.Instance.AddObserver(newScaleSync, syncAnchor);
                pointsScale[i] = newScaleSync;
            }
        }

        // Stop update color process
        public void StopColor()
        {
            foreach (var point in points)
                point.Stop();
        }

        // Setup new activity number in navigation with visual effect
        public void SetAnchorDynamic(int anchor)
        {
            // Check mistakes
            if (anchor < 0)
                anchor = 0;
            
            // Check mistakes
            if (anchor >= anchorsCount)
                anchor = anchorsCount - 1;
            
            // Start animation of old point
            points[currentAnchor].SetTargetByDynamic(0);
            points[currentAnchor].Speed = 0.047f;
            points[currentAnchor].Run();
            pointsScale[currentAnchor].SetTByDynamic(1);
            pointsScale[currentAnchor].Speed = 0.047f;
            pointsScale[currentAnchor].Run();
            
            // Start animation of new point
            points[anchor].SetTargetByDynamic(1);
            points[anchor].Speed = 0.1f;
            points[anchor].Run();
            pointsScale[anchor].SetTByDynamic(0);
            pointsScale[anchor].Speed = 0.1f;
            pointsScale[anchor].Run();
            
            // Save id of new point
            currentAnchor = anchor;
        }
        
        // Setup new activity number in navigation
        public void SetAnchorImmediately(int anchor)
        {
            // Get colors
            var defaultColor = ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), colorPassive);
            var selectedColor = ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), colorSelected);

            // Prepare all points to default state
            for (var i = 0; i < pointsGraphics.Length; i++)
            {
                pointsGraphics[i].color = defaultColor;
                points[i].AddElement(pointsGraphics[i], selectedColor);
                points[i].Stop();
                points[i].SetDefaultMarker(0);
                points[i].SetColor(0);
                pointsScale[i].Stop();
                pointsScale[i].SetDefaultT(1);
                pointsScale[i].SetT(1);
            }

            // Check mistakes
            if (anchor < 0)
                anchor = 0;
            
            // Check mistakes
            if (anchor >= anchorsCount)
                anchor = anchorsCount - 1;
            
            // Setup active state to new point
            points[anchor].SetColor(1);
            points[anchor].SetDefaultMarker(1);
            pointsScale[anchor].SetT(0);
            pointsScale[anchor].SetDefaultT(0);

            // Save id of new point
            currentAnchor = anchor;
        }
    }
}
