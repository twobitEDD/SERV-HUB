using Architecture.AcceptModule;
using Architecture.Components;
using Architecture.Data;
using Architecture.Elements;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Other;
using Modules.Notifications;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of clear data
    public class LineClearData
    {
        // Link to accept view
        private static AcceptModule.AcceptModule AcceptModule => AreasLocator.Instance.AcceptModule;
        // Link to main page
        private static WorkArea.WorkArea WorkArea => AreasLocator.Instance.WorkArea;
        
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob; // Button component

        // Create and setup
        public LineClearData(RectTransform rectTransform)
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
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsClearData);
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();
        
        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
         
            // Create session for clear and open view
            var session = new AcceptSession(null, ClearData, 
                TextLocalization.GetLocalization(TextKeysHolder.ClearDataAcceptTitle));
            
            // Open view of accept clear data
            AcceptModule.Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }

        // Clear app data
        private void ClearData()
        {
            AppData.ClearData();
            
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
    }
}
