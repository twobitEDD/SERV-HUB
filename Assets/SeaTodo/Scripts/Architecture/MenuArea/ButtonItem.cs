using System;
using HomeTools.Messenger;
using UnityEngine;

namespace Architecture.MenuArea
{
    // Item of menu movable button
    public class ButtonItem : IDisposable
    {
        // Rect of button
        private readonly RectTransform rectTransform;

        private Vector3 basePosition; // Base position of button
        private Vector3 targetPosition; // Target position of button
        private Vector3 currentPosition; // Current position of button

        private Vector3 firstCurrentPosition; // First current position of button
        private float markerAnimationDelay; // Index changing interpolation
        private float localMarker; // Interpolation of button position
        private bool touched; // Touched state flag
        private bool blackoutTouched; // Touched dark screen flag

        // Create and setup
        public ButtonItem(RectTransform rectTransform)
        {
            this.rectTransform = rectTransform;
            MainMessenger.AddMember(CleanWork, AppMessagesConst.MenuStartedShow);
        }

        // Setup base and target position for button animations
        public void SetupPositionParameters(Vector3 targetShift)
        {
            basePosition = rectTransform.anchoredPosition;
            targetPosition = basePosition - targetShift;
        }

        // Setup params for animation of button
        public void SetupAnimationParameters(Vector3 firstPosition)
        {
            firstCurrentPosition = firstPosition;
            CleanWork();
        }

        // Setup activity of button
        public void SetActive(bool active) => rectTransform.gameObject.SetActive(active);
        // Setup index changing interpolation
        public void SetupMarkerDelay(float markerDelay) => markerAnimationDelay = markerDelay;
        // Setup touched button state
        public void SetupTouched(bool touchedState) => touched = touchedState;
        // Setup touched dark screen state
        public void SetupBlackoutTouched(bool touchedState) => blackoutTouched = touchedState;
        // Setup interpolation position of button
        public void SetMarkerPosition(float marker)
        {
            var simple = Vector3.LerpUnclamped(basePosition, targetPosition, marker);
            
            currentPosition = CalculateAnimatedXPosition(currentPosition, simple, marker);
            rectTransform.anchoredPosition = currentPosition;
        }

        // Calculating button movements by states
        private Vector3 CalculateAnimatedXPosition(Vector3 position, Vector3 simplePosition, float marker)
        {
            // Move when touched
            if (touched && marker >= 1f && localMarker <= marker)
            {
                localMarker = marker;
                return Vector3.LerpUnclamped(position, simplePosition, 0.5f);
            }
            
            // Move back when touched
            if (!touched && marker >= 1f && localMarker > marker)
            {
                localMarker = marker;
                return Vector3.LerpUnclamped(position, simplePosition, 0.5f);
            }
            // Do not move whe touched forward
            if (!touched && marker >= 1f && localMarker < marker && currentPosition.x <= targetPosition.x && !blackoutTouched)
            {
                localMarker = 1f;
                currentPosition = targetPosition;
                return position;
            }
            // Do not move when touched when release button
            if (!touched && marker > 1f && localMarker <= 1.01f && currentPosition.x <= targetPosition.x && !blackoutTouched)
            {
                currentPosition = targetPosition;
                return position;
            }
            
            // Move squeezed forward
            if (!touched && marker >= 1f && currentPosition.x >= targetPosition.x && !blackoutTouched)
            {
                localMarker = 1f;
                return Vector3.LerpUnclamped(position, targetPosition, 0.5f);
            }
            
            // Move to target position 
            if (targetPosition.x > position.x && localMarker <= marker && marker >= 1f)
            {
                localMarker = marker;
                return Vector3.LerpUnclamped(position, simplePosition, markerAnimationDelay * 0.5f);
            }
            
            // Move to target position 
            if (targetPosition.x > position.x && localMarker >= marker && marker >= 1f)
            {
                localMarker = marker;
                return Vector3.LerpUnclamped(position, simplePosition, markerAnimationDelay);
            }
            
            // Move to target position 
            if (targetPosition.x < position.x && localMarker <= marker)
            {
                localMarker = marker;
                return Vector3.LerpUnclamped(position, simplePosition, markerAnimationDelay);
            }

            return simplePosition;
        }

        // Reset move button to target position
        private void CleanWork()
        {
            currentPosition = firstCurrentPosition;
            localMarker = 0;
        }

        public void Dispose()
        {
            MainMessenger.RemoveMember(CleanWork, AppMessagesConst.MenuStartedShow);
        }
    }
}
