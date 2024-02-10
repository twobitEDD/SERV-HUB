using HomeTools.Source.Design;
using HTools;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TutorialArea.TutorialElements
{
    // Component for main pages of tutorial
    public class StepOnlyView : ITutorialItem
    {
        // Rect object of page
        private readonly RectTransform rectTransform;
        // Animation component of alpha channel of page
        private readonly UIAlphaSync uiAlphaSync;
        // Animation component of rect of page
        private readonly RectTransformSync contentSync;
        // Animation speed
        private const float animationSpeed = 0.1f;

        // Create and setup
        public StepOnlyView(RectTransform rectTransform)
        {
            // Save component
            this.rectTransform = rectTransform;

            // Setup text of page and colorize
            var text = rectTransform.Find("Text").GetComponent<Text>();
            var localTextMaterial = new Material(text.material);
            text.material = localTextMaterial;
            AppTheming.AddElement(text, ColorTheme.TutorialText, AppTheming.AppItem.Tutorial);
            AppTheming.ColorizeElement(text, ColorTheme.TutorialText);

            // Setup content UI of page and colorize
            var content = rectTransform.Find("Content").GetComponent<Image>();
            AppTheming.AddElement(content, ColorTheme.ImagesColor, AppTheming.AppItem.Tutorial);
            AppTheming.ColorizeElement(content, ColorTheme.ImagesColor);

            // Create animation component of alpha channel of page
            uiAlphaSync = new UIAlphaSync 
            {
                Speed = animationSpeed, 
                SpeedMode = HomeTools.Source.Design.UIAlphaSync.Mode.Lerp
            };
            uiAlphaSync.AddElement(content);
            uiAlphaSync.AddElement(rectTransform.Find("Star").GetComponent<SVGImage>());
            uiAlphaSync.AddElement(rectTransform.Find("Star Title").GetComponent<Text>());
            uiAlphaSync.AddElement(rectTransform.Find("Star Description").GetComponent<Text>());
            uiAlphaSync.AddMaterialElement(text.material);
            uiAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiAlphaSync, AppSyncAnchors.TutorialArea);
            
            // Create animation component of scale of UI content of page
            contentSync = new RectTransformSync()
            {
                SpeedMode = RectTransformSync.Mode.Lerp,
                Speed = animationSpeed * 1.87f
            };
            contentSync.SetRectTransformSync(content.rectTransform);
            content.rectTransform.localScale = Vector3.one * 0.57f;
            contentSync.TargetScale = Vector3.one;
            contentSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(contentSync, AppSyncAnchors.TutorialArea);
        }

        // Get rect of page
        public RectTransform RectTransform() => rectTransform;

        // Get animation component of alpha channel
        public UIAlphaSync UIAlphaSync() => uiAlphaSync;

        // Reset animation component of page to default state
        public void ResetToDefault()
        {
            contentSync.SetT(1);
            contentSync.SetDefaultT(1);
            
            contentSync.SetTByDynamic(0);
            contentSync.Run();
        }

        // Setup activity of page
        public void SetActive(bool active) => rectTransform.gameObject.SetActive(active);
    }
}
