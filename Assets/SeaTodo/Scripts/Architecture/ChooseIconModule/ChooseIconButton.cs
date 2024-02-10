using Architecture.Components;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Pages;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace Architecture.ChooseIconModule
{
    // Class for button above app bar in create task page or edit task page
    public class ChooseIconButton
    {
        // View of icon setup
        private static ChooseIconModule ChooseIconModule => AreasLocator.Instance.ChooseIconModule;
        
        private readonly MainButtonJob mainButtonJob; // Button object
        private readonly RectTransform handler; // Rect of button
        private readonly Image handlerImage; // Image component of button
        private readonly SVGImage iconPlace; // Current icon in button
        
        // Animation components
        private readonly UIAlphaSync iconAlphaSync;
        private readonly RectTransformSync rectTransformSync;

        public int IconIdLocal { get; private set; } // Current icon id
        public int ColorIdLocal { get; private set; } // Current color id 
        public bool AutoScroll { private get; set; } // Activity of auto scroll when open view

        public ChooseIconButton(RectTransform icon)
        {
            // Find main components and create button component
            handler = icon.Find("Handler").GetComponent<RectTransform>();
            handlerImage = handler.GetComponent<Image>();
            mainButtonJob = new MainButtonJob(icon, TouchedButton, handler.gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour();

            // Create animation component for alpha channel of icon
            iconPlace = icon.Find("Icon").GetComponent<SVGImage>();
            iconAlphaSync = new UIAlphaSync();
            iconAlphaSync.AddElement(iconPlace);
            iconAlphaSync.Speed = 0.23f;
            iconAlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            iconAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(iconAlphaSync);
            
            // Create animation component of rect of icon
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(iconPlace.rectTransform);
            rectTransformSync.Speed = 0.33f;
            rectTransformSync.TargetScale = Vector3.one * 1.2f;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync);
        }
        
        // Setup activity of handler of button
        public void SetActiveHandlerButton(bool active) => handlerImage.enabled = active;

        // Update button with icon immediately
        public void UpdateImmediately(int icon, int color)
        {
            IconIdLocal = icon;
            ColorIdLocal = color;
            
            iconPlace.sprite = FlowIconLoader.LoadIconById(icon);
            var newColor = FlowColorLoader.LoadColorById(color);
            newColor.a = iconPlace.color.a;
            iconPlace.color = newColor;
        }

        // Calls when button touched
        private void TouchedButton()
        {
            var chooseIconSession = new ChooseIconSession(IconTracked) 
                { SelectedIconLocal = IconIdLocal, SelectedColorLocal = ColorIdLocal};
            ChooseIconModule.Open(GetWorldAnchoredPosition(), chooseIconSession, 
                autoScroll: (AutoScroll && IconIdLocal == 0 && ColorIdLocal == 0));
            PageScroll.Instance.Enabled = false;
            SetActiveHandlerButton(false);
        }

        // Calls when icon button should update (with new icon)
        private void IconTracked((int icon, int color) result)
        {
            SetActiveHandlerButton(true);

            if (result.icon != IconIdLocal || result.color != ColorIdLocal)
            {
                iconAlphaSync.SetAlpha(0.17f);
                iconAlphaSync.SetDefaultAlpha(0.17f);
                iconAlphaSync.SetAlphaByDynamic(1);
                iconAlphaSync.Run();
                
                rectTransformSync.SetT(0);
                rectTransformSync.SetDefaultT(0);
                rectTransformSync.SetTByDynamic(1);
                rectTransformSync.Run();
            }
            
            UpdateImmediately(result.icon, result.color);
        }
        
        // Get world position of button for place to open choose icon view
        private Vector2 GetWorldAnchoredPosition()
        {
            var parent = handler.transform.parent;

            handler.transform.SetParent(MainCanvas.RectTransform);
            var result = handler.anchoredPosition;
            handler.transform.SetParent(parent);

            return result;
        }
    }
}
