using Architecture.Components;
using Architecture.SettingsArea.CreateBackupModule;
using Architecture.SettingsArea.CreateBackupResultModule;
using Architecture.TextHolder;
using HomeTools.Other;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of create backup
    public class LineCreateBackup
    {
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob; // Button component
        // Create backup view
        private readonly CreateBackupModule.CreateBackupModule createBackupModule;
        // Backup results view
        private readonly CreateBackupResultModule.CreateBackupResultModule createBackupResultModule;
        // Create and setup
        public LineCreateBackup(RectTransform rectTransform, 
            CreateBackupResultModule.CreateBackupResultModule createBackupResultModule)
        {
            // Save components
            this.rectTransform = rectTransform;

            // Create button components
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.SettingsMain);
            mainButtonJob.SimulateWaveScale = 1.057f;

            // Find name and localize
            var name = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsCreateDataBackup);
            
            // Create views
            createBackupModule = new CreateBackupModule.CreateBackupModule();
            this.createBackupResultModule = createBackupResultModule;
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();
        
        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            
            // Create session for create backup file and open view
            var session = new CreateBackupSession(ShowResultAfterCreate);
            createBackupModule.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }

        // Call after close create backup view
        private void ShowResultAfterCreate(CreateBackupSession oldSession)
        {
            // Create session for show backup file results and open view
            var session = new CreateBackupResultSession(oldSession.Created, oldSession.FileName, oldSession.FilePath);
            createBackupResultModule.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }
    }
}
