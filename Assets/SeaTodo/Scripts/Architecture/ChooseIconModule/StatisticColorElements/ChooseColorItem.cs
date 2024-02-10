using Architecture.Components;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Statistics;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Architecture.ChooseIconModule.StatisticColorElements
{
    // Circle with color as item in page
    public class ChooseColorItem
    {
        // Choose Icon session
        private ChooseIconSession ChooseIconSession => AreasLocator.Instance.ChooseIconModule?.ChooseIconSession;

        // Object with circle with color
        private readonly GameObject place;
        // Image of circle with color
        private readonly Image circle;

        // Button of item
        private readonly MainButtonJob mainButtonJob;
        // Animation component of alpha channel for circle image
        public readonly UIAlphaSync UIAlphaSync;
        // Animation component of rect for circle image
        public readonly RectTransformSync RectTransformSync;

        // Activity state of item
        public bool LocalActive;
        // Saved color id
        private int localColorId;
        
        // Create item
        public ChooseColorItem(GameObject place)
        {
            this.place = place;
            
            // Setup button component
            var handle = place.transform.Find("Handler").gameObject;
            mainButtonJob = new MainButtonJob(place.GetComponent<RectTransform>(), Touched, handle);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.ChooseIconModule);

            // Find circle image
            circle = place.transform.Find("Circle").GetComponent<Image>();

            // Create animation component of alpha channel
            UIAlphaSync = new UIAlphaSync();
            UIAlphaSync.AddElement(circle);
            UIAlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            UIAlphaSync.Speed = 0.2f;
            UIAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(UIAlphaSync, AppSyncAnchors.ChooseIconModule);
            
            // Create animation component of rect
            RectTransformSync = new RectTransformSync();
            RectTransformSync.SetRectTransformSync(circle.rectTransform);
            RectTransformSync.TargetScale = Vector2.one * 1.5f;
            RectTransformSync.Speed = 0.37f;
            RectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            RectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(RectTransformSync, AppSyncAnchors.ChooseIconModule);
        }

        // Setup scroll event to button component
        public void SetupScrollEvents(StatisticsScroll statisticsScroll)
        {
            mainButtonJob.HandleObject.AddEvent(EventTriggerType.PointerDown, statisticsScroll.TouchDown);
            mainButtonJob.HandleObject.AddEvent(EventTriggerType.PointerUp, statisticsScroll.TouchUp);
        }

        // Update item by new color
        public void UpdateView(int colorId)
        {
            if (ChooseIconSession == null)
                return;

            // Save color id
            localColorId = colorId;
            
            // Try enable item
            TryEnable();

            // Check if item selected
            LocalActive = ChooseIconSession.SelectedColorLocal == localColorId;
            
            // Update color of item
            circle.color = FlowColorLoader.LoadColorById(colorId);
            
            // Update animation components
            
            UIAlphaSync.SetAlpha(0.37f);
            UIAlphaSync.SetDefaultAlpha(0.37f);
            UIAlphaSync.Stop();
            
            RectTransformSync.Stop();
            RectTransformSync.SetT(1);
            RectTransformSync.SetDefaultT(1);
        }

        // Setup item to selected state
        public void CheckToSelective()
        {
            ChooseIconSession.UpdateSelectedColor(localColorId, this);
            UIAlphaSync.SetAlpha(1f);
            UIAlphaSync.SetDefaultAlpha(1f);
            UIAlphaSync.Stop();
            RectTransformSync.SetDefaultT(0);
            RectTransformSync.SetT(0);
            RectTransformSync.Stop();
        }

        // Try enable item object
        private void TryEnable()
        {
            if (!place.activeSelf) place.SetActive(true);
        }
        
        // Disable item object
        public void Disable() => place.SetActive(false);

        // Call when item touched
        private void Touched()
        {
            // Break if item selected before touch
            if (LocalActive)
                return;
            
            // Send info to session about item and play animation components
            
            ChooseIconSession.UpdateSelectedColor(localColorId, this);
            UIAlphaSync.Speed = 0.37f;
            UIAlphaSync.SetAlphaByDynamic(1);
            UIAlphaSync.Run();
            
            RectTransformSync.SetDefaultT(1);
            RectTransformSync.SetTByDynamic(0);
            RectTransformSync.Run();

            LocalActive = true;
        }
    }
}
