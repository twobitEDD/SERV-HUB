using Architecture.Other;
using Architecture.TextHolder;
using Architecture.WorkArea.WeeklyActivity;
using HTools;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.StatisticsArea.Frequency
{
    // Item frequency activity of day of week
    public class FrequencyItem
    {
        public WeekInfo.DayOfWeek DayOfWeek; // Day of week for item
        
        // UI elements
        private readonly SVGImage percentage;
        private readonly Text day;
        private readonly Text frequencyText;
        // Animation component of frequency text
        private readonly TextUpdateAnimation textUpdateAnimation;
        // Position shift of global percentage text
        private readonly float frequencyShift;
        // Path to circle icons
        private const string percentagePath = "ActiveResources/CirclePercentage/CirclePercentage";

        // Create and setup
        public FrequencyItem(RectTransform rectTransform)
        {
            // Find UI components
            percentage = rectTransform.Find("Percentage").GetComponent<SVGImage>();
            day = rectTransform.Find("Day").GetComponent<Text>();
            frequencyText = rectTransform.Find("Frequency").GetComponent<Text>();
            frequencyShift = frequencyText.rectTransform.anchoredPosition.x;
            
            // Create animation component of percentage text
            textUpdateAnimation = new TextUpdateAnimation(frequencyText, "{0}%", "{0:0}");
            textUpdateAnimation.SetActionTextUpdated(SetupView);
            SyncWithBehaviour.Instance.AddObserver(textUpdateAnimation, AppSyncAnchors.StatisticsArea);
        }

        // Update frequency view
        public void Update(float frequency)
        {
            day.text = TextHolderTime.DaysOfWeekShort(DayOfWeek);

            var info = (int) Mathf.Clamp(frequency * 100, 0, 100);
            textUpdateAnimation.SetupProgressImmediately(info);
            var transform = frequencyText.rectTransform;
            transform.anchoredPosition = new Vector2(frequencyShift, transform.anchoredPosition.y);
        }
        
        // Update frequency view with animation
        public void UpdateDynamic(float frequency)
        {
            var info = (int) Mathf.Clamp(frequency * 100, 0, 100);
            textUpdateAnimation.SetupProgress(info);
        }
        
        // Update UI view by frequency
        private void SetupView(float frequency)
        {
            frequency /= 10;
            var prefix = (int)(frequency) * 10;

            prefix = Mathf.Clamp(prefix, 0, 100);
            percentage.sprite = LoadIconByPrefix(prefix);
        }
        
        // Load circle icon by prefix
        private static Sprite LoadIconByPrefix(int prefix) =>
                    Object.Instantiate(Resources.Load<Sprite>($"{percentagePath}{prefix}"));
    }
}
