using Architecture.Components;
using Architecture.Elements;
using Architecture.Other;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.AcceptModule
{
    // Main class of accept view
    public class AcceptModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private readonly VisualPart visualPart; // Class that contains visual part of this page
        private readonly Text textInfo;
        private readonly MainButtonJob mainButtonJob; // Class of accept button
        private readonly ApplyDelayView applyDelayView;
        
        private AcceptSession acceptSession;
        private bool active;

        public AcceptModule()
        {
            // Find and setup accept module
            var rectTransform = SceneResources.Get("Accept Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.AcceptModule);
            
            // Setup order in hierarchy
            var layerIndex = MainCanvas.RectTransform.childCount - 4;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            
            // Create component of visual part
            visualPart = new VisualPart(rectTransform, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            
            // Colorize parts that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            visualPart.Initialize();

            // Add close action to messenger
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutAcceptModuleClicked);
            
            // Get link for Info
            textInfo = rectTransform.Find("Info").GetComponent<Text>();
            
            // Setup accept button with load circle
            var buttonRect = rectTransform.Find("Accept").GetComponent<RectTransform>();
            var round = buttonRect.Find("Load Round").GetComponent<Image>();
            var roundCircle = buttonRect.Find("Load Circle").GetComponent<SVGImage>();
            var roundCircleStart = buttonRect.Find("Load Circle Start").GetComponent<SVGImage>();
            applyDelayView = new ApplyDelayView(round, roundCircle, roundCircleStart, ApplyAction);
            mainButtonJob = new MainButtonJob(buttonRect, StartApply, buttonRect.Find("Handle").gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.AcceptModule);
            SyncWithBehaviour.Instance.AddObserver(applyDelayView, AppSyncAnchors.AcceptModule);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, AcceptSession session)
        {
            acceptSession = session;
            visualPart.Open(startPosition);
            AppBarActions().AddAction(Close);
            textInfo.text = session.Title;
            mainButtonJob.Reactivate();
            applyDelayView.Reset();
            active = false;
        }
        
        // The method which is responsible for close of view
        private void Close()
        {
            if (active)
                return;
            
            visualPart.Close();
            AppBarActions().RemoveAction(Close);
            acceptSession.Closed();
        }

        // The method that runs close view
        private void StartApply()
        {
            applyDelayView.StartView();
            active = true;
        }
        
        // The method that runs after delay
        private void ApplyAction()
        {
            if (acceptSession.ChangeCloseTargetPosition)
                visualPart.UpdateTargetPosition(acceptSession.CloseTargetPosition);
            
            visualPart.Close();
            AppBarActions().RemoveAction(Close);
            acceptSession.Accepted();
        }
    }
}
