using System;
using System.Linq;
using HomeTools.Handling;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.ModuleTrackFlow
{
    // Component that is responsible for for moving numbers by vertical direction
    public class TrackScroll : IBehaviorSync
    {
        // Link to component that contains set of texts for scroll
        private readonly LineTrackSources lineTrackSources;
        // Function for convert delta screen to canvas delta
        private readonly Func<Vector2, Vector2> screenConverter;

        // Handle component of scroll
        private readonly HandleObject handleObject;
        // Component with objects for numbers that move with a scroll
        private readonly TrackPositions trackPositions;

        // Speed of scroll when touched and without touch
        private const float jointSpeedTouched = 0.17f;
        private const float jointSpeedFree = 0.1f;
        // Imaginary scroll position for interpolation for containers for numbers
        private float scrollPosition;
        /// <summary>
        /// Imaginary scroll position for interpolation for containers for numbers,
        /// which changes to float - scrollPosition
        /// </summary>
        private float scrollPositionJoint;
        // Delta scroll position to stop the number in the center position
        private readonly float clampDelta;
        // Count of collecting delta of moving
        private const int collectorSize = 5;
        // Min count of collecting delta of moving
        private readonly float minCollectedDelta;
        /// <summary>
        /// Array with delta scroll that updates each update
        /// (to have last fresh delta of screen moving)
        /// </summary>
        private readonly float[] deltaCollected;
        // The position in the array to which the scroll delta is written in the current update
        private int deltaCollectedMarker;
        // Inertia multiplier for scroll
        private float inertiaMultiplier = 3f;
        // Additional multiplier for scroll delta
        private float inertiaMultiplierDelta = 1f;

        public bool Touched { private set; get; } // Touched scroll state
        public readonly RectTransform TouchArea; // Rect object of touch area
        
        // Flag to enable synchronization of numbers with scrolling
        public bool SyncPosition { set => trackPositions.SyncPosition = value; }
        
        // Create and setup
        public TrackScroll(GameObject touchArea, Transform setTrack, LineTrackSources lineTrackSources)
        {
            // Create main components
            TouchArea = touchArea.GetComponent<RectTransform>(); 
            this.lineTrackSources = lineTrackSources;
            screenConverter = MainCanvas.ScreenToCanvasDelta;
            trackPositions = new TrackPositions(setTrack.GetComponent<RectTransform>(), this, lineTrackSources);
            SyncWithBehaviour.Instance.AddObserver(trackPositions, AppSyncAnchors.TrackFlowModule);

            // Create handle component of scroll
            handleObject = new HandleObject(touchArea);
            handleObject.AddEvent(EventTriggerType.PointerDown, TouchDown);
            handleObject.AddEvent(EventTriggerType.PointerUp, TouchUp);
            
            // Setup params for scroll
            clampDelta = 2f / TrackPositions.PositionsCount / trackPositions.DeltaScreenToMarker;
            deltaCollected = new float[collectorSize];
            minCollectedDelta = clampDelta / 10f;
        }

        public void Start() { }

        // Refresh every frame
        public void Update()
        {
            AddDeltaUnderTap();
            JointDelta();
            ResetDeltaWhenMax();
        }

        // Update scroll parameters by current text source
        public void UpdateParamsBySource()
        {
            inertiaMultiplier = lineTrackSources.GetActualSource().InertiaForScroll();
            inertiaMultiplierDelta = lineTrackSources.GetActualSource().ScrollDeltaMultiplier();
            trackPositions.UpdateCirclePower();
        }

        // Setup scroll to chosen position
        public void SetScrollToDefault(int originPosition)
        {
            scrollPositionJoint = scrollPosition;
            trackPositions.SetDefaultImmediately(originPosition);
        }

        // Remove scroll inertia
        public void NeedRemoveInertia()
        {
            if (Touched || Mathf.Abs(scrollPosition - scrollPositionJoint) < 1)
                return;
            
            scrollPosition = scrollPositionJoint;
        }

        // Normalize scroll
        public void AddDeltaBackToJoin(float delta)
        {
            scrollPositionJoint += delta;
        }

        // Add delta scroll back if clamped
        public void AddDeltaBack(float delta)
        {
            scrollPosition += delta;
        }

        // Touch down method
        private void TouchDown()
        {
            Touched = true;
        }

        // Touch up method
        private void TouchUp()
        {
          if (!Touched)
              return;
          
          AddDistanceInertia();
          AddPowerInertia();
          ClampPositionToCenter();
          
          ResetCollectedDelta();
          Touched = false;
        }
        
        // Add delta inertia by screen delta
        private void AddDeltaUnderTap()
        {
            if (!Touched)
                return;

            var delta = screenConverter.Invoke(InputHS.DeltaMove).y;
            
            scrollPosition += delta * inertiaMultiplierDelta;
            
            CollectDeltaTouched(delta);
        }
        
        // Add delta of scroll position for interpolation to track positions
        private void JointDelta()
        {
            // Save position
            var savedPosition = scrollPositionJoint;
            
            // Calculate move delta
            var joinSpeed = Touched ? jointSpeedTouched : jointSpeedFree;
            var newPosition = Mathf.Lerp(scrollPositionJoint, scrollPosition, joinSpeed);
            var delta = newPosition - savedPosition;
            
            // Set delta for move places for numbers
            trackPositions.AddDelta(delta);
        }

        // Reset delta to avoid floating point errors
        private void ResetDeltaWhenMax()
        {
            if (scrollPosition > 1500f && scrollPositionJoint > 1500f)
            {
                scrollPosition -= 1000f;
                scrollPositionJoint -= 1000f;
            }
            
            if (scrollPosition < -1500f && scrollPositionJoint < -1500f)
            {
                scrollPosition += 1000f;
                scrollPositionJoint += 1000f;
            }
        }

        // Update the array that stores displacement deltas for the last frames
        private void CollectDeltaTouched(float delta)
        {
            if (!Touched)
                return;

            // Add one to array index
            deltaCollectedMarker++;
            if (deltaCollectedMarker >= deltaCollected.Length)
                deltaCollectedMarker = 0;
            
            // Setup current delta to array
            deltaCollected[deltaCollectedMarker] = delta;
        }

        // Reset the array that stores displacement deltas for the last frames
        private void ResetCollectedDelta()
        {
            for (var i = 0; i < deltaCollected.Length; i++)
                deltaCollected[i] = 0;
        }
        
        // Updating the scroll position to stop the number in the center
        private void ClampPositionToCenter()
        {
            var delta = scrollPosition % clampDelta;

            var direction = 0;
            var collectedDelta = deltaCollected.Sum();
            
            if (Mathf.Abs(collectedDelta) > minCollectedDelta)
                direction = collectedDelta > 0 ? 1 : -1;
            else
                direction = delta > clampDelta / 2 ? 1 : -1;
            
            if (direction > 0)
                scrollPosition += clampDelta - delta;
            else
                scrollPosition -= delta;
        }

        // Add inertia on release based on roll length
        private void AddDistanceInertia()
        {
            var deltaInertia = deltaCollected.Sum();
            deltaInertia *= inertiaMultiplier * 0.7f;
            scrollPosition += deltaInertia;
        }
        
        // Add inertia on release depending on the rolling force
        private void AddPowerInertia()
        {
            var powerDelta = deltaCollected.Sum() / collectorSize;
            powerDelta *= inertiaMultiplier * 10;
            scrollPosition += powerDelta;
        }
    }
}
