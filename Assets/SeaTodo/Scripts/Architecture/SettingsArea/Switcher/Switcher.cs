using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;

namespace Architecture.SettingsArea.Switcher
{
    // Component for switch operation (UI)
    public class Switcher
    {
        public bool On; // State of switcher
        // Rect object of switcher
        private readonly RectTransform rectTransform;
        // Rect object of switch circle
        private readonly RectTransform switcherRect;
        // Animation components for switcher
        private readonly RectTransformSync rectTransformSync;
        // Animation component for color of switcher
        private readonly UIColorSync uiColorSync;

        // UI of switcher
        private readonly SVGImage switcher;
        private readonly SVGImage plane;

        // Colors for switcher states
        private readonly ColorTheme planeOff;
        private readonly ColorTheme planeOn;
        private readonly ColorTheme switcherOff;
        private readonly ColorTheme switcherOn;

        // Animation speed
        private const float switchSpeed = 0.23f;

        // Create and setup
        public Switcher(RectTransform rectTransform, 
            ColorTheme planeOff, ColorTheme planeOn, 
            ColorTheme switcherOff, ColorTheme switcherOn)
        {
            // Save components
            this.rectTransform = rectTransform;
            this.planeOff = planeOff;
            this.planeOn = planeOn;
            this.switcherOff = switcherOff;
            this.switcherOn = switcherOn;
            
            // Setup default position of switch circle
            switcherRect = rectTransform.Find("Switch").GetComponent<RectTransform>();
            var onPosition = Mathf.Abs(switcherRect.anchoredPosition.x);
            switcherRect.anchoredPosition = new Vector2(-onPosition, switcherRect.anchoredPosition.y);
            
            // Create animation component for switch circle
            rectTransformSync = new RectTransformSync 
            {
                Speed = switchSpeed,
                SpeedMode = RectTransformSync.Mode.Lerp
            };
            rectTransformSync.SetRectTransformSync(switcherRect);
            rectTransformSync.TargetPosition = new Vector2(onPosition, switcherRect.anchoredPosition.y);
            rectTransformSync.PrepareToWork();

            // Find UI of switcher and create color animation component
            switcher = rectTransform.Find("Switch").GetComponent<SVGImage>();
            plane = rectTransform.Find("Plane").GetComponent<SVGImage>();
            uiColorSync = new UIColorSync
            {
                Speed = switchSpeed, 
                SpeedMode = UIColorSync.Mode.Lerp
            };
            uiColorSync.AddElement(plane, ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), planeOn));
            uiColorSync.AddElement(switcher, ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), switcherOn));
            uiColorSync.PrepareToWork();
        }

        // Attach this component to updates module
        public void AttachToAnchorSync(int anchor)
        {
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, anchor);
            SyncWithBehaviour.Instance.AddObserver(uiColorSync, anchor);
        }

        // Update colors of switcher
        public void UpdateColors()
        {
            plane.color = ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), planeOff);
            switcher.color = ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), switcherOff);
            uiColorSync.AddElement(plane, ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), planeOn));
            uiColorSync.AddElement(switcher, ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), switcherOn));
            uiColorSync.PrepareToWork();
        }

        // Update state of switcher immediately
        public void UpdateStateImmediately()
        {
            rectTransformSync.SetT(On ? 0 : 1);
            rectTransformSync.SetDefaultT(On ? 0 : 1);
            rectTransformSync.Stop();
            
            uiColorSync.SetColor(On ? 1 : 0);
            uiColorSync.SetDefaultMarker(On ? 1 : 0);
            uiColorSync.Stop();
        }
        
        // Update state of switcher with animation
        public void UpdateState()
        {
            rectTransformSync.SetTByDynamic(On ? 0 : 1);
            rectTransformSync.Run();
            
            uiColorSync.SetTargetByDynamic(On ? 1 : 0);
            uiColorSync.Run();
        }

        // Set activity of switch object
        public void SetActive(bool active) => rectTransform.gameObject.SetActive(active);
    }
}
