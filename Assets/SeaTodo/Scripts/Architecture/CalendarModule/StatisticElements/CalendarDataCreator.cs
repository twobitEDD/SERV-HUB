using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;

namespace Architecture.CalendarModule.StatisticElements
{
    // Additional layer for CalendarDataGenerator
    public class CalendarDataCreator : IGraphicInfo
    {
        private readonly CalendarDataGenerator calendarDataGenerator;

        // Create calendar data generator
        public CalendarDataCreator() => calendarDataGenerator = new CalendarDataGenerator();
        
        // Get default step of pages of calendar
        public int DefaultStep() => CalendarDataGenerator.DefaultStep;

        public void ReloadData(HomeDay homeDay)
        {
        }
        
        // Get info about page
        public GraphDataStruct GetInfo(int step) => calendarDataGenerator.CreateStruct(step);

        // Get page number of calendar by day
        public int GetStepByHomeDay(HomeDay homeDay) => calendarDataGenerator.GetStepByHomeDay(homeDay);

        public void SendStepToOther(int current)
        {
        }
    }
}
