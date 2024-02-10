using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Other;
using HomeTools.Other;
using Calendar = Architecture.CalendarModule.Calendar;

namespace Architecture.WorkArea.Activity
{
    // Component for generate data of weeks
    public static class WeekDataGenerator
    {
        // Default page of calendar
        public static int DefaultStep { get; private set; }
        
        // Date started
        public static HomeDay DateStarted { get; private set; }
        // Today date
        public static HomeDay TodayDate { get; private set; }
        // Today clamped to end of current week
        public static HomeDay ClampToday { get; private set; }
        // Started day clamped to first day of week
        public static HomeDay ClampStart { get; private set; }
        // Cached data
        public static readonly Dictionary<int, int> CacheData = new Dictionary<int, int>();
        
        // Create and setup
        static WeekDataGenerator()
        {
            UpdateDates();
            CreateCashedData();
        }
        // Get step by day
        public static int GetStepByDay(HomeDay homeDay) => (int)(OtherHTools.DaysDistance(homeDay, ClampStart) / 7f);

        // Update dates for statistics
        public static void UpdateDates()
        {
            var allFlows = GetAllFlows();
            if (allFlows.Count > 0)
            {
                var startedByFlows = allFlows.Min(e => e.DateStarted);
                DateStarted = FlowExtensions.IntToHomeDay(startedByFlows);
            }
            else
            {
                DateStarted = AppData.AppInstalledDate;
            }

            TodayDate = Calendar.Today;

            var daysOrder = WeekInfo.DaysOrder();

            var startDayOfWeek = DateStarted.DayOfWeek;
            ClampStart = DateStarted;
            
            var newClamp = ClampStart;
            newClamp.AddDays(-Array.IndexOf(daysOrder, startDayOfWeek));
            ClampStart = newClamp;

            var currentDayOfWeek = TodayDate.DayOfWeek;
            newClamp = TodayDate;
            newClamp.AddDays(6 - Array.IndexOf(daysOrder, currentDayOfWeek));
            ClampToday = newClamp;

            
            if (TodayDate > DateStarted || TodayDate == DateStarted)
                DefaultStep = OtherHTools.DaysDistance(ClampToday, ClampStart) / 7;
            else
            {
                var tempClampToday = ClampToday;
                tempClampToday.AddDays(-6);
                if (ClampStart != tempClampToday)
                    DefaultStep = -OtherHTools.DaysDistance(ClampStart, ClampToday) / 7 - 1;
            }
        }

        // Update cached data
        private static void CreateCashedData()
        {
            var flows = GetAllFlows();
            var cacheDataSet = flows.Select(e => e.GetDaysByData());
            
            foreach (var set in cacheDataSet)
            foreach (var i in set)
            {
                var value = i.Value > 0 ? 1 : 0;
                var key = i.Key.HomeDayToInt();
                if (CacheData.ContainsKey(key))
                    CacheData[key] += value;
                else
                    CacheData.Add(key, value);
            }
        }
        
        // Update cached data of page
        public static void UpdateCashedDataInStep(int step)
        {
            var flows = GetAllFlows();
            var first = ClampStart;
            first.AddDays(step * 7);
            
            var days = new HomeDay[7];
            for (var i = 0; i < days.Length; i++)
            {
                days[i] = first;
                days[i].AddDays(i);
            }

            for (var i = 0; i < days.Length; i++)
            {
                var key = days[i].HomeDayToInt();
                CacheData[key] = 0;
                foreach (var flow in flows)
                    CacheData[key] += flow.GoalData.ContainsKey(key) && flow.GoalData[key] > 0 ? 1 : 0;
            }
        }

        // Get all tasks
        public static Flow[] GetAllFlowsOut() => GetAllFlows().ToArray();

        // Get all tasks
        private static List<Flow> GetAllFlows()
        {
            var flows = AppData.GetCurrentGroup().Flows.ToList();
            
            var maxOrder = 0;
            if (flows.Count > 0)
                maxOrder = flows.Max(e => e.Order) + 1;

            var archivedFlows = AppData.GetCurrentGroup().ArchivedFlows.OrderByDescending(e=>e.DateFinished).ToArray();

            for (var i = 0; i < archivedFlows.Length - 1; i++)
            {
                if (archivedFlows[i].DateFinished == archivedFlows[i + 1].DateFinished &&
                    archivedFlows[i].Id < archivedFlows[i + 1].Id)
                {
                    var backup = archivedFlows[i];
                    archivedFlows[i] = archivedFlows[i + 1];
                    archivedFlows[i + 1] = backup;
                }
            }
            
            for (var i = 0; i < archivedFlows.Length; i++)
            {
                archivedFlows[i].Order = (byte)(maxOrder + i);
                flows.Add(archivedFlows[i]);
            }
            
            return flows;
        }
        
        // Get number of week of year by date
        public static int WeekOfYear(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            var firstDayOfWeek = AppCurrentSettings.DaysFromSunday ? DayOfWeek.Sunday : DayOfWeek.Monday;
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek);
        }
    }
}
