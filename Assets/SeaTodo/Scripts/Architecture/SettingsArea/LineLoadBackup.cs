using Architecture.Components;
using Architecture.Elements;
using Architecture.SettingsArea.LoadBackupModule;
using Architecture.TextHolder;
using HomeTools.Other;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of load backup
    public class LineLoadBackup
    {
        // Link to load backup view
        private static LoadBackupModule.LoadBackupModule LoadBackupModule => AreasLocator.Instance.LoadBackupModule;
        
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob; // Button component

        // Create and setup
        public LineLoadBackup(RectTransform rectTransform)
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
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsLoadDataBackup);
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();
        
        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            
            // Create session for load backup file and open view
            var session = new LoadBackupSession(null);
            LoadBackupModule.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }
    }
}
