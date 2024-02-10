using System;
using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Elements;

namespace Architecture.TrackArea
{
    // Component of track task progress session
    public class TrackSession
    {
        private readonly Flow flow; // Current task
        private readonly Action finishSession; // Action when finish session
        // Link to calendar in track area
        private Calendar TrackCalendar => AreasLocator.Instance.TrackArea.TrackCalendar;

        // Create ans save components
        public TrackSession(Flow flow, Action finishAction, HomeDay inputDay)
        {
            this.flow = flow;
            finishSession = finishAction;
            TrackCalendar.CurrentDay = inputDay;
        }

        // Get task that name in session
        public string GetFlowName() => flow.Name;

        // Get task that in session
        public Flow GetFlow() => flow;
        
        // Finish session
        public void Finish(int linePosition)
        {
            flow.RemoveDayTrack(TrackCalendar.CurrentDay);
            flow.SetDayTrack(TrackCalendar.CurrentDay, linePosition);
            finishSession.Invoke();
        }

        // Day that in progress track
        public HomeDay CalendarDay() => TrackCalendar.CurrentDay;
    }
}
