using HTools;
using UnityEngine;

namespace Architecture.ModuleTrackFlow
{
    // The component that is responsible for moving containers for numbers by scroll
    public class TrackPositions : IBehaviorSync
    {
        // Link to component that contains set of texts for scroll
        private readonly LineTrackSources lineTrackSources;
        // Link to component that is responsible for for moving numbers by vertical direction
        private readonly TrackScroll trackScroll;
        // Minimum deviation to correct the position of the container for a number
        private const float circlePartMaxDeviation = 0.07f;
        // Deceleration rate while holding the scroll
        private const float slowWhenClampedTouched = 0.05f;
        // Count of containers for a number
        public const int PositionsCount = 11;
        // Convert screen delta to delta of point move in scroll 
        public readonly float DeltaScreenToMarker;
        // Length between containers for numbers
        private readonly float markerStep;
        // Rect object of tracker
        private readonly RectTransform trackerRect;
        // Containers for numbers (points)
        private TrackPoint[] points;
        // Count of scroll loops 
        private int circlesLoopCount;

        // Min and max line position of scroll
        private int minLinePosition;
        private int? maxLinePosition;

        // Current min and max container for text number
        private TrackPoint trackPointMin;
        private TrackPoint trackPointMax;

        // Setup sync containers with scroll moving
        public bool SyncPosition
        {
            set
            {
                foreach (var point in points)
                    point.SyncPositions = value;
            }
        }

        // Create and setup
        public TrackPositions(RectTransform tracker, TrackScroll scroll, LineTrackSources lineTrackSources)
        {
            // Save components
            this.lineTrackSources = lineTrackSources;
            trackScroll = scroll;
            trackerRect = tracker;
            
            // Calculate converter of screen delta to delta of point move in scroll 
            DeltaScreenToMarker = 1f / tracker.rect.height;
            // Setup main elements
            CreateMainElements();
            SetVisualToMainElements();
            // Setup generated number containers to component that contains set of texts for scroll
            lineTrackSources.Points = points;
            // Calculate length between containers for numbers
            markerStep = 2f / PositionsCount;
        }
        
        // Create container components for numbers
        private void CreateMainElements()
        {
            const float delta = 1f / PositionsCount;
            points = new TrackPoint[PositionsCount];
            for (var i = 0; i < PositionsCount; i++)
            {
                points[i] = new TrackPoint(delta - 1f + delta * i * 2, i, lineTrackSources);
                SyncWithBehaviour.Instance.AddObserver(points[i], AppSyncAnchors.TrackFlowModule);
            }
        }
        
        // Setup container components for numbers
        private void SetVisualToMainElements()
        {
            for (var i = 0; i < PositionsCount; i++)
            {
                // Create container object
                var newObj = new GameObject($"point {i}");
                newObj.transform.SetParent(trackerRect);
                
                // Setup rect of container
                var rect = newObj.AddComponent<RectTransform>();
                rect.transform.localScale = Vector3.one;
                rect.anchoredPosition3D = Vector3.zero;
                rect.sizeDelta = new Vector2(50, 10);

                // Setup visual params to container component
                var border = trackerRect.sizeDelta.y;
                points[i].SetVisualParameters(rect, -border, border);
            }
        }

        public void Start() { }

        public void Update()
        {
            ClampPositionsByOne();
            FixClampMiss();
        }

        /// <summary>
        /// Update position of points (containers)
        /// which went beyond the boundaries of the scroll
        /// </summary>
        private void UpdateClampedPoints()
        {
            // Detect points which went beyond the boundaries of the scroll
            DetectClampedPoints();
            
            // Reset previous points
            trackPointMin = null;
            trackPointMax = null;
            
            // Find points which went beyond the boundaries of the scroll
            foreach (var trackPoint in points)
            {
                if (trackPoint.OriginLineClamp == -1)
                    trackPointMin = trackPoint;

                if (trackPoint.OriginLineClamp == 1)
                    trackPointMax = trackPoint;
            }
        }

        // Update ratio of line rounding to circle
        public void UpdateCirclePower()
        {
            foreach (var point in points)
            {
                point.UpdateCirclePowerBySource();
                point.UpdateTargetScaleBySource();
            }
        }

        // Edit and add delta to points
        public void AddDelta(float delta)
        {
            delta *= DeltaScreenToMarker;
            delta = ConvertDeltaByClamp(delta);
            AddDeltaMarker(delta, true);
        }

        // Add delta to points
        private void AddDeltaMarker(float delta, bool backToScroll)
        {
            foreach (var trackPoint in points)
                trackPoint.AddDelta(delta);

            // Return the rest of the movement back to the scroll
            if (backToScroll)
                trackScroll.AddDeltaBackToJoin(delta / DeltaScreenToMarker);
        }

