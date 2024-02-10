namespace Architecture.ModuleTrackTime
{
    /// <summary>
    /// Interface for components that store display objects in a scroll,
    /// as well as for storing parameters in accordance with their type of task
    /// </summary>
    public interface ITrackLine
    {
        int ElementsCount { get; } // Elements count
         
        int CenterOriginPosition { get; set; } // Original ordered position in scroll
        
        void LinkPoint(TrackPoint trackPoint); // Place text component at point
        
        void UpdatePoint(TrackPoint trackPoint); // Update text view by point position

        void SetupElementsToPoints(TrackPoint[] points);  // Reset

        int MinLinePosition(); // Min clamped position of points
        int? MaxLinePosition(); // Max clamped position of points

        float DeltaScrollMultiplier(); // Delta multiplier for scroll
    }
}
