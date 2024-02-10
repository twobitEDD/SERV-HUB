using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.AboutSeaCalendar
{
    // Responsible for visual behaviour of UI
    public class VisualPart : IBehaviorSync
    {
        private const float animationSpeed = 0.27f;
        
        private readonly RectTransform rectTransform;
        private readonly UIAlphaSync uiSyncItem;
        private readonly RectTransformSync contentSync;
        private readonly RectTransformSync titleSync;
        private readonly RectTransform contentRead;
        
        public VisualPart(RectTransform rectTransform)
        {
            // Save base of rect
            this.rectTransform = rectTransform;

            // update material of text
            var text = rectTransform.Find("Scroll View/Viewport/Content/Text").GetComponent<Text>();
            var localTextMaterial = new Material(text.material);
            text.material = localTextMaterial;

            // Create UI Sync Item for control alpha channel of UI elements
            var content = rectTransform.Find("Content").GetComponent<Image>();
            var title = rectTransform.Find("Title").GetComponent<Text>();
            contentRead = rectTransform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            uiSyncItem = new UIAlphaSync();
            uiSyncItem.AddElement(rectTransform.Find("Background").GetComponent<Image>());
            uiSyncItem.AddElement(content);
            uiSyncItem.AddElement(title);
            uiSyncItem.AddElement(rectTransform.Find("Close").GetComponent<Text>());
            uiSyncItem.AddElement(contentRead.Find("Text").GetComponent<Text>());
            uiSyncItem.AddElement(rectTransform.Find("Scroll View/Scrollbar Vertical").GetComponent<Image>());
            uiSyncItem.AddElement(rectTransform.Find("Scroll View/Scrollbar Vertical/Sliding Area/Handle").GetComponent<Image>());
            uiSyncItem.AddMaterialElement(localTextMaterial);
            uiSyncItem.Speed = animationSpeed;
            uiSyncItem.SpeedMode = UIAlphaSync.Mode.Lerp;
            uiSyncItem.PrepareToWork();
            uiSyncItem.DisableWithRays = true;
            SyncWithBehaviour.Instance.AddObserver(uiSyncItem, AppSyncAnchors.AboutSeaCalendarArea);

            // Create component for animation of rects of content object
            contentSync = new RectTransformSync()
            {
                SpeedMode = RectTransformSync.Mode.Lerp,
                Speed = animationSpeed * 0.87f
            };
            contentSync.SetRectTransformSync(content.rectTransform);
            content.rectTransform.localScale = Vector3.one * 0.57f;
            contentSync.TargetScale = Vector3.one;
            contentSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(contentSync, AppSyncAnchors.AboutSeaCalendarArea);
            
            // Create component for animation of rects of title
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
            SyncWithBehaviour.Instance.AddObserver(titleSync, AppSyncAnchors.AboutSeaCalendarArea);
        }

        // Setup animation elements to default state
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
        
        // Play animation elements for open animation
        public void Open()
        {
            uiSyncItem.EnableByAlpha(1);
            uiSyncItem.SetAlphaByDynamic(1);
            uiSyncItem.Run();
            
            contentSync.SetTByDynamic(0);
            contentSync.Run();
            
            titleSync.SetTByDynamic(0);
            titleSync.Run();
            
            contentRead.anchoredPosition = new Vector2(0, -300);
        }

        // Play animation elements for close animation
        public void Close()
        {
            uiSyncItem.SetAlphaByDynamic(0);
            uiSyncItem.Run();
            uiSyncItem.UpdateEnableByAlpha = true;
            
            contentSync.SetTByDynamic(1);
            contentSync.Run();
            
            titleSync.SetTByDynamic(1);
            titleSync.Run();
        }

        // Play animation elements for close animation in one update call
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
        public void Update() { }
    }
}
