using HomeTools.Source.Design;
using HTools;
using UnityEngine;

namespace MainActivity.AppBar
{
    // Component for create animation components for edit task page
    public static class AppBarEditFlowModeCreate
    {
        // Create animation components for task name that in task view page
        public static RectTransformSync CreateNameSync(RectTransform rectTransform, RectTransformSync rectTransformSync)
        {
            var xPosition = rectTransformSync.TargetPosition.x - rectTransformSync.RectTransform.sizeDelta.x / 2 + rectTransform.sizeDelta.x / 2;
            
            var transformSync = new RectTransformSync();
            transformSync.SetRectTransformSync(rectTransform);
            transformSync.TargetPosition = new Vector2(xPosition + 10, rectTransform.anchoredPosition.y);
            transformSync.TargetScale = new Vector3(0.5f, 0.5f, 0.5f);
            transformSync.Speed = 0.4f;
            transformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            transformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(transformSync);

            return transformSync;
        }
    }
}
