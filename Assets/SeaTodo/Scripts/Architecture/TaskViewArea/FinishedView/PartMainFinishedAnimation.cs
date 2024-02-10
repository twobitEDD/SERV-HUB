using HomeTools.Source.Design;
using HTools;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.FinishedView
{
    // Component for animation UI of finished task view
    public class PartMainFinishedAnimation : IBehaviorSync
    {
        private const float animationSpeed = 0.15f;
        
        // Animation component of alpha channel for image
        private readonly UIAlphaSync imageAlphaSync;
        // Animation component of alpha channel for title
        private readonly UIAlphaSync titleTextAlphaSync;
        // Animation component of alpha channel for description
        private readonly UIAlphaSync descriptionTextAlphaSync;
        // Animation component of alpha channel for additional info about task
        private readonly UIAlphaSync infoTextAlphaSync;
        
        // Animation components for move UI 
        private readonly RectTransformSync titleTextSync;
        private readonly RectTransformSync descriptionTextSync;
        private readonly RectTransformSync infoTextSync;

        private int openTimer;

        // Create and setup
        public PartMainFinishedAnimation(Image finishedImage, Text titleText, Text descriptionText, Text infoText)
        {
            // Create animation components of motion
            titleTextSync = CreatePositionAnimation(titleText.rectTransform);
            descriptionTextSync = CreatePositionAnimation(descriptionText.rectTransform);
            infoTextSync = CreatePositionAnimation(infoText.rectTransform);
            // Create animation components of alpha channel
            titleTextAlphaSync = CreateAlphaSync(titleText);
            descriptionTextAlphaSync = CreateAlphaSync(descriptionText);
            infoTextAlphaSync = CreateAlphaSync(infoText);
            // Create animation components of alpha channel of image
            imageAlphaSync = CreateAlphaSync(finishedImage);
        }

        // Start show UI by activate animation components
        public void Show()
        {
            openTimer = 16;

            titleTextAlphaSync.SetDefaultAlpha(0);
            titleTextAlphaSync.SetAlpha(0);

            descriptionTextAlphaSync.SetDefaultAlpha(0);
            descriptionTextAlphaSync.SetAlpha(0);

            infoTextAlphaSync.SetDefaultAlpha(0);
            infoTextAlphaSync.SetAlpha(0);

            imageAlphaSync.Speed = animationSpeed;
            imageAlphaSync.SetDefaultAlpha(0);
            imageAlphaSync.SetAlpha(0);
            imageAlphaSync.SetAlphaByDynamic(1);
            imageAlphaSync.Run();
        }

        // Start hide UI by activate animation components
        public void Hide()
        {
            imageAlphaSync.Speed = animationSpeed * 3;
            imageAlphaSync.SetAlphaByDynamic(0);
            imageAlphaSync.Run();
            
            titleTextAlphaSync.Speed = animationSpeed * 3;
            titleTextAlphaSync.SetAlphaByDynamic(0);
            titleTextAlphaSync.Run();
            
            descriptionTextAlphaSync.Speed = animationSpeed * 3;
            descriptionTextAlphaSync.SetAlphaByDynamic(0);
            descriptionTextAlphaSync.Run();
            
            infoTextAlphaSync.Speed = animationSpeed * 3;
            infoTextAlphaSync.SetAlphaByDynamic(0);
            infoTextAlphaSync.Run();
        }

        public void Start() { }

        public void Update()
        {
            StartViewElements();
        }

        // Start show UI by activate animation components with order
        private void StartViewElements()
        {
            if (openTimer < 0)
                return;

            openTimer--;
            
            switch (openTimer)
            {
                case 15:
                    RunTextAlpha(titleTextSync, titleTextAlphaSync);
                    break;
                case 10:
                    RunTextAlpha(infoTextSync, infoTextAlphaSync);
                    break;
                case 5:
                    RunTextAlpha(descriptionTextSync, descriptionTextAlphaSync);
                    break;
            }
        }

        // Activate animation component for texts
        private static void RunTextAlpha(RectTransformSync rectSync, UIAlphaSync syncAlpha)
        {
            rectSync.SetDefaultT(0);
            rectSync.SetTByDynamic(1);
            rectSync.Run();
            syncAlpha.Speed = animationSpeed * 0.23f;
            syncAlpha.SetAlphaByDynamic(1);
            syncAlpha.Run();
        }

        // Create animation component for rect moving
        private static RectTransformSync CreatePositionAnimation(RectTransform rectTransform)
        {
            var sync = new RectTransformSync();
            sync.SetRectTransformSync(rectTransform);
            sync.Speed = animationSpeed * 0.8f;
            sync.SpeedMode = RectTransformSync.Mode.Lerp;
            sync.TargetPosition = rectTransform.anchoredPosition + new Vector2(0, 37);
            sync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(sync, AppSyncAnchors.FlowViewFinished);

            return sync;
        }
        
        // Create animation component for alpha channel
        private static UIAlphaSync CreateAlphaSync(Graphic text)
        {
            var sync = new UIAlphaSync();
            sync.AddElement(text);
            sync.SpeedMode = UIAlphaSync.Mode.Lerp;
            sync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(sync, AppSyncAnchors.FlowViewFinished);

            return sync;
        }
    }
}
