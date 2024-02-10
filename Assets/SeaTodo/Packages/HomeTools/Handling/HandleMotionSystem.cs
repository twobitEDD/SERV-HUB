using System;
using UnityEngine;

namespace HomeTools.Handling
{
    // Component for detect touch motions
    public class HandleMotionSystem
    {
        // Motion types
        public enum Motion
        {
            swipeDown,
            swipeRight,
            touch
        }
        
        // Delta time
        private float DeltaTime() => Time.deltaTime;
        
        // Detected motion action
        public Action ResultAction;
        // Motion type
        public Motion MotionType;

        private float time; // Default motion time
        private float deltaMin; // Min delta motion
        private float deltaMax; // Max delta motion

        private float currentTime; // Current motion type
        private Vector2 currentDelta; // Current motion delta
        
        // Setup params for motion detection
        public void SetParameters(float motionTime, float minMotionDelta, float maxMotionDelta)
        {
            time = motionTime;
            deltaMin = minMotionDelta;
            deltaMax = maxMotionDelta;
        }

        // Touch down method
        public void TouchDown()
        {
            currentDelta = Vector2.zero;
            currentTime = 0;
        }
        
        // Touch up method
        public void TouchUp()
        {
            DetectSwipeDown();
            DetectSwipeRight();
            DetectTouch();
        }

        // Add move delta
        public void AddMoveDelta(Vector2 delta)
        {
            currentDelta += delta;
            currentTime += DeltaTime();
        }

        // Process of detect down swipe
        private void DetectSwipeDown()
        {
            if (MotionType != Motion.swipeDown)
                return;

            if (currentDelta.y < -deltaMin && Mathf.Abs(currentDelta.y) > deltaMin && currentTime < time)
                ResultAction?.Invoke();
        }
        
        // Process of detect right swipe
        private void DetectSwipeRight()
        {
            if (MotionType != Motion.swipeRight)
                return;

            if (currentDelta.x > deltaMin && Mathf.Abs(currentDelta.x) > deltaMin && currentTime < time)
                ResultAction?.Invoke();
        }
        
        // Process of detect touch
        private void DetectTouch()
        {
            if (MotionType != Motion.touch)
                return;

            if (Mathf.Abs(currentDelta.x) < deltaMin && currentTime < time)
                ResultAction?.Invoke();
        }
    }
}
