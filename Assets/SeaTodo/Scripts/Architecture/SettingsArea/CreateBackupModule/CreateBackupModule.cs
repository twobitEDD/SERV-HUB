using Architecture.Components;
using Architecture.Data;
using Architecture.Data.Components;
using Architecture.Elements;
using Architecture.Other;
using Architecture.TextHolder;
using HomeTools.Input;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea.CreateBackupModule
{
    // The class that is responsible for the create backup window
    public class CreateBackupModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private readonly VisualPart visualPart; // Component for visual update of UI
        private readonly MainButtonJob mainButtonJob; // Button component
        private readonly ApplyDelayView applyDelayView; // Component for delay for button

        // Field for input name
        private readonly HInputField inputField;
        
        // Applied flag
        private bool apply;
        
        private bool created; // Created flag
        private string createdFileName; // Current file name
        private string createdFilePath; // Current file path
        // Current create backup session
        private CreateBackupSession currentBackupSession; 

        // Create and setup
        public CreateBackupModule()
        {
            // Setup rect of view
            var rectTransform = SceneResources.Get("Create Backup Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            
            // Setup order in hierarchy
            var layerIndex = MainCanvas.RectTransform.childCount - 6;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.CreateBackupModule);
            
            // Create visual part
            visualPart = new VisualPart(rectTransform, ActivateInput, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            
            // Colorize view and initialize component with UI
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            visualPart.Initialize();

            // Send message to messenger when closed
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutCreateBackupModuleClicked);
            
            // Find and localize title text
            var textInfo = rectTransform.Find("Info").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textInfo, TextKeysHolder.CreateBackupModuleTitle);

            // Find and localize description text
            var textDescription = rectTransform.Find("Description").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textDescription, TextKeysHolder.CreateBackupModuleDescription);
            
            // Create button component
            var buttonRect = rectTransform.Find("Accept").GetComponent<RectTransform>();
            var buttonApplyHandle = buttonRect.Find("Handle").GetComponent<RectTransform>();
            mainButtonJob = new MainButtonJob(buttonRect, ApplyAction, buttonApplyHandle.gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.CreateBackupModule);

            // Setup input field
            inputField = rectTransform.Find("Edit Name Input").GetComponent<HInputField>();
            var placeholder = rectTransform.Find("Edit Name Input/Placeholder").GetComponent<Text>();
            placeholder.text = AutoBackupData.BackupFileName;
            inputField.AddActionWhenSelected(visualPart.ActivateInput);
            inputField.AddActionWhenDeselected(visualPart.DeactivateInput);
            
            // Find UI for delay line around apply button
            var loadRound = buttonRect.Find("Load Round").GetComponent<Image>();
            var circleRound = buttonRect.Find("Load Circle").GetComponent<SVGImage>();
            var circleRoundStart = buttonRect.Find("Load Circle Start").GetComponent<SVGImage>();
            // Setup component delay when touched apply
            applyDelayView = new ApplyDelayView(loadRound, circleRound, circleRoundStart, StartApply);
            SyncWithBehaviour.Instance.AddObserver(applyDelayView, AppSyncAnchors.CreateBackupModule);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, CreateBackupSession createBackupSession)
        {
            // Save session
            currentBackupSession = createBackupSession;
            // Add close action to app bar
            AppBarActions().AddAction(Close);
            // Reset state of button component
            mainButtonJob.Reactivate();

            // Setup name to input field
            inputField.text = AutoBackupData.BackupFileName;
            // Start visual component and add action of close
            visualPart.Open(startPosition);

            // Reset component with delay
            applyDelayView.Reset();
            apply = false;
            created = false;
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

        // Activate input component
        private void ActivateInput() => inputField.enabled = true;

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
        private void StartApply()
        {
            // Method of close UI of view
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
            
            // Try build new backup file
            
            TryBuildNewBackup();

            currentBackupSession.Created = created;
            currentBackupSession.FileName = createdFileName;
            currentBackupSession.FilePath = createdFilePath;
            currentBackupSession.UpdateAction();
        }

        // Try apply new name to day characteristic item
        private void TryBuildNewBackup()
        {
            if (!apply)
                return;
            
            // Ask permissions
            PermissionWriteAndRead.TryAskPermissions();
            
            if (!PermissionWriteAndRead.HasPermissions)
                return;

            // Create backup file
                    
            var text = inputField.text;
            createdFileName = text.Length > 0 ? text : AutoBackupData.BackupFileName;
            createdFileName += AppBackupSerialization.BackupFormat;
            
            if (AppBackupSerialization.FileExists(createdFileName))
                AppBackupSerialization.RemoveFile(createdFileName);
            
            AppBackupSerialization.CreateFile(createdFileName);
            createdFilePath = AppBackupSerialization.GetPathToFile(createdFileName);
            
            created = true;
        }
    }
}
