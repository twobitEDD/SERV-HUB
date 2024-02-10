using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.CalendarModule;
using Architecture.Data.Structs;
using Architecture.Other;
using HomeTools.Other;
using UnityEngine;

namespace Architecture.Data.Components
{
    // Class with dynamic calculation of user activity
    public class DataFrequencyInfo
    {
        private HomeDay dayFrom; // Calculation of activity from a given day
        private HomeDay dayToday; // Calculation of activity to a given day
        // Given progress for the last 30 days
        private readonly Dictionary<int, HomeDay> last30Days = new Dictionary<int, HomeDay>();
        
        public DataFrequencyInfo() => GenerateFrequencyInfo();

        // Calculate main
        private void GenerateFrequencyInfo()
        {
            // Setup day from and day to
            dayToday = Calendar.Today;
            dayFrom = dayToday;
            dayFrom.AddDays(-30);
            // Collect activity data for the last 30 days
            GenerateLast30Days();
        }

        // Activity update for global package with tasks
        public GroupFrequency RecalculateFrequencyForGroup(FlowGroup group)
        {
            var groupInfo = new GroupFrequency(); // Create data packages
            var actualFlows = GetActualFlows(group); // Get actual tasks
            SetupFlowsInGroup(groupInfo, actualFlows); // Grouping tasks into a batch

            // Calculating the number of days due
            FillDaysCount(groupInfo);
            // Calculating the number of active days due
            FillDaysInTrack(groupInfo);
            // Calculating the percentage of active days due
            FillDaysFlowPercentage(groupInfo);
            
            // Checking for the number of tasks
            if (group.Flows.Length == 0)
                return groupInfo;
            
            // Activity calculation for each task
            FillFlowActivity(groupInfo);
            // Calculation of the weight of each task in general accounting
            FillFlowWeights(groupInfo);
            // Summation of activity taking into account the weights of each task
            FillGroupFrequency(groupInfo);
            
            return groupInfo;
        }

        // Get actual tasks with filter by dates
        private List<Flow> GetActualFlows(FlowGroup group)
        {
            var intDayFrom = dayFrom.HomeDayToInt();
            
            var result = new List<Flow>();
            foreach (var flow in group.Flows)
            {
                if (!flow.Finished) result.Add(flow);
                else if (flow.DateFinished > intDayFrom) result.Add(flow);
            }
            
            foreach (var flow in group.ArchivedFlows)
            {
                if (!flow.Finished) result.Add(flow);
                else if (flow.DateFinished > intDayFrom) result.Add(flow);
            }
            
            return result;
        }

        // Grouping tasks into a batch
        private void SetupFlowsInGroup(GroupFrequency groupFrequency, List<Flow> flows)
        {
            foreach (var flow in flows)
                groupFrequency.FlowsData.Add(new FrequencyFlowsData(flow));
        }
        
        // Calculating the number of days due
        private void FillDaysCount(GroupFrequency groupFrequency)
        {
            foreach (var flowsData in groupFrequency.FlowsData)
            {
                var flow = flowsData.Flow;
                var fromDate = flow.DateStarted < dayFrom.HomeDayToInt() ? dayFrom : flow.GetStartedDay();
                var toDate = flow.Finished ? FlowExtensions.IntToHomeDay(flow.DateFinished) : dayToday;
                var daysDistance = OtherHTools.DaysDistance(toDate, fromDate);
                flowsData.DaysCount = daysDistance;
                
                var startWeekDayPosition = (int) fromDate.DayOfWeek;

                while (daysDistance > 0)
                {
                    flowsData.WeekDaysCount[(WeekInfo.DayOfWeek) startWeekDayPosition]++;
                    startWeekDayPosition++;
                    if (startWeekDayPosition > 6) startWeekDayPosition = 0;
                    
                    daysDistance--;
                }
            }
        }
        
        // Calculating the number of active days due
        private void FillDaysInTrack(GroupFrequency groupFrequency)
        {
            foreach (var flowsData in groupFrequency.FlowsData)
            {
                var flow = flowsData.Flow;
                var dayFromInt = dayFrom.HomeDayToInt();
                
                var fromDate = flow.DateStarted < dayFromInt ? dayFromInt : flow.DateFinished;
                var toDate = flow.Finished ? flow.DateFinished : dayToday.HomeDayToInt();
                var daysInRange = 
                    flow.GoalData.Where(e => e.Key >= fromDate && e.Key <= toDate &&
                                             e.Value > 0).Select(e=>e.Key).ToArray();
                flowsData.DaysTracked = daysInRange.Length;
                
                foreach (var i in daysInRange)
                {
                    if (!last30Days.ContainsKey(i)) continue;
                    flowsData.DaysInTrack[last30Days[i].DayOfWeek] ++;
                }
            }
        }

