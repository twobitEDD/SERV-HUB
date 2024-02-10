using HomeTools.Source.Design;
using HTools;
using UnityEngine;

namespace Architecture.ModuleTrackFlow
{
    /// <summary>
    /// A container that scrolls in a scroll and
    /// contains the current value of the task number
    /// </summary>
    public class TrackPoint : IBehaviorSync
    {
        public readonly int Id; // Id of point
        private readonly float minMarkerStep; // Percentage of distance between points
        private int idInPhase; // Floating identifier depending on the place in the scroll
     
        // Link to component that contains set of texts for scroll
        private readonly LineTrackSources lineTrackSources;
        // Points count in scroll circle
        private static int positionsCountInCircle = TrackPositions.PositionsCount;
        
        // The percentage of bending the scroll to a circle
        private const float circlePower = 0.97f;
        // The force of bending the scroll into a circle
        private float circleWide = 0.97f;
        // Default rect size of this point in circle
        private float targetScale = 1f;

        // Point position in the scroll
        public float Marker;
        
        // Rect object of point
        private RectTransform rectTransform;
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
        public TrackPoint(float startMarker, int id, LineTrackSources lineTrackSources)
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
        public void AddDelta(float delta) => Marker += delta;

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
            if (lineTrackSources.GetActualSource() == null)
                return;
            
            // Update number 
            if (direction == 1)
                lineTrackSources.GetActualSource().UpdatePoint(this);

            if (direction == -1)
                lineTrackSources.GetActualSource().UpdatePoint(this);

            // Setup global position number in the scroll loop to other components
            InCenterPositionSet();
        }

        // Recalculate global position number in the scroll loop
        public void RecalculateOriginPosition(int circlesCount)
        {
            if (lineTrackSources.GetActualSource() == null)
                return;
            
            // Calculate position number by circles count
            var positionOfCircles = circlesCount * positionsCountInCircle;
            // Recalculate global position number in the scroll loop
            OriginLinePosition = positionOfCircles + idInPhase;
            // Update number text 
            lineTrackSources.GetActualSource().UpdatePoint(this);
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

        // Update force of bending the scroll into a circle
        public void UpdateCirclePowerBySource() => circleWide = lineTrackSources.GetActualSource().CirclePower();

        // Update default rect size of this point in circle
        public void UpdateTargetScaleBySource() => targetScale = lineTrackSources.GetActualSource().TargetSizeInCenter();

        // Refresh the visual part of the point
        private void VisualMoving()
        {
            if (!SyncPositions)
                return;
            
            if (rectTransform == null)
                return;
            
            // Min and max borders for point
            var currentMin = minPosition * circleWide;
            var currentMax = maxPosition * circleWide;
            
            // Calculation of position taking into account visual rounding
            var circledMarker = Marker * (1 - Marker * Marker * 1f * (circleWide * 1.5f)) / circlePower;
            var markerConvert = (circledMarker + 1f) / 2;
            var position = Mathf.Lerp(currentMin, currentMax, markerConvert);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, position);
            
            // Updating size in relation to position
            CalculateVisualScale(Mathf.Abs(Marker) * 1.3f);
            
            if (tempAlphaSync == null)
                return;

            // Updating transparency to position
            CalculateVisualAlpha(Mathf.Abs(Marker));
        }
        
        // Updating size in relation to position
        private void CalculateVisualScale(float absMarker)
        {
            const float visibleMarkerFull = 0.03f;
            const float visibleMarkerMiddle = 0.17f;
            
            var localScale = 1f;

            if (absMarker < visibleMarkerFull)
            {
                localScale = targetScale;
            }
            
            else if (absMarker < visibleMarkerMiddle)
            {
                var markerDelta = (absMarker - visibleMarkerFull) / (visibleMarkerMiddle - visibleMarkerFull);
                
                
                var startScale = targetScale;
                var normalScale = 1f - Mathf.Clamp(visibleMarkerMiddle, 0f, 1f);
                
                localScale = Mathf.Lerp(startScale, normalScale, markerDelta);
            }
            else
            {
                localScale = 1f - Mathf.Clamp(absMarker, 0f, 1f);
            }

            rectTransform.localScale = Vector3.one * localScale;
        }
        
        // Updating transparency to position
        private void CalculateVisualAlpha(float absMarker)
        {
            if (tempAlphaSync == null)
                return;
            
            const float visibleMarkerFull = 0.03f;
            const float visibleMarkerMiddle = 0.15f;

            const float visibleMarkerMiddleEnd = 0.3f;
            const float visibleMarkerHide = 0.4f;

            const float alphaMiddle = 0.7f;
            
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
            var a = Mathf.Clamp(1f - visualPercent, 0f, 1f);
            tempAlphaSync.SetAlpha(a);
        }

        // Setup global position number in the scroll loop to other components
        private void InCenterPositionSet()
        {
            if (Mathf.Abs(Marker) > minMarkerStep || lineTrackSources.GetActualSource() == null)
                return;
            
            lineTrackSources.GetActualSource().CenterOriginPosition = OriginLinePosition;
            
            if (OriginLinePosition >= lineTrackSources.GetActualSource().MinLinePosition() &&
                OriginLinePosition <= lineTrackSources.GetActualSource().MaxLinePosition())
                lineTrackSources.SetCenterOriginPosition(OriginLinePosition);
        }
    }
}
