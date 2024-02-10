using System;
using System.Linq;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.Statistics.Interfaces;
using HomeTools.Handling;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.Statistics
{
    // The main class of the statistics scroll module
    public class StatisticsScroll : IBehaviorSync
    {
        // Link to page scroll
        private PageScroll PageScroll() => Pages.PageScroll.Instance;
        // Link to track task page
        private TrackArea.TrackArea TrackArea() => AreasLocator.Instance.TrackArea;
        // Function of convert delta screen to delta canvas
        private readonly Func<Vector2, Vector2> screenConverter;
        // Statistics view object
        private readonly IStatistic statistic;
        
        // Position of statistics object line
        private float scrollPosition; 
        /// <summary>
        /// Scroll position for interpolation for objects,
        /// which changes to float - scrollPosition
        /// </summary>
        private float scrollPositionJoint;
        // Save "scrollPosition"
        private float scrollPositionBackup;
        // Width of statistics page
        private readonly float clampDelta;

        // Params for collect last move delta
        private const int collectorSize = 5;
        private readonly float minCollectedDelta;
        private readonly float[] deltaCollected;
        private int deltaCollectedMarker;

        // Scroll parameters
        private readonly float jointSpeedTouched;
        private readonly float jointSpeedFree;
        private readonly float inertiaMultiplier;
        private readonly float inertiaMultiplierDelta;

        private bool touched; // Touched flag
        
        // Lock statistics scroll by page scroll
        private bool pageScrollChecked;
        // Collect scroll direction
        private Vector2 scrollDirectCollector;
        // Scroll delta when chose move page of statistics
        private const float scrollDirectSize = 9f;

        // Reset scroll inertia when untouched
        private bool resetInertiaWhenUntouched;

        // Create and setup
        public StatisticsScroll(HandleObject handleArea, IStatistic statistic, ScrollProperties scrollProperties)
        {
            // Save screen converter function
            screenConverter = MainCanvas.ScreenToCanvasDelta;
            // Save statistics object
            this.statistic = statistic;
            // Setup scroll to statistics pages
            statistic.ViewLine().ConnectWithScroll(this);
            
            // Add touch events
            handleArea.AddEvent(EventTriggerType.PointerDown, TouchDown);
            handleArea.AddEvent(EventTriggerType.PointerUp, TouchUp);
            
            // Setup additional scroll params
            clampDelta = this.statistic.StatisticsViewWidth;
            deltaCollected = new float[collectorSize];
            minCollectedDelta = clampDelta / 10f;

            // Save scroll parameters
            jointSpeedTouched = scrollProperties.JointSpeedTouched;
            jointSpeedFree = scrollProperties.JointSpeedFree;
            inertiaMultiplier = scrollProperties.InertiaMultiplier;
            inertiaMultiplierDelta = scrollProperties.InertiaMultiplierDelta;

            // Add delta for scroll initialize
            AddDeltaEmpty();
        }

        public void Start() { }

        // Updates each frame
        public void Update()
        {
            // Check move statistics scroll or page scroll by delta direction
            CheckLockByPageScroll();
            // Add delta scroll when touched
            AddDeltaUnderTap();
            // Move statistics pages process
            JointDelta();
            // Reset scroll to avoid floating point errors
            ResetDeltaWhenMax();
        }

        // Update scroll position by added pages steps
        public void SetScrollBySteps(int deltaStep)
        {
            var delta = deltaStep * clampDelta;
            scrollPosition += delta;
        }

        // Back scroll to normal state when clamped position
        public void BackToNormalStateWhenClamped(float deflection)
        {
            scrollPosition = scrollPositionJoint;
            scrollPosition -= deflection;
        }

        // Check for reset inertia when untouched
        public void CheckForResetInertiaWhenUntouched()
        {
            if (touched || !resetInertiaWhenUntouched)
                return;
            
            statistic.ViewLine().TouchUpReaction();
            resetInertiaWhenUntouched = false;
        }

        // Normalize scroll position
        public void AddDeltaBackToJoin(float delta)
        {
            scrollPositionJoint += delta;
        }

        // Check the scroll stop
        public bool CheckSleepMotion() => statistic.ViewLine().CheckSleepMotion();

        // Touch down method
        public void TouchDown()
        {
            touched = true;
            pageScrollChecked = false;
            scrollDirectCollector = Vector2.zero;
            scrollPositionBackup = scrollPosition;
            resetInertiaWhenUntouched = true;
        }

        // Touch up method
        public void TouchUp()
        {
          if (!touched)
              return;
          
          AddDistanceInertia();
          AddPowerInertia();

          ClampPositionToCenter();
          ResetCollectedDelta();
          touched = false;
          
          if (!TrackArea().HasSession)
              PageScroll().Enabled = true;
        }
        
        // Add delta for scroll initialize
        private void AddDeltaEmpty() => statistic.ViewLine().AddDelta(0);
        
        // Add delta scroll when touched
        private void AddDeltaUnderTap()
        {
            if (!touched)
                return;

            var delta = screenConverter.Invoke(InputHS.DeltaMove).x;
            
            scrollPosition += delta * inertiaMultiplierDelta;
            CollectDeltaTouched(delta);
        }
        
        // Move statistics pages process
        private void JointDelta()
        {
            var savedPosition = scrollPositionJoint;
            
            var joinSpeed = touched ? jointSpeedTouched : jointSpeedFree;
            var newPosition = Mathf.Lerp(scrollPositionJoint, scrollPosition, joinSpeed);
            
            var delta = newPosition - savedPosition;
            
            if (Mathf.Abs(delta) > 0.01f)
                statistic.ViewLine().AddDelta(delta);
            if (Mathf.Abs(delta) < 5f)
                statistic.ViewLine().SleepMotion();
        }
        
        // Reset scroll to avoid floating point errors
        private void ResetDeltaWhenMax()
        {
            if (scrollPosition > clampDelta * 3 && scrollPositionJoint > clampDelta * 3)
            {
                scrollPosition -= clampDelta * 2;
                scrollPositionJoint -= clampDelta * 2;
                scrollPositionBackup -= clampDelta * 2;
            }
            
            if (scrollPosition < -clampDelta * 3 && scrollPositionJoint < -clampDelta * 3)
            {
                scrollPosition += clampDelta * 2;
                scrollPositionJoint += clampDelta * 2;
                scrollPositionBackup += clampDelta * 2;
            }
        }

        // Collect delta each frame to array for add inertia when untouched
        private void CollectDeltaTouched(float delta)
        {
            if (!touched)
                return;

            deltaCollectedMarker++;
            if (deltaCollectedMarker >= deltaCollected.Length)
                deltaCollectedMarker = 0;
            
            deltaCollected[deltaCollectedMarker] = delta;
        }

        // Reset collected delta that fro inertia when untouched
        private void ResetCollectedDelta()
        {
            for (var i = 0; i < deltaCollected.Length; i++)
                deltaCollected[i] = 0;
        }
        
        // Normalize scroll position to stop page in center
        private void ClampPositionToCenter()
        {
            if (!pageScrollChecked)
                return;
            
            var delta = scrollPosition % clampDelta;
            var moveForward = (scrollPosition - scrollPositionBackup) > 0;

            var direction = 0;
            var collectedDelta = deltaCollected.Sum();
            
            if (Mathf.Abs(collectedDelta) > minCollectedDelta)
                direction = collectedDelta > 0 ? 1 : -1;
            else
            {
                if (moveForward)
                {
                    if (scrollPosition > 0)
                        direction = Mathf.Abs(delta) > clampDelta / 2 ? 1 : -1;
                    else
                        direction = Mathf.Abs(delta) > clampDelta / 2 ? -1 : 1;
                }
                else
                {
                    if (scrollPosition > 0)
                        direction = Mathf.Abs(delta) > clampDelta / 2 ? -1 : 1;
                    else
                        direction = Mathf.Abs(delta) > clampDelta / 2 ? -1 : 1;
                }
            }

            var positionBefore = scrollPosition;
            if (moveForward)
            {
                if (direction > 0)
                    scrollPosition += clampDelta - delta;
                else
                    scrollPosition -= delta;
            }
            else
            {
                if (direction > 0)
                    scrollPosition -= delta;
                else
                    scrollPosition -= delta + clampDelta;
            }

            var postDelta = scrollPosition - positionBefore;
            if (Mathf.Abs(postDelta) > clampDelta)
            {
                scrollPosition += postDelta < 0 ? clampDelta : -clampDelta;
            }
        }

        // Add inertia by touched motion distance
        private void AddDistanceInertia()
        {
            var deltaInertia = deltaCollected.Sum();
            deltaInertia *= inertiaMultiplier * 1f;
            scrollPosition += deltaInertia;
        }
        
        // Add inertia by power of scrolling
        private void AddPowerInertia()
        {
            var powerDelta = deltaCollected.Sum() / collectorSize;
            powerDelta *= inertiaMultiplier * 3;
            scrollPosition += powerDelta;
        }

        // Check move statistics scroll or page scroll by delta direction
        private void CheckLockByPageScroll()
        {
            if (!touched || pageScrollChecked)
                return;

            scrollDirectCollector += screenConverter.Invoke(InputHS.DeltaMove);

            if (Mathf.Abs(scrollDirectCollector.x) < scrollDirectSize &&
                Mathf.Abs(scrollDirectCollector.y) < scrollDirectSize) 
                return;

            if (Mathf.Abs(scrollDirectCollector.y) > Mathf.Abs(scrollDirectCollector.x))
                TouchUp();
            else
                PageScroll().Enabled = false;

            pageScrollChecked = true;
        }
    }
}
