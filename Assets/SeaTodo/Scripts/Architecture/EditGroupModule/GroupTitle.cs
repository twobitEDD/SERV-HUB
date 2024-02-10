using Architecture.Data;
using Architecture.Other;
using HomeTools.Source.Design;
using HTools;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.EditGroupModule
{
    // Component for control icon and title of app bar in main page 
    public class GroupTitle
    {
        private readonly Text name; // Title components
        public readonly SVGImage Icon; // Image component

        private string nameLocal; // Current title 
        private int iconLocal; // Current id of icon
        
        // Animation component of alpha channel for title
        private readonly UIAlphaSync uiAlphaSyncName;
        // Animation component of alpha channel for icon
        private readonly UIAlphaSync uiAlphaSyncIcon;
        // Animation component of rect for icon
        private readonly RectTransformSync rectTransformSyncIcon;
        
        // Create and setup
        public GroupTitle(Text name, SVGImage icon)
        {
            // Save main
            this.name = name;
            Icon = icon;
            
            // Add name to Color Theming module and colorize
            AppTheming.AddElement(name, ColorTheme.AppBarElements, AppTheming.AppItem.Other);
            AppTheming.ColorizeElement(name, ColorTheme.AppBarElements);
            
            // Create animation component of alpha channel for name
            uiAlphaSyncName = new UIAlphaSync();
            uiAlphaSyncName.AddElement(this.name);
            uiAlphaSyncName.Speed = 0.07f;
            uiAlphaSyncName.SpeedMode = UIAlphaSync.Mode.Lerp;
            uiAlphaSyncName.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiAlphaSyncName);
            
            // Create animation component of alpha channel for icon
            uiAlphaSyncIcon = new UIAlphaSync();
            uiAlphaSyncIcon.AddElement(icon);
            uiAlphaSyncIcon.Speed = 0.23f;
            uiAlphaSyncIcon.SpeedMode = UIAlphaSync.Mode.Lerp;
            uiAlphaSyncIcon.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiAlphaSyncIcon);
            
            // Create animation component of scale for icon
            rectTransformSyncIcon = new RectTransformSync();
            rectTransformSyncIcon.SetRectTransformSync(icon.rectTransform);
            rectTransformSyncIcon.Speed = 0.33f;
            rectTransformSyncIcon.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSyncIcon.TargetScale = Vector3.one * 1.2f;
            rectTransformSyncIcon.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSyncIcon);
        }

        // Update title and icon immediately
        public void UpdateImmediately()
        {
            nameLocal = AppData.GetCurrentGroup().Name;
            iconLocal = AppData.GetCurrentGroup().Icon;

            name.text = nameLocal;
            Icon.sprite = GroupIconLoader.LoadIconById(iconLocal);
        }
        
        // Update title and icon with animation
        public void UpdateByNewInfo()
        {
            // Get new title and icon id
            var newLocalText = AppData.GetCurrentGroup().Name;
            var newLocalIcon = AppData.GetCurrentGroup().Icon;

            // Check name and start animation of title
            if (newLocalText != nameLocal)
                RunUiAlpha(uiAlphaSyncName);

            // Check icon and start animation of icon
            if (newLocalIcon != iconLocal)
            {
                RunUiScale(rectTransformSyncIcon);
                RunUiAlpha(uiAlphaSyncIcon);
            }

            // Update title and icon immediately
            UpdateImmediately();
        }

        // Stop animation of title and icon alpha channel 
        public void StopUiAlpha()
        {
            uiAlphaSyncName.Stop();
            uiAlphaSyncIcon.Stop();
        }

        // Start animation of title
        private void RunUiAlpha(UIAlphaSync uiAlphaSync)
        {
            uiAlphaSync.SetAlpha(0.17f);
            uiAlphaSync.SetDefaultAlpha(0.17f);
            uiAlphaSync.SetAlphaByDynamic(1);
            uiAlphaSync.Run();
        }
        
        // Start animation of icon
        private void RunUiScale(RectTransformSync uiRectSync)
        {
            uiRectSync.SetT(0f);
            uiRectSync.SetDefaultT(0f);
            uiRectSync.SetTByDynamic(1);
            uiRectSync.Run();
        }
    }
}
