using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.CreateTaskArea.ChooseTypePart
{
    // Animation part for task type item
    public class AnimationPart
    {
        private const float animationSpeed = 0.1f; // Animation speed

        // Animation component of rect
        private readonly RectTransformSync rectTransformSync;
        
        // Animation component of alpha channel of icon
        private readonly UIColorSync iconBackgroundColorSync;
        // Animation component of alpha channel of goal text
        private readonly UIAlphaSync goalAlphaSync;
        // Animation component of goal text movement
        private readonly RectTransformSync goalRectSync;
        // Icon of item
        private readonly SVGImage iconBackground;
        private readonly Text name; // Name of item
        private readonly Text description; // Description of item
        private readonly Text goal; // Goal text of item

        // Animation component of circle of item
        private readonly UIColorSync iconChosenCircleColorSync;
        // Animation component of selection
        private readonly UIAlphaSync selectionAlphaSync;

        // Create
        public AnimationPart(RectTransform container)
        {
            // Find objects for item
            iconBackground = container.Find("Icon background").GetComponent<SVGImage>();
            name = container.Find("Name").GetComponent<Text>();
            description = container.Find("Description").GetComponent<Text>();
            goal = container.Find("Goal").GetComponent<Text>();
            
            // Create animation color component for background of item
            iconBackgroundColorSync = new UIColorSync {Speed = animationSpeed, SpeedMode = UIColorSync.Mode.Lerp};
            SyncWithBehaviour.Instance.AddObserver(iconBackgroundColorSync, AppSyncAnchors.CreateFlowAreaChooseType);

            // Add to Color Theming and colorize goal text
            AppTheming.AddElement(goal, ColorTheme.SecondaryColorP1, AppTheming.AppItem.CreateFlowArea);
            AppTheming.ColorizeElement(goal, ColorTheme.SecondaryColorP1);
            
            // Create animation component of alpha channel of goal text
            goalAlphaSync = new UIAlphaSync() {Speed = animationSpeed, SpeedMode = UIAlphaSync.Mode.Lerp};
            goalAlphaSync.AddElement(goal);
            goalAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(goalAlphaSync, AppSyncAnchors.CreateFlowAreaChooseType);
            
            // Find background of type task item
            var chosenObject = container.Find("Chosen");

            // Find circle under icon and create animation component for it
            var iconChosenCircle = chosenObject.GetComponent<Image>();
            AppTheming.AddElement(iconChosenCircle, ColorTheme.SecondaryColorD4, AppTheming.AppItem.CreateFlowArea);
            selectionAlphaSync = new UIAlphaSync();
            selectionAlphaSync.AddElement(iconChosenCircle);
            selectionAlphaSync.Speed = animationSpeed;
            selectionAlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            selectionAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(selectionAlphaSync, AppSyncAnchors.CreateFlowAreaChooseType);
            
            // Create animation component for icon background
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(iconBackground.rectTransform);
            rectTransformSync.Speed = animationSpeed;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.TargetScale = Vector3.one * 1.1f;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.CreateFlowAreaChooseType);
            
            // Create animation component for goal text
            goalRectSync = new RectTransformSync();
            goalRectSync.SetRectTransformSync(goal.rectTransform);
            goalRectSync.Speed = animationSpeed;
            goalRectSync.SpeedMode = RectTransformSync.Mode.Lerp;
            goalRectSync.TargetPosition = goal.rectTransform.anchoredPosition - new Vector2(27, 0);
            goalRectSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(goalRectSync, AppSyncAnchors.CreateFlowAreaChooseType);
        }

        // Reset visual state for item
        public void PrepareToJob()
        {
            // Update colors of texts
            name.color = ThemeLoader.GetCurrentTheme().CreateFlowAreaTypeText;
            description.color = ThemeLoader.GetCurrentTheme().WorkAreaFlowTextStats;
            iconBackground.color = ThemeLoader.GetCurrentTheme().SecondaryColorD1;
            
            // Update animation component with texts colors
            iconBackgroundColorSync.AddElement(name, ThemeLoader.GetCurrentTheme().SecondaryColorP1);
            iconBackgroundColorSync.AddElement(description, ThemeLoader.GetCurrentTheme().CreateFlowAreaTypeDescription);
            iconBackgroundColorSync.AddElement(iconBackground, ThemeLoader.GetCurrentTheme().SecondaryColorP1);
            iconBackgroundColorSync.PrepareToWork();
            iconBackgroundColorSync.Stop();
            
            // Reset state of other animation components
            
            goalAlphaSync.SetAlpha(0);
            goalAlphaSync.SetDefaultAlpha(0);
            goalAlphaSync.Stop();

            rectTransformSync.Stop();
            goalRectSync.Stop();
            
            goalRectSync.SetT(0);
            goalRectSync.SetDefaultT(0);

            selectionAlphaSync.SetAlpha(0);
            selectionAlphaSync.SetDefaultAlpha(0);
            selectionAlphaSync.Stop();
        }
        
        // Set visual active state immediately
        public void SetActiveImmediately(bool active)
        {
            iconBackgroundColorSync.SetColor(active ? 1 : 0);
            iconBackgroundColorSync.SetDefaultMarker(active ? 1 : 0);
            iconBackgroundColorSync.Stop();
            
            goalAlphaSync.SetAlpha(0);
            goalAlphaSync.SetDefaultAlpha(0);
            goalAlphaSync.Stop();
            
            rectTransformSync.SetT(active ? 0 : 1);
            rectTransformSync.SetDefaultT(active ? 0 : 1);
            rectTransformSync.Stop();
            
            goalRectSync.SetT(active ? 1 : 0);
            goalRectSync.SetDefaultT(active ? 1 : 0);
            goalRectSync.Stop();
            
            selectionAlphaSync.SetAlpha(active ? 1 : 0);
            selectionAlphaSync.SetDefaultAlpha(active ? 1 : 0);
            selectionAlphaSync.Stop();
        }

        // Run method by active state - Activate of Deactivate
        public void SetupActive(bool active)
        {
            if (active) Activate();
            else Deactivate();
        }

        // Play animation components when need to show selection of item
        private void Activate()
        {
            iconBackgroundColorSync.Speed = animationSpeed * 2f;
            goalAlphaSync.Speed = animationSpeed * 0.37f;
            rectTransformSync.Speed = animationSpeed * 2f;
            goalRectSync.Speed = animationSpeed * 2f;
            selectionAlphaSync.Speed = animationSpeed * 3f;

            rectTransformSync.SetTByDynamic(0);
            rectTransformSync.Run();
            
            goalRectSync.SetTByDynamic(1);
            goalRectSync.Run();
            
            iconBackgroundColorSync.SetTargetByDynamic(1);
            iconBackgroundColorSync.Run();
            
            goalAlphaSync.SetAlphaByDynamic(1);
            goalAlphaSync.Run();
            
            selectionAlphaSync.SetAlphaByDynamic(1);
            selectionAlphaSync.Run();
        }

        // Play animation components when need to show deselection of item
        private void Deactivate()
        {
            iconBackgroundColorSync.Speed = animationSpeed * 2f;
            goalAlphaSync.Speed = animationSpeed * 10f;
            rectTransformSync.Speed = animationSpeed * 2f;
            goalRectSync.Speed = animationSpeed * 2f;
            selectionAlphaSync.Speed = animationSpeed * 3f;

            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();
            
            goalRectSync.SetTByDynamic(0);
            goalRectSync.Run();
            
            iconBackgroundColorSync.SetTargetByDynamic(0);
            iconBackgroundColorSync.Run();
            
            goalAlphaSync.SetAlphaByDynamic(0);
            goalAlphaSync.Run();
            
            selectionAlphaSync.SetAlphaByDynamic(0);
            selectionAlphaSync.Run();
        }

        // Play animation when goal has been updated
        public void AnimateGoalUpdate()
        {
            goalAlphaSync.Speed = animationSpeed * 1f;
            goalAlphaSync.SetDefaultAlpha(0.5f);
            goalAlphaSync.SetAlphaByDynamic(1);
            goalAlphaSync.Run();
        }
    }
}
