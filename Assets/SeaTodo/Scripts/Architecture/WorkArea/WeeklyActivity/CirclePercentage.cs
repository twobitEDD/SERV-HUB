using HTools;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.WorkArea.WeeklyActivity
{
    // Component of circle percentage
    public class CirclePercentage : IBehaviorSync
    {
        // UI elements
        public readonly Image RoundPercentage;
        public readonly Image StartLineCircle;
        public readonly Image EndLineCircle;

        // Current and target progress
        private float currentPercentage;
        private float targetPercentage;
        
        // Circle radius
        private const float radius = 29.24f;
        
        // Create and find UI elements
        public CirclePercentage(Transform icon)
        {
            RoundPercentage = icon.Find("Percentage").GetComponent<Image>();
            StartLineCircle = icon.Find("StartLineCircle").GetComponent<Image>();
            EndLineCircle = icon.Find("EndLineCircle").GetComponent<Image>();
        }
        
        public void Start() { }
        
        // Updates each frame
        public void Update()
        {
            if (Mathf.Abs(currentPercentage - targetPercentage) < 0.01f) 
                return;
            
            currentPercentage = Mathf.Lerp(currentPercentage, targetPercentage, 0.05f);
            UpdatePercentage();
        }

        // Update current circle percentage
        public void SetupPercentage(float percentage, bool immediately)
        {
            if (immediately)
            {
                targetPercentage = percentage;
                currentPercentage = percentage;
                UpdatePercentage();
            }
            else
            {
                targetPercentage = percentage;
            }
        }

        // Update circle by percentage
        private void UpdatePercentage()
        {
            RoundPercentage.enabled = currentPercentage > 0.001f;
            StartLineCircle.enabled = currentPercentage > 0.001f;
            EndLineCircle.enabled = currentPercentage > 0.001f;
            
            if (!RoundPercentage.enabled)
                return;

            RoundPercentage.fillAmount = currentPercentage;
            
            var angle = 360 * currentPercentage;
            EndLineCircle.rectTransform.anchoredPosition = new Vector2(
                radius * Mathf.Cos(-DegreesToRadians(angle - 90)), 
                radius * Mathf.Sin(-DegreesToRadians(angle - 90)));
        }
        
        // Convert degrees to radians
        private static float DegreesToRadians(float degrees)
        {
            return degrees * Mathf.PI / 180f;
        }
    }
}
