using Architecture.Components;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Statistics;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using Theming;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace Architecture.ChooseIconModule.StatisticIconElements
{
    // Icon as item in page
    public class ChooseIconItem
    {
        // Choose Icon session
        private ChooseIconSession ChooseIconSession => AreasLocator.Instance.ChooseIconModule?.ChooseIconSession;

        // Object with icon
        private readonly GameObject place;
        // Image of circle of icon
        public readonly Image Circle;
        // Image of icon
        public readonly SVGImage Icon;
        
        // Button of item
        private readonly MainButtonJob mainButtonJob;
        // Animation component of color for circle image
        public readonly UIColorSync UIColorSync;
        // Animation component of rect for place item
        public readonly RectTransformSync RectTransformSync;

        // Activity state of item
        public bool LocalActive;
        // Saved icon id
        private int localIconId;
        // Saved color id
        private int localColorId;
        
        // Create item
        public ChooseIconItem(GameObject place)
        {
            this.place = place;
            
            // Find circle of place
            Circle = place.transform.Find("Circle").GetComponent<Image>();
            
            // Setup button component
            var handle = place.transform.Find("Handler").gameObject;
            mainButtonJob = new MainButtonJob(Circle.rectTransform, Touched, handle);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.ChooseIconModule);

            // Find icon object and prepare color by default
            Icon = place.transform.Find("Icon").GetComponent<SVGImage>();
            Icon.color = ThemeLoader.GetCurrentTheme().ImagesColor;
            AppTheming.AddElement(Icon, ColorTheme.ImagesColor, AppTheming.AppItem.Other);
            
            // Create animation component of color
            UIColorSync = new UIColorSync();
            UIColorSync.AddElement(Circle, ThemeLoader.GetCurrentTheme().ImagesColor);
            UIColorSync.SpeedMode = UIColorSync.Mode.Lerp;
            UIColorSync.Speed = 0.2f;
            UIColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(UIColorSync, AppSyncAnchors.ChooseIconModule);
            
            // Create animation component of rect
            RectTransformSync = new RectTransformSync();
            RectTransformSync.SetRectTransformSync(place.GetComponent<RectTransform>());
            RectTransformSync.TargetScale = Vector2.one * 1.1f;
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

        // Update item by new icon
        public void UpdateView(int iconId)
        {
            if (ChooseIconSession == null)
                return;

            // Save icon id
            localIconId = iconId;
            
            // Try enable item
            TryEnable();
            
            // Setup icon sprite and color
            Icon.sprite = FlowIconLoader.LoadIconById(localIconId);
            Icon.color = ThemeLoader.GetCurrentTheme().ImagesColor;
            
            // Check if item selected
            LocalActive = ChooseIconSession.SelectedIconLocal == localIconId;
            
            // Get color id for icon
            localColorId = ChooseIconSession.SelectedColorLocal;
            
            // Update animation components
            
            Circle.color = ThemeLoader.GetCurrentTheme().EditGroupIconCirclePassive;
            UIColorSync.AddElement(Circle, FlowColorLoader.LoadColorById(localColorId));
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
            ChooseIconSession.UpdateSelectedIcon(localIconId, this);
            LocalActive = true;
            
            UIColorSync.Stop();
            UIColorSync.SetColor(0);
            UIColorSync.SetDefaultMarker(0);
            Circle.color = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            UIColorSync.AddElement(Circle, FlowColorLoader.LoadColorById(localColorId));
            UIColorSync.SetColor(1);
            UIColorSync.SetDefaultMarker(1);
        }

        // Update color of icon
        public void UpdateColor(int colorId)
        {
            if (!LocalActive)
                return;
            
            // Update color id
            localColorId = colorId;

            // Start animation process for change color
            
            var color = Circle.color;
            UIColorSync.SetColor(0);
            UIColorSync.SetDefaultMarker(0);
            Circle.color = color;
            UIColorSync.AddElement(Circle, FlowColorLoader.LoadColorById(colorId));
            
            UIColorSync.SetTargetByDynamic(1);
            UIColorSync.Run();
        }
        
        // Start animation process to return color to default
        public void DeselectColor()
        {
            var color = Circle.color;
            Circle.color = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            
            UIColorSync.AddElement(Circle, color);
            UIColorSync.Speed = 0.17f;
            UIColorSync.SetTargetByDynamic(0);
            UIColorSync.Run();
        }
        
        // Update color in animation component
        private void UpdateColor()
        {
            var colorId = ChooseIconSession?.SelectedColorLocal ?? localColorId;
            var color = FlowColorLoader.LoadColorById(colorId);
            Circle.color = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            
            UIColorSync.AddElement(Circle, color);
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
            
            ChooseIconSession.UpdateSelectedIcon(localIconId, this);
            UpdateColor();
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
