using System.Collections.Generic;
using System.Linq;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Data.Structs;
using Architecture.DaysMarkerArea.DaysColors;
using Architecture.TextHolder;
using HomeTools.Source.Calendar;

namespace Modules.CSVCreator
{
    // Component to hold data about day
    public struct CsvDayItem
    {
        public readonly string Date;
        public readonly string Day;
        public readonly string SeaCalendar;
        public readonly List<string> Tasks;
        public readonly List<string> Results;

        // Has information
        public bool HasInformation => SeaCalendar != string.Empty || Tasks.Count > 0;

        // Create and setup
        public CsvDayItem(HomeDay homeDay, IEnumerable<Flow> flows)
        {
            Date = $"{homeDay.Year} {CalendarNames.GetMonthShortName(homeDay.Month)}";
            Day = $" {homeDay.Day}, {TextHolderTime.DaysOfWeekShort(homeDay.DayOfWeek)}";

            var dayInt = homeDay.HomeDayToInt();
            SeaCalendar = AppData.PaletteDays.ContainsKey(dayInt) ? 
                ColorMarkersDescriptor.GetColorName(AppData.PaletteDays[dayInt]) : 
                string.Empty;
            
            Tasks = new List<string>();
            Results = new List<string>();
            
            var trackedFlows = flows.Where(e => e.GoalData.ContainsKey(dayInt)).OrderBy(e=>e.Order);
            foreach (var flow in trackedFlows)
            {
                Tasks.Add($"{flow.Name}:");
                Results.Add(FlowInfoAll.GetWorkProgressFlowCsv(flow.Type, flow.GetIntProgress(homeDay)));
            }
        }
    }
}
