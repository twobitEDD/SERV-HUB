using System;
using Architecture.Components;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.Pages;
using HomeTools.Messenger;
using HomeTools.Other;
using UnityEngine;

namespace Architecture.CalendarModule
{
    //Calendar icon in Main Area and Track Area
    public class Calendar
    {
        // Link to calendar view
        private CalendarTrackModule CalendarTrackModule => AreasLocator.Instance.CalendarTrackModule;
        
        public HomeDay CurrentDay { set; get; } // Struct with current day
        public static HomeDay Today => OtherHTools.GetDayBySystem(DateTime.Today);
        public bool TodayIsCurrent => Today.Equals(CurrentDay);

        private string messageUpdatedDay;

        // Animation names - plays when user touches this icon
        private const string animationIconTo = "CalendarDayTo";
        private const string animationIconFrom = "CalendarDayFrom";
        
        private readonly MainButtonJob mainButtonJob; // Icon button
        private readonly Animation animation; // Animation component of icon
        private TrackCalendarSession trackCalendarSession;
        private readonly RectTransform handler;
        private readonly Action calendarUpdated; // Calendar updated action
        private readonly Action updateGeneratorAction; // Update available days 

        // Create calendar icon
        public Calendar(RectTransform icon, Action calendarUpdated, Action updateGeneratorAction)
        {
            // Save components
            this.calendarUpdated = calendarUpdated;
            this.updateGeneratorAction = updateGeneratorAction;
            animation = icon.GetComponent<Animation>();
            
            // Create button component
            handler = icon.Find("Handler").GetComponent<RectTransform>();
            mainButtonJob = new MainButtonJob(icon, TouchedButton, handler.gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour();
        }

        // Setup current day as today
        public void SetCurrentDayAsToday()
        {
            CurrentDay = Today;
        }

        // Call actions and etc. when touched
        private void TouchedButton()
        {
            // Update info before open calendar view
            updateGeneratorAction.Invoke(); 
            // Create calendar session
            trackCalendarSession = new TrackCalendarSession(CurrentDay, CalendarTracked);
            // Open calendar view
            CalendarTrackModule.Open(GetWorldAnchoredPosition(), trackCalendarSession);
            // Play animation of icon
            PlayAnimation(true);
            // Lock page scroll
            PageScroll.Instance.Enabled = false;
        }

        // Call when calendar view closed with new day
        private void CalendarTracked(HomeDay homeDay)
        {
            PlayAnimation(false);
            CurrentDay = homeDay;
            calendarUpdated.Invoke();
        }
        
        // Get world positions of icon and setup it to start point of open calendar view
        private Vector2 GetWorldAnchoredPosition()
        {
            var parent = handler.transform.parent;

            handler.transform.SetParent(MainCanvas.RectTransform);
            var result = handler.anchoredPosition;
            handler.transform.SetParent(parent);

            return result;
        }

        // Play animation of icon
        private void PlayAnimation(bool open) => animation.CrossFade(open ? animationIconFrom : animationIconTo);

        // If need to send message when calendar updated
        public void SetMessageForUpdates(string message) => messageUpdatedDay = message;
        private void SendMessageUpdated() => MainMessenger.SendMessage(messageUpdatedDay);
    }
}
