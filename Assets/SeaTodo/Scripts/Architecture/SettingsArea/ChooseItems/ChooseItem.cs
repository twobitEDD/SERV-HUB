using Architecture.Components;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea.ChooseItems
{
    // Item with text for chose items view
    public class ChooseItem
    {
        public int CurrentOrder; // Order in view
        public readonly RectTransform RectTransform; // Rect object of item
        private readonly MainButtonJob mainButtonJob; // Button component
        
        // UI components of item
        public readonly Text Name;
        public readonly SVGImage Circle; 
        public readonly SVGImage DoneIcon;
        
        // Rect object of icon
        private readonly RectTransform icon;
        // Animation components of item
        private readonly UIColorSync uiColorSync;
        private readonly RectTransformSync iconRectTransformSync;
        private readonly RectTransformSync nameRectTransformSync;
        
        // Link to panel view
        private readonly ChooseItemsPanel chooseItemsPanel;
        // Selected state flag
        private bool selected;
        // Distance of circle to begin of text side
        private const int distanceOfCircle = 47;

        // Create and setup
        public ChooseItem(RectTransform rectTransform, ChooseItemsPanel chooseItemsPanel)
        {
            // Save main components 
            RectTransform = rectTransform;
            this.chooseItemsPanel = chooseItemsPanel;

            // Find UI components
            icon = rectTransform.Find("Icon").GetComponent<RectTransform>();
            Name = rectTransform.Find("Name").GetComponent<Text>();
            Circle = icon.Find("Circle").GetComponent<SVGImage>();
            DoneIcon = icon.Find("Done").GetComponent<SVGImage>();
            
            // Create button components
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.ChooseItemsPanel);
            mainButtonJob.SimulateWaveScale = 1.1f;
            mainButtonJob.Reactivate();
            
            // Create animation component of color for icon
            uiColorSync = new UIColorSync {Speed = 0.27f, SpeedMode = UIColorSync.Mode.Lerp};
            uiColorSync.AddElement(Name, Color.white);
            uiColorSync.AddElement(Circle, Color.white);
            uiColorSync.AddElement(DoneIcon, Color.white);
            uiColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiColorSync, AppSyncAnchors.ChooseItemsPanel);
            
            // Create animation component for scale of item icon when select
            iconRectTransformSync = new RectTransformSync();
            iconRectTransformSync.SetRectTransformSync(icon);
            iconRectTransformSync.Speed = 0.25f;
            iconRectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            iconRectTransformSync.TargetScale = Vector3.one * 1.07f;
            iconRectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(iconRectTransformSync, AppSyncAnchors.ChooseItemsPanel);
            
            // Create animation component for scale of item when select
            nameRectTransformSync = new RectTransformSync();
            nameRectTransformSync.SetRectTransformSync(Name.rectTransform);
            nameRectTransformSync.Speed = 0.25f;
            nameRectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            nameRectTransformSync.TargetScale = Vector3.one * 1.027f;
            nameRectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(nameRectTransformSync, AppSyncAnchors.ChooseItemsPanel);
        }

        // Update name of item and setup item X position
        public void UpdateName(string name)
        {
            Name.text = name;
            
            icon.anchoredPosition = new Vector2(Name.preferredWidth / 2 + distanceOfCircle, 0);
            
            var anchoredPosition = RectTransform.anchoredPosition;
            anchoredPosition = new Vector2(anchoredPosition.x - (distanceOfCircle + Circle.rectTransform.sizeDelta.x) / 4, anchoredPosition.y);
            RectTransform.anchoredPosition = anchoredPosition;
        }

        // Update animation components before open view
        public void UpdateBeforeJob(bool active)
        {
            selected = active;
            
            Name.color = ThemeLoader.GetCurrentTheme().CreateFlowAreaDescription;
            uiColorSync.AddElement(Name, ThemeLoader.GetCurrentTheme().SecondaryColor);
            
            Circle.color = ThemeLoader.GetCurrentTheme().ChooseItemPanelCirclePassive;
            uiColorSync.AddElement(Circle, ThemeLoader.GetCurrentTheme().SecondaryColorD1);
            
            DoneIcon.color = ThemeLoader.GetCurrentTheme().ChooseItemPanelDonePassive;
            uiColorSync.AddElement(DoneIcon, ThemeLoader.GetCurrentTheme().ImagesColor);
            
            uiColorSync.PrepareToWork();
            uiColorSync.SetColor(selected ? 1 : 0);
            uiColorSync.SetDefaultMarker(selected ? 1 : 0);
            uiColorSync.Stop();
            
            iconRectTransformSync.SetT(selected ? 0 : 1);
            iconRectTransformSync.SetDefaultT(selected ? 0 : 1);
            iconRectTransformSync.Stop();
            nameRectTransformSync.SetT(selected ? 0 : 1);
            nameRectTransformSync.SetDefaultT(selected ? 0 : 1);
            nameRectTransformSync.Stop();
            
            mainButtonJob.Reactivate();
        }

        // Start animation components for deactivation state
        public void Deactivate()
        {
            selected = false;
            
            uiColorSync.SetTargetByDynamic(0);
            uiColorSync.Run();
            
            iconRectTransformSync.SetTByDynamic(1);
            iconRectTransformSync.Run();
            
            nameRectTransformSync.SetTByDynamic(1);
            nameRectTransformSync.Run();
        }

        // Prepare item to close view
        public void PrepareToClose()
        {
            uiColorSync.Stop();
        }

        // Touched item
        private void Touched()
        {
            // Reset state of button component 
            mainButtonJob.Reactivate();
            
            if (selected)
                return;
            
            // Try setup current item
            chooseItemsPanel.SetupCurrentItem(this);
            
            // Start animation components for selected state
            
            uiColorSync.SetTargetByDynamic(1);
            uiColorSync.Run();
            
            iconRectTransformSync.SetTByDynamic(0);
            iconRectTransformSync.Run();
            
            nameRectTransformSync.SetTByDynamic(0);
            nameRectTransformSync.Run();

            selected = true;
        }
    }
}
