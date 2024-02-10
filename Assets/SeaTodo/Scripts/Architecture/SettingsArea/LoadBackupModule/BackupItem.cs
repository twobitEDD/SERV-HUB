using Architecture.AcceptModule;
using Architecture.Components;
using Architecture.Data;
using Architecture.Data.Components;
using Architecture.Elements;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Other;
using HomeTools.Source.Design;
using HTools;
using Modules.Notifications;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea.LoadBackupModule
{
    // View backup item in list with backups 
    public class BackupItem
    {
        // Link to accept view
        private static AcceptModule.AcceptModule AcceptModule => AreasLocator.Instance.AcceptModule;
        // Link to main page
        private static WorkArea.WorkArea WorkArea => AreasLocator.Instance.WorkArea;
       
        // Link to load backup view
        private static LoadBackupModule LoadBackupModule => AreasLocator.Instance.LoadBackupModule;

        // Rect object of view
        public readonly RectTransform RectTransform;
        // Button component
        private readonly MainButtonJobBehaviour mainButtonJob;
        // Delete backup button component
        private readonly MainButtonJobBehaviour removeButtonJob;
        // Component for lock button clicks when scroll
        private readonly DeltaScrollLockerOut deltaScrollLocker;
        
        // Backup file name
        private readonly Text backupName;
        // Additional UI
        private readonly Image bottomLine;
        private readonly SVGImage removeIcon;
        
        private readonly float nameHeightDefault; // Height of name
        private const float nameWidthMax = 427; // Max width of backup name
        // Animation component of alpha channel of UI
        private readonly UIAlphaSync alphaSync;
        // Default backup name
        private string originBackupName;

        // Create and setup
        public BackupItem(RectTransform rectTransform)
        {
            RectTransform = rectTransform;

            // Create scroll locker component
            deltaScrollLocker = new DeltaScrollLockerOut(1);
            SyncWithBehaviour.Instance.AddObserver(deltaScrollLocker, AppSyncAnchors.LoadBackupModule);
            
            // Create button component of item
            var handle = rectTransform.Find("Handle");
            mainButtonJob = handle.gameObject.AddComponent<MainButtonJobBehaviour>();
            mainButtonJob.Setup(rectTransform, Touched, handle.gameObject);
            mainButtonJob.SimulateWaveScale = 1.037f;
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.LoadBackupModule);
            mainButtonJob.AddTouchDownAction(deltaScrollLocker.StartCheck);
            mainButtonJob.AddTouchUpAction(deltaScrollLocker.EndCheck);
            mainButtonJob.Reactivate();
            
            // Create button component for delete backup
            var removeButton = rectTransform.Find("Remove").GetComponent<RectTransform>();
            var handleRemove = removeButton.Find("Handle");
            removeButtonJob = handleRemove.gameObject.AddComponent<MainButtonJobBehaviour>();
            removeButtonJob.Setup(removeButton, TouchedRemove, handleRemove.gameObject);
            removeButtonJob.SimulateWaveScale = 1.17f;
            removeButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.LoadBackupModule);
            removeButtonJob.AddTouchDownAction(deltaScrollLocker.StartCheck);
            removeButtonJob.AddTouchUpAction(deltaScrollLocker.EndCheck);
            removeButtonJob.Reactivate();
            
            // Setup backup name
            backupName = rectTransform.Find("Backup Name").GetComponent<Text>();
            backupName.text = "Default";
            nameHeightDefault = backupName.preferredHeight;
            
            // Add text name to App Theming module
            AppTheming.AddElement(backupName, ColorTheme.ViewFlowAreaDescription, AppTheming.AppItem.Settings);
            AppTheming.ColorizeElement(backupName, ColorTheme.ViewFlowAreaDescription);
            
            // Find bottom backup border and add to App Theming module
            bottomLine = rectTransform.Find("Split Border").GetComponent<Image>();
            AppTheming.AddElement(bottomLine, ColorTheme.DaysMarkerAreaModuleSplitLine, AppTheming.AppItem.Settings);
            AppTheming.ColorizeElement(bottomLine, ColorTheme.DaysMarkerAreaModuleSplitLine);
            
            // Find bottom backup icon and add to App Theming module
            removeIcon = removeButton.Find("Icon").GetComponent<SVGImage>();
            AppTheming.AddElement(removeIcon, ColorTheme.SecondaryColorD3, AppTheming.AppItem.Settings);
            AppTheming.ColorizeElement(removeIcon, ColorTheme.SecondaryColorD3);
            
            // Create animation component for UI elements
            alphaSync = new UIAlphaSync();
            alphaSync.AddElements(backupName);
            alphaSync.AddElements(removeIcon);
            alphaSync.Speed = 0.1f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync, AppSyncAnchors.LoadBackupModule);
        }

        // Setup parent for item
        public void SetParent(RectTransform pool)
        {
            RectTransform.SetParent(pool);
            RectTransform.localScale = Vector3.one;
        }

        // Reset backup file name
        public void ResetByName(string name)
        {
            originBackupName = name;
            var viewName = name.Substring(0, name.Length - AppBackupSerialization.BackupFormat.Length);
            backupName.text = viewName;
            NameCorrection();
        }

        // Set activity state for border
        public void SetActivateLineBorder(bool active)
        {
            if (bottomLine.enabled != active)
                bottomLine.enabled = active;
        }

        // Call when touched item
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            
            if (!deltaScrollLocker.Access)
                return;

            // Prepare session for accept module and open accept view
            var title = $"{TextLocalization.GetLocalization(TextKeysHolder.LoadBackupAcceptTitle)}?";
            var session = new AcceptSession(LoadBackupModule.Close, AcceptLoad, title);
            AcceptModule.Open(OtherHTools.GetWorldAnchoredPosition(backupName.rectTransform), session);
        }
        
        // Update app by new data
        private void AcceptLoad()
        {
            if (originBackupName == string.Empty)
                return;

            var path = $"{AppBackupSerialization.GetPathToFile(originBackupName)}";
            AppData.LoadNewData(path);
            
            WorkArea.WorkCalendar.SetCurrentDayAsToday();
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaByNewDayImmediately);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaImmediately);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
            MainMessenger.SendMessage(AppMessagesConst.UpdateSelectiveInCalendar);
            MainMessenger.SendMessage(AppMessagesConst.ShouldUpdateColorsCountInCalendar);
            MainMessenger.SendMessage(AppMessagesConst.TrackedInStatisticArea);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaGraphicDays);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaGraphic);
            MainMessenger.SendMessage(AppMessagesConst.WorkCalendarUpdated);
            MainMessenger.SendMessage(AppMessagesConst.TrackCalendarUpdated);
            AppNotifications.ShouldUpdate = true;
        }
        
        // Call when touched delete backup
        private void TouchedRemove()
        {
            // Reset state of button component
            removeButtonJob.Reactivate();
            
            if (!deltaScrollLocker.Access)
                return;

            // Prepare session for accept module and open accept view
            var title = $"{TextLocalization.GetLocalization(TextKeysHolder.Remove)}?";
            var session = new AcceptSession(null, AcceptRemove, title);
            AcceptModule.Open(OtherHTools.GetWorldAnchoredPosition(removeIcon.rectTransform), session);
        }
        
        // Call when delete backup file accepted
        private void AcceptRemove()
        {
            if (originBackupName == string.Empty)
                return;
            
            AppBackupSerialization.RemoveFile(originBackupName);
            MainMessenger.SendMessage(AppMessagesConst.UpdateLoadBackupContent);
        }

        // Correct backup text name by height and width
        private void NameCorrection()
        {
            var corrected = false;
            while (backupName.preferredHeight > nameHeightDefault * 1.5f)
            {
                var text = backupName.text;
                text = text.Remove(text.Length - 1);
                backupName.text = text;

                corrected = true;
                
                if (text.Length == 0)
                    break;
            }

            while (backupName.preferredWidth > nameWidthMax)
            {
                var text = backupName.text;
                text = text.Remove(text.Length - 1);
                backupName.text = text;
                
                corrected = true;
                
                if (text.Length == 0)
                    break;
            }

            if (corrected)
                backupName.text += "...";
        }
    }
}
