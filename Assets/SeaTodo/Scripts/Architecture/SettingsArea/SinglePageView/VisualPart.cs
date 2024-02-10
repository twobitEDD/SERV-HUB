using System.Collections.Generic;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea.SinglePageView
{
    // Component for visual of UI for page view
    public class VisualPart : IBehaviorSync
    {
        private const float animationSpeed = 0.27f;
        
        // Animation component of alpha channel of UI
        private readonly UIAlphaSync uiSyncItem;
        // Animation components of elements when open view
        private readonly RectTransformSync contentSync;
        private readonly RectTransformSync titleSync;

        private bool checkGraphicActive; // Setup content to scene resources flag
        // Animation components for alpha channel for elements of content
        public List<UIAlphaSync> ContentAlphaSync; 
        public RectTransform ContentView; // Rect object of content
        
        // Create and setup
        public VisualPart(RectTransform rectTransform)
        {
            // Find UI components
            var content = rectTransform.Find("Icon").GetComponent<SVGImage>();
            var title = rectTransform.Find("Title").GetComponent<Text>();

            // Create animation component of alpha channel for view
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.Find("Background").GetComponent<Image>());
            uiSyncItem.AddElement(content);
            uiSyncItem.AddElement(title);
            uiSyncItem.AddElement(rectTransform.Find("Close").GetComponent<Text>());
            uiSyncItem.AddElement(rectTransform.Find("Scroll View/Scrollbar Vertical").GetComponent<Image>());
            uiSyncItem.AddElement(rectTransform.Find("Scroll View/Scrollbar Vertical/Sliding Area/Handle").GetComponent<Image>());
            uiSyncItem.Speed = animationSpeed;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            uiSyncItem.PrepareToWork();
            uiSyncItem.DisableWithRays = true;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem, AppSyncAnchors.SinglePageView);

            // Create animation component of rect of content
            contentSync = new RectTransformSync()
            {
                SpeedMode = RectTransformSync.Mode.Lerp,
                Speed = animationSpeed * 0.87f
            };
            contentSync.SetRectTransformSync(content.rectTransform);
            content.rectTransform.localScale = Vector3.one * 0.57f;
            contentSync.TargetScale = Vector3.one;
            contentSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(contentSync, AppSyncAnchors.SinglePageView);
            
            // Create animation component of title of page
            titleSync = new RectTransformSync()
            {
                SpeedMode = RectTransformSync.Mode.Lerp,
                Speed = animationSpeed * 0.87f
            };
            titleSync.SetRectTransformSync(title.rectTransform);
            titleSync.TargetPosition = title.rectTransform.anchoredPosition;
            titleSync.TargetScale = Vector3.one;
            title.rectTransform.anchoredPosition += new Vector2(0, 27);
            title.rectTransform.localScale = Vector3.one * 0.95f;
            titleSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(titleSync, AppSyncAnchors.SinglePageView);
        }

        // Initialize dark screen and animation components
        public void Initialize()
        {
            uiSyncItem.PrepareToWork();
            uiSyncItem.SetDefaultAlpha(0);
            uiSyncItem.SetAlpha(0);
            uiSyncItem.EnableByAlpha(0);
            
            contentSync.SetT(1);
            contentSync.SetDefaultT(1);
            
            titleSync.SetT(1);
            titleSync.SetDefaultT(1);
        }
        
        // The method which is responsible for open view
        public void Open()
        {
            // Start play animation components
            
            uiSyncItem.EnableByAlpha(1);
            uiSyncItem.SetAlphaByDynamic(1);
            uiSyncItem.Run();
            
            contentSync.SetTByDynamic(0);
            contentSync.Run();
            
            titleSync.SetTByDynamic(0);
            titleSync.Run();

            if (ContentAlphaSync == null)
                return;
            
            foreach (var uiAlphaSync in ContentAlphaSync)
            {
                uiAlphaSync.Speed = animationSpeed;
                uiAlphaSync.SetAlpha(0);
                uiAlphaSync.SetDefaultAlpha(0);
                uiAlphaSync.EnableByAlpha(1);
                uiAlphaSync.SetAlphaByDynamic(1);
                uiAlphaSync.Run();
            }
        }

        // Start animation components for close animation
        public void Close()
        {
            // Start play animation components
            
            uiSyncItem.SetAlphaByDynamic(0);
            uiSyncItem.Run();
            uiSyncItem.UpdateEnableByAlpha = true;
            
            contentSync.SetTByDynamic(1);
            contentSync.Run();
            
            titleSync.SetTByDynamic(1);
            titleSync.Run();
            
            checkGraphicActive = true;

            if (ContentAlphaSync == null)
                return;
            
            foreach (var uiAlphaSync in ContentAlphaSync)
            {
                uiAlphaSync.SetAlphaByDynamic(0);
                uiAlphaSync.Run();
                uiAlphaSync.UpdateEnableByAlpha = true;
            }
        }

        // Start animation components for close without animation
        public void CloseImmediately()
        {
            uiSyncItem.SetAlpha(0);
            uiSyncItem.SetDefaultAlpha(0);
            uiSyncItem.Stop();
            
            contentSync.SetTByDynamic(1);
            contentSync.Run();
            
            titleSync.SetTByDynamic(1);
            titleSync.Run();
        }

        public void Start() { }
        public void Update() => CheckGraphicActive();

        // Checking for placing content in scene resources when closing a window
        private void CheckGraphicActive()
        {
            if (!checkGraphicActive)
                return;
            
            if (uiSyncItem.LocalAlpha > 0.01f)
                return;
            
            if (ContentView != null)
                SceneResources.Set(ContentView.gameObject);
            
            checkGraphicActive = false;
        }
    }
}
