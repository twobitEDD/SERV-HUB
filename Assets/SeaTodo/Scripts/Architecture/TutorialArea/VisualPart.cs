using System.Collections.Generic;
using HomeTools.Source.Design;
using HTools;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TutorialArea
{
    // Component for visual of UI for tutorial view
    public class VisualPart : IBehaviorSync
    {
        private const float animationSpeed = 0.27f;
        
        // Animation component of alpha channel of UI of other elements
        public UIAlphaSync UiSyncElements;
        // Animation component of alpha channel of skip button text
        public UIAlphaSync SkipAlpha;
        // State of tutorial
        public bool Opened;
        // Rect of view
        private readonly RectTransform rectTransform;
        // Animation component of alpha channel of UI
        private readonly UIAlphaSync uiSyncItem;
        // Object of tutorial pages
        private readonly GameObject graphic;
        // Flag tutorial pages is active
        private bool checkGraphicActive;
        
        // Create view
        public VisualPart(RectTransform rectTransform)
        {
            // Save main components
            this.rectTransform = rectTransform;
            graphic = rectTransform.Find("Tutorial Graphic").gameObject;
            
            // Create animation component of alpha channel for view
            
            var points = new List<Graphic>();
            for (var i = 0; i < 4; i++)
                points.Add(rectTransform.Find($"NavItem {i + 1}").GetComponent<Graphic>());
            
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.Find("Background").GetComponent<Image>());
            uiSyncItem.AddElement(rectTransform.Find("Skip").GetComponent<Text>());
            uiSyncItem.AddElement(rectTransform.Find("Icon").GetComponent<SVGImage>());
            uiSyncItem.AddElement(rectTransform.Find("Name").GetComponent<Text>());
            uiSyncItem.AddElements(points);
            uiSyncItem.Speed = animationSpeed;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            uiSyncItem.PrepareToWork();
            uiSyncItem.DisableWithRays = true;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem, AppSyncAnchors.TutorialArea);
        }

        // Initialize dark screen and animation components
        public void Initialize()
        {
            uiSyncItem.PrepareToWork();
            uiSyncItem.SetDefaultAlpha(0);
            uiSyncItem.SetAlpha(0);
            uiSyncItem.EnableByAlpha(0);
        }

        // The method which is responsible for open view without animation
        public void OpenImmediately()
        {
            uiSyncItem.SetDefaultAlpha(1);
            uiSyncItem.SetAlpha(1);
            uiSyncItem.EnableByAlpha(1);
            graphic.SetActive(true);

            OpenOutAlphaImmediately(UiSyncElements);
            OpenOutAlphaImmediately(SkipAlpha);

            Opened = true;
        }

        // Activate animation for other alpha components without animation
        private void OpenOutAlphaImmediately(UIAlphaSync uiAlphaSync)
        {
            if (uiAlphaSync == null)
                return;

            uiAlphaSync.Speed = animationSpeed;
            uiAlphaSync.SetAlpha(1);
            uiAlphaSync.SetDefaultAlpha(1);
            uiAlphaSync.Stop();
        }

        // The method which is responsible for open view
        public void Open()
        {
            uiSyncItem.EnableByAlpha(1);
            uiSyncItem.SetAlphaByDynamic(1);
            uiSyncItem.Run();
            graphic.SetActive(true);

            OpenOutAlpha(UiSyncElements);
            OpenOutAlpha(SkipAlpha);
            
            Opened = true;
        }

        // Activate animation for other alpha components
        private void OpenOutAlpha(UIAlphaSync uiAlphaSync)
        {
            if (uiAlphaSync == null)
                return;

            uiAlphaSync.Speed = animationSpeed;
            uiAlphaSync.SetAlpha(0);
            uiAlphaSync.SetDefaultAlpha(0);
            uiAlphaSync.SetAlphaByDynamic(1);
            uiAlphaSync.Run();
        }

        // Start animation components for close animation
        public void Close()
        {
            uiSyncItem.SetAlphaByDynamic(0);
            uiSyncItem.Run();
            uiSyncItem.UpdateEnableByAlpha = true;
            
            checkGraphicActive = true;

            CloseOutAlpha(UiSyncElements);
            CloseOutAlpha(SkipAlpha);
            
            Opened = false;
        }

        // Start animation components for close without animation
        public void CloseImmediately()
        {
            uiSyncItem.SetAlpha(0);
            uiSyncItem.SetDefaultAlpha(0);
            uiSyncItem.Stop();
            graphic.SetActive(false);
        }

        // Activate animation for other alpha components
        private void CloseOutAlpha(UIAlphaSync uiAlphaSync)
        {
            if (uiAlphaSync == null)
                return;

            uiAlphaSync.Speed = animationSpeed * 1.27f;
            uiAlphaSync.SetAlphaByDynamic(0);
            uiAlphaSync.Run();
        }

        public void Start() { }

        public void Update() => CheckGraphicActive();

        // Check deactivate tutorial pages by tutorial state
        private void CheckGraphicActive()
        {
            if (!checkGraphicActive)
                return;
            
            if (uiSyncItem.LocalAlpha > 0.01f)
                return;
            
            graphic.SetActive(false);
            checkGraphicActive = false;
        }
    }
}
