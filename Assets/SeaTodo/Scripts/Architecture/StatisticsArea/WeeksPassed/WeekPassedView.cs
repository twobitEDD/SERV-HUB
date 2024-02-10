using System;
using System.Collections.Generic;
using Architecture.WorkArea.Activity;
using InternalTheming;
using Theming;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Architecture.StatisticsArea.WeeksPassed
{
    // View of passed weeks
    public class WeekPassedView
    {
        // Positions for generate week circles
        private const float startY = -3f;
        private const float shiftY = 67f;
        private const float shiftWeeksX = 63f;
        private const float startCountX = -320f;
        private const float startWeeksX = -260f;

        private Text[] countsLeft; // Left text of weeks count
        private List<Image> weeks; // List of week circles
        private readonly RectTransform weekCurrent; // Circle object for current week

        // Create and setup
        public WeekPassedView(RectTransform rectTransform)
        {
            // Find UI examples
            var exampleCircle = rectTransform.Find("Week Circle").GetComponent<Image>();
            var exampleCount = rectTransform.Find("Week Count").GetComponent<Text>();
            // Find circle object for current week
            weekCurrent = rectTransform.Find("Week Current").GetComponent<RectTransform>();
            // Generate weeks view
            GenerateCountInfo(exampleCount);
            GenerateWeeksInfo(exampleCircle);
        }

        // Update weeks view
        public void UpdateView()
        {
            // Get active week number
            var weekActive = WeekDataGenerator.WeekOfYear(WeekDataGenerator.ClampToday.GetSystemDay());
            
            // Calculate weeks count in year
            var lastDayOfMonth = DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
            var lastDateOfYear = new DateTime(DateTime.Today.Year, 12, lastDayOfMonth);
            var weeksAll = WeekDataGenerator.WeekOfYear(lastDateOfYear);

            // Get colors for active and passive weeks
            var activeColor = ThemeLoader.GetCurrentTheme().SecondaryColor;
            var passiveColor = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            // Setup activity and colors for weeks
            for (var i = 0; i < weeks.Count; i++)
            {
                if (i < weekActive)
                {
                    weeks[i].enabled = true;
                    weeks[i].color = activeColor;
                }
                else if (i < weeksAll - 1)
                {
                    weeks[i].enabled = true;
                    weeks[i].color = passiveColor;
                }
                else
                {
                    weeks[i].enabled = false;
                }
            }

            // Setup position of circle object for current week
            weekCurrent.anchoredPosition = weeks[weekActive - 1].rectTransform.anchoredPosition;
        }

        // Generate left text for weeks count
        private void GenerateCountInfo(Text example)
        {
            countsLeft = new Text[6];
            countsLeft[0] = example;
            for (var i = 1; i < countsLeft.Length; i++)
                countsLeft[i] = Object.Instantiate(example, example.rectTransform.parent);
            
            for (var i = 0; i < countsLeft.Length; i++)
            {
                countsLeft[i].rectTransform.anchoredPosition = new Vector2(startCountX, startY - shiftY * i);
                AppTheming.AddElement(countsLeft[i], ColorTheme.ViewFlowAreaDescription, AppTheming.AppItem.StatisticsArea);
                countsLeft[i].text = (10 * i).ToString();
            }
        }

        // Generate grid with week circles
        private void GenerateWeeksInfo(Image example)
        {
            var startX = startWeeksX;
            weeks = new List<Image>();
            for (var j = 0; j < 6; j++)
            {
                for (var i = 0; i < 10; i++)
                {
                    var week = Object.Instantiate(example, example.rectTransform.parent);
                    week.rectTransform.anchoredPosition = new Vector2(startX + shiftWeeksX * i, startY - shiftY * j);
                    weeks.Add(week);
                }
            }
            
            example.enabled = false;
        }
    }
}