        // Edit delta move based on stopping precision
        private float ConvertDeltaByClamp(float delta)
        {
            // Find points which went beyond the boundaries of the scroll
            UpdateClampedPoints();

            // Correct delta
            
            if (trackPointMin != null && trackPointMin.Marker > 0 && delta > 0)
            {
                var percentage = Mathf.Clamp(1 - trackPointMin.Marker / circlePartMaxDeviation, 0, 1);
                delta *= percentage * slowWhenClampedTouched;
            }
            
            if (trackPointMax != null && trackPointMax.Marker < 0 && delta < 0)
            {
                var percentage =  Mathf.Clamp(1 + trackPointMax.Marker / circlePartMaxDeviation, 0, 1);
                delta *= percentage * slowWhenClampedTouched;
            }

            return delta;
        }
        
        // Add a scroll depending on the number of movement numbers
        public void SetDefaultImmediately(int originPosition)
        {
            // Count of scroll loops
            circlesLoopCount = 0;
            // Interpolation position of first scroll point
            var marker = points[0].Marker;
            
            // Correct all points by delta
            if (Mathf.Abs(marker) > 0.01f)
                AddDeltaMarker(-marker, false);

            // Calculate scroll position by target position
            var nextPosition = originPosition * markerStep;
            AddDeltaMarker(-nextPosition, false);

            // Calculate count of loops
            if (originPosition > PositionsCount)
                circlesLoopCount = Mathf.RoundToInt((float) originPosition / PositionsCount - 1);
            
            // Recalculate position of points by new position
            foreach (var point in points)
                point.RecalculateOriginPosition(CalculateLocalCircle(point));
            
            // Detect points which went beyond the boundaries of the scroll
            DetectClampedPoints();
            // Update scroll borders
            GetClampedPositions();
            
            // Refresh visual part of points
            foreach (var point in points)
                point.Update();
        }

        // Refresh points that went beyond the scrolling boundaries
        private void ClampPositionsByOne()
        {
            foreach (var point in points)
            {
                if (Mathf.Abs(point.Marker) > 1f)
                {
                    // Add value to the number of scroll cycles
                    UpdateCircleLoopCount(point);
                    // Update number in point
                    point.Updated(point.Marker > 1f ? 1 : - 1);
                    // Reset marker value to scroll
                    point.Marker = CycleFloatByOne(point.Marker);
                    // Recalculate text value at point
                    point.RecalculateOriginPosition(CalculateLocalCircle(point));
                    // Try to mark the point as the one that is extreme
                    point.DetectClampedPoint(minLinePosition, maxLinePosition);
                }
            }
        }

        // Detect points which went beyond the boundaries of the scroll
        private void DetectClampedPoints()
        {
            foreach (var point in points)
                point.DetectClampedPoint(minLinePosition, maxLinePosition);
        }
        
        // Reset marker value to scroll
        private float CycleFloatByOne(float t)
        {
            while (Mathf.Abs(t) > 1)
            {
                if (t > 1) t = -2f + t;
                if (t < -1) t = 2f + t;
            }
            
            return t;
        }
        
        // Return the number of scroll cycles for a point
        private int CalculateLocalCircle(TrackPoint trackPoint)
        {
            if (trackPoint.Id == 0)
                return circlesLoopCount;

            var positionInSide = (PositionsCount - 1) / 2;
            
            if (trackPoint.Id > positionInSide && trackPoint.Marker > points[0].Marker)
                return circlesLoopCount + 1;
            
            if (trackPoint.Id <= positionInSide && trackPoint.Marker < points[0].Marker)
                return circlesLoopCount - 1;

            return circlesLoopCount;
        }

        // Add value to the number of scroll cycles
        private void UpdateCircleLoopCount(TrackPoint trackPoint)
        {
            if (trackPoint.Id != 0)
                return;

            if (trackPoint.Marker > 0)
                circlesLoopCount--;

            if (trackPoint.Marker < 0)
                circlesLoopCount++;
        }

        // Update scroll borders
        private void GetClampedPositions()
        {
            minLinePosition = lineTrackSources.GetActualSource().MinLinePosition();
            maxLinePosition = lineTrackSources.GetActualSource().MaxLinePosition();
        }
        
        // Correcting the amount of scrolling in TrackScroll component
        private void JointDeltaToClampedMin()
        {
            if (trackPointMin == null)
                return;

            if (trackPointMin.Marker > 0)
            {
                var delta = trackPointMin.Marker / DeltaScreenToMarker;
                trackScroll.NeedRemoveInertia();
                trackScroll.AddDeltaBack(-delta);
            }
        }
        
        // Correcting the amount of scrolling in TrackScroll component
        private void JointDeltaToClampedMax()
        {
            if (trackPointMax == null)
                return;

            if (trackPointMax.Marker < 0)
            {
                var delta = trackPointMax.Marker / DeltaScreenToMarker;
                trackScroll.NeedRemoveInertia();
                trackScroll.AddDeltaBack(-delta);
            }
        }

        // Catching errors of fixing going out of the scroll
        private void FixClampMiss()
        {
            if (trackScroll.Touched)
                return;
            
            UpdateClampedPoints();
            JointDeltaToClampedMin();
            JointDeltaToClampedMax();
        }
    }
}
