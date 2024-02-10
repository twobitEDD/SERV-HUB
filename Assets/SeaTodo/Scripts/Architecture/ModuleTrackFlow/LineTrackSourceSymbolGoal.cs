﻿using Architecture.Data.FlowTrackInfo;
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
    public class LineTrackSourceSymbolGoal : ITrackLine
    {
        private readonly RectTransform pool;
        
        private readonly Text[] texts;
        private readonly RectTransform[] rectTransforms;
        private readonly UIAlphaSync[] alphaSyncs;

        public LineTrackSourceSymbolGoal(RectTransform pool, Text[] texts,
            RectTransform[] rectTransforms, UIAlphaSync[] alphaSyncs)
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
            texts[trackPoint.Id].text = trackPoint.Id.ToString();
            trackPoint.PutElement(rectTransforms[trackPoint.Id], alphaSyncs[trackPoint.Id], Vector3.zero);
        }

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
            
            texts[trackPoint.Id].text = $"{FlowInfoSymbol.CountGoalByLine(trackPoint.OriginLinePosition)} {AppFontCustomization.Up}";
        }

        public void CleanWork()
        {
            foreach (var rect in rectTransforms)
                rect.SetParent(pool);
        }

        public int MinLinePosition() => 0;

        public int? MaxLinePosition() => 559;
        public float InertiaForScroll() => 1f;
        public float ScrollDeltaMultiplier() => 1f;
        public float CirclePower() => 1.25f;
        
        public float TargetSizeInCenter() =>  1.2f;
    }
}
