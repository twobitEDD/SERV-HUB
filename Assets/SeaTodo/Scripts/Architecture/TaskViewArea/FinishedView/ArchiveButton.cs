using System;
using Architecture.Components;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.FinishedView
{
    // Component of button for archive task
    public class ArchiveButton : IBehaviorSync
    {
        private readonly MainButtonJob mainButtonJob; // Button component
        private readonly RectTransform rectTransform; // Rect of button
        // Animation component of button
        private RectTransformSync rectTransformSync;
        // Invoke action when archive
        private readonly Action archiveAction;
        // Link pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Parameter position
        private const float shiftFromBottom = 277f;
        // Default height position
        private float defaultHeightPosition;
        // Delay for touch reaction
        private int timerToFinish;
        
        // Create and setup
        public ArchiveButton(Action archiveAction)
        {
            // Save action
            this.archiveAction = archiveAction;

            // Find component and create button component
            rectTransform = SceneResources.Get("ViewFlow Archive Button").GetComponent<RectTransform>();
            mainButtonJob = new MainButtonJob(rectTransform, Archive, rectTransform.Find("Circle").gameObject);

            // Find button text and add to Color Theming module and localization
            var markAsCompletedText = rectTransform.Find("Name").GetComponent<Text>();
            AppTheming.AddElement(markAsCompletedText, 
                ColorTheme.CreateFlowAreaButtonTextElements, 
                AppTheming.AppItem.CreateFlowArea);
            TextLocalization.Instance.AddLocalization(markAsCompletedText, TextKeysHolder.Archive);
            
            timerToFinish = -1;
        }
        
        public void Start() { }

        public void Update()
        {
            CheckToCloseProcess();
        }

        // Setup parameters of button
        public void SetupMovableParameters()
        {
            // Setup position
            defaultHeightPosition = -MainCanvas.RectTransform.sizeDelta.y + shiftFromBottom;
            rectTransform.anchoredPosition = new Vector2(0, defaultHeightPosition);
            // Create animation component of button
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.TargetPosition = new Vector3(0, SetupBasePositions(), 0);
            rectTransformSync.Speed = 0.3f;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync, AppSyncAnchors.FlowViewFinished);
            SyncWithBehaviour.Instance.AddObserver(this, AppSyncAnchors.FlowViewFinished);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.FlowViewFinished);
        }
        
        // Setup part from page
        public void SetPartFromPage()
        {
            var centeredPosition = PageTransition().CurrentPage().Page.anchoredPosition.y +
                                   rectTransform.anchoredPosition.y;
            var direction = centeredPosition > 0 ? -1 : 1;

            rectTransformSync.Speed = 0.27f;
            rectTransformSync.SetDefaultT(1);
            rectTransformSync.TargetPosition = new Vector3(0, rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y * 4 * direction, 0);
            rectTransformSync.SetTByDynamic(0);
            rectTransformSync.Run();
        }
        
        // Show UI of page and reset button state
        public void SetPartToNewPage()
        {
            rectTransformSync.Speed = 0.12f;
            rectTransformSync.SetDefaultT(0);
            rectTransformSync.TargetPosition = new Vector3(0, rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y * 1.5f, 0);
            rectTransformSync.SetTByDynamic(1);
            rectTransformSync.Run();
            mainButtonJob.Reactivate();
        }
        
        // Setup this part to page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(rectTransform);
        
        // Call when clicked button
        private void Archive()
        {
            mainButtonJob.Deactivate();
            mainButtonJob.SimulateWave();
            archiveAction.Invoke();
        }
        
        // Setup base position for animation of button
        private float SetupBasePositions() => MainCanvas.RectTransform.sizeDelta.y - 
                                              AppElementsInfo.DistanceContentToBottom;

        // Check to close process after button touch
        private void CheckToCloseProcess()
        {
            if (timerToFinish < 0)
                return;

            if (timerToFinish > 0)
            {
                timerToFinish--;
                return;
            }
            
            archiveAction.Invoke();
            timerToFinish--;
        }
    }
}
