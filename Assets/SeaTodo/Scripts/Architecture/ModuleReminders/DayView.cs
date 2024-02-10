using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.ModuleTrackTime;
using Architecture.Other;
using Architecture.TextHolder;
using HomeTools.Other;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using Packages.HomeTools.Source.Design;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleReminders
{
    // Day of the week component with reminder
    public class DayView
    {
        // Link to track time view
        private ModuleTrackTime.ModuleTrackTime ModuleTrackTime => AreasLocator.Instance.ModuleTrackTime;
        
        private readonly RectTransform rectTransform; //  Rect object of day item
        private readonly Text dayText; // Text of day name
        private readonly ReminderHandlePart reminderHandlePart; // Handle part of day item
        private readonly ReminderAnimation reminderAnimation; // Animation part of day item
        private readonly RectTransform buttonPart; // Rect object of button
        private readonly UIColorSync uiColorSync; // Animation component of color for name of day
        
        private WeekInfo.DayOfWeek currentDayOfWeek; // Week day of item
        private readonly Text timeInfo; // Reminders time
        
        private HomeTime currentTime; // Current reminder time
        private bool tracked; // Tracked reminder flag
        private bool activeDay; // Has reminder flag
        
        private bool restDay; // Day without reminder flag

        // Create and setup
        public DayView(RectTransform parent)
        {
            // Copy day item object and setup
            rectTransform = SceneResources.GetPreparedCopy("Reminders Day View").GetComponent<RectTransform>();
            rectTransform.SetParent(parent);
            rectTransform.SetRectTransformAnchorHorizontal(0, 0);

            // Setup text of day name
            dayText = rectTransform.Find("Day").GetComponent<Text>();
            dayText.color = ThemeLoader.GetCurrentTheme().RemindersModuleDays;
            // Create animation component for day name text
            uiColorSync = new UIColorSync();
            uiColorSync.AddElement(dayText, ThemeLoader.GetCurrentTheme().SecondaryColor);
            uiColorSync.Speed = 0.2f;
            uiColorSync.SpeedMode = UIColorSync.Mode.Lerp;
            uiColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiColorSync, AppSyncAnchors.RemindersModule);

            // Setup text of reminder
            timeInfo = rectTransform.Find("Time").GetComponent<Text>();
            AppTheming.AddElement(timeInfo, ColorTheme.SecondaryColor, AppTheming.AppItem.RemindersModule);
            
            // Create button component
            buttonPart = rectTransform.Find("Button").GetComponent<RectTransform>();
            reminderHandlePart = new ReminderHandlePart(rectTransform.Find("Handle").gameObject, timeInfo.gameObject, this);
            SyncWithBehaviour.Instance.AddObserver(reminderHandlePart, AppSyncAnchors.RemindersModule);
            
            // Setup circle image of reminder
            var active = buttonPart.Find("Active").GetComponent<RectTransform>();
            AppTheming.AddElement(active.GetComponent<SVGImage>(), ColorTheme.SecondaryColor, AppTheming.AppItem.RemindersModule);
            
            // Setup icon image of reminder
            var done = active.Find("Done").GetComponent<RectTransform>();
            AppTheming.AddElement(done.GetComponent<SVGImage>(), ColorTheme.ImagesColor, AppTheming.AppItem.RemindersModule);
            
            // Create reminder animation component 
            reminderAnimation = new ReminderAnimation(active, done, timeInfo);
        }

        // Setup Y position of day item
        public void SetPosition(float positionHeight)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, positionHeight);
        }

        // Setup day of week
        public void SetupDay(WeekInfo.DayOfWeek dayOfWeek)
        {
            currentDayOfWeek = dayOfWeek;
            dayText.text = TextHolderTime.DaysOfWeekFull(currentDayOfWeek);
        }

        // Setup reminder state of day item
        public void SetupState(bool active, HomeTime homeTime)
        {
            restDay = AppCurrentSettings.DayOff == currentDayOfWeek;

            activeDay = active;
            tracked = activeDay;
            
            ColorizeDayImmediately(activeDay);
            
            RenameDayText();

            reminderHandlePart.SetDefaultState(activeDay);
            reminderAnimation.SetDefaultState(activeDay);

            UpdateNotificationText(homeTime);
        }

        // Update reminder state of day item
        public void UpdateActivateState(bool active)
        {
            if (!tracked && !activeDay && active)
            {
                var (time, constructed) = ModuleTrackTime.LastConstructedTime;

                if (constructed)
                    UpdateNotificationText(time);
            }
            
            activeDay = active;
            
            if (activeDay)
            {
                reminderAnimation.SetApplied();
                ColorizeDay(true);
            }
            else
            {
                reminderAnimation.SetToEmpty();
                ColorizeDay(false);
            }
        }

        // Open track time view
        public void OpenTimeTracker()
        {
            var session = new TrackTimeSession(currentTime, UpdateNotificationTextWithAnimation);
            ModuleTrackTime.Open(GetWorldAnchoredPosition(), session);
        }

        // Get reminder time
        public (bool active, HomeTime time) GetNotification() => (activeDay, currentTime);

        // Update day item with animation
        private void UpdateNotificationTextWithAnimation(HomeTime homeTime)
        {
            UpdateNotificationText(homeTime);
            reminderAnimation.UpdateTimeAnimation();
            tracked = true;
        }

        // Update notification text
        private void UpdateNotificationText(HomeTime homeTime)
        {
            currentTime = homeTime;

            var englishFormat = AppCurrentSettings.EnglishFormat;
            string time;

            if (englishFormat)
            {
                var (hours, minutes, am) = homeTime.GetTime(true);
                time = $"{hours}:{minutes:00} {(am ? AppTimeCustomization.Am : AppTimeCustomization.Pm)}";
            }
            else
            {
                var (hours, minutes, am) = homeTime.GetTime();
                time = $"{hours}:{minutes:00}";
            }

            timeInfo.text = time;
        }

        // Start animation component of day item
        private void ColorizeDay(bool active)
        {
            uiColorSync.SetTargetByDynamic(active ? 1 : 0);
            uiColorSync.Run();
        }
        
        // Set state of animation component of day item immediately
        private void ColorizeDayImmediately(bool active)
        {
            uiColorSync.SetColor(active ? 1 : 0);
            uiColorSync.Stop();
        }

        // Update text of day item
        private void RenameDayText()
        {
            if (!restDay)
            {
                dayText.text = TextHolderTime.DaysOfWeekFull(currentDayOfWeek);
            }

            if (restDay)
            {
                dayText.text = $"{TextHolderTime.DaysOfWeekFull(currentDayOfWeek)} ({TextLocalization.GetLocalization(TextKeysHolder.DayOfRest)})";
            }
        }

        // Get position for open track time view
        private Vector2 GetWorldAnchoredPosition()
        {
            var rt = timeInfo.rectTransform;
            var parent = timeInfo.transform.parent;

            timeInfo.transform.SetParent(MainCanvas.RectTransform);

            var result = rt.anchoredPosition + new Vector2(MainCanvas.RectTransform.sizeDelta.x / 2 + 
                                                                               rt.sizeDelta.x / 3, 0);

            timeInfo.transform.SetParent(parent);

            return result;
        }
    }
}
