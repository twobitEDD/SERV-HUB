using HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleTrackTime
{
    /// <summary>
    /// A component for storing display objects in a scroll
    /// and also for storing parameters in accordance with its type of task
    /// </summary>
    public class LineTrackSourceHours : ITrackLine
    {
        private readonly Text[] texts;
        private readonly RectTransform[] rectTransforms;
        private readonly UIAlphaSync[] alphaSyncs;

        public int ElementsCount { get; } = TrackPositions.PositionsCount;
        public int CenterOriginPosition { get; set; }
        
        public Text[] GetCreatedTexts => texts;

        public LineTrackSourceHours(RectTransform pool)
        {
            texts = SourceItemsCreator.CreateTextElements(pool, TrackPositions.PositionsCount, "Item Hours"); 
            rectTransforms = SourceItemsCreator.GetRectElements(texts);
            alphaSyncs = SourceItemsCreator.GetSyncsByText(texts);
            SourceItemsCreator.SetItemsToTheme(texts);
        }

        public void LinkPoint(TrackPoint trackPoint)
        {
            texts[trackPoint.Id].text = trackPoint.Id.ToString();
            trackPoint.PutElement(rectTransforms[trackPoint.Id], alphaSyncs[trackPoint.Id], Vector3.zero);
        }

        // Setup view to point container
        public void UpdatePoint(TrackPoint trackPoint)
        {
            if (trackPoint.OriginLinePosition < MinLinePosition())
            {
                texts[trackPoint.Id].text = string.Empty;
                return;
            }
            
            if (MaxLinePosition() != null && trackPoint.OriginLinePosition > MaxLinePosition())
            {
                texts[trackPoint.Id].text = string.Empty;
                return;
            }
            
            texts[trackPoint.Id].text = $"{HoursPositionLoop.GetActualHour(trackPoint.OriginLinePosition):00}";
        }

        public void SetupElementsToPoints(TrackPoint[] points)
        {
            for (var i = 0; i < ElementsCount; i++)
                LinkPoint(points[i]);
        }

        public int MinLinePosition() => -1000000;

        public int? MaxLinePosition() => 1000000;
        
        public float DeltaScrollMultiplier() => 0.517f;
    }
}
