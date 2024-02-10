using Architecture.Data;
using UnityEngine;

namespace Architecture.ModuleTrackTime
{
    // Component for update UI by 12 hours or 24 hours format
    public class VisualByTimeMode
    {
        // UI rect objects
        private readonly RectTransform dayPartRect;
        private readonly RectTransform hoursPartRect;
        private readonly RectTransform minutesPartRect;
        private readonly RectTransform colonPartRect;
        
        // Positions and scales for day part for different formats 
        private readonly Vector2 dayPartEnglish = new Vector2(153, 0);
        private readonly Vector2 dayPartEnglishSize = new Vector2(100, 600);
        private readonly Vector2 dayPartFull = new Vector2(-175, 0);
        private readonly Vector2 dayPartFullSize = new Vector2(100, 600);
        
        // Positions and scales for hours part for different formats
        private readonly Vector2 hoursPartEnglish = new Vector2(-147, 0);
        private readonly Vector2 hoursPartEnglishSize = new Vector2(150, 600);
        private readonly Vector2 hoursPartFull = new Vector2(-100, 0);
        private readonly Vector2 hoursPartFullSize = new Vector2(200, 600);
        
        // Positions and scales for minutes part for different formats
        private readonly Vector2 minutesPartEnglish = new Vector2(17, 0);
        private readonly Vector2 minutesPartEnglishSize = new Vector2(170, 600);
        private readonly Vector2 minutesPartFull = new Vector2(100, 0);
        private readonly Vector2 minutesPartFullSize = new Vector2(200, 600);
        
        // Positions for colon part for different formats
        private readonly Vector2 colonPartEnglish = new Vector2(-67f, 9.4f);
        private readonly Vector2 colonPartFull = new Vector2(0f, 9.4f);

        // Save components
        public VisualByTimeMode(RectTransform dayPart, RectTransform hoursPart, RectTransform minutesPart, RectTransform colonPart)
        {
            dayPartRect = dayPart;
            hoursPartRect = hoursPart;
            minutesPartRect = minutesPart;
            colonPartRect = colonPart;
        }

        // Update UI by current time format
        public void Update()
        {
            var englishFormat = AppCurrentSettings.EnglishFormat;

            dayPartRect.anchoredPosition = englishFormat ? dayPartEnglish : dayPartFull;
            dayPartRect.sizeDelta = englishFormat ? dayPartEnglishSize : dayPartFullSize;
            dayPartRect.gameObject.SetActive(englishFormat);

            hoursPartRect.anchoredPosition = englishFormat ? hoursPartEnglish : hoursPartFull;
            hoursPartRect.sizeDelta = englishFormat ? hoursPartEnglishSize : hoursPartFullSize;

            minutesPartRect.anchoredPosition = englishFormat ? minutesPartEnglish : minutesPartFull;
            minutesPartRect.sizeDelta = englishFormat ? minutesPartEnglishSize : minutesPartFullSize;
            
            colonPartRect.anchoredPosition = englishFormat ? colonPartEnglish : colonPartFull;
        }
    }
}
