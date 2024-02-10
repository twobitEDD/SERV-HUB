using Architecture.Components;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Elements;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;

namespace Architecture.ModuleTrackTime
{
    // The class for which is responsible for the track time of reminders
    public class ModuleTrackTime
    {
        private HomeTime lastTime; // Last time amount in tracker
        // Flag responsible for the success of creating a structure for time
        private bool timeConstructed;

        // Return the last tracked time and the success of the creation process
        public (HomeTime time, bool constructed) LastConstructedTime => (lastTime, timeConstructed);

        // Use 12 hours format
        private static bool EnglishFormat => AppCurrentSettings.EnglishFormat;
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private readonly VisualPart visualPart; // Component for UI visual control
        // Component for updating the interface depending on the time format
        private readonly VisualByTimeMode visualByTimeMode;

        private TrackItem trackItemDayPart; // Object to select part of the day
        private TrackItem trackItemHours; // Object to select hours
        private TrackItem trackItemMinutes; // Object to select minutes

        // Set of UI for setup line of day part
        private readonly LineTrackSourceDayPart lineTrackSourceDayPart;
        // Set of UI for setup line of hours
        private readonly LineTrackSourceHours lineTrackSourceHours;
        // Set of UI for setup line of minutes
        private readonly LineTrackSourceMinutes lineTrackSourceMinutes;

        // Track time session
        private TrackTimeSession timeSession;

        // Create and setup
        public ModuleTrackTime()
        {
            // Get rect from scene resources and setup
            var rectTransform = SceneResources.Get("TrackTime Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            rectTransform.transform.SetSiblingIndex(4);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.TrackTimeModule);
            
            // Create visual part of UI
            visualPart = new VisualPart(rectTransform, FinishOpenedSession);
            SyncWithBehaviour.Instance.AddObserver(visualPart);

            // Create sets of UI for setup time
            var poolObject = rectTransform.Find("Elements Pool").GetComponent<RectTransform>();
            lineTrackSourceDayPart = new LineTrackSourceDayPart(poolObject);
            lineTrackSourceHours = new LineTrackSourceHours(poolObject);
            lineTrackSourceMinutes = new LineTrackSourceMinutes(poolObject);
            
            // Create object to select part of the day
            CreateTrackTimeItems(rectTransform);

            // Get colon UI object and setup component for update UI
            var colon = rectTransform.Find("Colon").GetComponent<RectTransform>();
            visualByTimeMode = new VisualByTimeMode(trackItemDayPart.RectTransform, trackItemHours.RectTransform, trackItemMinutes.RectTransform, colon);
            // Update colors of UI that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            // Initialize component of UI
            visualPart.Initialize();

            // Add close method to messenger for dark background clicks
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutTimeTrackClicked);
        }

        // Create and setup components for track time
        private void CreateTrackTimeItems(RectTransform rectTransform)
        {
            var dayPart = rectTransform.Find("DayPart").GetComponent<RectTransform>();
            foreach (var texts in lineTrackSourceDayPart.GetCreatedTexts) visualPart.UiSyncItemPoints.AddElement(texts);
            trackItemDayPart = new TrackItem(dayPart, lineTrackSourceDayPart);

            var hoursPart = rectTransform.Find("HoursPart").GetComponent<RectTransform>();
            foreach (var texts in lineTrackSourceHours.GetCreatedTexts) visualPart.UiSyncItemPoints.AddElement(texts);
            trackItemHours = new TrackItem(hoursPart, lineTrackSourceHours);

            var minutesPart = rectTransform.Find("MinutesPart").GetComponent<RectTransform>();
            foreach (var texts in lineTrackSourceMinutes.GetCreatedTexts) visualPart.UiSyncItemPoints.AddElement(texts);
            trackItemMinutes = new TrackItem(minutesPart, lineTrackSourceMinutes);
            
            SetupSyncPositions(false);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, TrackTimeSession trackTimeSession)
        {
            // Save setup time session
            timeSession = trackTimeSession;
            
            // Update UI
            visualByTimeMode.Update();
            SetupPositionToItems(trackTimeSession.HomeTime);
            // Open of visual UI
            visualPart.Open(startPosition);
            // Turn on synchronize track time and scroll
            SetupSyncPositions(true);
            // Add close action to app bar
            AppBarActions().AddAction(Close);
        }

        // Setup created time
        public void SetupConstructedTime(bool active, HomeTime time)
        {
            lastTime = time;
            timeConstructed = active;
        }

        // Setup time to scroll items
        private void SetupPositionToItems(HomeTime homeTime)
        {
            var (hours, minutes, am) = homeTime.GetTime(EnglishFormat);
            trackItemDayPart.PrepareToSession(am ? 1 : 0);
            trackItemHours.PrepareToSession(HoursPositionLoop.GetPositionByHour(hours, EnglishFormat));
            trackItemMinutes.PrepareToSession(MinutesPositionLoop.GetPositionByMinutes(minutes));
        }

        // The method which is responsible for close view
        private void Close()
        {
            // Visual UI close
            visualPart.Close();
            // Turn off synchronize track time and scroll
            SetupSyncPositions(false);
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
        }
        
        // Finish session
        private void FinishOpenedSession() => timeSession.FinishSession(ConstructTime());

        // Create time object by visual tracked time
        private HomeTime ConstructTime()
        {
            var hours = EnglishFormat ? HoursPositionLoop.GetHourEnglish(lineTrackSourceHours.CenterOriginPosition) : 
                                          HoursPositionLoop.GetHourFull(lineTrackSourceHours.CenterOriginPosition);
            var minutes = MinutesPositionLoop.GetMinutes(lineTrackSourceMinutes.CenterOriginPosition);

            var am = lineTrackSourceDayPart.CenterOriginPosition == 1;
            
            var time = new HomeTime(hours, minutes);
            time.SetTime(hours, minutes, am, EnglishFormat);
            
            lastTime = time;
            timeConstructed = true;
            
            return time;
        }

        // Set activity of synchronization of items
        private void SetupSyncPositions(bool active)
        {
            trackItemDayPart.SyncPosition = active;
            trackItemHours.SyncPosition = active;
            trackItemMinutes.SyncPosition = active;
        }
    }
}