        // Calculating the percentage of active days due
        private void FillDaysFlowPercentage(GroupFrequency groupFrequency)
        {
            foreach (var flowsData in groupFrequency.FlowsData)
            {
                foreach (var weekDay in flowsData.WeekDaysCount.Keys)
                {
                    if (flowsData.WeekDaysCount[weekDay] == 0)
                        continue;
                    
                    flowsData.DaysFlowPercentage[weekDay] =
                        (float) flowsData.DaysInTrack[weekDay] / flowsData.WeekDaysCount[weekDay];
                }
            }
        }

        // Activity calculation for each task
        private void FillFlowActivity(GroupFrequency groupFrequency)
        {
            var maxDaysCount = groupFrequency.FlowsData.Select(e => e.DaysCount).Max();
            
            foreach (var flowsData in groupFrequency.FlowsData)
            {
                flowsData.FlowActivity = flowsData.DaysTracked / (float) maxDaysCount;
            }
        }

        // Calculation of the weight of each task in general accounting
        private void FillFlowWeights(GroupFrequency groupFrequency)
        {
            var maxActivity = groupFrequency.FlowsData.Select(e => e.FlowActivity).Max();

            foreach (var flowsData in groupFrequency.FlowsData)
            {
                flowsData.Weight = flowsData.FlowActivity / maxActivity;
            }
        }

        // Summation of activity taking into account the weights of each task
        private void FillGroupFrequency(GroupFrequency groupFrequency)
        {
            var weightSum = 0f;
            foreach (var flowData in groupFrequency.FlowsData)
                weightSum += flowData.Weight;
            
            for (var i = 0; i < groupFrequency.Frequency.Count; i++)
            {
                var key = groupFrequency.Frequency.ElementAt(i).Key;
                var percentagesSum = groupFrequency.FlowsData.Select(e => e.DaysFlowPercentage[key]).Sum();

                if (percentagesSum == 0 || weightSum == 0)
                    continue;
                
                groupFrequency.Frequency[key] = percentagesSum / weightSum;
            }
        }
        
        // Collect activity data for the last 30 days
        private void GenerateLast30Days()
        {
            var tempDay = dayToday;
            for (var i = 0; i <= 30; i++)
            {
                last30Days.Add(tempDay.HomeDayToInt(), tempDay);
                tempDay--;
            }
        }
        
        // Ultimate global activity information
        public class GroupFrequency
        {
            public readonly Dictionary<WeekInfo.DayOfWeek, float> Frequency;
            public readonly List<FrequencyFlowsData> FlowsData = new List<FrequencyFlowsData>();

            public GroupFrequency() => Frequency = CreateNewDictionaryForUsingFloat();
        }
        
        // Data for intermediate calculation
        public class FrequencyFlowsData
        {
            public readonly Flow Flow;
            public readonly Dictionary<WeekInfo.DayOfWeek, int> WeekDaysCount;
            public readonly Dictionary<WeekInfo.DayOfWeek, int> DaysInTrack;
            public readonly Dictionary<WeekInfo.DayOfWeek, float> DaysFlowPercentage;
            public int DaysCount;
            public int DaysTracked;
            public float FlowActivity;
            public float Weight;

            public FrequencyFlowsData(Flow flow)
            {
                Flow = flow;
                WeekDaysCount = CreateNewDictionaryForUsingInt();
                DaysInTrack = CreateNewDictionaryForUsingInt();
                DaysFlowPercentage = CreateNewDictionaryForUsingFloat();
            }
        }
        
        private static Dictionary<WeekInfo.DayOfWeek, int> CreateNewDictionaryForUsingInt() =>
            new Dictionary<WeekInfo.DayOfWeek, int>()
            {
                {WeekInfo.DayOfWeek.Sunday, 0},
                {WeekInfo.DayOfWeek.Monday, 0},
                {WeekInfo.DayOfWeek.Tuesday, 0},
                {WeekInfo.DayOfWeek.Wednesday, 0},
                {WeekInfo.DayOfWeek.Thursday, 0},
                {WeekInfo.DayOfWeek.Friday, 0},
                {WeekInfo.DayOfWeek.Saturday, 0},
            };
            
        private static Dictionary<WeekInfo.DayOfWeek, float> CreateNewDictionaryForUsingFloat() =>
            new Dictionary<WeekInfo.DayOfWeek, float>()
            {
                {WeekInfo.DayOfWeek.Sunday, 0f},
                {WeekInfo.DayOfWeek.Monday, 0f},
                {WeekInfo.DayOfWeek.Tuesday, 0f},
                {WeekInfo.DayOfWeek.Wednesday, 0f},
                {WeekInfo.DayOfWeek.Thursday, 0f},
                {WeekInfo.DayOfWeek.Friday, 0f},
                {WeekInfo.DayOfWeek.Saturday, 0f},
            };
    }
    
}
