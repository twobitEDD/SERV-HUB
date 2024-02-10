using System;
using System.Linq;
using Architecture.Data.Structs;
using Architecture.Other;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using Architecture.TextHolder;
using Architecture.WorkArea.Activity;
using HomeTools.Messenger;
using HomeTools.Source.Calendar;
using HTools;
using InternalTheming;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.WorkArea.WeeklyActivity
{
    // Page of weekly activity
    public class ViewAdditionalElements : IViewAdditional
    {
        // UI elements
        private RectTransform viewRect; 
        private Text mothAndYear;
        private Text weekNumber;
        private Text completedPercentage;
        private SVGImage completedIcon;
        private SVGImage completedIconDone;

        // Position params
        private const float previewsPositionY = -45;
        private const float widthDistance = 97;
        // Example of day view
        private RectTransform originFlowPreview;
        // Array of day previews
        private DayPreview[] dayPreviews;
        // Component of scroll
        private StatisticsScroll statisticsScroll;
        // Component for text animation that in bottom part of view
        private TextUpdateAnimation completedTextUpdateAnimation;
        // Current page data structure
        private GraphDataStruct currentDataStruct;
        private float localPercentage; // Percentage of completed of all task

        // Setup
        public void Setup(RectTransform view)
        {
            // Find UI objects
            viewRect = view;
            originFlowPreview = view.parent.parent.parent.Find("Pool/DayView").GetComponent<RectTransform>();
            mothAndYear = view.Find("Month And Year").GetComponent<Text>();
            weekNumber = view.Find("Week Number").GetComponent<Text>();
            completedPercentage = view.Find("Completed Percentage").GetComponent<Text>();
            completedIcon = view.Find("Completed Icon").GetComponent<SVGImage>();
            completedIconDone = completedIcon.rectTransform.Find("Done").GetComponent<SVGImage>();
            
            // Create text animation component
            completedTextUpdateAnimation = new TextUpdateAnimation(completedPercentage, "{0}%", "{0:0.0000}");
            SyncWithBehaviour.Instance.AddObserver(completedTextUpdateAnimation, AppSyncAnchors.WorkAreaWeeklyActivity);
            
            AppTheming.AddElement(weekNumber, ColorTheme.ActivityWeekDayDefault, AppTheming.AppItem.StatisticsArea);
            
            MainMessenger.AddMember(UpdateColorsInDays, string.Format(AppMessagesConst.ColorizedArea, 
                AppTheming.AppItem.WorkArea));
            
            MainMessenger.AddMember(()=> UpdateActions(currentDataStruct, false), AppMessagesConst.LanguageUpdated);
        }

        // Setup scroll
        public void SetupScroll(StatisticsScroll scroll)
        {
            statisticsScroll = scroll;
            CreateDayPreviews();
        }

        // Update by new data
        public void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            UpdateActions(current, false);

        // Update by new data
        public void Update(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            UpdateActions(current, true);

        // Update view
        private void UpdateActions(GraphDataStruct current, bool dynamic)
        {
            // Check for empty
            currentDataStruct = current;
            if (current.EmptyActivity())
            {
                viewRect.gameObject.SetActive(false);
                return;
            }

            // Setup month of week
            var dayMiddleInfo = current.GraphElementsDescription[2];
            mothAndYear.text = $"{CalendarNames.GetMonthFullName(dayMiddleInfo[1])} {dayMiddleInfo[0]}";

            // Setup week number
            var middleDate = new DateTime(dayMiddleInfo[0], dayMiddleInfo[1], dayMiddleInfo[2]);
            var weekInt = WeekDataGenerator.WeekOfYear(middleDate);
            weekNumber.text = $"{TextLocalization.GetLocalization(TextKeysHolder.Week)} {weekInt}";

            localPercentage = current.GraphElementsInfo.Sum();
            // Update percentage of all tasks progress
            UpdatePercentage(dynamic);
            // Update day previews
            UpdateDayPreviews(current, dynamic);
            // Activate page object
            viewRect.gameObject.SetActive(true);
        }

        // Update percentage of all tasks progress
        private void UpdatePercentage(bool dynamic)
        {
            if (dynamic)
            {
                completedTextUpdateAnimation.SetupProgress(localPercentage);
            }
            else
            {
                completedTextUpdateAnimation.SetupProgressImmediately(localPercentage);
            }

            // Update colors
            
            var completedColor = localPercentage > 0.00001f
                ? ThemeLoader.GetCurrentTheme().SecondaryColor
                : ThemeLoader.GetCurrentTheme().SecondaryColorD2;

            completedIcon.color = completedColor;
            
            completedColor = localPercentage > 0.00001f
                ? ThemeLoader.GetCurrentTheme().ImagesColor
                : ThemeLoader.GetCurrentTheme().ActivityResultDonePassive;
            
            completedIconDone.color = completedColor;
            
            completedColor = localPercentage > 0.00001f
                ? ThemeLoader.GetCurrentTheme().SecondaryColorD1
                : ThemeLoader.GetCurrentTheme().SecondaryColorD2;
            
            completedPercentage.color = completedColor;
        }
        
        // Update day previews
        private void UpdateDayPreviews(GraphDataStruct current, bool dynamic)
        {
            var daysOrder = WeekInfo.DaysOrder();
            for (var i = 0; i < dayPreviews.Length; i++)
                dayPreviews[i].DayOfWeek = daysOrder[i];

            for (var i = 0; i < dayPreviews.Length; i++)
            {
                var flowsPercentage = (float) current.GraphElementsDescription[i][4] /
                                                        current.GraphElementsDescription[i][3];
                
                var todayDate = new HomeDay(current.GraphElementsDescription[i][0],
                                          current.GraphElementsDescription[i][1],
                                            current.GraphElementsDescription[i][2]);
                
                var today = todayDate == WeekDataGenerator.TodayDate;

                if (!dynamic)
                {
                    dayPreviews[i].UpdateView(current.GraphElementsDescription[i][2], 
                        flowsPercentage, current.GraphElementActive[i], today);
                }
                else
                {
                    dayPreviews[i].UpdateViewDynamic(current.GraphElementsDescription[i][2], 
                        flowsPercentage, current.GraphElementActive[i], today);
                }
            }
        }

        // Generate day previews
        private void CreateDayPreviews()
        {
            const int daysInWeek = 7;
            dayPreviews = new DayPreview[daysInWeek];
            
            for (var i = 0; i < daysInWeek; i++)
            {
                var dayPreview = new DayPreview(originFlowPreview, statisticsScroll);
                dayPreviews[i] = dayPreview;
            }

            var positions = GetPreviewsPositions(daysInWeek);

            for (var i = 0; i < positions.Count(); i++)
            {
                dayPreviews[i].RectTransform.SetParent(viewRect);
                dayPreviews[i].RectTransform.anchoredPosition = positions[i];
            }
        }

        // Create array of positions for previews
        private static Vector2[] GetPreviewsPositions(int count)
        {
            var result = new Vector2[count];
  
            var width = (count - 1) * widthDistance;
            var firstPosition = -width / 2;

            for (var i = 0; i < count; i++)
                result[i] = new Vector2(firstPosition + widthDistance * i, previewsPositionY);

            return result;
        }

        // Update colors of days and progress percentage text
        private void UpdateColorsInDays()
        {
            foreach (var dayPreview in dayPreviews)
                dayPreview.UpdateColors();

            UpdatePercentage(false);
        }
    }
}
