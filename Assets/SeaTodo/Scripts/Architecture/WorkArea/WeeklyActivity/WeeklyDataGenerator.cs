using System.Collections.Generic;
using System.Linq;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.WorkArea.Activity;

namespace Architecture.WorkArea.WeeklyActivity
{
    // Create data for weekly activity pages
    public class WeeklyDataGenerator
    {
        // Started date pf first track
        private static HomeDay DateStarted => WeekDataGenerator.DateStarted;
        // Today date
        private static HomeDay TodayDate => WeekDataGenerator.TodayDate;
        // Updated started date for graphic
        private static HomeDay ClampStart => WeekDataGenerator.ClampStart;
        
        // Cached data
        private static Dictionary<int, int> CacheData => WeekDataGenerator.CacheData;
        
        // Update by day
        public void Update(HomeDay day)
        {
            WeekDataGenerator.UpdateDates();
            var step = WeekDataGenerator.GetStepByDay(day);
            WeekDataGenerator.UpdateCashedDataInStep(step);
        }

        // Create data structure for graphic
        public GraphDataStruct CreateStruct(int step)
        {
            // Setup day objects for graphic
            var days = new HomeDay[7];
            var daysInStep = step * 7;
            for (var i = 0; i < days.Length; i++)
            {
                days[i] = ClampStart;
                days[i].AddDays(daysInStep + i);
            }

            // Create data structure
            var dataStruct = new GraphDataStruct
            {
                GraphElementsDescription = new List<int>[7],
                GraphElementActive = new bool[7],
                GraphElementsInfo = new float[7],
                FirstElement = -1,
                Highlighted = -1,
                NoData = CacheData.Count == 0 || TodayDate < DateStarted
            };

            // Get active tasks
            var activeFlows = WeekDataGenerator.GetAllFlowsOut();
            // Temp list for data
            var flowsByDays = new List<Flow>[days.Length];
            // Setup structure data
            for (var i = 0; i < dataStruct.GraphElementsDescription.Length; i++)
            {
                // Get tasks that in day
                flowsByDays[i] = GetFlowsByDay(activeFlows, days[i]);
                
                dataStruct.GraphElementsDescription[i] = new List<int>
                {
                    days[i].Year, 
                    days[i].Month, 
                    days[i].Day,
                    flowsByDays[i].Count,
                    CountOfTrackedFlows(flowsByDays[i], days[i]),
                };
            }
            
            // Setup activity of days 
            for (var i = 0; i < dataStruct.GraphElementActive.Length; i++)
            {
                dataStruct.GraphElementActive[i] = days[i] >= DateStarted && days[i] <= TodayDate;
            }
            
            // Setup completed percentage of all tasks for each day
            for (var i = 0; i < dataStruct.GraphElementsInfo.Length; i++)
            {
                dataStruct.GraphElementsInfo[i] = CompletedPercentageByDay(flowsByDays[i], days[i]);
            }
            
            // Mark as highlighted today date
            for (var i = 0; i < days.Length; i++)
            {
                if (days[i] == TodayDate)
                    dataStruct.Highlighted = i;
            }

            return dataStruct;
        }

        // Calculate percentage of completed tasks for day
        private float CompletedPercentageByDay(IEnumerable<Flow> flows, HomeDay day)
        {
            var percentage = 0f;
            var dayInt = day.HomeDayToInt();

            var flowsArray = flows as Flow[] ?? flows.ToArray();
            
            foreach (var flow in flowsArray)
            {
                if (!flow.GoalData.ContainsKey(dayInt)) 
                    continue;
                
                var goalInt = FlowInfoAll.GetGoalByOriginFlowInt(flow.Type, flow.GoalInt);
                var progressInt = flow.GetIntProgress(day);
                percentage += (float) progressInt / goalInt;
            }

            if (flowsArray.Length > 0)
                percentage /= flowsArray.Length;

            return percentage;
        }

        // Get tasks list that active in chosen day
        private List<Flow> GetFlowsByDay(IEnumerable<Flow> flows, HomeDay day)
        {
            var result = new List<Flow>();
            var dayInt = day.HomeDayToInt();
            
            foreach (var activeFlow in flows)
            {
                if (activeFlow.DateStarted <= dayInt && !activeFlow.Finished)
                {
                    result.Add(activeFlow);
                    continue;
                }
                    
                if (activeFlow.DateStarted <= dayInt && activeFlow.Finished && activeFlow.DateFinished >= dayInt)
                {
                    result.Add(activeFlow);
                }
            }

            return result;
        }

        // Get count of tasks that has tracked progress in chosen day
        private int CountOfTrackedFlows(IEnumerable<Flow> flows, HomeDay day)
        {
            var result = 0;
            var dayInt = day.HomeDayToInt();

            foreach (var flow in flows)
            {
                if (flow.GoalData.ContainsKey(dayInt))
                    result += flow.GoalData[dayInt] > 0 ? 1 : 0;
            }

            return result;
        }
    }
}
