using Architecture.Other;
using Architecture.TextHolder;
using InternalTheming;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace Architecture.TaskViewArea.NormalView
{
    // Component of reminder icon item
    public class ReminderItem
    {
        public WeekInfo.DayOfWeek DayOfWeek; // Day of week of reminder

        // UI components
        private readonly Text day;
        private readonly SVGImage circle;
        private readonly SVGImage icon;
        private readonly Image line;

        // Create and find UI components
        public ReminderItem(RectTransform rectTransform)
        {
            day = rectTransform.Find("Day").GetComponent<Text>();
            circle = rectTransform.Find("Circle").GetComponent<SVGImage>();
            icon = rectTransform.Find("Icon").GetComponent<SVGImage>();
            line = rectTransform.Find("Line").GetComponent<Image>();
        }

        // Update activity of reminder item (show if reminder activity)
        public void Update(bool active)
        {
            day.text = TextHolderTime.DaysOfWeekShort(DayOfWeek);
            SetupColors(active);
            icon.enabled = true;
        }
        
        // Setup colors of UI by reminder activity
        private void SetupColors(bool active)
        {
            if (active)
            {
                circle.color = ThemeLoader.GetCurrentTheme().SecondaryColor;
                icon.color = ThemeLoader.GetCurrentTheme().ImagesColor;
                day.color = ThemeLoader.GetCurrentTheme().SecondaryColorP1;
                circle.rectTransform.sizeDelta = Vector2.one * 67;
                line.enabled = false;
            }
            else
            {
                circle.color = ThemeLoader.GetCurrentTheme().ViewFlowAreaRemindersCirclePassive;
                icon.color = ThemeLoader.GetCurrentTheme().ViewFlowAreaRemindersIconPassive;
                day.color = ThemeLoader.GetCurrentTheme().ViewFlowAreaRemindersDayPassive;
                circle.rectTransform.sizeDelta = Vector2.one * 57;
                line.color = ThemeLoader.GetCurrentTheme().ViewFlowAreaRemindersIconPassive;
                line.enabled = true;
            }
        }
    }
}
