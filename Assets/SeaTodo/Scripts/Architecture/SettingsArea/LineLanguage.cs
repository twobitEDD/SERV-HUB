using Architecture.Components;
using Architecture.Data.FlowTrackInfo;
using Architecture.SettingsArea.ChooseItems;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Other;
using HomeTools.Source.Calendar;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of language
    public class LineLanguage
    {
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob; // Button component
        private readonly ChooseItemsPanel chooseItemsPanel; // Choose items panel component

        // Create and setup
        public LineLanguage(RectTransform rectTransform, ChooseItemsPanel chooseItemsPanel)
        {
            // Save components
            this.rectTransform = rectTransform;
            this.chooseItemsPanel = chooseItemsPanel;
            
            // Create button components
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.SettingsMain);
            mainButtonJob.SimulateWaveScale = 1.057f;

            // Find name and localize
            var name = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsButtonLanguage);
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();
        
        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            
            // Create session for choose language and open view 
            var session = new ChooseItemsSession(ApplyLanguage, 
                LanguageSettings.LanguagesArray, 
                LanguageSettings.CurrentLanguage,
                TextKeysHolder.LanguagePanelTitle);
            chooseItemsPanel.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }

        // Update language by choose items session results
        private static void ApplyLanguage(int order)
        {
            LanguageSettings.SetupLanguage(LanguageSettings.LanguagesArray[order]);
            TextHolderTime.UpdateByLanguage();
            CalendarNames.UpdateByLanguage();
            FlowSymbols.UpdateByLanguage();
            MainMessenger.SendMessage(AppMessagesConst.LanguageUpdated);
        }
    }
}
