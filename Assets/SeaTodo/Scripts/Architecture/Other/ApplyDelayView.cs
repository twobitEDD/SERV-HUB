using System;
using HomeTools.Other;
using HTools;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.Other
{
    // Class for working the waiting bar around the button
    public class ApplyDelayView : IBehaviorSync
    {
        // UI components
        private readonly Image round;
        private readonly SVGImage circle;
        private readonly SVGImage circleStart;
        // Action invoke when wait animation is complete
        private readonly Action finishAction;
        // Rect component of circle
        private readonly RectTransform circleRect;

        // Main animation params
        private readonly float roundRadius;
        private bool active;
        private float process;
        private const float speed = 0.077f;
        
        //Create and setup
        public ApplyDelayView(Image round, SVGImage circle, SVGImage circleStart, Action finishAction)
        {
            this.round = round;
            this.circle = circle;
            this.circleStart = circleStart;
            this.finishAction = finishAction;
            circleRect = circle.rectTransform;
            roundRadius = circleRect.anchoredPosition.y;
        }
        
        public void Start() { }

        // Reset state
        public void Reset()
        {
            process = 0;
            active = false;
            round.enabled = false;
            circle.enabled = false;
            circleStart.enabled = false;
        }

        // Activate animation
        public void StartView()
        {
            active = true;
            round.enabled = true;
            circle.enabled = true;
            circleStart.enabled = true;
        }

        public void Update()
        {
            ActiveProcess();
        }

        // Animation process
        private void ActiveProcess()
        {
            if (!active)
                return;

            // Check for finish of animation
            if (process >= 1.2f)
            {
                active = false;
                finishAction.Invoke();
                return;
            }

            // Update circle view
            round.fillAmount = process;
            var angle = 360f * process;
            circleRect.anchoredPosition = new Vector2(
                roundRadius * Mathf.Cos(-OtherHTools.DegreesToRadians(angle - 90)), 
                roundRadius * Mathf.Sin(-OtherHTools.DegreesToRadians(angle - 90)));
            
            process += speed;
        }
    }
}
