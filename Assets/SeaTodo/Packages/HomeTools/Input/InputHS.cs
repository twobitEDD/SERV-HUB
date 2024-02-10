using System;
using HomeTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HTools
{
    // Class for get input of screen 
    public static class InputHS
    {
        // Delta move by last frame
        public static Vector2 DeltaMove { get; private set; }
        // Get touch position 
        public static Vector2 TouchPosition { get; private set; }

        private static bool touchedLastFrame;

        // Check if device
        private static readonly bool device = AppParameters.Device;

        // Updates each frame
        public static void Update()
        {
            PositionHandling();
        }

        // Calculate handle params
        private static void PositionHandling()
        {
            if (Touched && touchedLastFrame)
            {
                if (device)
                {
                    DeltaMove = DeltaOfTouches();
                }
                else
                {
                    DeltaMove = EventSystem.current.currentInputModule.input.mousePosition - TouchPosition;
                }

                CalculateTouchPosition();
            }
            else
            {
                if (Touched)
                    CalculateTouchPosition();
                else
                    DeltaMove = TouchPosition = Vector2.zero;

                touchedLastFrame = Touched;
            }
        }

        // Get delta of touches
        private static Vector2 DeltaOfTouches()
        {
            var result = Vector2.zero;
            var touches = EventSystem.current.currentInputModule.input.touchCount;
            for (var i = 0; i < touches; i++)
            {
                result += EventSystem.current.currentInputModule.input.GetTouch(i).deltaPosition;
            }

            return result / touches;
        }

        // Update touch position
        private static void CalculateTouchPosition()
        {
            if (device)
                TouchPosition = Input.touchCount > 0 ? Input.touches[0].position : Vector2.zero;
            else
                TouchPosition =  EventSystem.current.currentInputModule.input.mousePosition;
        }

        // Check if touched
        public static bool Touched => device ? Input.touchCount > 0 : Input.GetMouseButton(0);
    }
}