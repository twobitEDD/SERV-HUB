using Architecture.Components;
using Architecture.TextHolder;
using HomeTools.Messenger;
using InternalTheming;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of dark theme
    public class LineDarkTheme
    {
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob;  // Button component
        private readonly Switcher.Switcher darkThemeSwitcher; // Switcher component

        // Create and setup
        public LineDarkTheme(RectTransform rectTransform)
        {
            // Save components
            this.rectTransform = rectTransform;

            // Create button components
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.SettingsMain);
            mainButtonJob.SimulateWaveScale = 1.057f;
            
            // Create switcher component
            darkThemeSwitcher = new Switcher.Switcher(
                rectTransform.Find("Switcher").GetComponent<RectTransform>(),
                ColorTheme.SecondaryColorD4, ColorTheme.SecondaryColorD2, 
                ColorTheme.SecondaryColorD2, ColorTheme.SecondaryColor);
            darkThemeSwitcher.AttachToAnchorSync(AppSyncAnchors.SettingsMain);
            
            // Find name and localize
            var name = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsButtonDarkTheme);
            
            // Check if dark theme locked
            CheckLockedVisual();
            MainMessenger.AddMember(CheckLockedVisual, AppMessagesConst.PurchaseUpdated);
        }
        
        // Reset button
        public void Reactivate()
        {
            mainButtonJob.Reactivate();
            darkThemeSwitcher.On = AppTheming.DarkTheme;
            darkThemeSwitcher.UpdateColors();
            darkThemeSwitcher.UpdateStateImmediately();
        }

        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            TouchedDarkTheme();
        }

        // Switch app theme
        private void TouchedDarkTheme()
        {
            darkThemeSwitcher.On = !darkThemeSwitcher.On;
            darkThemeSwitcher.UpdateState();
            AppTheming.UpdateTheme(darkThemeSwitcher.On ? ThemeLoader.ThemeType.Dark : ThemeLoader.ThemeType.White);
            darkThemeSwitcher.UpdateColors();
        }

        // Check if dark theme locked
        private void CheckLockedVisual()
        {
            darkThemeSwitcher.SetActive(true);
        }
    }
}
