using HomeTools.Source.Design;
using HTools;
using Theming;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace MainActivity.AppBar
{
    // Component for create animation components for edit task page
    public static class AppBarViewFlowModeCreate
    {
        private static readonly Vector2 NameShift = new Vector2(0, 2);
        
        // Create animation components of alpha channel for title that in edit task page
        public static UIAlphaSync CreateProjectNameAlpha(RectTransform bar, Text text)
        {
            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(text);
            alphaSync.Speed = 0.4f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync);
            
            return alphaSync;
        }
        
        // Create text element for task name
        public static Text CreateFlowName(Text projectName)
        {
            var text = Object.Instantiate(projectName, projectName.transform.parent, true).GetComponent<Text>();
            text.rectTransform.anchoredPosition += NameShift;
            text.text = "English";
            return text;
        }

        // Create animation components of alpha channel for task name text
        public static UIAlphaSync CreateFlowNameAlpha(Text flowText)
        {
            AppTheming.AddElement(flowText, ColorTheme.AppBarElements, AppTheming.AppItem.Other);
            AppTheming.ColorizeElement(flowText, ColorTheme.AppBarElements);
            var color = flowText.color;
            color.a = 1;
            flowText.color = color;
            
            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(flowText);
            alphaSync.Speed = 0.4f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            alphaSync.SetAlpha(0);
            alphaSync.SetDefaultAlpha(0);
            SyncWithBehaviour.Instance.AddObserver(alphaSync);
            
            color.a = 0;
            flowText.color = color;
            
            return alphaSync;
        }

        // Create animation components of alpha channel for icon image
        public static UIAlphaSync CreateIconAlpha(SVGImage icon)
        {
            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(icon);
            alphaSync.Speed = 0.4f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync);
            
            return alphaSync;
        }

        // Create animation components of alpha channel for background of icon
        public static UIAlphaSync CreateBackAlpha(SVGImage icon)
        {
            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(icon);
            alphaSync.Speed = 0.4f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync);
            
            return alphaSync;
        }
        
        // Create animation components for background rect
        public static RectTransformSync CreateBackSync(RectTransform rectTransform)
        {
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(rectTransform);
            
            transformSync.TargetPosition = rectTransform.anchoredPosition - new Vector2(50, 0);
            transformSync.Speed = 0.4f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(transformSync);

            return transformSync;
        }
    }
}
