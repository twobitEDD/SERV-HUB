using Architecture.Components;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.Statistics;
using HomeTools.Messenger;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Architecture.CalendarModule.StatisticElements
{
    // Item of day in calendar page
    public class CalendarDayItem
    {
        private CalendarTrackModule CalendarTrackModule => AreasLocator.Instance.CalendarTrackModule;
        
        private readonly Text text; // View of day
        private readonly SelectiveDayItem selectiveDayItem; // Circle for selected day
        private readonly MainButtonJob mainButtonJob; // day as button
        private readonly UIColorSync uiColorSync; // Color animation of day
        
        // Info about date in this day
        private int yearLocal;
        private int monthLocal;
        private int dayLocal;

        // If this day is active
        private bool localActive;
        
        public CalendarDayItem(Text text, SelectiveDayItem selectiveDayItem)
        {
            // Setup components
            this.text = text;
            this.selectiveDayItem = selectiveDayItem;

            // Create button component for day
            mainButtonJob = new MainButtonJob(text.rectTransform, Touched, text.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.CalendarObject);
            mainButtonJob.Reactivate();

            // Create animation color component for day 
            uiColorSync = new UIColorSync();
            uiColorSync.AddElement(text, ThemeLoader.GetCurrentTheme().ImagesColor);
            uiColorSync.SpeedMode = UIColorSync.Mode.Lerp;
            uiColorSync.Speed = 0.2f;
            uiColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiColorSync, AppSyncAnchors.CalendarObject);
        }

        // Add scroll events for day button (when user scrolls calendar page by this item)
        public void SetupScrollEvents(StatisticsScroll statisticsScroll)
        {
            mainButtonJob.HandleObject.AddEvent(EventTriggerType.PointerDown, statisticsScroll.TouchDown);
            mainButtonJob.HandleObject.AddEvent(EventTriggerType.PointerUp, statisticsScroll.TouchUp);
        }

        // Update date
        public void UpdateDay(int year, int month, int day)
        {
            yearLocal = year;
            monthLocal = month;
            dayLocal = day;
        }

        // Update day view
        public void UpdateView(bool active)
        {
            TryEnable();
            text.text = dayLocal.ToString();

            var todayInt = Calendar.Today.HomeDayToInt();
            localActive = FlowExtensions.HomeDayToInt(yearLocal, monthLocal, dayLocal) <= todayInt;
            
            // Detect color of text
            var color = Color.white;

            if (localActive && active)
                color = ThemeLoader.GetCurrentTheme().CalendarItemActive;
            
            if (localActive && !active)
                color = ThemeLoader.GetCurrentTheme().CalendarItemActiveOut;
            
            if (!localActive && !active)
                color = ThemeLoader.GetCurrentTheme().CalendarItemPassive;

            text.color = color;
            uiColorSync.AddElement(text, ThemeLoader.GetCurrentTheme().ImagesColor);
            
            // Check if day is current
            CheckToSelective();
        }

        // Check if day is current
        public bool CheckToSelective()
        {
            if (!localActive)
                return false;

            if (CalendarTrackModule?.TrackCalendarSession == null)
                return false;
            
            // Get day from calendar session object
            var currentHomeDay = CalendarTrackModule.TrackCalendarSession.HomeDay;
            
            // Check if this day is the same
            if (currentHomeDay.Year == yearLocal && currentHomeDay.Month == monthLocal &&
                currentHomeDay.Day == dayLocal)
            {
                selectiveDayItem.SetupToPlace(text.rectTransform, uiColorSync);
                uiColorSync.SetColor(1);
                uiColorSync.SetDefaultMarker(1);
                
                return true;
            }

            return false;
        }

        // Try enable this day
        private void TryEnable()
        {
            if (!text.enabled) text.enabled = true;
        }
        
        public void Disable() => text.enabled = false;

        // Call this method when user touches this day
        private void Touched()
        {
            if (!localActive)
                return;
            
            var currentHomeDay = CalendarTrackModule.TrackCalendarSession.HomeDay;
            
            if (currentHomeDay.Year == yearLocal && currentHomeDay.Month == monthLocal &&
                currentHomeDay.Day == dayLocal)
                return;
            
            // Try to set as active day
            selectiveDayItem.SetupToPlace(text.rectTransform, uiColorSync);
            uiColorSync.SetColor(1);
            uiColorSync.SetDefaultMarker(1);
            CalendarTrackModule.TrackCalendarSession.HomeDay = new HomeDay(yearLocal, monthLocal, dayLocal);
            
            // Send message for other listeners that day of calendar has been updated
            MainMessenger.SendMessage(AppMessagesConst.UpdateSelectiveInCalendar);
        }
    }
}
