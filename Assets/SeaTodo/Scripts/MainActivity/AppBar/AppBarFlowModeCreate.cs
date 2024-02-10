using HomeTools.Source;
using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace MainActivity.AppBar
{
    public static class AppBarFlowModeCreate 
    {
        // Component position params
        private const float scaleOfProjectName = 0.7f;
        private const float distanceOfNameFromBottom = 0.3f;
        private const float shiftLineName = 0f;
        private const float shiftOfFlowIcon = 100;
        
        // Create animation component for bar plane 
        public static RectTransformSync CreateBarPlane()
        {
            var rectTransform = AppBarMaterial.RectTransform;
            
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(rectTransform);
            transformSync.TargetPosition = new Vector3(0, -rectTransform.sizeDelta.y / 4, 0);
            transformSync.Speed = 0.4f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(transformSync);
            
            return transformSync;
        }

        // Create animation component for move and scale of title name 
        public static RectTransformSync CreateProjectNameSync(RectTransform bar)
        {
            var rectTransform = bar.Find("BarTitle").GetComponent<RectTransform>();
            var icon = bar.Find("Image").GetComponent<RectTransform>();
            
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(rectTransform);

            var rightIconSide = icon.sizeDelta.x / 2 * (1 - scaleOfProjectName);
            var shiftDistance = icon.anchoredPosition.x + icon.sizeDelta.x / 2;
            var leftRectSide = rectTransform.anchoredPosition.x - rectTransform.sizeDelta.x / 2;
            var iconShift = icon.sizeDelta.x / 2 * (1f - scaleOfProjectName);
            var targetPosition = new Vector3(rectTransform.anchoredPosition.x - rectTransform.sizeDelta.x / 2 * (1f - scaleOfProjectName) - iconShift
                                             - (leftRectSide - shiftDistance) - rightIconSide, 
                rectTransform.anchoredPosition.y + bar.sizeDelta.y *  distanceOfNameFromBottom * 0.5f, 0);
            
            transformSync.TargetPosition = targetPosition;
            transformSync.TargetScale = Vector3.one * scaleOfProjectName;
            transformSync.Speed = 0.4f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(transformSync);

            return transformSync;
        }
        
        // Create animation component of alpha channel for title name 
        public static UIAlphaSync CreateProjectNameAlpha(RectTransform bar)
        {
            var text = bar.Find("BarTitle").GetComponent<Text>();

            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(text);
            alphaSync.Speed = 0.4f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync);
            
            return alphaSync;
        }
        
        // Create animation component for move and scale of title icon
        public static RectTransformSync CreateProjectIconSync(RectTransform bar)
        {
            var image = bar.Find("Image").GetComponent<RectTransform>();
            
            var iconShift = image.sizeDelta.x / 2 * (1f - scaleOfProjectName);
            
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(image);
            transformSync.TargetPosition = new Vector3(image.anchoredPosition.x - iconShift, 
                image.anchoredPosition.y + bar.sizeDelta.y *  distanceOfNameFromBottom * 0.5f, 0);
            transformSync.TargetScale = Vector3.one * scaleOfProjectName;
            transformSync.Speed = 0.4f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(transformSync);
            
            return transformSync;
        }
        
        // Create animation component of alpha channel for title icon
        public static UIAlphaSync CreateProjectIconAlpha(RectTransform bar)
        {
            var image = bar.Find("Image").GetComponent<SVGImage>();

            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(image);
            alphaSync.Speed = 0.6f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync);

            return alphaSync;
        }
        
        // Create animation component for move and scale of enter name field
        public static RectTransformSync CreateNameFieldSync(RectTransform bar, RectTransformSync syncProjectName, RectTransform inputField)
        {
            var leftNameProjectsSide = syncProjectName.TargetPosition.x 
                                       - syncProjectName.RectTransform.sizeDelta.x / 2 * syncProjectName.TargetScale.x;
            
            var position = new Vector2(leftNameProjectsSide + inputField.sizeDelta.x / 2 + shiftLineName, 
                -bar.sizeDelta.y * (1.5f - distanceOfNameFromBottom) + inputField.sizeDelta.y * 0.5f);
            inputField.anchoredPosition = position;
            
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(inputField);
            transformSync.TargetPosition = new Vector3(inputField.anchoredPosition.x, inputField.anchoredPosition.y + bar.sizeDelta.y * 0.2f, 0);
            transformSync.Speed = 0.4f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(transformSync);

            return transformSync;
        }
        
        // Create animation component of alpha channel for placeholder
        public static UIAlphaSync CreateNameFieldPlaceholderAlpha(RectTransform bar, RectTransform inputField)
        {
            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(inputField.Find("Placeholder").GetComponent<Text>());
            alphaSync.Speed = 0.4f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync);

            return alphaSync;
        }
        
        // Create animation component of alpha channel for input name field
        public static UIAlphaSync CreateNameFieldTextAlpha(RectTransform bar, RectTransform inputField)
        {
            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(inputField.Find("Text").GetComponent<Text>());
            alphaSync.Speed = 0.4f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync);

            return alphaSync;
        }
        
        // Create animation component for line under input name field
        public static RectTransformSync CreateLineFieldSync(RectTransform bar, RectTransformSync syncProjectName, RectTransform inputLine)
        {
            var rectTransform = inputLine;
            rectTransform.SetParent(bar);
            rectTransform.SetSiblingIndex(rectTransform.GetSiblingIndex() - 1);
            var leftNameProjectsSide = syncProjectName.TargetPosition.x 
                                       - syncProjectName.RectTransform.sizeDelta.x / 2 * syncProjectName.TargetScale.x;
            
            var position = new Vector2(leftNameProjectsSide + rectTransform.sizeDelta.x / 2 + shiftLineName, 
                -bar.sizeDelta.y * (1.5f - distanceOfNameFromBottom) + rectTransform.sizeDelta.y * 0.5f);
            rectTransform.anchoredPosition = position;
            
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(rectTransform);
            transformSync.TargetPosition = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + bar.sizeDelta.y * 0.3f, 0);
            transformSync.Speed = 0.4f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(transformSync);

            return transformSync;
        }
        
        // Create animation component of alpha channel for line under input name field
        public static UIAlphaSync CreateLineFieldAlpha(RectTransform bar, RectTransform inputLine)
        {
            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(inputLine.GetComponent<Image>());
            alphaSync.Speed = 0.4f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync);
            
            return alphaSync;
        }
        
        // Create animation component of rect for edit task icon object
        public static RectTransformSync CreateIconSync(RectTransform bar, RectTransform iconRect)
        {
            var position = new Vector2(-iconRect.sizeDelta.x / 2 - shiftOfFlowIcon, -bar.sizeDelta.y * 1.5f);
            iconRect.anchoredPosition = position;
            
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(iconRect);
            transformSync.TargetPosition = new Vector3(iconRect.anchoredPosition.x, iconRect.anchoredPosition.y + bar.sizeDelta.y * 0.5f, 0);
            transformSync.Speed = 0.3f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            transformSync.SetT(0);
            transformSync.SetDefaultT(0);
            SyncWithBehaviour.Instance.AddObserver(transformSync);

            return transformSync;
        }
        
        // Create animation component of alpha channel for edit task icon object
        public static UIAlphaSync CreateIconAlpha(RectTransform bar, RectTransform iconRect)
        {
            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(iconRect.Find("Shadow").GetComponent<Image>());
            alphaSync.AddElement(iconRect.Find("Circle").GetComponent<SVGImage>());
            alphaSync.AddElement(iconRect.Find("Icon").GetComponent<SVGImage>());
            alphaSync.Speed = 0.5f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            alphaSync.SetAlpha(0);
            alphaSync.SetDefaultAlpha(0);
            SyncWithBehaviour.Instance.AddObserver(alphaSync);
            
            return alphaSync;
        }
        
        // Create animation component of alpha channel for element of empty name object
        public static UIAlphaSync CreateEmptySignalAlpha(RectTransform bar, RectTransformSync nameField, 
                                                                            RectTransformSync projectIcon, SVGImage image)
        {
            var rectTransform = image.rectTransform;
            
            rectTransform.SetParent(bar);
            var position = new Vector2(nameField.TargetPosition.x - nameField.RectTransform.sizeDelta.x / 2f, nameField.RectTransform.anchoredPosition.y);
            
            rectTransform.anchoredPosition = position;

            var alphaSync = new UIAlphaSync();
            alphaSync.AddElement(image);
            alphaSync.Speed = 0.05f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            alphaSync.SetAlpha(0);
            alphaSync.SetDefaultAlpha(0);
            SyncWithBehaviour.Instance.AddObserver(alphaSync);
            
            return alphaSync;
        }
        
        // Create animation component for element of empty name object
        public static RectTransformSync CreateEmptySignalSync(RectTransform iconRect)
        {
            var targetPosition = iconRect.anchoredPosition + new Vector2(270, 0);
            
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(iconRect);
            transformSync.TargetPosition = targetPosition;
            transformSync.Speed = 0.05f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            transformSync.SetT(0);
            transformSync.SetDefaultT(0);
            SyncWithBehaviour.Instance.AddObserver(transformSync);

            return transformSync;
        }
    }
}
