using System.Collections.Generic;
using System.Linq;
using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Data.Structs;

namespace Modules.CSVCreator
{
    // Component for data generator
    public static class CsvDaysGenerator
    {
        // Generate days with data for csv
        public static CsvDayItem[] GenerateDaysList()
        {
            var flows = new List<Flow>();
            flows.AddRange(AppData.GetCurrentGroup().ArchivedFlows);
            flows.AddRange(AppData.GetCurrentGroup().Flows);
            var orderedFlows = flows.OrderBy(e => e.Order).ToArray();
            
            var (dayFrom, dayTo) = GetDayBorders(orderedFlows);
            var current = dayFrom;

            var days = new List<CsvDayItem>();
            while (current <= dayTo)
            {
                days.Add(new CsvDayItem(current, orderedFlows));
                current++;
            }

            return days.ToArray();
        }

        // Get day started of all tasks and end day of all tasks
        private static (HomeDay from, HomeDay to) GetDayBorders(IEnumerable<Flow> flows)
        {
            var installedDay = AppData.AppInstalledDate.HomeDayToInt();

            var maxDaysList = new List<int> {installedDay};

            foreach (var flow in flows)
            {
                if (flow.GoalData.Count == 0)
                    continue;
                
                var max = flow.GoalData.Max(e => e.Key);
                
                if (max > installedDay)
                    maxDaysList.Add(max);
            }

            var today = Calendar.Today.HomeDayToInt();
            if (today > installedDay)
                maxDaysList.Add(today);

            var maxDay = maxDaysList.Max();
            if (maxDay < installedDay)
                maxDay = installedDay;
            
            var to = FlowExtensions.IntToHomeDay(maxDay);
            var from = FlowExtensions.IntToHomeDay(installedDay);

            return (from, to);
        }
    }
}
