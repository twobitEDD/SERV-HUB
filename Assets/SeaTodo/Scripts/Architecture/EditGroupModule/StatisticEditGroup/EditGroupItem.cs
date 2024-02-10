using Architecture.Components;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Statistics;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Architecture.EditGroupModule.StatisticEditGroup
{
    // Icon as item in page
    public class EditGroupItem
    {
        // Choose Icon session
        private EditGroupSession EditGroupSession => AreasLocator.Instance.UpdateTitleModule?.EditGroupSession;

        // Object with icon
        private readonly GameObject place;
        // Icon and circle components
        public readonly Image Circle;
        public readonly SVGImage Icon;
        
        // Button of item
        private readonly MainButtonJob mainButtonJob;
        // Animation component of alpha channel for icon
        public readonly UIColorSync UIColorSync;
        // Animation component of rect for icon
        public readonly RectTransformSync RectTransformSync;

        // Activity state of item
        public bool LocalActive;
        // Saved color id
        private int localIconId;

        // Create item
        public EditGroupItem(GameObject place)
        {
            this.place = place;
            
            // Find and colorize circle under icon
            Circle = place.transform.Find("Circle").GetComponent<Image>();
            AppTheming.AddElement(Circle, ColorTheme.SecondaryColor, AppTheming.AppItem.Other);
            
            // Setup button component
            var handle = place.transform.Find("Handler").gameObject;
            mainButtonJob = new MainButtonJob(Circle.rectTransform, Touched, handle);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.EditTitleModule);

            // Find icon and setup
            Icon = place.transform.Find("Icon").GetComponent<SVGImage>();
            Icon.color = ThemeLoader.GetCurrentTheme().ImagesColor;
            AppTheming.AddElement(Icon, ColorTheme.ImagesColor, AppTheming.AppItem.Other);
            
            // Create animation component of color of icon
            UIColorSync = new UIColorSync();
            UIColorSync.AddElement(Circle, ThemeLoader.GetCurrentTheme().ImagesColor);
            UIColorSync.AddElement(Icon, ThemeLoader.GetCurrentTheme().ImagesColor);
            UIColorSync.SpeedMode = UIColorSync.Mode.Lerp;
            UIColorSync.Speed = 0.2f;
            UIColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(UIColorSync, AppSyncAnchors.EditTitleModule);
            
            // Create animation component of rect
            RectTransformSync = new RectTransformSync();
            RectTransformSync.SetRectTransformSync(place.GetComponent<RectTransform>());
            RectTransformSync.TargetScale = Vector2.one * 1.1f;
            RectTransformSync.Speed = 0.37f;
            RectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            RectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(RectTransformSync, AppSyncAnchors.EditTitleModule);
        }

        // Setup scroll event to button component
        public void SetupScrollEvents(StatisticsScroll statisticsScroll)
        {
            mainButtonJob.HandleObject.AddEvent(EventTriggerType.PointerDown, statisticsScroll.TouchDown);
            mainButtonJob.HandleObject.AddEvent(EventTriggerType.PointerUp, statisticsScroll.TouchUp);
        }

        // Update item by new id
        public void UpdateView(int iconId)
        {
            if (EditGroupSession == null)
                return;

            // Save icon id
            localIconId = iconId;
            
            // Try enable item
            TryEnable();
            // Load new icon image
            Icon.sprite = GroupIconLoader.LoadIconById(localIconId);
            // Check if item selected
            LocalActive = EditGroupSession.SelectedIconLocal == localIconId;
            
            // Update animation components
            
            Circle.color = ThemeLoader.GetCurrentTheme().EditGroupIconCirclePassive;
            UIColorSync.AddElement(Circle, ThemeLoader.GetCurrentTheme().SecondaryColor);
            UIColorSync.SetColor(0);
            UIColorSync.SetDefaultMarker(0);
            UIColorSync.Stop();
            
            RectTransformSync.Stop();
            RectTransformSync.SetT(1);
            RectTransformSync.SetDefaultT(1);
        }

        // Setup item to selected state
        public void CheckToSelective()
        {
            EditGroupSession.UpdateSelectedIcon(localIconId, this);
            LocalActive = true;
            
            UIColorSync.Stop();
            UIColorSync.SetColor(0);
            UIColorSync.SetDefaultMarker(0);
            Circle.color = ThemeLoader.GetCurrentTheme().EditGroupIconCirclePassive;
            UIColorSync.AddElement(Circle, ThemeLoader.GetCurrentTheme().SecondaryColor);
            UIColorSync.SetColor(1);
            UIColorSync.SetDefaultMarker(1);
        }

        // Start color animation component to default selective state
        public void DeselectColor()
        {
            UIColorSync.Speed = 0.17f;
            UIColorSync.SetTargetByDynamic(0);
            UIColorSync.Run();
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
            
            EditGroupSession.UpdateSelectedIcon(localIconId, this);
            UIColorSync.Speed = 0.37f;
            UIColorSync.SetTargetByDynamic(1);
            UIColorSync.Run();
            
            RectTransformSync.SetDefaultT(1);
            RectTransformSync.SetTByDynamic(0);
            RectTransformSync.Run();
            
            LocalActive = true;
        }
    }
}
