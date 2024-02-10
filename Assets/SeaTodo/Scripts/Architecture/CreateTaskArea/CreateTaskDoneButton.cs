using System;
using Architecture.Components;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using MainActivity.AppBar;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.CreateTaskArea
{
    // Button in bottom of create task page
    public class CreateTaskDoneButton : IBehaviorSync
    {
        private readonly MainButtonJob mainButtonJob; // Button object
        private readonly RectTransform rectTransform; // Rect of button
        private RectTransformSync rectTransformSync; // Animation component of rect
        private readonly Action createFlow; // Action of create new task
        private readonly Text createTaskText; // Text of button
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Link to app bar system
        private AppBar AppBar() => AreasLocator.Instance.AppBar;
        
        // Position info
        public readonly float GetBottomSide;
        private const float shiftToBottom = 17f;
        private readonly float defaultHeightPosition;

        private int timerToFinish;
        
        // Create button
        public CreateTaskDoneButton(float upperSidePosition, Action createFlow)
        {
            // Setup main and create button object
            this.createFlow = createFlow;
            rectTransform = SceneResources.Get("CreateFlow Create Button").GetComponent<RectTransform>();
            mainButtonJob = new MainButtonJob(rectTransform, CreateFlow, rectTransform.Find("Circle").gameObject);
            mainButtonJob.AttachToSyncWithBehaviour();

            // Calculate position of button in page
            defaultHeightPosition = upperSidePosition + AppElementsInfo.DistanceBetweenBackgrounds - shiftToBottom;
            rectTransform.anchoredPosition = new Vector2(0, defaultHeightPosition);
            GetBottomSide = rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y - shiftToBottom * 3.7f;

            // Setup name of button
            createTaskText = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(createTaskText, TextKeysHolder.CreateTask);
            timerToFinish = -1;
        }
        
        public void Start() { }

        public void Update()
        {
            CheckToCloseProcess();
        }

        // Create animation component for move in page
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
        
        // Play animation  - move from page
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
        
        // Prepare button to page
        public void SetPartToNewPage()
        {
            rectTransform.anchoredPosition = new Vector2(0, defaultHeightPosition);
            mainButtonJob.Reactivate();
        }
        
        // Setup button to page object
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(rectTransform);
        
        // Calls when touched
        private void CreateFlow()
        {
            // Check if name of task exists
            if (!AppBar().CheckEnteredNameOrSignal())
            {
                mainButtonJob.SimulateWave();
                return;
            }

            // Play button animation
            timerToFinish = 7;
            mainButtonJob.Deactivate();
            mainButtonJob.SimulateWave();
            // Invoke create task action 
            createFlow.Invoke();
            // Update colors in main page with new task
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.WorkArea);
        }

        // Setup base position of button in page
        private float SetupBasePositions() => MainCanvas.RectTransform.sizeDelta.y - 
                                              AppElementsInfo.DistanceContentToBottom;

        // Starts when task created
        private void CheckToCloseProcess()
        {
            if (timerToFinish < 0)
                return;

            // wait timer
            if (timerToFinish > 0)
            {
                timerToFinish--;
                return;
            }
                
            // Update app bar view
            AppBar().CloseCreateFlowMode();
            // Vibrate
            Vibration.Vibrate(25);
            timerToFinish--;
        }
    }
}
