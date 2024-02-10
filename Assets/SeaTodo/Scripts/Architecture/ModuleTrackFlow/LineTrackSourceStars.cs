using HomeTools.Source;
using HomeTools.Source.Design;
using Packages.HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleTrackFlow
{
    /// <summary>
    /// A component for storing display objects in a scroll
    /// and also for storing parameters in accordance with its type of task
    /// </summary>
    public class LineTrackSourceStars : ITrackLine
    {
        private readonly RectTransform pool;
        
        private readonly Text[] texts;
        private readonly RectTransform[] rectTransforms;
        private readonly UIAlphaSync[] alphaSyncs;
        
        public LineTrackSourceStars(RectTransform pool, Text[] texts, RectTransform[] rectTransforms, UIAlphaSync[] alphaSyncs)
        {
            this.pool = pool;
            this.texts = texts;
            this.rectTransforms = rectTransforms;
            this.alphaSyncs = alphaSyncs;
        }

        public int ElementsCount { get; } = TrackPositions.PositionsCount;
        public int CenterOriginPosition { get; set; }

        public void LinkPoint(TrackPoint trackPoint)
        {
            if (trackPoint.Id < 0 || trackPoint.Id > 5)
                return;
            
            texts[trackPoint.Id].text = $"{trackPoint.Id}{AppFontCustomization.Star}";
            trackPoint.PutElement(rectTransforms[trackPoint.Id], alphaSyncs[trackPoint.Id], Vector3.zero);
        }

        public void UpdatePoint(TrackPoint trackPoint) { }

        public void CleanWork()
        {
            foreach (var rect in rectTransforms)
                rect.SetParent(pool);
        }

        public int MinLinePosition() => 0;

        public int? MaxLinePosition() => 5;
        public float InertiaForScroll() => 0.5f;
        public float ScrollDeltaMultiplier() => 1f;

        public float CirclePower() => 1.25f;
        
        public float TargetSizeInCenter() => 1.1f;
    }
}
