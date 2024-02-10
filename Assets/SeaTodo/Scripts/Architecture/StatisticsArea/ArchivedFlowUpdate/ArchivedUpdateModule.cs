using Architecture.AcceptModule;
using Architecture.Components;
using Architecture.Elements;
using Architecture.Other;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.StatisticsArea.ArchivedFlowUpdate
{
    // Update archived task status - remove or setup in progress state
    public class ArchivedUpdateModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        // Link to accept view
        private AcceptModule.AcceptModule AcceptModule() => AreasLocator.Instance.AcceptModule;

        private readonly VisualPart visualPart; // Component for UI visual control
        private readonly Text textInfo; // Title text
        //  Button component of activate task
        private readonly MainButtonJob mainButtonJobActivate;
        // Delay component for await for activate
        private readonly ApplyDelayView applyDelayViewActivate;
        // Button component of remove task
        private readonly MainButtonJob mainButtonJobRemove;
        // Delay component for await for remove
        private readonly ApplyDelayView applyDelayViewRemove;
        
        // Current update task session
        private UpdateSession updateSession;
        // Accept session
        private AcceptSession acceptSession;
        private bool active;

        // Create and setup
        public ArchivedUpdateModule()
        {
            // Find object of view in scene resources and setup
            var rectTransform = SceneResources.Get("Archived Update Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.ArchivedUpdateModule);
            
            // Setup order in hierarchy
            var layerIndex = MainCanvas.RectTransform.childCount - 7;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            
            // Create visual part
            visualPart = new VisualPart(rectTransform, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            
            // Colorize view and initialize component with UI
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            visualPart.Initialize();

            // Send message to messenger when closed
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutUpdateArchivedModuleClicked);
            
            // Find and localize title text
            textInfo = rectTransform.Find("Info").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textInfo, TextKeysHolder.Update);
            
            // Create button components of activate button
            var buttonRectActivate = rectTransform.Find("Activate").GetComponent<RectTransform>();
            var round = buttonRectActivate.Find("Load Round").GetComponent<Image>();
            var roundCircle = buttonRectActivate.Find("Load Circle").GetComponent<SVGImage>();
            var roundCircleStart = buttonRectActivate.Find("Load Circle Start").GetComponent<SVGImage>();
            applyDelayViewActivate = new ApplyDelayView(round, roundCircle, roundCircleStart, ActivateAction);
            mainButtonJobActivate = new MainButtonJob(buttonRectActivate, StartApplyActivate, buttonRectActivate.Find("Handle").gameObject);
            mainButtonJobActivate.Reactivate();
            mainButtonJobActivate.AttachToSyncWithBehaviour(AppSyncAnchors.ArchivedUpdateModule);
            SyncWithBehaviour.Instance.AddObserver(applyDelayViewActivate, AppSyncAnchors.ArchivedUpdateModule);
            var textActivate = buttonRectActivate.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textActivate, TextKeysHolder.Todo);

            // Create button components of remove button
            var buttonRectRemove = rectTransform.Find("Remove").GetComponent<RectTransform>();
            round = buttonRectRemove.Find("Load Round").GetComponent<Image>();
            roundCircle = buttonRectRemove.Find("Load Circle").GetComponent<SVGImage>();
            roundCircleStart = buttonRectRemove.Find("Load Circle Start").GetComponent<SVGImage>();
            applyDelayViewRemove = new ApplyDelayView(round, roundCircle, roundCircleStart, RemoveAccept);
            mainButtonJobRemove = new MainButtonJob(buttonRectRemove, StartApplyRemove, buttonRectRemove.Find("Handle").gameObject);
            mainButtonJobRemove.Reactivate();
            mainButtonJobRemove.AttachToSyncWithBehaviour(AppSyncAnchors.ArchivedUpdateModule);
            SyncWithBehaviour.Instance.AddObserver(applyDelayViewRemove, AppSyncAnchors.ArchivedUpdateModule);
            var textRemove = buttonRectRemove.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textRemove, TextKeysHolder.Remove);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, UpdateSession session)
        {
            // Save session
            updateSession = session;
            // Start visual component and add action of close
            visualPart.Open(startPosition);
            // Add close action to app bar
            AppBarActions().AddAction(Close);
            // Reset state of buttons component
            mainButtonJobActivate.Reactivate();
            mainButtonJobRemove.Reactivate();
            // Reset components with delay
            applyDelayViewActivate.Reset();
            applyDelayViewRemove.Reset();
            active = false;
        }
        
        // The method which is responsible for close view
        private void Close()
        {
            if (active)
                return;
            
            // Visual UI close
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
        }

        // Start delay to activate task
        private void StartApplyActivate()
        {
            applyDelayViewActivate.StartView();
            active = true;
        }
        
        // Start delay to remove task
        private void StartApplyRemove()
        {
            applyDelayViewRemove.StartView();
            active = true;
        }

        // Close view when activated
        private void ActivateAction()
        {
            if (updateSession.ChangeCloseTargetPosition)
                visualPart.UpdateTargetPosition(updateSession.CloseTargetPosition);
            
            visualPart.Close();
            AppBarActions().RemoveAction(Close);
            updateSession.Accepted();
        }

        // Close view when removed
        private void RemoveAccept()
        {
            var title = TextLocalization.GetLocalization(TextKeysHolder.Remove) + "?";
            acceptSession = new AcceptSession(null, RemoveAction, title);
            AcceptModule().Open(Vector2.zero, acceptSession);
            
            if (updateSession.ChangeCloseTargetPosition)
                visualPart.UpdateTargetPosition(updateSession.CloseTargetPosition);
            
            visualPart.Close();
            AppBarActions().RemoveAction(Close);
        }
        
        // Invoke remove action in session
        private void RemoveAction()
        {
            updateSession.Removed();
        }
    }
}
