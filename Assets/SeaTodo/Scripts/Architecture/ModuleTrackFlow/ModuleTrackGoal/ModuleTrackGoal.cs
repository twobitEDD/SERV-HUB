using Architecture.Components;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleTrackFlow.ModuleTrackGoal
{
    // Component of the task goal recording module
    public class ModuleTrackGoal
    {
        // Link to track module
        private static TrackFlowModule ModuleTrackFlow() => AreasLocator.Instance.TrackFlowModule;
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;

        private readonly VisualPart visualPart; // Component for UI visual control
        private readonly RectTransform rectTransform; // Rect object of view
        private readonly Text trackUnitsText; // Title text of view

        private TrackGoalSession trackSession; // Current track session

        // Create and setup
        public ModuleTrackGoal()
        {
            // Setup main rect of view
            rectTransform = SceneResources.Get("TrackGoal Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            rectTransform.transform.SetSiblingIndex(4);

            // Get and colorize title elements
            trackUnitsText = rectTransform.Find("Info").GetComponent<Text>();
            var trackLine = rectTransform.Find("Line").GetComponent<Image>();
            AppTheming.AddElement(trackUnitsText, ColorTheme.UpdateIconModuleTitle, AppTheming.AppItem.CreateFlowArea);
            AppTheming.AddElement(trackLine, ColorTheme.TipsModuleSplitLine, AppTheming.AppItem.CreateFlowArea);
           
            // Create component of visual part
            visualPart = new VisualPart(rectTransform, trackUnitsText, trackLine, FinishOpenedSession);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            
            visualPart.Initialize();
            // Add close action to messenger
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutGoalTrackClicked);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, TrackGoalSession trackTimeSession, int originPosition)
        {
            trackSession = trackTimeSession;
            ModuleTrackFlow().SetTrackerToPlace(rectTransform);
            
            ModuleTrackFlow().RectTransform.anchoredPosition = new Vector2(0, 0);
            ModuleTrackFlow().RectTransform.localScale = Vector3.one;
            ModuleTrackFlow().UiAlphaSync.Speed = VisualPart.AnimationSpeed;
            ModuleTrackFlow().UiAlphaSync.SetAlpha(1);
            ModuleTrackFlow().UiAlphaSync.Stop();

            ModuleTrackFlow().PrepareToJob(trackTimeSession.Flow, originPosition, TrackFlowModule.Mode.goal);
            visualPart.Open(startPosition);
            ModuleTrackFlow().SetupSyncPositions(true);
            
            trackUnitsText.text = FlowSymbols.GetNameFullByFlowType(trackTimeSession.Flow.Type);
            
            AppBarActions().AddAction(Close);
        }

        // The method which is responsible for close of view
        private void Close()
        {
            visualPart.Close();
            ModuleTrackFlow().SetupSyncPositions(false);
            ModuleTrackFlow().UiAlphaSync.Speed = VisualPart.AnimationSpeed;
            ModuleTrackFlow().UiAlphaSync.SetAlphaByDynamic(0);
            ModuleTrackFlow().UiAlphaSync.PrepareToWork();
            ModuleTrackFlow().UiAlphaSync.Run();
            AppBarActions().RemoveAction(Close);
        }
        
        // Finish session
        private void FinishOpenedSession() => trackSession.FinishSession();
    }
}
