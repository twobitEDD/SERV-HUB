using Architecture.Data;

namespace Architecture.Other
{
    /// <summary>
    /// A class that contains an enumerator for the days of the week
    /// and that forms an ordered array of days of the week
    /// </summary>
    public static class WeekInfo 
    {
        public enum DayOfWeek
        {
            Sunday,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday,
        }
        
        public static DayOfWeek[] DaysOrder()
        {
            var daysOfWeek = new DayOfWeek[7];
            var fromSunday = AppCurrentSettings.DaysFromSunday;

            if (fromSunday)
            {
                daysOfWeek[0] = DayOfWeek.Sunday;
                daysOfWeek[1] = DayOfWeek.Monday;
                daysOfWeek[2] = DayOfWeek.Tuesday;
                daysOfWeek[3] = DayOfWeek.Wednesday;
                daysOfWeek[4] = DayOfWeek.Thursday;
                daysOfWeek[5] = DayOfWeek.Friday;
                daysOfWeek[6] = DayOfWeek.Saturday;
            }
            else
            {
                daysOfWeek[0] = DayOfWeek.Monday;
                daysOfWeek[1] = DayOfWeek.Tuesday;
                daysOfWeek[2] = DayOfWeek.Wednesday;
                daysOfWeek[3] = DayOfWeek.Thursday;
                daysOfWeek[4] = DayOfWeek.Friday;
                daysOfWeek[5] = DayOfWeek.Saturday;
                daysOfWeek[6] = DayOfWeek.Sunday;
            }

            return daysOfWeek;
        }
    }
}
