using System;
using System.Linq;
using Architecture.Elements;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.Pages
{
    // Object for scrolling application pages
    public class PageScroll : IBehaviorSync
    {
        // Get current page
        private static PageItem CurrentPage() => AreasLocator.Instance.PageTransition.CurrentPage();
        
        // State of scroll
        private bool enabled = true;

        // Setup activity of scroll
        public bool Enabled
        {
            set
            {
                enabled = value;
                if (!enabled)
                {
                    deltaPosition = deltaPositionJoint;
                    ResetDeltaSpeedCollector();
                }
            }
        }

        // Animation params
        private const float joinSpeed = 0.217f;
        private const float joinSpeedDivider = 0.98f;
        private const float joinSpeedInertiaMultiplier = 0.7f;
        private float joinSpeedInertia;
        // Collect last scroll deltas to array
        private float[] deltaSpeedCollector;
        private int inertiaCycle;
        
        // Animation params when untouched
        private const int inertiaCycleBackup = 7;
        private const float inertiaMultiplier = 1.07f;
        // Additional scroll animation params
        private const float joinSpeedAutoJoin = 0.8f;
        private const float joinSpeedPosition = 50f;

        // Singleton
        public static PageScroll Instance => instance ?? (instance = new PageScroll());
        private static PageScroll instance;

        // Touched state
        private bool touched;
        // Function of convert screen delta to canvas delta
        private Func<Vector2, Vector2> screenConverter;
        
        // Scroll position for interpolation for containers for numbers
        private float deltaPosition;
        /// <summary>
        /// Scroll position for interpolation for containers for numbers,
        /// which changes to float - scrollPosition
        /// </summary>
        private float deltaPositionJoint;
        
        // Delay to update stet of unlock scroll flag component
        private int unlockDelay;

        // Flags indicating whether the scroll goes to the scroll border
        private bool joinPageToUp;
        private bool joinPageToBottom;
        
        // Main setup
        public void Start()
        {
            deltaSpeedCollector = new float[inertiaCycleBackup];
            screenConverter = MainCanvas.ScreenToCanvasDelta;
        }

        // Updates each frame
        public void Update()
        {
            if (!enabled)
                return;
            
            // If mode of join to scroll border than break
            if (JoinPageToClamp())
                return;
            
            // Move calculation for touched and free states
            if (touched)
            {
                // Calculate delta position
                var delta = screenConverter.Invoke(InputHS.DeltaMove);
                var scrollDelta = delta.y;
                deltaPosition += scrollDelta;
                
                // Save delta
                CollectInertia(scrollDelta);
                // Move page by delta
                MovePage(joinSpeed);
                // Set delta to access scroll component
                ScrollHandler.AddDeltaMove(screenConverter.Invoke(delta));
            }
            else
            {
                // Update inertia
                joinSpeedInertia *= joinSpeedDivider;
                // Move page by delta
                MovePage(joinSpeedInertia);
                // Reset scroll values to avoid floating point errors
                ClampDeltaGrowing();
                // Save delta
                CollectInertia(0);
            }

            // Calling a method waiting to unlock access for other by scroll
            UnlockScrollAccessWithDelay();
        }
        
        // Process of move page to border
        private bool JoinPageToClamp()
        {
            if (!joinPageToUp && !joinPageToBottom)
                return false;

            var currentPage = CurrentPage().Page;
            
            if (joinPageToUp)
                deltaPosition -= joinSpeedPosition;
            
            if (joinPageToBottom)
                deltaPosition += joinSpeedPosition;

            MovePage(joinSpeedAutoJoin);

            if (joinPageToUp && currentPage.anchoredPosition.y <= CurrentPage().MinAnchor)
                joinPageToUp = false;
            
            if (joinPageToBottom && currentPage.anchoredPosition.y >= CurrentPage().MaxAnchor)
                joinPageToBottom = false;
            
            if (!joinPageToUp && !joinPageToBottom)
                deltaPositionJoint = deltaPosition;
            
            return true;
        }

        // Move page process
        private void MovePage(float join)
        {
            var startJoint = deltaPositionJoint;
            deltaPositionJoint = Mathf.Lerp(deltaPositionJoint, deltaPosition, join);
            var currentDelta = deltaPositionJoint - startJoint;
            CurrentPage().ScrollUpdate(currentDelta);
        }

        // Reset inertia of page moving
        public void ResetInertia()
        {
            deltaPosition = deltaPositionJoint;
        }

        // Reset scroll values to avoid floating point errors
        private void ClampDeltaGrowing()
        {
            if (deltaPosition > 100 && deltaPositionJoint > 100)
            {
                deltaPosition -= 100;
                deltaPositionJoint -= 100;
            }
            if (deltaPosition < -100 && deltaPositionJoint < -100)
            {
                deltaPosition += 100;
                deltaPositionJoint += 100;
            }
        }

        // Save delta to array "deltaSpeedCollector"
        private void CollectInertia(float deltaMove)
        {
            if (inertiaCycle <= 0)
                inertiaCycle = inertiaCycleBackup;

            deltaSpeedCollector[inertiaCycle - 1] = deltaMove;
            inertiaCycle--;
        }

        // Add inertia by scrolled distance
        private void AddDistanceInertia()
        {
            var deltaInertia = deltaSpeedCollector.Sum();
            deltaInertia *= inertiaMultiplier;
            deltaPosition += deltaInertia;
        }
        
        // Reset values of "deltaSpeedCollector"
        private void ResetDeltaSpeedCollector()
        {
            for (var i = 0; i < deltaSpeedCollector.Length; i++)
                deltaSpeedCollector[i] = 0;
        }
        
        // Add inertia by power of scroll speed
        private void AddPowerInertia()
        {
            var powerDelta = deltaSpeedCollector.Sum() / inertiaCycleBackup;
            powerDelta *= inertiaMultiplier * 5;
            deltaPosition += powerDelta;
        }

        // Touched down method
        public void PointerDown(BaseEventData eventData)
        {
            deltaPosition = deltaPositionJoint;
            touched = true;
            inertiaCycle = 0;
        }

        // Touched up method
        public void PointerUp(BaseEventData eventData)
        {
            touched = false;
            joinSpeedInertia = joinSpeed * joinSpeedInertiaMultiplier;
            AddDistanceInertia();
            AddPowerInertia();
            unlockDelay = 1;
        }

        public void PointerEnter(BaseEventData eventData) { }
        public void PointerExit(BaseEventData eventData) { }

        // Calling a method waiting to unlock access for other by scroll
        private void UnlockScrollAccessWithDelay()
        {
            if (unlockDelay == 0)
            {
                ScrollHandler.Unlock();
                unlockDelay--;
            }
            else if (unlockDelay > 0)
                unlockDelay--;
        }
    }
}
