using HomeTools.Other;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.CreateTaskArea.ChooseTypePart
{
    // Main part of type items
    public class MainPart
    {
        // Position parameters inside part background
        private const float orderAnchorUp = -33;
        private const float height = 170;
        
        private readonly ChooseTypeView[] chooseTypeViews; // Type items array
        private readonly RectTransform[] linesSplit; // Lines between type items

        public FlowType CurrentType { get; private set; } // Selected type
        
        // Create part with items
        public MainPart()
        {
            // Create array with items
            chooseTypeViews = new ChooseTypeView[5]
            {
                new ChooseTypeView(FlowType.count, this),
                new ChooseTypeView(FlowType.done, this),
                new ChooseTypeView(FlowType.timeM, this),
                new ChooseTypeView(FlowType.timeS, this),
                new ChooseTypeView(FlowType.stars, this)
            };

            // Create lines between items
            linesSplit = new RectTransform[chooseTypeViews.Length - 1];
            for (var i = 0; i < linesSplit.Length; i++)
                linesSplit[i] = SceneResources.GetPreparedCopy("Reminders Line");

            // Setup default task type
            CurrentType = FlowType.count;
        }

        // Setup to background task type items
        public void SetupToBackground(RectTransform backgroundRect)
        {
            // Setup type items
            for (var i = 0; i < chooseTypeViews.Length; i++)
            {
                var view = chooseTypeViews[i];
                view.RectTransform.SetParent(backgroundRect);
                view.RectTransform.anchoredPosition = new Vector2(0, orderAnchorUp - height * i);
                view.RectTransform.SetRectTransformAnchorHorizontal(35, 35);
            }
            
            // Setup lines
            for (var i = 0; i < linesSplit.Length; i++)
            {
                var line = linesSplit[i];
                line.SetParent(backgroundRect);
                line.SetRectTransformAnchorHorizontal(70, 70);
                line.anchoredPosition = new Vector2(0, orderAnchorUp - height * i - height * 0.5f);
            }
            
            // Add text element to localization
            chooseTypeViews[0].Setup(TextHolder.TextKeysHolder.FlowTypeCount, TextHolder.TextKeysHolder.FlowTypeCountDescription);
            chooseTypeViews[1].Setup(TextHolder.TextKeysHolder.FlowTypeDone, TextHolder.TextKeysHolder.FlowTypeDoneDescription);
            chooseTypeViews[2].Setup(TextHolder.TextKeysHolder.FlowTypeHours, TextHolder.TextKeysHolder.FlowTypeHoursDescription);
            chooseTypeViews[3].Setup(TextHolder.TextKeysHolder.FlowTypeMinutes, TextHolder.TextKeysHolder.FlowTypeMinutesDescription);
            chooseTypeViews[4].Setup(TextHolder.TextKeysHolder.FlowTypeStars, TextHolder.TextKeysHolder.FlowTypeStarsDescription);
        }
        
        // Reset state of items
        public void SetupCountCurrentType()
        {
            foreach (var typeView in chooseTypeViews) typeView.ResetState();
            
            GetViewType(CurrentType).SetupActiveImmediately(false);
            CurrentType = FlowType.count;
            GetViewType(CurrentType).SetupActiveImmediately(true);
        }

        // Update selected task type
        public void SetupCurrentType(FlowType flowType)
        {
            var update = CurrentType != flowType;
            if (update) GetViewType(CurrentType).SetupActive(false);
            CurrentType = flowType;
            GetViewType(CurrentType).SetupActive(true);
        }

        // Get goal of selected task type
        public int GetGoalOrigin() => GetViewType(CurrentType).OriginGoal;

        // Get item by type
        private ChooseTypeView GetViewType(FlowType flowType)
        {
            switch (flowType)
            {
                case FlowType.count:
                    return chooseTypeViews[0]; 
                case FlowType.done:
                    return chooseTypeViews[1]; 
                case FlowType.timeM:
                    return chooseTypeViews[2];
                case FlowType.timeS:
                    return chooseTypeViews[3]; 
                case FlowType.stars:
                    return chooseTypeViews[4]; 
                default:
                    return null;
            }
        }
    }
}
