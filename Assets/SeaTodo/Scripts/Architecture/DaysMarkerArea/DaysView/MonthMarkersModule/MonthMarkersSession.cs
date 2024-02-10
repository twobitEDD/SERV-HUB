using System;

namespace Architecture.DaysMarkerArea.DaysView.MonthMarkersModule
{
    // Component for choose day of month in Sea Calendar page session
    public class MonthMarkersSession
    {
        private readonly Action closeAction; // Call action when finished
        public readonly MonthMarkersPackage MonthMarkersPackage; // Month data package
        public readonly string Title; // Title for view

        // Create session
        public MonthMarkersSession(Action closeAction, string title, MonthMarkersPackage monthMarkersPackage)
        {
            this.closeAction = closeAction;
            MonthMarkersPackage = monthMarkersPackage;
            Title = title;
        }
        
        // Finish session
        public void Closed()
        {
            closeAction?.Invoke();
        }
    }
}
