using Architecture.Components;
using Architecture.DaysMarkerArea.DaysColors.UpdateModule;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Other;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using Theming;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace Architecture.DaysMarkerArea.DaysColors
{
    // Item of day characteristic in view
    public class PaletteItem
    {
        // Rect of day characteristic
        private readonly RectTransform rectTransform;
        // Name of item
        private readonly Text name;
        // Count of days that marked with this characteristic
        private readonly Text daysCount;
        // Circle with color of characteristic
        private readonly SVGImage circle;
        // Button component
        private readonly MainButtonJob mainButtonJob;
        // Order number in list
        private readonly int queue;
        // Link to color descriptor component
        private readonly ColorMarkersDescriptor colorMarkersDescriptor;
        // Link to update characteristic name module
        private readonly UpdateMarkerModule updateMarkerModule;
        // Animation component of color
        private readonly UIColorSync uiColorSync;
        
        // Create item
        public PaletteItem(RectTransform rectTransform, ColorMarkersDescriptor colorMarkersDescriptor, 
                                                        UpdateMarkerModule updateMarkerModule, int queue)
        {
            // Save params
            this.queue = queue;
            this.updateMarkerModule = updateMarkerModule;
            this.colorMarkersDescriptor = colorMarkersDescriptor;
            this.rectTransform = rectTransform;
            // Save UI to components
            name = rectTransform.Find("Name").GetComponent<Text>();
            daysCount = rectTransform.Find("Days Count").GetComponent<Text>();
            circle = rectTransform.Find("Color").GetComponent<SVGImage>();

            // Create button component
            var handle = rectTransform.Find("Handle").gameObject;
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handle)
            {
                SimulateWaveScale = 1.03f
            };
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.MarkersAreaColors);
            AddScrollEventsCustom.AddEventActions(handle);
            
            // Add name of part to Color Theming module
            AppTheming.AddElement(name, ColorTheme.DaysMarkerAreaDaysColor, AppTheming.AppItem.DaysMarkerArea);
            
            // Create animation component of color for item
            uiColorSync = new UIColorSync();
            uiColorSync.AddElement(circle, ThemeLoader.GetCurrentTheme().SecondaryColor);
            uiColorSync.AddElement(daysCount, ThemeLoader.GetCurrentTheme().SecondaryColor);
            uiColorSync.Speed = 0.17f;
            uiColorSync.SpeedMode = UIColorSync.Mode.Lerp;
            uiColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiColorSync, AppSyncAnchors.MarkersAreaColors);
        }

        // Update item immediately
        public void UpdateImmediately()
        {
            // Stop animation component
            uiColorSync.Stop();
            uiColorSync.SetDefaultMarker(0);
            
            // Update colors
            daysCount.color = ColorMarkersDescriptor.GetColor(queue);
            circle.color = ColorMarkersDescriptor.GetColor(queue);
            
            // Update characteristic name by data
            var nameOfItem = ColorMarkersDescriptor.GetColorName(queue);
            UpdateTextView(nameOfItem);
        }

        // The method that is called when the object is clicked
        private void Touched()
        {
            // Reset button component after touch
            mainButtonJob.Reactivate();
            // Create session for rename of item
            var session = new UpdateMarkerSession(queue, MarkerUpdated);
            // Block scroll
            PageScroll.Instance.Enabled = false;
            // Get start position of rename view when animation
            var startPosition = OtherHTools.GetWorldAnchoredPosition(rectTransform);
            // Open view for item rename 
            updateMarkerModule.Open(startPosition, session);
        }

        // Update item when rename view closed
        private void MarkerUpdated()
        {
            // Update text of item
            var colorName = ColorMarkersDescriptor.GetColorName(queue);
            UpdateTextView(colorName);
            
            // Start animation component for colors update
            uiColorSync.SetDefaultMarker(0);
            var color = ColorMarkersDescriptor.GetColor(queue);
            uiColorSync.AddElement(circle, color);
            uiColorSync.AddElement(daysCount, color);
            uiColorSync.SetTargetByDynamic(1);
            uiColorSync.Run();
        }

        // Update text of item
        private void UpdateTextView(string colorName)
        {
            // Update name
            name.text = colorName;
            // Get days with this characteristic item
            var daysCountInt = colorMarkersDescriptor.GetDaysCountByMarkerId(queue);
            // Get text for days count
            var keyDay = daysCountInt == 1 ? TextKeysHolder.Day : TextKeysHolder.Days;
            var localizedText = TextLocalization.GetLocalization(keyDay);
            // Update days count text
            daysCount.text = $"{daysCountInt} {localizedText}";
        }
    }
}
