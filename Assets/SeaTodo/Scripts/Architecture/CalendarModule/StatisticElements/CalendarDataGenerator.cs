using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Other;
using Architecture.Statistics;
using HomeTools.Other;
using UnityEngine;

namespace Architecture.CalendarModule.StatisticElements
{
    // Create data for calendar pages
    public class CalendarDataGenerator
    {
        public static int DefaultStep { get; private set; } // Default page of calendar
        
        private static HomeDay TodayDate; // Today date

        //Create generator
        public CalendarDataGenerator() => UpdateDates();

        // Update dates
        public static void UpdateDates()
        {
            TodayDate = Calendar.Today;
            DefaultStep = 0;
        }
        
        // Update days of calendar by flow (task)
        public static void UpdateDatesByFlow(Flow flow)
        {
            if (flow == null)
                return;
            
            TodayDate = Calendar.Today;
            DefaultStep = 0;
        }

        // Get page of calendar by day
        public int GetStepByHomeDay(HomeDay homeDay) => OtherHTools.MonthDistance(TodayDate, homeDay);
        
        
        // Create structure with data of month by page number (month)
        public GraphDataStruct CreateStruct(int step)
        {
            // Create first day of month
            var first = new HomeDay(TodayDate.Year, TodayDate.Month, (byte) 1);
            first.AddMonths(step);

            // create order of days by week
            var daysOrder = WeekInfo.DaysOrder();
            var shift = Array.IndexOf(daysOrder, first.DayOfWeek);

            // calculate sides of months (preview and next month)
            var previewMonth = first;
            previewMonth.AddMonths(-1);
            var nextMonth = first;
            nextMonth.AddMonths(1);
            
            var daysInLastMonth = DateTime.DaysInMonth(previewMonth.Year, previewMonth.Month);
            var daisInThisMonth = DateTime.DaysInMonth(first.Year, first.Month);

            // Create array for days in page
            var daysCount = 42;
            var dataStruct = new GraphDataStruct
            {
                GraphElementsDescription = new List<int>[daysCount],
                GraphElementActive = new bool[daysCount],
                GraphElementsInfo = new float[daysCount],
                FirstElement = -1,
                Highlighted = step == 0 ? 1 : -1,
            };
            
            // fill days before this month
            for (var i = 0; i < shift; i++)
            {
                dataStruct.GraphElementsDescription[i] = new List<int>()
                {
                    daysInLastMonth - shift + i + 1,
                    previewMonth.Month,
                    previewMonth.Year,
                };
            }
            
            // fill days
            for (var i = 0; i < daisInThisMonth; i++)
            {
                var id = i + shift;
                dataStruct.GraphElementsDescription[id] = new List<int>()
                {
                    i + 1,
                    first.Month,
                    first.Year,
                };
                
                dataStruct.GraphElementActive[id] = step < 0 || (step == 0 && i < TodayDate.Day);
            }
            
            // fill days after this month
            for (var i = 0; i < daysCount - (daisInThisMonth + shift); i++)
            {
                var id = daisInThisMonth + shift + i;
                dataStruct.GraphElementsDescription[id] = new List<int>()
                {
                    i + 1,
                    nextMonth.Month,
                    nextMonth.Year,
                };
            }

            return dataStruct;
        }
    }
}
