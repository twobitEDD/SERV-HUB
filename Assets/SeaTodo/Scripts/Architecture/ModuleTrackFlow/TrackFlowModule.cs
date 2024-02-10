using System;
using Architecture.Data;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;

namespace Architecture.ModuleTrackFlow
{
    /// <summary>
    /// The class that is responsible for displaying the scroll bar
    /// for selecting the number of progress for the task
    /// </summary>
    public class TrackFlowModule
    {
        // Animation component for alpha channel of scroll bar numbers
        public readonly UIAlphaSync UiAlphaSync;
        // Rect object of view
        public readonly RectTransform RectTransform;
        // Scroll component for numbers
        private readonly TrackScroll trackScroll;
        // Component with text elements packages for each task type
        private LineTrackSources LineTrackSources { get; }
        //Create and setup
        public TrackFlowModule()
        {
            // Find object of view in scene resources
            RectTransform = SceneResources.Get("Track Module").GetComponent<RectTransform>();
            SyncWithBehaviour.Instance.AddAnchor(RectTransform.gameObject, AppSyncAnchors.TrackFlowModule);
            
            // Create and deactivate pool object
            var poolForElements = RectTransform.Find("Elements Pool").GetComponent<RectTransform>();
            poolForElements.gameObject.SetActive(false);
            
            // Create component for text elements for view
            LineTrackSources = new LineTrackSources(poolForElements, this);
            
            // Find scroll area and crete scroll component
            var handleScrollArea = RectTransform.transform.Find("Tracker");
            trackScroll = new TrackScroll(handleScrollArea.gameObject, RectTransform, LineTrackSources);
            SyncWithBehaviour.Instance.AddObserver(trackScroll, AppSyncAnchors.TrackFlowModule);
            
            // Create animation component for alpha channel of text items
            UiAlphaSync = new UIAlphaSync {SpeedMode = UIAlphaSync.Mode.Lerp};
            SyncWithBehaviour.Instance.AddObserver(UiAlphaSync, AppSyncAnchors.TrackFlowModule);
            foreach (var text in LineTrackSources.GetTextItems()) UiAlphaSync.AddElement(text);
            UiAlphaSync.PrepareToWork();
        }
        
        // Set scroll bar to place
        public void SetTrackerToPlace(RectTransform placeForGoal) => RectTransform.SetParent(placeForGoal);
        
        // Set scroll bar to place
        public void SetTrackerToPlace(Transform placeForGoal) => RectTransform.SetParent(placeForGoal);

        // Prepare view to choose number by scrolling
        public void PrepareToJob(Flow flow, int originPosition, Mode mode)
        {
            // Setup action moved line numbers to center
            SetActionCenterOrigin(null);
            // Setup handle size
            SetupHandleSize(mode);
            // Update components of text sources by flow and track mode
            LineTrackSources.PutFlow(flow, mode);
            // Setup scroll to default track position
            trackScroll.SetScrollToDefault(0);
            // Update scroll parameters by current text source
            trackScroll.UpdateParamsBySource();
            // Setup scroll to chosen position
            trackScroll.SetScrollToDefault(originPosition);
            // Setup chosen position to text source component
            LineTrackSources.GetActualSource().CenterOriginPosition = originPosition;
        }
        
        // Update position of line of numbers in process
        public void UpdateInJob(int originPosition)
        {
            trackScroll.SetScrollToDefault(originPosition);
        }

        // Update action that calls when number in center of scroll updated
        public void SetActionCenterOrigin(Action<int> action) => LineTrackSources.SetCenterOrigin = action;

        // Update color of text components
        public void SetColorToArea(Color color) => LineTrackSources.ColorPackage.ColorizeItems(color);

        // Update handle size
        private void SetupHandleSize(Mode mode)
        {
            var width = mode == Mode.goal ? 300 : 500;
            trackScroll.TouchArea.sizeDelta = new Vector2(width, trackScroll.TouchArea.sizeDelta.y);
        }

        // Get number position that in center of scroll
        public int GetOriginPosition() => LineTrackSources.GetActualSource().CenterOriginPosition;
        
        // Activate sync positions of numbers with scroll movement
        public void SetupSyncPositions(bool active) => trackScroll.SyncPosition = active;

        // Modes of tracking
        public enum Mode
        {
            goal,
            track
        }
    }
}
