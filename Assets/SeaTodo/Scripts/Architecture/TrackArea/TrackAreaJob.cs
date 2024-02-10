using System;
using Architecture.CalendarModule;
using Architecture.CalendarModule.StatisticElements;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.ModuleTrackFlow;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Source.Calendar;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TrackArea
{
    // Component that is responsible for work of area
    public class TrackAreaJob : IDisposable
    {
        public Calendar TrackCalendar { get; } // Calendar
        private HomeDay lastWorkDay; // Day in track
        // Link to calendar in main page
        private static Calendar WorkCalendar => AreasLocator.Instance.WorkArea.WorkCalendar;
        // Link to track task progress view
        private static TrackFlowModule TrackFlowArea() => AreasLocator.Instance.TrackFlowModule;

        // UI elements
        private readonly Text flowName;
        private readonly Text trackDayName;
        private readonly Transform trackArea;
        // Animation component of alpha channel of day name
        private readonly UIAlphaSync calendarDaySync;

        private TrackSession currentSession; // Session
        
        // Position of task progress view in area
        private readonly Vector2 trackFlowAreaPosition = new Vector2(0, 130);
        
        // Create and setup
        public TrackAreaJob(GameObject trackArea)
        {
            this.trackArea = trackArea.transform;

            // Find UI elements
            flowName = trackArea.transform.Find("Flow Name").GetComponent<Text>();
            trackDayName = trackArea.transform.Find("Day In Track").GetComponent<Text>();

            // Setup calendar icon
            var calendarIcon = trackArea.transform.Find("Calendar Button").GetComponent<RectTransform>();
            TrackCalendar = new Calendar(calendarIcon, CalendarUpdated, UpdateCalendarGeneratorAction){CurrentDay = WorkCalendar.CurrentDay};
            TrackCalendar.SetMessageForUpdates(AppMessagesConst.TrackCalendarUpdated);
            MainMessenger.AddMember(UpdateDayByCalendar, AppMessagesConst.TrackCalendarUpdated);
            
            // Create animation component of alpha channel of day name
            calendarDaySync = new UIAlphaSync();
            calendarDaySync.AddElement(trackDayName);
            calendarDaySync.Speed = 0.05f;
            calendarDaySync.SpeedMode = UIAlphaSync.Mode.Lerp;
            calendarDaySync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(calendarDaySync);
        }

        // Prepare area by new session
        public void PrepareToSession(TrackSession trackSession, Action<int> resultUpdateAction)
        {
            currentSession = trackSession;
            flowName.text = currentSession.GetFlowName();

            var currentDay = TrackCalendar.CurrentDay;

            TrackFlowArea().PrepareToJob(trackSession.GetFlow(), trackSession.GetFlow().GetTrackedDayOrigin(currentDay),
                TrackFlowModule.Mode.track);
            TrackFlowArea().SetActionCenterOrigin(resultUpdateAction);
            TrackFlowArea().SetupSyncPositions(true);
            
            TrackFlowArea().SetTrackerToPlace(trackArea);
            TrackFlowArea().RectTransform.anchoredPosition = trackFlowAreaPosition;
            TrackFlowArea().RectTransform.localScale = Vector3.one;
            TrackFlowArea().UiAlphaSync.Stop();
            TrackFlowArea().UiAlphaSync.SetAlpha(0);
            TrackFlowArea().SetColorToArea(ThemeLoader.GetCurrentTheme().TrackFlowAreaItems);

            UpdateDayByCalendar();
        }
        
        // Update track day view by session
        private void UpdateDayByCalendar()
        {
            lastWorkDay = TrackCalendar.CurrentDay;
            
            if (TrackCalendar.TodayIsCurrent)
            {
                trackDayName.text = TextLocalization.GetLocalization(TextKeysHolder.Today);
                return;
            }
            
            if (Calendar.Today - TrackCalendar.CurrentDay == 1)
            {
                trackDayName.text = TextLocalization.GetLocalization(TextKeysHolder.Yesterday);
                return;
            }

            trackDayName.text = $"{TrackCalendar.CurrentDay.Day} {CalendarNames.GetMonthShortName(TrackCalendar.CurrentDay.Month)}";
        }

        // Calendar updated action
        private void CalendarUpdated()
        {
            if (lastWorkDay != TrackCalendar.CurrentDay)
            {
                calendarDaySync.SetAlpha(0.3f);
                calendarDaySync.SetDefaultAlpha(0.3f);
                calendarDaySync.SetAlphaByDynamic(1);
                calendarDaySync.Run();
            }
            
            TrackFlowArea().UpdateInJob(currentSession.GetFlow().GetTrackedDayOrigin(TrackCalendar.CurrentDay));
            UpdateDayByCalendar();
            WorkCalendar.CurrentDay = TrackCalendar.CurrentDay;
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaByNewDayImmediately);
        }
        
        private void UpdateCalendarGeneratorAction()
        {
            CalendarDataGenerator.UpdateDatesByFlow(currentSession.GetFlow());
        }
        
        public void Dispose()
        {
            MainMessenger.RemoveMember(UpdateDayByCalendar, AppMessagesConst.TrackCalendarUpdated);
        }
    }
}
