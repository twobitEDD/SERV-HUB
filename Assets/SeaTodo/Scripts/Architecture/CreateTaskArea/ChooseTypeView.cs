using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.CreateTaskArea.ChooseTypePart;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using Architecture.ModuleTrackFlow.ModuleTrackGoal;
using Architecture.TextHolder;
using HomeTools.Other;
using HTools;
using MainActivity.MainComponents;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Architecture.CreateTaskArea
{
    // Class of choose type in create task page
    public class ChooseTypeView: IDisposable
    {
        // Link to Track goal module
        private static ModuleTrackGoal ModuleTrackGoal() => AreasLocator.Instance.ModuleTrackGoal;
        
        // Rect of item
        public readonly RectTransform RectTransform;
        // Type of task for this item
        public readonly FlowType FlowType;
        
        private readonly HandlePart handlePart; // Handle of item
        private readonly AnimationPart animationPart; // Animation object of item
        private readonly Text textName; // Name of item
        private readonly Text textDescription; // Description of item
        private readonly Text textGoal; // Goal of task
        // Pivot for start position of open track goal view
        private readonly RectTransform pivot; 
        private SVGImage icon; // Icon of task
        
        // Goal of task in item
        public int OriginGoal { private set; get; }

        // Names of icons in UI objects
        private readonly Dictionary<FlowType, string> iconNames = new Dictionary<FlowType, string>()
        {
            {FlowType.count, "Count"}, {FlowType.stars, "Stars"},
            {FlowType.done, "Done"}, {FlowType.timeH, "Time Hours"},
            {FlowType.timeM, "Time Minutes"}, {FlowType.timeS, "Time Seconds"}
        };
        
        // Create item
        public ChooseTypeView(FlowType flowType, MainPart mainPart)
        {
            FlowType = flowType; // Sae task type for item
            // Create Object for item from resources and find text components
            RectTransform = SceneResources.GetPreparedCopy("ChooseFlow Type");
            textName = RectTransform.Find("Name").GetComponent<Text>();
            textDescription = RectTransform.Find("Description").GetComponent<Text>();
            textGoal = RectTransform.Find("Goal").GetComponent<Text>();
            
            // Find handler object
            var handler = RectTransform.Find("Handler").GetComponent<Image>();

            // Create handler for item
            handlePart = new HandlePart(this, mainPart, handler.gameObject);
            SyncWithBehaviour.Instance.AddObserver(handlePart, AppSyncAnchors.CreateFlowAreaChooseType);

            // Create animation part
            animationPart = new AnimationPart(RectTransform);

            // Find pivot for start position of animation of track goal view
            pivot = RectTransform.Find("Pivot").GetComponent<RectTransform>();
            
            // Prepare icon
            PrepareIcon();
        }

        // Localize name and description of task
        public void Setup(string name, string description)
        {
            TextLocalization.Instance.AddLocalization(textName, name);
            TextLocalization.Instance.AddLocalization(textDescription, description);
        }

        // Reset goal of item and reset animation part
        public void ResetState()
        {
            OriginGoal = 4;
            animationPart.PrepareToJob();
        }

        // Setup item to active state immediately
        public void SetupActiveImmediately(bool active)
        {
            handlePart.SetDefaultState(active);
            animationPart.SetActiveImmediately(active);
            UpdateTextStatsAnimation(OriginGoal, active);
        }

        // Set active state for item
        public void SetupActive(bool active)
        {
            handlePart.SetDefaultState(active);
            animationPart.SetupActive(active);
            if (active)
                UpdateTextStats(OriginGoal);
        }

        // Open track goal panel for this item
        public void OpenGoalPanel()
        {
            ModuleTrackGoal().Open(OtherHTools.GetWorldAnchoredPosition(pivot), 
                new TrackGoalSession(new Flow() {Type = FlowType}, UpdateTextStats), OriginGoal);
        }

        // Prepare icon
        private void PrepareIcon()
        {
            // Find icons that don`t need
            var iconsList = iconNames.Values.ToList();
            iconsList.Remove(iconNames[FlowType]);
            
            // Find icon background and delete other icons
            var container = RectTransform.Find("Icon background");
            DeleteOtherIcons(container, iconsList);
            
            // Find icon and setup to Color Theming system
            icon = container.Find(iconNames[FlowType]).GetComponent<SVGImage>();
            AppTheming.AddElement(icon, ColorTheme.CreateFlowAreaTypeIcons, AppTheming.AppItem.CreateFlowArea);
        }
        
        // Delete icons that don`t need
        private void DeleteOtherIcons(Transform container, List<string> icons)
        {
            foreach (var cleanIcon in icons)
                Object.Destroy(container.Find(cleanIcon).gameObject);
        }
        
        // Update goal view after
        private void UpdateTextStats(int newOriginPosition)
        {
            OriginGoal = newOriginPosition;
            var goal = FlowInfoAll.GetGoalByOriginFlowInt(FlowType, newOriginPosition);
            textGoal.text = FlowInfoAll.GetShortViewGoalByOriginFlow(FlowType, goal, textGoal.fontSize);
            
            animationPart.AnimateGoalUpdate();
        }
        
        // Update goal view after with animation
        private void UpdateTextStatsAnimation(int newOriginPosition, bool animate)
        {
            OriginGoal = newOriginPosition;
            var goal = FlowInfoAll.GetGoalByOriginFlowInt(FlowType, newOriginPosition);
            textGoal.text = FlowInfoAll.GetShortViewGoalByOriginFlow(FlowType, goal, textGoal.fontSize);
            
            if (animate)
                animationPart.AnimateGoalUpdate();
        }

        public void Dispose()
        {
            handlePart?.Dispose();
        }
    }
}
