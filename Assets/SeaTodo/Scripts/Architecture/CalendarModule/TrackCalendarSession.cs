using System;
using Architecture.CalendarModule.StatisticElements;
using Architecture.Data.Structs;
using Packages.HomeTools.Source.Design;

namespace Architecture.CalendarModule
{
    public class TrackCalendarSession
    {
        public HomeDay HomeDay; // Default day for calendar
        private readonly Action<HomeDay> closedAction; // Call actions when closed

        public TrackCalendarSession(HomeDay homeDay, Action<HomeDay> closedAction)
        {
            HomeDay = homeDay;
            this.closedAction = closedAction;
        }
        
        // Finish session
        public void FinishSession() => closedAction?.Invoke(HomeDay);
    }
}
