using System;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Source.Calendar;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Architecture.CalendarModule.StatisticElements
{
    // Page of calendar
    public class ViewAdditionalElements : IViewAdditional
    {
        private CalendarTrackModule CalendarTrackModule => AreasLocator.Instance.CalendarTrackModule;
        private RectTransform rectTransform; // rect of this page
        private Text month; // Text of month
        private Text[] weeks = new Text[0]; // Text items of days of week

        private CalendarDayItem[] calendarDayItem = new CalendarDayItem[0]; // Days array
        private SelectiveDayItem selectiveDayItem; // Circle (Selected day)

        private float viewPlaceWidth; // Width of page

        // Animation alpha channel component of all page elements
        public UIAlphaSync UIAlphaSyncLocal;

        // Setup page method
        public void Setup(RectTransform view)
        {
            rectTransform = view;
            
            // Create alpha channel animation component
            UIAlphaSyncLocal = new UIAlphaSync();
            UIAlphaSyncLocal.Speed = 0.2f;
            UIAlphaSyncLocal.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(UIAlphaSyncLocal, AppSyncAnchors.CalendarObject);
            
            // Setup other params and components
            month = view.Find("Month").GetComponent<Text>();
            viewPlaceWidth = view.sizeDelta.x;
            UIAlphaSyncLocal.AddElement(month);
            
            GenerateWeeks(); // Generate items for days of week
            CreateSelectiveItem(); // Create circle for selected day
            GenerateDays(); // Create array of days for this calendar page
            
            UIAlphaSyncLocal.PrepareToWork(); // Prepare to using animation component
            
            //Add Method for update selected day from other plaes
            MainMessenger.AddMember(UpdateSelectiveInJob, AppMessagesConst.UpdateSelectiveInCalendar);
        }

        // Setup scroll component for each day item
        public void SetupScroll(StatisticsScroll statisticsScroll)
        {
            foreach (var dayItem in calendarDayItem) dayItem.SetupScrollEvents(statisticsScroll);
        }

        // Update page
        public void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next)
        {
            UpdateMonthText(current); // Update name of month
            UpdateWeekOrder(); // Update order of week day names
            UpdateDays(current); // Update day items
        }

        // Update page
        public void Update(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            FullUpdate(preview, current, next);

        // Update name of month
        private void UpdateMonthText(GraphDataStruct graphDataStruct)
        {
            // Select color for month by if current month
            var color = graphDataStruct.Highlighted == -1
                ? ThemeLoader.GetCurrentTheme().CalendarMonthPassive
                : ThemeLoader.GetCurrentTheme().CalendarMonthActive;
            month.color = color;

            // Get from data month
            var monthInfo = new DateTime(
                graphDataStruct.GraphElementsDescription[20][2],
                graphDataStruct.GraphElementsDescription[20][1], 1);

            month.text = $"{CalendarNames.GetMonthFullName(monthInfo.Month)} {monthInfo:yyyy}";
            month.enabled = !graphDataStruct.EmptyActivity();
        }

        // Update days of month by data
        private void UpdateDays(GraphDataStruct graphDataStruct)
        {
            UpdateActiveWeekDays(!graphDataStruct.EmptyActivity()); // Update week names
            selectiveDayItem.SetActive(false); // Deactivate circle (selected day)
            
            // Check if month is empty
            if (graphDataStruct.EmptyActivity())
            {
                foreach (var day in calendarDayItem)
                    day.Disable();

                return;
            }
            
            // Try get current day from session info
            var currentDay = CalendarTrackModule?.TrackCalendarSession?.HomeDay;

            // Setup each day by data
            for (var i = 0; i < calendarDayItem.Length; i++)
            {
                // Get info of day date
                var info = graphDataStruct.GraphElementsDescription[i];
                if (info == null) continue;
                // Update day
                calendarDayItem[i].UpdateDay(info[2], info[1], info[0]);
                calendarDayItem[i].UpdateView(graphDataStruct.GraphElementActive[i]);

                if (currentDay == null)
                    continue;

                // Setup day as selected if is current
                if (currentDay.Value.Year == info[2] && currentDay.Value.Month == info[1] &&
                    currentDay.Value.Day == info[0])
                {
                    selectiveDayItem.SetActive(true);
                }
            }
        }

        // Generate day names of week
        private void GenerateWeeks()
        {
            weeks = new Text[7];

            weeks[0] = rectTransform.Find("Week").GetComponent<Text>();
            var color = ThemeLoader.GetCurrentTheme().CalendarModuleWeeks;

            // Create object
            for (var i = 1; i < weeks.Length; i++)
                weeks[i] = Object.Instantiate(weeks[0], rectTransform);

            // Attach to Color Theming
            foreach (var week in weeks)
            {
                AppTheming.AddElement(week, ColorTheme.CalendarModuleWeeks, AppTheming.AppItem.Other);
                week.color = color;
            }

            // Calculate info for generate
            var widthStep = viewPlaceWidth / ((weeks.Length) * 2);
            var firstPosition = -(viewPlaceWidth / 2) + widthStep;

            // Setup position for items
            for (var i = 0; i < weeks.Length; i++)
            {
                weeks[i].rectTransform.anchoredPosition = new Vector2(firstPosition + widthStep * i * 2,
                    weeks[i].rectTransform.anchoredPosition.y);
            }
            
            // Add items to alpha channel animation component
            for (var i = 0; i < weeks.Length; i++)
                UIAlphaSyncLocal.AddElement(weeks[i]);
        }

        // Update order of days of week
        private void UpdateWeekOrder()
        {
            var order = WeekInfo.DaysOrder();
            for (var i = 0; i < weeks.Length; i++)
                weeks[i].text = TextHolderTime.DaysOfWeekShort(order[i]);
        }

        // Update activity of week day names
        private void UpdateActiveWeekDays(bool active)
        {
            if (weeks[0].enabled == active)
                return;

            foreach (var text in weeks)
                text.enabled = active;
        }

        // Generate days items
        private void GenerateDays()
        {
            var days = new Text[42];

            days[0] = rectTransform.Find("Day").GetComponent<Text>();

            for (var i = 1; i < days.Length; i++)
                days[i] = Object.Instantiate(days[0], rectTransform);

            var widthStep = viewPlaceWidth / ((weeks.Length) * 2);
            var firstPosition = -(viewPlaceWidth / 2) + widthStep;
            const float firstPositionHeight = -185f;
            const float heightShift = 73f;

            for (var i = 0; i < days.Length; i++)
            {
                var xStep = i;
                var yStep = 0;
                while (xStep > 6)
                {
                    xStep -= 7;
                    yStep++;
                }

                days[i].rectTransform.anchoredPosition = new Vector2(firstPosition + widthStep * xStep * 2,
                    firstPositionHeight - yStep * heightShift);
            }

            calendarDayItem = new CalendarDayItem[days.Length];
            for (var i = 0; i < days.Length; i++)
                calendarDayItem[i] = new CalendarDayItem(days[i], selectiveDayItem);
            
            for (var i = 0; i < days.Length; i++)
                UIAlphaSyncLocal.AddElement(days[i]);
        }

        // Create selective icon "circle" 
        private void CreateSelectiveItem()
        {
            var foundObject = rectTransform.Find("Selective").GetComponent<SVGImage>();
            foundObject.color = ThemeLoader.GetCurrentTheme().SecondaryColor;
            AppTheming.AddElement(foundObject, ColorTheme.SecondaryColor, AppTheming.AppItem.Other);
            selectiveDayItem = new SelectiveDayItem(foundObject);
        }

        // Set activity for selective circle by days info
        private void UpdateSelectiveInJob()
        {
            var hasSelective = false;
            
            foreach (var dayItem in calendarDayItem)
            {
                if (!hasSelective)
                    hasSelective = dayItem.CheckToSelective();
                else
                    break;
            }
            
            selectiveDayItem.SetActive(hasSelective);
        }
    }
}