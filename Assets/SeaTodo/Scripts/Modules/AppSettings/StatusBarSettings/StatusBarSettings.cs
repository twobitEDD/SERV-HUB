using HomeTools;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace AppSettings.StatusBarSettings
{
    // Tools for setup of app bar
    public static class StatusBarSettings
    {
        // Height of navigation bar
        public static float Height;
        
        // Calculate height of 
        public static void Setup(bool spawn)
        {
            var height = 0;
            
            if (AppParameters.Android)
            {
                height = AndroidBarSettings.GetAndroidBarHeight();
            }
            if (AppParameters.Ios)
            {
                height = 1;
            }

            if (height == 0)
                height = 25;

            Height = height * (MainCanvas.CanvasScaler.referenceResolution.x / Camera.main.aspect) / Screen.height;

            if (spawn)
            {
                SetupInScene(Height);
            }
        }

        // Create status bap rect
        private static void SetupInScene(float pixelsHeight)
        {
            var imageObj = GameObject.Find("StatusBar");
            
            var result = imageObj.gameObject.AddComponent<Image>();
            var rect = imageObj.GetComponent<RectTransform>();
            rect.localPosition = new Vector3(0, 0, 0);
            rect.localScale = Vector3.one;
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, pixelsHeight * 2);
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0);
            AppTheming.AddElement(result, ColorTheme.StatusBar, AppTheming.AppItem.WorkArea);
        }
    }
}
