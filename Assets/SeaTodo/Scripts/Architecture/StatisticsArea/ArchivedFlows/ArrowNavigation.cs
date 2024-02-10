using System;
using Architecture.Components;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using Unity.VectorGraphics;
using UnityEngine;

namespace Architecture.StatisticsArea.ArchivedFlows
{
    // Component for arrows that for switch tasks
    public class ArrowNavigation
    {
        // Animation component of color of arrow image
        private readonly UIColorSync colorSync;
        // UI of image
        private readonly SVGImage image;
        // Button component of arrow
        private readonly MainButtonJob mainButtonJob;
        // Invoke action when touched arrow
        private readonly Action touchedAction;
        // State of arrow
        private bool active; 
        
        // Create and setup
        public ArrowNavigation(RectTransform rectTransform, Action touchedAction)
        {
            // Save action
            this.touchedAction = touchedAction;
            
            // Setup UI and create animation component of arrow color
            image = rectTransform.GetComponent<SVGImage>();
            colorSync = new UIColorSync();
            image.color = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            colorSync.AddElement(image, ThemeLoader.GetCurrentTheme().SecondaryColorD1);
            colorSync.Speed = 0.1f;
            colorSync.SpeedMode = UIColorSync.Mode.Lerp;
            colorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(colorSync);
            
            // Create button component
            mainButtonJob = new MainButtonJob(rectTransform, Touched, rectTransform.Find("Handler").gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour();
        }

        // Update color of arrow
        public void UpdateColors()
        {
            image.color = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            colorSync.AddElement(image, ThemeLoader.GetCurrentTheme().SecondaryColorD1);
        }

        // Update activity status of arrow
        public void UpdateActivityStatus(bool activeState) => active = activeState;

        // Update arrow without animation by activity
        public void UpdateActivityImmediately()
        {
            colorSync.Stop();
            colorSync.SetColor(active ? 1 : 0);
            colorSync.SetDefaultMarker(active ? 1 : 0);
            
            UpdateTouchable();
        }
        
        // Update arrow with animation by activity
        public void UpdateActivity()
        {
            colorSync.SetTargetByDynamic(active ? 1 : 0);
            colorSync.Run();
            
            UpdateTouchable();
        }

        // Update state of button component
        private void UpdateTouchable()
        {
            if (active)
                mainButtonJob.Reactivate();
            else
                mainButtonJob.Deactivate();
        }

        // Invoke when arrow touched
        private void Touched()
        {
            mainButtonJob.Reactivate();
            touchedAction.Invoke();
        }
    }
}
