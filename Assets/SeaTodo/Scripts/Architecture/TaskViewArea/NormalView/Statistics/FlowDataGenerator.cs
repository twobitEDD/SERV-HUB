using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Statistics;

namespace Architecture.TaskViewArea.NormalView.Statistics
{
    // Create data for task statistics pages
    public class FlowDataGenerator
    {
        public int DefaultStep { get; private set; }  // Default page of statistics

        // Started statistics date
        private HomeDay dateStarted;
        // Today date
        private HomeDay todayDate;

        // Today day clamped to last day of month
        private HomeDay clampToday;
        // Started day clamped to first day of month
        private HomeDay clampStart;
        
        // Data for statistics
        private readonly Dictionary<int, int> cacheData = new Dictionary<int, int>();

        // Current task for statistics
        private Flow currentFlow;

        // Update generator by new task
        public void Update(Flow flow)
        {
            currentFlow = flow;
            
            UpdateDates(flow);
            CreateCachedData(flow);
        }

        // Update dates for statistics by new task
        private void UpdateDates(Flow flow)
        {
            dateStarted = flow.GetStartedDay();
            todayDate = Calendar.Today;

            clampStart = new HomeDay(dateStarted.Year, dateStarted.Month, (byte) 1);

            var daysInMonth = DateTime.DaysInMonth(todayDate.Year, todayDate.Month);
            clampToday = new HomeDay(todayDate.Year, todayDate.Month, daysInMonth);

            if (todayDate > dateStarted || todayDate == dateStarted)
            {
                DefaultStep = ((clampToday.Year - clampStart.Year) * 12) + clampToday.Month - clampStart.Month;
            }
            else
            {
                DefaultStep = 0;
            }
        }

        // Created data cache for statistics
        private void CreateCachedData(Flow flow)
        {
            cacheData.Clear();

            foreach (var i in flow.GoalData.Where(i => i.Value > 0))
                cacheData.Add(i.Key, i.Value);
        }

        // Create structure with data of month by page number (month)
        public GraphDataStruct CreateStruct(int step)
        {
            // Check for current task
            if (currentFlow == null)
                return new GraphDataStruct();
            
            // Setup first day of month
            var first = clampStart;
            first.AddMonths(step);

            // Calculate days count in month
            var daysInMonth = DateTime.DaysInMonth(first.Year, first.Month);
            // Calculate last day of month
            var last = new HomeDay(first.Year, first.Month, daysInMonth);

            // Create data package
            var dataStruct = new GraphDataStruct
            {
                GraphElementsDescription = new List<int>[daysInMonth],
                GraphElementActive = new bool[daysInMonth],
                GraphElementsInfo = new float[daysInMonth],
                FirstElement = -1,
                Highlighted = -1,
                NoData = cacheData.Count == 0 || todayDate < dateStarted
            };

            // Setup id of first active day in statistics page
            dataStruct.FirstElement = dateStarted > first && dateStarted <= last ? dateStarted.Day : -1;
            // Check if statistics page is for current month
            dataStruct.Highlighted = last == clampToday ? 1 : -1;

            // Setup days dates
            for (var i = 0; i < daysInMonth; i++)
            {
                dataStruct.GraphElementsDescription[i] = new List<int>()
                {
                    first.Year,
                    first.Month,
                    currentFlow.Type == FlowType.done ? 1 : 0,
                };
            }

            // Setup activity of days in month
            for (var i = 0; i < daysInMonth; i++)
            {
                dataStruct.GraphElementActive[i] = !dataStruct.NoData;
                
                if ((i < dateStarted.Day - 1 && first == clampStart) || first < clampStart)
                    dataStruct.GraphElementActive[i] = false;

                if ((i >= todayDate.Day && last == clampToday) || last > clampToday)
                {
                    dataStruct.GraphElementActive[i] = false;
                }
            }

            // Get range of task days
            var firstInt = currentFlow.GetStartedDay().HomeDayToInt();
            var lastInt = todayDate.HomeDayToInt();
            
            // Check if cache has range of task days
            var infoInRange = cacheData.Where(e=>e.Key >= firstInt && e.Key <= lastInt).ToArray();
            // Find max tracked count
            var maxInfo = infoInRange.Any() ? infoInRange.Select(e=>e.Value).Max() : 1;

            // Calculate first date fo statistics
            var firstKey = first.Year * 10000 + first.Month * 100;

            // Setup track info from cache to data
            for (var i = 0; i < daysInMonth; i++)
            {
                if (!dataStruct.GraphElementActive[i]) 
                    continue;
                
                var key = firstKey + i + 1;
                if (!cacheData.ContainsKey(key))
                    continue;
                
                dataStruct.GraphElementsInfo[i] = (float)cacheData[key] / maxInfo;
            }
            
            return dataStruct;
        }
    }
}
