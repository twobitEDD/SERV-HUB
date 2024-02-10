using Architecture.Components;
using Architecture.Data;
using Architecture.Other;
using Architecture.SettingsArea.ChooseItems;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Other;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of first day
    public class LineFirstDay
    {
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob; // Button component
        private readonly Text firstDayShortView; // Text with first day of week
        private readonly ChooseItemsPanel chooseItemsPanel; // Choose items panel component

        // Create and setup
        public LineFirstDay(RectTransform rectTransform, ChooseItemsPanel chooseItemsPanel)
        {
            // Save components
            this.rectTransform = rectTransform;
            this.chooseItemsPanel = chooseItemsPanel;
            
            // Create button components
            firstDayShortView = rectTransform.Find("Short").GetComponent<Text>();
            firstDayShortView.text = TextHolderTime.DaysOfWeekShort(FirstDay);
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.SettingsMain);
            mainButtonJob.SimulateWaveScale = 1.057f;
            
            // Find name and localize
            var name = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsButtonFirstDayOfWeek);
            MainMessenger.AddMember(UpdateByLanguageChanged, AppMessagesConst.LanguageUpdated);
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();
        
        // Call when touched
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            
            // Create session for choose first day of week and open view 
            var session = new ChooseItemsSession(ApplyFirstDay, 
                new []
                {
                    TextHolderTime.WeekDaysFull[WeekInfo.DayOfWeek.Sunday], 
                    TextHolderTime.WeekDaysFull[WeekInfo.DayOfWeek.Monday]
                },
                TextHolderTime.WeekDaysFull[FirstDay],
                TextKeysHolder.FirstDayOfWeekPanelTitle);
            chooseItemsPanel.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }

        // Update first day of week by choose items session results
        private void ApplyFirstDay(int order)
        {
            if (order < 0 || order >= 2)
                return;

            FirstDay = order == 0 ? WeekInfo.DayOfWeek.Sunday : WeekInfo.DayOfWeek.Monday;
            firstDayShortView.text = TextHolderTime.DaysOfWeekShort(FirstDay);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaGraphicDays);
        }
        
        // Get and set first day of week
        private static WeekInfo.DayOfWeek FirstDay
        {
            get => AppCurrentSettings.DaysFromSunday
                ? WeekInfo.DayOfWeek.Sunday
                : WeekInfo.DayOfWeek.Monday;
            
            set => AppCurrentSettings.DaysFromSunday = value == WeekInfo.DayOfWeek.Sunday;
        }
        
        // Update text of first day of week
        private void UpdateByLanguageChanged() => firstDayShortView.text = TextHolderTime.DaysOfWeekShort(FirstDay);
    }
}
