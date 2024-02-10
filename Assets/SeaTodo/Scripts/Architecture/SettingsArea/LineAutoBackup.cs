using System.Collections.Generic;
using System.Linq;
using Architecture.Components;
using Architecture.Data;
using Architecture.Data.Components;
using Architecture.Elements;
using Architecture.SettingsArea.ChooseItems;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Other;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of auto backup
    public class LineAutoBackup
    {
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob; // Button component
        private readonly Text dayOffShortView; // Text component with day of week view
        private readonly ChooseItemsPanel chooseItemsPanel; // Choose items panel component

        // Create and setup
        public LineAutoBackup(RectTransform rectTransform, ChooseItemsPanel chooseItemsPanel)
        {
            // Save components
            this.rectTransform = rectTransform;
            this.chooseItemsPanel = chooseItemsPanel;

            // Fixed and setup day of week view
            dayOffShortView = rectTransform.Find("Short").GetComponent<Text>();
            dayOffShortView.text = GetShortView();
            
            // Create button components
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.SettingsMain);
            mainButtonJob.SimulateWaveScale = 1.057f;

            // Find name and localize
            var name = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsAutoBackup);
         
            MainMessenger.AddMember(UpdateByLanguageChanged, AppMessagesConst.LanguageUpdated);
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();
        
        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();

            // Create list of day of week
            var checkList = new List<string> {TextLocalization.GetLocalization(TextKeysHolder.BackupOff)};
            checkList.AddRange(TextHolderTime.WeekDaysFull.Values);
            
            // Create session for choose auto backup day and open view 
            var session = new ChooseItemsSession(Apply, checkList.ToArray(), GetFullView(), TextKeysHolder.AutoBackupPanelTitle);
            chooseItemsPanel.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }
        
        // Apply chosen day
        private void Apply(int order)
        {
            if (order < 0 || order > TextHolderTime.WeekDaysFull.Count)
                return;

            AppCurrentSettings.AutoBackupOn = order != 0;

            if (order > 0)
            {
                AppCurrentSettings.AutoBackup = TextHolderTime.WeekDaysFull.ElementAt(order - 1).Key;
                PermissionWriteAndRead.TryAskPermissions();
            }
            
            dayOffShortView.text = GetShortView();
        }

        // Get short text of day of auto backup
        private static string GetShortView()
        {
            return !AppCurrentSettings.AutoBackupOn 
                ? TextLocalization.GetLocalization(TextKeysHolder.BackupOff) 
                : TextHolderTime.DaysOfWeekShort(AppCurrentSettings.AutoBackup);
        }
        
        // Get full text of day of auto backup
        private static string GetFullView()
        {
            return !AppCurrentSettings.AutoBackupOn 
                ? TextLocalization.GetLocalization(TextKeysHolder.BackupOff) 
                : TextHolderTime.DaysOfWeekFull(AppCurrentSettings.AutoBackup);
        }
        
        // Update day text
        private void UpdateByLanguageChanged() => dayOffShortView.text = GetShortView();
    }
}
