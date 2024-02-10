using Architecture.Components;
using Architecture.Data;
using Architecture.TextHolder;
using HomeTools.Messenger;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of time format
    public class LineHourFormat
    {
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob;  // Button component
        private readonly Switcher.Switcher hourFormatSwitcher; // Switcher component

        // Create and setup
        public LineHourFormat(RectTransform rectTransform)
        {
            // Save components
            this.rectTransform = rectTransform;

            // Create button components
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.SettingsMain);
            mainButtonJob.SimulateWaveScale = 1.057f;
            
            // Create switcher component
            hourFormatSwitcher = new Switcher.Switcher(
                rectTransform.Find("Switcher").GetComponent<RectTransform>(),
                ColorTheme.SecondaryColorD4, ColorTheme.SecondaryColorD2, 
                ColorTheme.SecondaryColorD2, ColorTheme.SecondaryColor);
            hourFormatSwitcher.AttachToAnchorSync(AppSyncAnchors.SettingsMain);

            // Find name and localize
            var name = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsButtonHourFormat);
            
            // Add update colors of switcher method to messenger
            MainMessenger.AddMember(UpdateColors, string.Format(AppMessagesConst.ColorizedArea, AppTheming.AppItem.Settings));
        }
        
        // Reset button
        public void Reactivate()
        {
            mainButtonJob.Reactivate();
            hourFormatSwitcher.On = !AppCurrentSettings.EnglishFormat;
            hourFormatSwitcher.UpdateColors();
            hourFormatSwitcher.UpdateStateImmediately();
        }

        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();

            // Switch time format
            AppCurrentSettings.EnglishFormat = !AppCurrentSettings.EnglishFormat;
            hourFormatSwitcher.On = !AppCurrentSettings.EnglishFormat;
            hourFormatSwitcher.UpdateState();
            hourFormatSwitcher.UpdateColors();
        }

        // Update colors of switcher
        private void UpdateColors() => hourFormatSwitcher.UpdateColors();
    }
}
