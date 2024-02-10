using Architecture.Components;
using Architecture.Elements;
using Architecture.SettingsArea.CreateBackupModule;
using Architecture.SettingsArea.CreateBackupResultModule;
using Architecture.SettingsArea.LoadBackupModule;
using Architecture.TextHolder;
using HomeTools.Other;
using Modules.CSVCreator;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of export file
    public class LineExportFile
    {
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob; // Button component
        
        private readonly CreateBackupResultModule.CreateBackupResultModule createBackupResultModule;

        // Create and setup
        public LineExportFile(RectTransform rectTransform, 
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
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsExportFile);

            this.createBackupResultModule = createBackupResultModule;
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();
        
        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            // Create csv file 
            CsvBuilder.GenerateNewCsv();
            
            // Create session for show backup file results and open view
            var session = new CreateBackupResultSession(true, CsvBuilder.DataFileName, CsvBuilder.FilePath);
            createBackupResultModule.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }
    }
}
