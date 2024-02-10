using Architecture.Components;
using Architecture.Data;
using Architecture.Other;
using Architecture.SettingsArea.ChooseItems;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Other;
using Modules.Notifications;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of day off
    public class LineDayOff
    {
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob;  // Button component
        private readonly Text dayOffShortView; // Text with current day off name
        private readonly ChooseItemsPanel chooseItemsPanel; // Choose items panel component

        // Create and setup
        public LineDayOff(RectTransform rectTransform, ChooseItemsPanel chooseItemsPanel)
        {
            // Save components
            this.rectTransform = rectTransform;
            this.chooseItemsPanel = chooseItemsPanel;
            
            // Create button components
            dayOffShortView = rectTransform.Find("Short").GetComponent<Text>();
            dayOffShortView.text = TextHolderTime.DaysOfWeekShort(AppCurrentSettings.DayOff);
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.SettingsMain);
            mainButtonJob.SimulateWaveScale = 1.057f;
            
            // Find name and localize
            var name = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsButtonDayOff);
            MainMessenger.AddMember(UpdateByLanguageChanged, AppMessagesConst.LanguageUpdated);
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();

        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            
            // Create session for choose day off and open view 
            var session = new ChooseItemsSession(ApplyDayOff, 
                GetDaysOrder(),
                TextHolderTime.DaysOfWeekFull(AppCurrentSettings.DayOff),
                TextKeysHolder.DayOffPanelTitle);
            chooseItemsPanel.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }

        // Update day off by choose items session results
        private void ApplyDayOff(int order)
        {
            if (order < 0 || order >= TextHolderTime.WeekDaysFull.Count)
                return;
            
            AppCurrentSettings.DayOff = WeekInfo.DaysOrder()[order];
            UpdateByLanguageChanged();
            
            AppNotifications.ShouldUpdate = true;
        }

        // Update text of day off
        private void UpdateByLanguageChanged() => dayOffShortView.text = TextHolderTime.DaysOfWeekShort(AppCurrentSettings.DayOff);

        // Get days order actual order
        private string[] GetDaysOrder()
        {
            var days = WeekInfo.DaysOrder();
            var namesFull = TextHolderTime.WeekDaysFull;
            
            var result = new string[days.Length];

            for (var i = 0; i < days.Length; i++)
                result[i] = namesFull[days[i]];

            return result;
        }
    }
}
