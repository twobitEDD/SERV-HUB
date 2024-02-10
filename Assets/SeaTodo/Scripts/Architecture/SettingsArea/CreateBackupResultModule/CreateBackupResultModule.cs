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

namespace Architecture.SettingsArea.CreateBackupResultModule
{
    // The class that is responsible for the backup result window
    public class CreateBackupResultModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;

        private readonly RectTransform rectTransform; // Rect object of view
        private readonly VisualPart visualPart; // Component for visual update of UI
        private readonly Text description; // Description text

        private readonly MainButtonJob mainButtonJob; // Apply button component
        private readonly ApplyDelayView applyDelayView; // Component for delay for button
        private readonly GameObject iconSuccess; // Success icon object
        private readonly GameObject iconFailed; // Failed icon object

        private bool apply; // Applied flag
        
        // Height for view
        private const int heightWhenSuccess = 277;
        private const int heightWhenFailed = 207;
        // Current create backup session
        private CreateBackupResultSession currentSession;

        // Create and setup
        public CreateBackupResultModule()
        {
            // Setup rect of view
            rectTransform = SceneResources.Get("Create Backup Result Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            
            // Setup order in hierarchy
            var layerIndex = MainCanvas.RectTransform.childCount - 6;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.CreateBackupResultModule);
            
            // Create visual part
            visualPart = new VisualPart(rectTransform, null,null, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            
            // Colorize view and initialize component with UI
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            visualPart.Initialize();

            // Send message to messenger when closed
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutCreateBackupModuleResultClicked);

            // Find description text
            description = rectTransform.Find("Description").GetComponent<Text>();
            
            // Create button component
            var buttonRect = rectTransform.Find("Accept").GetComponent<RectTransform>();
            var buttonApplyHandle = buttonRect.Find("Handle").GetComponent<RectTransform>();
            mainButtonJob = new MainButtonJob(buttonRect, ApplyAction, buttonApplyHandle.gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.CreateBackupResultModule);
            
            // Find UI for delay line around apply button
            var loadRound = buttonRect.Find("Load Round").GetComponent<Image>();
            var circleRound = buttonRect.Find("Load Circle").GetComponent<SVGImage>();
            var circleRoundStart = buttonRect.Find("Load Circle Start").GetComponent<SVGImage>();
            // Setup component delay when touched apply
            applyDelayView = new ApplyDelayView(loadRound, circleRound, circleRoundStart, CloseByApply);
            SyncWithBehaviour.Instance.AddObserver(applyDelayView, AppSyncAnchors.CreateBackupResultModule);

            // Find additional icon objects
            iconSuccess = buttonRect.Find("Icon Success").gameObject;
            iconFailed = buttonRect.Find("Icon Failed").gameObject;
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, CreateBackupResultSession session)
        {
            // Save session
            currentSession = session;
            
            // Calculate height of view
            var height = session.Created ? (float) heightWhenSuccess : heightWhenFailed;
            height += description.preferredHeight;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
            
            // Set activity of additional icons
            iconSuccess.SetActive(session.Created);
            iconFailed.SetActive(!session.Created);

            apply = false;
            
            // Setup description text
            var descriptionKey = session.Created
                ? TextKeysHolder.CreateBackupResultSuccess
                : TextKeysHolder.CreateBackupResultFailed;

            description.text = TextLocalization.GetLocalization(descriptionKey);
            
            // Add close action to app bar
            AppBarActions().AddAction(Close);
            mainButtonJob.Reactivate();
            // Start visual component and add action of close
            visualPart.Open(startPosition);
            // Reset component with delay
            applyDelayView.Reset();
        }
        
        // The method which is responsible for close view
        private void Close()
        {
            if (apply)
                return;
            
            // Visual UI close
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
        }

        // Start delay to close
        private void ApplyAction()
        {
            if (apply)
                return;
            
            // Start delay to close
            applyDelayView.StartView();
            apply = true;
        }

        // Close view when applied
        private void CloseByApply()
        {
            // Method of close UI of view
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
        }
    }
}
