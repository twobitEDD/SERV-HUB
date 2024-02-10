using Architecture.Components;
using Architecture.Elements;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TutorialArea.TutorialElements
{
    // Last page of tutorial
    public class StepStart : ITutorialItem
    {
        // Link to tutorial object
        private Tutorial Tutorial => AreasLocator.Instance.Tutorial;
        
        // Rect of page
        private readonly RectTransform rectTransform;
        // Animation controller of page
        private readonly UIAlphaSync uiAlphaSync;
        // Button component
        private readonly MainButtonJob mainButtonJob;

        // Create and step
        public StepStart(RectTransform rectTransform)
        {
            // Save component
            this.rectTransform = rectTransform;

            // Find text component of page
            var text = rectTransform.Find("Text").GetComponent<Text>();
            var localTextMaterial = new Material(text.material);
            text.material = localTextMaterial;
            
            // Create animation component for page
            uiAlphaSync = new UIAlphaSync 
            {
                Speed = 0.1f, 
                SpeedMode = HomeTools.Source.Design.UIAlphaSync.Mode.Lerp
            };
            uiAlphaSync.AddElement(rectTransform.Find("Content").GetComponent<Image>());
            uiAlphaSync.AddMaterialElement(text.material);
            uiAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiAlphaSync, AppSyncAnchors.TutorialArea);
            
            // Create button component
            mainButtonJob = new MainButtonJob(text.rectTransform, Touched, text.gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.TutorialArea);
        }

        // Get rect of page
        public RectTransform RectTransform() => rectTransform;

        // Get animation component of alpha channel
        public UIAlphaSync UIAlphaSync() => uiAlphaSync;

        public void ResetToDefault()
        {
        }

        // Touched page
        private void Touched()
        {
            mainButtonJob.Reactivate();
            Tutorial.CloseTouched();
        }

        // Setup activity of page
        public void SetActive(bool active) => rectTransform.gameObject.SetActive(active);
    }
}
