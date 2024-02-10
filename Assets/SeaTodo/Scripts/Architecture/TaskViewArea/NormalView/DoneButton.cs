using System;
using Architecture.Components;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Handling;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.NormalView
{
    // Component of done button part in task normal view
    public class DoneButton : IBehaviorSync
    {
        // Button component
        private readonly MainButtonJob mainButtonJob;
        // Rect of button
        private readonly RectTransform rectTransform;
        // Animation component of button move
        private RectTransformSync rectTransformSync;
        // Touched button action
        private readonly Action markAsCompletedAction;
        // Invoke action when move from page
        private readonly Action setupFromPageAction;
        // Button text
        private readonly Text markAsCompletedText;
        // Handle component
        private readonly HandleObject handleObject;
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;

        // Bottom position of part in page
        public readonly float GetBottomSide;
        // Parameter position
        private const float shiftToBottom = 17f;
        // Default height position
        private readonly float defaultHeightPosition;
        // Delay for touch reaction
        private int timerToFinish;
        
        // Create and setup
        public DoneButton(float upperSidePosition, Action setupFromPageAction, Action markAsCompletedAction)
        {
            // Save actions
            this.markAsCompletedAction = markAsCompletedAction;
            this.setupFromPageAction = setupFromPageAction;
            
            // Find component and create button component
            rectTransform = SceneResources.Get("ViewFlow Mark Completed Button").GetComponent<RectTransform>();
            mainButtonJob = new MainButtonJob(rectTransform, FinishFlow, rectTransform.Find("Circle").gameObject);
            mainButtonJob.AttachToSyncWithBehaviour();

            // Calculate button position
            defaultHeightPosition = upperSidePosition + AppElementsInfo.DistanceBetweenBackgrounds - shiftToBottom * 1.7f;
            rectTransform.anchoredPosition = new Vector2(0, defaultHeightPosition);
            GetBottomSide = rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y - shiftToBottom * 1.7f;

            // Find button text and add to Color Theming module and localization
            markAsCompletedText = rectTransform.Find("Name").GetComponent<Text>();
            AppTheming.AddElement(markAsCompletedText, ColorTheme.CreateFlowAreaButtonTextElements, AppTheming.AppItem.CreateFlowArea);
            TextLocalization.Instance.AddLocalization(markAsCompletedText, TextKeysHolder.MarksAsCompleted);
            
            // Add scroll events to button
            AddScrollEventsCustom.AddEventActions(rectTransform.Find("Scroll Handle").gameObject);

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
            rectTransformSync = new RectTransformSync();
            rectTransformSync.SetRectTransformSync(rectTransform);
            rectTransformSync.TargetPosition = new Vector3(0, SetupBasePositions(), 0);
            rectTransformSync.Speed = 0.3f;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rectTransformSync);
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
            rectTransform.anchoredPosition = new Vector2(0, defaultHeightPosition);
            mainButtonJob.Reactivate();
        }
        
        // Setup this part to page
        public void SetToPage(PageItem pageItem)
        {
            pageItem.AddContent(rectTransform);
            rectTransform.anchoredPosition = new Vector2(-10000, 0);
        }
        
        // Setup task as finished
        private void FinishFlow()
        {
            timerToFinish = 10;
            mainButtonJob.Deactivate();
            mainButtonJob.SimulateWave();
            setupFromPageAction.Invoke();
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
            
            markAsCompletedAction.Invoke();
            timerToFinish--;
        }
    }
}
