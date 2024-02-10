using System.Collections.Generic;
using System.Linq;
using Architecture.CalendarModule;
using Architecture.Data.FlowTrackInfo;
using Architecture.Data.Structs;
using Architecture.Elements;
using HomeTools.Other;

namespace Architecture.Data
{
    // Additional methods for tasks
    public static class FlowExtensions
    {
        // Link to calendar of main page
        private static Calendar WorkCalendar() => AreasLocator.Instance.WorkArea.WorkCalendar;
        
        // Return the day the task was created
        public static HomeDay GetStartedDay(this Flow flow) => IntToHomeDay(flow.DateStarted);
        
        // Set the day of creation
        public static void SetStartedDate(this Flow flow, HomeDay dateTime) => flow.DateStarted = HomeDayToInt(dateTime);

        // Record progress in the task for the selected day
        public static void SetDayTrack(this Flow flow, HomeDay day, int originPosition)
        {
            if (originPosition <= 0)
                return;

            var dayData = HomeDayToInt(day);
            flow.GoalData[dayData] = originPosition;

            if (flow.GetStartedDay() > day)
                flow.SetStartedDate(day);
        }
        
        // Delete progress record for selected day
        public static void RemoveDayTrack(this Flow flow, HomeDay day)
        {
            var dayData = HomeDayToInt(day);
            if (flow.GoalData.ContainsKey(dayData))
                flow.GoalData.Remove(dayData);
        }
        
        // Return the amount of task progress for the selected day
        public static int GetIntProgress(this Flow flow, HomeDay day)
        {
            var dayInt = day.HomeDayToInt();
            
            if (!flow.GoalData.ContainsKey(dayInt))
                return 0;
            
            return CountTrackedDay(flow, flow.GoalData[dayInt]);
        }

        // Return the amount of task progress for all days
        public static int GetIntProgress(this Flow flow)
        {
            var trackedData = GetTrackedData(flow);
            var result = 0;
            
            foreach (var track in trackedData)
            {
                var convertedTrack = CountTrackedDay(flow, track);
                result += convertedTrack;
            }

            return result;
        }

        // Return a batch of progress records for the current task
        private static Dictionary<HomeDay, int> GetGoalByData(IReadOnlyDictionary<int, int> data) => 
            data.ToDictionary(i => IntToHomeDay(i.Key), i => i.Value);
        
        // Convert day entry to structure. Example: 20200101 => 2020 Jan 1
        public static HomeDay IntToHomeDay(int info)
        {
            var year = info / 10000;
            var month = (info - year * 10000) / 100;
            var day = (info - year * 10000 - month * 100);
            return new HomeDay(year, month, day);
        }
        
        // Convert day info structure to int format
        public static int HomeDayToInt(this HomeDay dateTime) => dateTime.Year * 10000 + dateTime.Month * 100 + dateTime.Day;
        
        // Convert day info structure to int format
        public static int HomeDayToInt(int year, int month, int day) => year * 10000 + month * 100 + day;

        // Check if there is a progress record for the selected day
        public static bool HasTrackedDay(this Flow flow, HomeDay day)
        {
            var intDay = HomeDayToInt(day);
            return flow.GoalData.ContainsKey(intDay);
        }
        
        // Return progress records for the selected day (not converted to real progress number)
        public static int GetTrackedDayOrigin(this Flow flow, HomeDay day)
        {
            var intDay = HomeDayToInt(day);
            return flow.GoalData.ContainsKey(intDay) ? flow.GoalData[intDay] : 0;
        }
        
        // Return progress records for the selected day (converted to real progress number)
        public static int GetTrackedDay(this Flow flow, HomeDay day)
        {
            var intDay = HomeDayToInt(day);
            var type = flow.Type;
            var lineResult = flow.GoalData.ContainsKey(intDay) ? flow.GoalData[intDay] : 0;
            
            switch (type)
            {
                case FlowType.count:
                    return FlowInfoCount.CountByLine(lineResult);
                case FlowType.done:
                    return FlowInfoDone.CountByLine(lineResult); 
                case FlowType.stars:
                    return FlowInfoStars.CountByLine(lineResult); 
                case FlowType.symbol:
                    return FlowInfoSymbol.CountByLine(lineResult); 
                case FlowType.timeS:
                    return FlowInfoTimeSeconds.CountByLine(lineResult); 
                case FlowType.timeM:
                    return FlowInfoTimeMinutes.CountByLine(lineResult); 
                case FlowType.timeH:
                    return FlowInfoTimeHours.CountByLine(lineResult); 
            }

            return 0;
        }
        
        // Convert progress to real progress number
        public static int CountTrackedDay(this Flow flow, int lineTrack)
        {
            var type = flow.Type;

            switch (type)
            {
                case FlowType.count:
                    return FlowInfoCount.CountByLine(lineTrack);
                case FlowType.done:
                    return FlowInfoDone.CountByLine(lineTrack); 
                case FlowType.stars:
                    return FlowInfoStars.CountByLine(lineTrack); 
                case FlowType.symbol:
                    return FlowInfoSymbol.CountByLine(lineTrack); 
                case FlowType.timeS:
                    return FlowInfoTimeSeconds.CountByLine(lineTrack); 
                case FlowType.timeM:
                    return FlowInfoTimeMinutes.CountByLine(lineTrack); 
                case FlowType.timeH:
                    return FlowInfoTimeHours.CountByLine(lineTrack); 
            }

            return 0;
        }
        
        // Return a batch of progress records for the current task
        public static Dictionary<HomeDay, int> GetDaysByData(this Flow flow) => GetGoalByData(flow.GoalData);

        // Return tracked progress numbers
        private static IEnumerable<int> GetTrackedData(Flow flow) => flow.GoalData.Values.ToArray();
    }
}
