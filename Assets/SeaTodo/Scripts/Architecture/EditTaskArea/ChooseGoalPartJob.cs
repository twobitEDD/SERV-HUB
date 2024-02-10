using System;
using Architecture.Components;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using Architecture.ModuleTrackFlow;
using Architecture.ModuleTrackFlow.ModuleTrackGoal;
using Architecture.TextHolder;
using HomeTools.Other;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.EditTaskArea
{
    // Component that is responsible for choose goal UI
    public class ChooseGoalPartJob
    {
        // Link to view of track numbers line
        private static TrackFlowModule TrackFlowModule() => AreasLocator.Instance.TrackFlowModule;
        // Link to view of track module
        private static ModuleTrackGoal ModuleTrackGoal() => AreasLocator.Instance.ModuleTrackGoal;

        private readonly RectTransform view; // Main rect of UI
        private readonly Text text; // Text of goal of task
        private readonly MainButtonJob mainButtonJob; // Button component of view
        private readonly UIAlphaSync uiAlphaSync; // Animation component of alpha channel of UI
        private readonly GameObject[] icons; // List of icons for different task types
        private GameObject activeIcon; // Active icon of task type
        private readonly Text nameOfFlow; // Text of task name
        private readonly Text descriptionOfFlow; // Text of description name
        private readonly RectTransform pivot; // Pivot rect for start animation of track goal view

        private Flow currentFlow; // Current task in editing
        private int originPosition; // Task track position of track numbers line
        
        // Create and setup
        public ChooseGoalPartJob()
        {
            // Get view and other UI components from scene resources
            view = SceneResources.Get("EditFlow Goal").GetComponent<RectTransform>();
            text = view.transform.Find("Goal").GetComponent<Text>();
            
            // Create button component for UI
            var handler = view.Find("Handler").gameObject;
            mainButtonJob = new MainButtonJob(text.rectTransform, TouchedAction, handler);
            mainButtonJob.AttachToSyncWithBehaviour();
            // Set goal text to Color Theming module
            AppTheming.AddElement(text, ColorTheme.SecondaryColorP1, AppTheming.AppItem.CreateFlowArea);

            // Find icons of task types
            var iconBackground = view.Find("Icon background");
            icons = new GameObject[6];
            icons[0] = iconBackground.Find("Count").gameObject;
            icons[1] = iconBackground.Find("Stars").gameObject;
            icons[2] = iconBackground.Find("Done").gameObject;
            icons[3] = iconBackground.Find("Time Hours").gameObject;
            icons[4] = iconBackground.Find("Time Minutes").gameObject;
            icons[5] = iconBackground.Find("Time Seconds").gameObject;

            // Find task name and task description UI
            nameOfFlow = view.Find("Name").GetComponent<Text>();
            descriptionOfFlow = view.Find("Description").GetComponent<Text>();
            
            // Create animation component of alpha channel for task goal UI
            uiAlphaSync = new UIAlphaSync();
            uiAlphaSync.AddElement(text);
            uiAlphaSync.Speed = 0.04f;
            uiAlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            uiAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiAlphaSync);

            // Find pivot rect
            pivot = view.Find("Pivot").GetComponent<RectTransform>();
        }

        // Setup UI to other rect
        public void SetupToPlace(RectTransform parent, float position)
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            
            // Setup parent and position
            view.SetParent(parent);
            view.anchoredPosition = new Vector2(0, position);
            view.SetRectTransformAnchorHorizontal(35, 35);
        }

        // Prepare to work track goal view
        public void PrepareTrackGoalPanel()
        {
            TrackFlowModule().SetColorToArea(ThemeLoader.GetCurrentTheme().TrackFlowAreaItems);
            uiAlphaSync.SetAlpha(1);
            uiAlphaSync.SetDefaultAlpha(1);
            uiAlphaSync.Stop();
        }

        // Update UI view by new task
        public void UpdateTrackType(Flow flow)
        {
            // Save new task
            currentFlow = flow;
            // Prepare track goal view by new task
            TrackFlowModule().PrepareToJob(flow, flow.GoalInt, ModuleTrackFlow.TrackFlowModule.Mode.goal);
            // Update texts by new task
            UpdateTextStats(flow.GoalInt);
            // Update icon and description of task
            UpdateView(flow.Type);
        }
        
        // Return goal of task
        public int GetGoal() => TrackFlowModule().GetOriginPosition();

        // Call when user touched to open track goal view
        private void TouchedAction()
        {
            ModuleTrackGoal().Open(OtherHTools.GetWorldAnchoredPosition(pivot), 
                new TrackGoalSession(currentFlow, UpdateTextStats), originPosition);
        }

        // Update text of goal by position
        private void UpdateTextStats(int newOriginPosition)
        {
            originPosition = newOriginPosition;
            var goal = FlowInfoAll.GetGoalByOriginFlowInt(currentFlow.Type, newOriginPosition);
            text.text = FlowInfoAll.GetViewGoalByOriginFlow(currentFlow.Type, goal, text.fontSize);
            
            // Update animation component of alpha channel
            uiAlphaSync.SetDefaultAlpha(0.5f);
            uiAlphaSync.SetAlphaByDynamic(1);
            uiAlphaSync.Run();
        }

        // Update main UI by task type
        private void UpdateView(FlowType flowType)
        {
            // Deactivate current icon of task type
            activeIcon?.SetActive(false);
            
            // Update icon and description by new task type
            switch (flowType)
            {
                case FlowType.count:
                    activeIcon = icons[0];
                    descriptionOfFlow.text = TextLocalization.GetLocalization(TextKeysHolder.FlowTypeCountDescription);
                    break;
                case FlowType.done:
                    activeIcon = icons[1];
                    descriptionOfFlow.text = TextLocalization.GetLocalization(TextKeysHolder.FlowTypeDoneDescription);
                    break;
                case FlowType.stars:
                    activeIcon = icons[2];
                    descriptionOfFlow.text = TextLocalization.GetLocalization(TextKeysHolder.FlowTypeStarsDescription);
                    break;
                case FlowType.timeS:
                    activeIcon = icons[3];
                    descriptionOfFlow.text = TextLocalization.GetLocalization(TextKeysHolder.FlowTypeMinutesDescription);
                    break;
                case FlowType.timeM:
                    activeIcon = icons[4];
                    descriptionOfFlow.text = TextLocalization.GetLocalization(TextKeysHolder.FlowTypeHoursDescription);
                    break;
                case FlowType.timeH:
                    activeIcon = icons[5];
                    descriptionOfFlow.text = TextLocalization.GetLocalization(TextKeysHolder.FlowTypeDaysDescription);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flowType), flowType, null);
            }
            
            // Update name by new task type
            nameOfFlow.text = GetFlowNameByType(flowType);
            activeIcon.SetActive(true);
        }

        // Get localized name of task by task type
        public static string GetFlowNameByType(FlowType flowType)
        {
            switch (flowType)
            {
                case FlowType.count:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeCount);
                case FlowType.done:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeDone);
                case FlowType.stars:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeStars);
                case FlowType.timeS:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeMinutes);
                case FlowType.timeM:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeHours);
                case FlowType.timeH:
                    return TextLocalization.GetLocalization(TextKeysHolder.FlowTypeDays);
                default:
                    throw new ArgumentOutOfRangeException(nameof(flowType), flowType, null);
            }
        }
    }
}
