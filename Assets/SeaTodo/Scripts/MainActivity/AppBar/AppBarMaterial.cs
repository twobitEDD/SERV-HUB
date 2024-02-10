using AppSettings.StatusBarSettings;
using UnityEngine;

namespace MainActivity.AppBar
{
    // Component for setup app bar background
    public static class AppBarMaterial
    {
        public static RectTransform RectTransform { get; private set; }
        public static float BarHeight;
    
        public static void Reset()
        {
            RectTransform = GameObject.Find("AppBar").GetComponent<RectTransform>();

            var height = Screen.width * 0.155f;
            BarHeight = height * (MainCanvas.CanvasScaler.referenceResolution.x / Camera.main.aspect) / Screen.height;

            RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, BarHeight * 2 + StatusBarSettings.Height * 2);
            RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, 0);
        }
    }
}
