using System;
using Architecture.Components;
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
    // Component of button for move task back to progress (or active state)
    public class ToProgressButton : IBehaviorSync
    {
        private readonly MainButtonJob mainButtonJob; // Button component
        private readonly RectTransform rectTransform; // Rect of button
        private UIAlphaSync alphaSync; // Animation component of alpha channel
        // Action for task mark as completed
        private readonly Action markAsCompletedAction;
        // Action for setup from page when moved to in progress
        private readonly Action setupFromPageAction;
        // Button text
        private readonly Text markAsCompletedText;
        // Parameter position
        private const float shiftFromBottom = 397f;
        // Default height position
        private float defaultHeightPosition;
        // Delay for touch reaction
        private int timerToFinish;
        
        // Create and setup
        public ToProgressButton(Action setupFromPageAction, Action markAsCompletedAction)
        {
            // Save actions
            this.markAsCompletedAction = markAsCompletedAction;
            this.setupFromPageAction = setupFromPageAction;
            
            // Find components and create button component
            markAsCompletedText = SceneResources.Get("ViewFlow To Progress Button").GetComponent<Text>();
            rectTransform = markAsCompletedText.rectTransform;
            mainButtonJob = new MainButtonJob(rectTransform, MoveToProgress, rectTransform.Find("Circle").gameObject);

            // Add button text to Color Theming module
            AppTheming.AddElement(markAsCompletedText, ColorTheme.SecondaryColorD2, AppTheming.AppItem.CreateFlowArea);
            // Localize button text
            TextLocalization.Instance.AddLocalization(markAsCompletedText, TextKeysHolder.BackToProgress);
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
            
            // Create animation component of alpha channel
            alphaSync = new UIAlphaSync();
            alphaSync.AddElement(markAsCompletedText);
            alphaSync.Speed = 0.3f;
            alphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            alphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(alphaSync, AppSyncAnchors.FlowViewFinished);
            SyncWithBehaviour.Instance.AddObserver(this, AppSyncAnchors.FlowViewFinished);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.FlowViewFinished);
        }
        
        // Setup part from page 
        public void SetPartFromPage()
        {
            alphaSync.Speed = 0.27f;
            alphaSync.SetAlphaByDynamic(0);
            alphaSync.Run();
        }
        
        // Show UI of page and reset button state
        public void SetPartToNewPage()
        {
            rectTransform.anchoredPosition = new Vector2(0, defaultHeightPosition);
            
            alphaSync.Speed = 0.04f;
            alphaSync.SetAlphaByDynamic(1);
            alphaSync.Run();
            
            mainButtonJob.Reactivate();
        }
        
        // Setup this part to page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(rectTransform);
        
        // Call when clicked button
        private void MoveToProgress()
        {
            timerToFinish = 10;
            mainButtonJob.Deactivate();
            mainButtonJob.SimulateWave();
            setupFromPageAction.Invoke();
        }

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

        // Set activity of element
        public void SetActive(bool active) => rectTransform.gameObject.SetActive(active);
    }
}
