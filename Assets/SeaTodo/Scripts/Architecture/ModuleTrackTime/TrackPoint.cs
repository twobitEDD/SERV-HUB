using HomeTools.Source;
using HomeTools.Source.Design;
using HTools;
using Packages.HomeTools.Source.Design;
using UnityEngine;

namespace Architecture.ModuleTrackTime
{
    /// <summary>
    /// A container that scrolls in a scroll and
    /// contains the current value of the time number
    /// </summary>
    public class TrackPoint : IBehaviorSync
    {
        public readonly int Id; // Id of point
        private readonly float minMarkerStep; // Percentage of distance between points
        private int idInPhase; // Floating identifier depending on the place in the scroll
     
        // Link to component that contains set of texts for scroll
        private readonly ITrackLine lineTrackSources;
        // Points count in scroll circle
        private static readonly int positionsCountInCircle = TrackPositions.PositionsCount;
        
        // Multiplier of alpha channel of items
        private const float alphaVisible = 2f;

        // Point position in the scroll
        public float Marker;
        
        // Rect object of point
        private RectTransform rectTransform;
        // Animation component of item rect
        private RectTransformSync rectTransformSync;
        // Min and max position when scroll
        private float minPosition;
        private float maxPosition;
        
        // Element displaying number in scroll 
        private RectTransform tempElement;
        // Animation component of element displaying number in scroll 
        private UIAlphaSync tempAlphaSync;
        
        // Global position number in the scroll loop
        public int OriginLinePosition { get; private set; }
        // Is the point outside the scroll
        public int OriginLineClamp { get; private set; }
        // Position synchronization enabled flag
        public bool SyncPositions { set; private get; }
        
        // Create point
        public TrackPoint(float startMarker, int id, ITrackLine lineTrackSources)
        {
            this.lineTrackSources = lineTrackSources;
            Marker = startMarker;
            Id = id;
            CalculateIdInPhase();

            minMarkerStep = 1f / (positionsCountInCircle - 1);
        }

        // Calculation of the local ordinal number in the scroll
        private void CalculateIdInPhase()
        {
            var positionsForSide = positionsCountInCircle - 1;
            idInPhase = Id;
            if (Id > positionsForSide / 2)
                idInPhase = -(positionsCountInCircle - Id);
        }

        // Setup rect object and min max positions for scroll
        public void SetVisualParameters(RectTransform transform, float min, float max)
        {
            rectTransform = transform;
            minPosition = min;
            maxPosition = max;
        }
        
        public void Start()
        {
        }

        public void Update()
        {
            VisualMoving();
            InCenterPositionSet();
        }

        // Add delta to scroll
        public void AddDelta(float delta)
        {
            Marker += delta;
        }

        // Set an element with a number display in a scroll
        public void PutElement(RectTransform element, UIAlphaSync alphaElement, Vector3 position)
        {
            tempElement = element;
            tempElement.SetParent(rectTransform);
            tempElement.localScale = Vector3.one;
            tempElement.anchoredPosition = position;

            tempAlphaSync = alphaElement;
        }

        // Update number by order
        public void Updated(int direction)
        {
            // Update number 
            if (direction == 1)
                lineTrackSources.UpdatePoint(this);

            // Update number 
            if (direction == -1)
                lineTrackSources.UpdatePoint(this);

            // Setup global position number in the scroll loop to other components
            InCenterPositionSet();
        }

        // Recalculate global position number in the scroll loop
        public void RecalculateOriginPosition(int circlesCount)
        {
            // Calculate position number by circles count
            var positionOfCircles = circlesCount * positionsCountInCircle;
            // Recalculate global position number in the scroll loop
            OriginLinePosition = positionOfCircles + idInPhase;
            // Update number text 
            lineTrackSources.UpdatePoint(this);
            // Setup global position number in the scroll loop to other components
            InCenterPositionSet();
        }

        // Detect if point went beyond the boundaries of the scroll
        public void DetectClampedPoint(int minOrigin, int? maxOrigin)
        {
            OriginLineClamp = 0; 
            
            if (OriginLinePosition == minOrigin)
                OriginLineClamp = -1;

            if (maxOrigin != null && OriginLinePosition == maxOrigin)
                OriginLineClamp = 1;
        }

        // Refresh the visual part of the point
        private void VisualMoving()
        {
            if (!SyncPositions)
                return;
            
            if (rectTransform == null)
                return;
            
            // Calculation of position taking into account visual rounding
            var circledMarker = Marker * (1 - Marker * Marker * 0.7f) * 1.3f;
            var markerConvert = (circledMarker + 1f) / 2;
            var position = Mathf.Lerp(minPosition, maxPosition, markerConvert);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, position);

            // Updating size in relation to position
            CalculateVisualScale(Mathf.Abs(Marker));
            // Updating transparency to position
            CalculateVisualAlpha(Mathf.Abs(Marker));
        }

        // Updating size in relation to position
        private void CalculateVisualScale(float absMarker)
        {
            const float visibleMarkerFull = 0.03f;
            const float visibleMarkerMiddle = 0.1f;

            const float middleScale = 0.7f;

            if (absMarker < visibleMarkerFull)
            {
                absMarker = 0;
            }
            else if (absMarker < visibleMarkerMiddle)
            {
                var markerDelta = (absMarker - visibleMarkerFull) / (visibleMarkerMiddle - visibleMarkerFull);
                absMarker = Mathf.Clamp(markerDelta, 0f, 1) * (1f - middleScale);
            }
            else
            {
                absMarker = (1f - middleScale);
            }

            rectTransform.localScale = Vector3.one * (1f - absMarker);
        }

        // Updating transparency to position
        private void CalculateVisualAlpha(float absMarker)
        {
            if (tempAlphaSync == null)
                return;
            
            const float visibleMarkerFull = 0.03f;
            const float visibleMarkerMiddle = 0.1f;

            const float visibleMarkerMiddleEnd = 0.3f;
            const float visibleMarkerHide = 0.7f;

            const float alphaMiddle = 0.4f;
            
            var resultMarker = 0f;
            
            if (absMarker < visibleMarkerFull)
            {
                resultMarker = 0;
            }
            else if (absMarker < visibleMarkerMiddle)
            {
                var markerDelta = (absMarker - visibleMarkerFull) / (visibleMarkerMiddle - visibleMarkerFull);
                resultMarker = Mathf.Clamp(markerDelta, 0f, 1) * alphaMiddle;
            }
            else if (absMarker < visibleMarkerMiddleEnd)
            {
                resultMarker = alphaMiddle;
            }
            else if (absMarker >= visibleMarkerMiddleEnd && absMarker < visibleMarkerHide)
            {
                var markerDelta = (absMarker - visibleMarkerMiddleEnd) / (visibleMarkerHide - visibleMarkerMiddleEnd);
                resultMarker = alphaMiddle + Mathf.Clamp(markerDelta, 0f, 1f) * (1 - alphaMiddle);
            }
            else if (absMarker >= visibleMarkerMiddleEnd)
            {
                resultMarker = 1f;
            }

            var visualPercent = Mathf.Clamp(resultMarker, 0f, 1f);
            var a = Mathf.Clamp(1f - visualPercent * alphaVisible, 0f, 1f);
            tempAlphaSync.SetAlpha(a);
        }

        // Setup global position number in the scroll loop to other components
        private void InCenterPositionSet()
        {
            if (Mathf.Abs(Marker) > minMarkerStep)
                return;
            
            lineTrackSources.CenterOriginPosition = OriginLinePosition;
        }
    }
}
