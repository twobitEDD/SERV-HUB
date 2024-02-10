using Architecture.CalendarModule.StatisticElements;
using Architecture.Components;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Elements;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;

namespace Architecture.CalendarModule
{
    // Class of calendar view
    public class CalendarTrackModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private readonly VisualPart visualPart; // Class that contains visual part of view
        private readonly MainStats mainStats; // System of calendar view

        // Current calendar session
        public TrackCalendarSession TrackCalendarSession { get; private set; }


        public CalendarTrackModule()
        {
            // Prepare calendar view from resources
            var rectTransform = SceneResources.Get("TrackCalendar Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            var layerIndex = MainCanvas.RectTransform.childCount - 5;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.CalendarObject);
            
            // Create visual part
            visualPart = new VisualPart(rectTransform, FinishOpenedSession, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);

            // Create main systems of calendar view
            mainStats = new MainStats(rectTransform.Find("Calendar Graphic").GetComponent<RectTransform>());
            
            // Colorize parts that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            visualPart.Initialize();

            // Send message to messenger when closed
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutCalendarTrackClicked);
        }
        
        // The method which is responsible for open view
        public void Open(Vector2 startPosition, TrackCalendarSession trackTimeSession)
        {
            TrackCalendarSession = trackTimeSession;

            // Start visual component and add action of close
            visualPart.UiSyncElements = mainStats.CalendarStatistics.CurrentAlphaSync();
            visualPart.Open(startPosition);
            AppBarActions().AddAction(Close);
            
            mainStats.CalendarStatistics.UpdateToDefaultStep(TrackCalendarSession.HomeDay);
        }

        // Setup position of close button
        private void Close()
        {
            visualPart.UiSyncElements = mainStats.CalendarStatistics.CurrentAlphaSync();
            visualPart.Close();
            AppBarActions().RemoveAction(Close);
        }
        
        // Call finish current session from systems of calendar
        private void FinishOpenedSession() => TrackCalendarSession.FinishSession();
    }
}
