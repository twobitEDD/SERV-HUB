using System.Collections.Generic;
using System.Linq;
using Architecture.TextHolder;

namespace Architecture.Data
{
    // Build default data at application startup
    public static class TempData
    {
        // Build global package
        public static FlowGroup[] GetTempData()
        {
            // Create one task
            var defaultFlows = new Flow[1];
            
            defaultFlows[0] = new Flow()
            {
                Id = 0,
                Order = 0,
                Name = TextLocalization.GetLocalization(TextKeysHolder.TaskExample),
                GoalInt = 9,
                Icon = 0,
                Type = FlowType.count,
                Color = 2,
                DateStarted = CalendarModule.Calendar.Today.HomeDayToInt(),
                GoalData = new Dictionary<int, int>(),
            };

            // Create info about global package
            var groups = new FlowGroup[1];
            groups[0] = new FlowGroup
            {
                Name = TextLocalization.GetLocalization(TextKeysHolder.TitleExample),
                Id = 1,
                Order = 0,
                Icon = 1,
                Reminders = true,
                DateStarted = CalendarModule.Calendar.Today.HomeDayToInt(),
                Flows = defaultFlows.OrderBy(e => e.Order).ToArray(),
                ArchivedFlows = new Flow[0],
            };

            return groups;
        }
    }
}
