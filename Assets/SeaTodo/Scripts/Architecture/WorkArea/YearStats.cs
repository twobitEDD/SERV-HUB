using System;
using AppSettings.StatusBarSettings;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Handling;
using HomeTools.Messenger;
using HomeTools.Other;
using HomeTools.Source.Calendar;
using MainActivity;
using MainActivity.AppBar;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Architecture.WorkArea
{
    // Component of year view in main page
    public class YearStats
    {
        // Link to app bar
        private AppBar AppBar() => AreasLocator.Instance.AppBar;
        // Link to activity page
        private StatisticsArea.StatisticsArea StatisticsArea() => AreasLocator.Instance.StatisticsArea;
        
        private HandleObject objectHandle; // Handle component
        public RectTransform RectTransform { get; } // Rect of view

        // Ui elements
        private readonly Text leftDays;
        private readonly Text date;

        // Position params
        private static float ShiftByAppBar => -17f -StatusBarSettings.Height;
        private const float yearRadius = 165.7f;

        // Create and setup
        public YearStats()
        {
            RectTransform = SceneResources.Get("Year stats").GetComponent<RectTransform>();
            leftDays = RectTransform.Find("Left days").GetComponent<Text>();
            date = RectTransform.Find("Date").GetComponent<Text>();
            SetupYearInfo();
            MainMessenger.AddMember(UpdateByLanguage, AppMessagesConst.LanguageUpdated);
        }

        // Setup position of year view
        public void SetupAnchoredPosition()
        {
            RectTransform.anchoredPosition = new Vector2(0, 
                AppBarMaterial.RectTransform.anchoredPosition.y - 
                AppBarMaterial.RectTransform.sizeDelta.y / 2 - 
                ShiftByAppBar -
                yearRadius
            );
        }

        // Update year view by actual info
        private void SetupYearInfo()
        {
            var currentData = SceneDataTransfer.CurrentData;
            var year = currentData.Year.ToString();
            RectTransform.Find("Year").GetComponent<Text>().text = year;

            leftDays.text = $"{GetDaysLeft(currentData)} {FlowSymbols.DaysFull}";
            date.text = $"{string.Format($"{currentData:dd}").TrimStart('0')} {CalendarNames.GetMonthShortName(currentData.Month)}";

            RectTransform.Find("Circle").GetComponent<Image>().fillAmount = GetYearPercent(currentData);

            var dynamicCircle = RectTransform.Find("Dynamic circle").GetComponent<RectTransform>();;

            var angle = 360 * GetYearPercent(currentData);
            dynamicCircle.anchoredPosition = new Vector2(
                yearRadius * Mathf.Cos(-OtherHTools.DegreesToRadians(angle - 90)), 
                yearRadius * Mathf.Sin(-OtherHTools.DegreesToRadians(angle - 90)));

            var background = RectTransform.Find("Background").gameObject;
            objectHandle = new HandleObject(background);
            objectHandle.AddEvent(EventTriggerType.PointerClick, OpenStatisticsArea);
        }
    
        // Get left days
        private static int GetDaysLeft(DateTime time)
        {
            var all = DateTime.MaxValue.DayOfYear;
            var current = time.DayOfYear;

            return all - current;
        }
    
        // Get year percentage of passed days
        private static float GetYearPercent(DateTime time)
        {
            var all = DateTime.MaxValue.DayOfYear;
            float current = time.DayOfYear;

            return current / all;
        }

        // Open activity page method
        private void OpenStatisticsArea()
        {
            AppBar().OpenStatisticsMode(TextKeysHolder.Activity);
            PageTransitionTemplates.OpenPageFromWorkArea(StatisticsArea().SetupParents, 
                StatisticsArea().SetupToPage);
        }

        // Update view by actual language
        private void UpdateByLanguage()
        {
            var currentData = SceneDataTransfer.CurrentData;
            leftDays.text = $"{GetDaysLeft(currentData)} {FlowSymbols.DaysFull}";
            date.text = $"{string.Format($"{currentData:dd}").TrimStart('0')} {CalendarNames.GetMonthShortName(currentData.Month)}";
        }
    }
}
