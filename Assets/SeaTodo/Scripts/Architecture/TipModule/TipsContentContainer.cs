using System;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TipModule
{
    // Component that contains tip objects for tips view
    public class TipsContentContainer
    {
        // Tip markers
        public enum Type
        {
            TaskLimit,
            SimilarMarker,
        }

        // Rect and animation component of task limit tip
        // when click in menu on create task button
        private readonly RectTransform tasksLimitTip;
        private readonly UIAlphaSync tasksLimitTipAlphaSync;
        
        // Rect and animation component of similar marker
        // when try to create characteristic name that already exists
        private readonly RectTransform similarMarkerTip;
        private readonly UIAlphaSync similarMarkerTipAlphaSync;

        // Create and setup
        public TipsContentContainer()
        {
            // Setup task limit tip
            tasksLimitTip = SceneResources.Get("Tasks Limit Tip").GetComponent<RectTransform>();
            tasksLimitTipAlphaSync = CreateAlphaSync(tasksLimitTip);
            var textLimit = tasksLimitTip.GetChild(0).GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textLimit, TextKeysHolder.TasksLimitTip);
            
            // Setup similar characteristic name tip
            similarMarkerTip = SceneResources.Get("Similar Marker Tip").GetComponent<RectTransform>();
            similarMarkerTipAlphaSync = CreateAlphaSync(similarMarkerTip);
            var textMarker = similarMarkerTip.GetChild(0).GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textMarker, TextKeysHolder.SimilarMarkerTip);
        }

        // Get tip rect by marker
        public RectTransform GetTipContent(Type type)
        {
            switch (type)
            {
                case Type.TaskLimit:
                    return tasksLimitTip;
                case Type.SimilarMarker:
                    return similarMarkerTip;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        // Get animation component of tip by marker
        public UIAlphaSync GetTipContentAlphaSync(Type type)
        {
            switch (type)
            {
                case Type.TaskLimit:
                    return tasksLimitTipAlphaSync;
                case Type.SimilarMarker:
                    return similarMarkerTipAlphaSync;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        // Create animation component of alpha channel for tip text
        private static UIAlphaSync CreateAlphaSync(RectTransform content)
        {
            var result = new UIAlphaSync
            {
                SpeedMode = UIAlphaSync.Mode.Lerp
            };

            var texts = content.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                var mat = new Material(text.material);
                text.material = mat;
                result.AddMaterialElement(mat);
            }
            
            result.AddElements(content.GetComponentsInChildren<Graphic>());
            result.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(result, AppSyncAnchors.TipModule);
            
            return result;
        }
    }
}
