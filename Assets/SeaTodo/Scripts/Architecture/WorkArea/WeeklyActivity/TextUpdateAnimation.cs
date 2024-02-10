using System;
using HTools;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.WorkArea.WeeklyActivity
{
    // Component for dynamic text update ("float" view)
    public class TextUpdateAnimation : IBehaviorSync
    {
        private readonly Text text; // Text component
        private readonly string textPattern; // Text pattern
        private readonly string textFormat; // Text format

        private float mainProgress; // Target view of "float"
        private float localProgress; // Current view of "float"
        
        private Action<float> actionUpdated; // Action updated

        // Create and save component
        public TextUpdateAnimation(Text text, string textPattern, string textFormat)
        {
            this.text = text;
            this.textPattern = textPattern;
            this.textFormat = textFormat;
        }

        // Set actions that invokes when text updated
        public void SetActionTextUpdated(Action<float> action) => actionUpdated = action;

        // Set text view immediately
        public void SetupProgressImmediately(float progress)
        {
            mainProgress = progress;
            localProgress = progress;
            UpdateText();
        }
        
        // Setup new text view for animation
        public void SetupProgress(float progress)
        {
            mainProgress = progress;
        }

        public void Start() { }

        public void Update()
        {
            if (Mathf.Abs(mainProgress - localProgress) > 0.00005f)
            {
                // Update text view
                localProgress = Mathf.Lerp(localProgress, mainProgress, 0.27f);
                UpdateText();
            }
        }
        
        // Update text view
        private void UpdateText()
        {
            var percentage = string.Format(textFormat, localProgress);
            text.text = string.Format(textPattern, percentage);
            actionUpdated?.Invoke(localProgress);
        }
    }
}
