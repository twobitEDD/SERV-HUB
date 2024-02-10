using HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleTrackTime
{
    /// <summary>
    /// A component for storing display objects in a scroll
    /// and also for storing parameters in accordance with its type of task
    /// </summary>
    public class LineTrackSourceDayPart : ITrackLine
    {
        private readonly Text[] texts;
        private readonly RectTransform[] rectTransforms;
        private readonly UIAlphaSync[] alphaSyncs;

        public int ElementsCount { get; } = TrackPositions.PositionsCount;
        public int CenterOriginPosition { get; set; }
        
        public Text[] GetCreatedTexts => texts;

        public LineTrackSourceDayPart(RectTransform pool)
        {
            texts = SourceItemsCreator.CreateTextElements(pool, 2, "Item Days"); 
            rectTransforms = SourceItemsCreator.GetRectElements(texts);
            alphaSyncs = SourceItemsCreator.GetSyncsByText(texts);
            SourceItemsCreator.SetItemsToTheme(texts);
        }

        // Setup view to point container
        public void LinkPoint(TrackPoint trackPoint)
        {
            if (trackPoint.Id > MaxLinePosition())
                return;
            
            texts[trackPoint.Id].text = string.Empty;
            trackPoint.PutElement(rectTransforms[trackPoint.Id], alphaSyncs[trackPoint.Id], Vector3.zero);

            texts[0].text = AppTimeCustomization.Pm;
            texts[1].text = AppTimeCustomization.Am;
        }

        public void UpdatePoint(TrackPoint trackPoint) { }

        public void SetupElementsToPoints(TrackPoint[] points)
        {
            for (var i = 0; i < ElementsCount; i++)
                LinkPoint(points[i]);
        }

        public int MinLinePosition() => 0;

        public int? MaxLinePosition() => 1;
        
        public float DeltaScrollMultiplier() => 0.517f;
    }
}
