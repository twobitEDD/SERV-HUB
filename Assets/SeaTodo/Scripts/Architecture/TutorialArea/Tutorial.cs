using System.Collections.Generic;
using AppSettings.StatusBarSettings;
using Architecture.ChooseIconModule;
using Architecture.Components;
using Architecture.Elements;
using Architecture.TextHolder;
using Architecture.TutorialArea.TutorialElements;
using HomeTools.Other;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TutorialArea
{
    // Main component for tutorial
    public class Tutorial : IBehaviorSync
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        //Component that contains pages
        public StepsPackage StepsPackage;
        
        private bool started; // Initialized flag
        public RectTransform RectTransform; // Rect object of tutorial
        private VisualPart visualPart; // Component for control visual of UI 
        private MainButtonJob skipButton; // Component of skip button
        private MainStats mainStats; // Component of statistics for scroll
        private NavigationPointsElement navigationPointsElement; // Component of navigation
        private UIAlphaSync skipAlpha; // Animation component of alpha channel
        private TutorialAutoSwipe tutorialAutoSwipe; // Component that make auto swipes
        private UIAlphaSync[] graphAlphaSyncs; // Animation components of alpha channel of pages

        private int currentStep; // Current page that visible
        private const int stepsCount = 3; // Count of steps for skip button visible

        public void Start()
        {
            started = false;
        }

        public void Update()
        {
            if (started)
                return;

            Initialize();
            Setup();
            OpenImmediately();
            
            started = true;
        }

        // Setup rect object of tutorial
        private void Initialize()
        {
            RectTransform = SceneResources.Get("Tutorial").GetComponent<RectTransform>();
            RectTransform.SetParent(MainCanvas.RectTransform);
            RectTransform.anchoredPosition = Vector2.zero;
            RectTransform.SetRectTransformAnchorHorizontal(0, 0);
            RectTransform.SetRectTransformAnchorVertical(0, 0);
            SyncWithBehaviour.Instance.AddAnchor(RectTransform.gameObject, AppSyncAnchors.TutorialArea);
        }
        
        // Open tutorial
        public void Open()
        {
            foreach (var graphAlphaSync in graphAlphaSyncs)
            {
                graphAlphaSync.SetAlpha(1);
                graphAlphaSync.SetDefaultAlpha(1);
                graphAlphaSync.Stop();
            }
            
            navigationPointsElement.SetAnchorImmediately(0);
            mainStats.TutorialStatistics.UpdateToDefaultStep(0);
            visualPart.UiSyncElements = mainStats.TutorialStatistics.CurrentAlphaSync();
            visualPart.Open();
            tutorialAutoSwipe.Open();
            AppBarActions().AddAction(CloseTouched);
        }

        // Close tutorial
        public void CloseTouched()
        {
            TutorialInfo.ApplyTutorial();
            SkipTouched();
            tutorialAutoSwipe.Close();
            AppBarActions().RemoveAction(CloseTouched);
        }
        
        // Initialize tutorial
        private void Setup()
        {
            // Create component for control visual UI
            visualPart = new VisualPart(RectTransform);
            SyncWithBehaviour.Instance.AddObserver(visualPart, AppSyncAnchors.TutorialArea);
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.Tutorial);
            visualPart.Initialize();

            // Create button for skip
            var skipText = RectTransform.Find("Skip").GetComponent<Text>();
            SetupSkipPosition(skipText.rectTransform);
            TextLocalization.Instance.AddLocalization(skipText, TextKeysHolder.Skip);
            skipButton = new MainButtonJob(skipText.rectTransform, SkipTouched, skipText.gameObject);
            skipButton.Reactivate();
            skipButton.AttachToSyncWithBehaviour(AppSyncAnchors.TutorialArea);

            // Create animation component
            skipAlpha = new UIAlphaSync {Speed = 0.1f, SpeedMode = UIAlphaSync.Mode.Lerp};
            skipAlpha.AddElement(skipText);
            skipAlpha.PrepareToWork(1);
            SyncWithBehaviour.Instance.AddObserver(skipAlpha, AppSyncAnchors.TutorialArea);
            visualPart.SkipAlpha = skipAlpha;

            var pool = RectTransform.Find("Tutorial Graphic/Pool").GetComponent<RectTransform>();
            StepsPackage = new StepsPackage(pool);
            
            var graphic = RectTransform.Find("Tutorial Graphic").GetComponent<RectTransform>();
            mainStats = new MainStats(graphic);

            // Create navigation
            var points = new List<Graphic>();
            for (var i = 0; i < 4; i++)
                points.Add(RectTransform.Find($"NavItem {i + 1}").GetComponent<Graphic>());
            navigationPointsElement = new NavigationPointsElement(points.ToArray());
            navigationPointsElement.SetupComponents(AppSyncAnchors.TutorialArea, ColorTheme.SecondaryColor, ColorTheme.SecondaryColorD3, 1.27f);
            mainStats.TutorialDataCreator.SetActionGetStep(navigationPointsElement.SetAnchorDynamic);
            mainStats.TutorialDataCreator.SetActionGetStep(SetupSkipView);
            
            // Create auto swipe component
            tutorialAutoSwipe = new TutorialAutoSwipe(NextSwipeByTimer);
            SyncWithBehaviour.Instance.AddObserver(tutorialAutoSwipe, AppSyncAnchors.TutorialArea);
            
            mainStats.TutorialStatistics.UpdateToDefaultStep(0);
            visualPart.UiSyncElements = mainStats.TutorialStatistics.CurrentAlphaSync();
            visualPart.CloseImmediately();

            graphAlphaSyncs = StepsPackage.UIAlphaSyncs();
        }
        
        // Setup position of skip button
        private static void SetupSkipPosition(RectTransform rectTransform)
        {
            var closeHeightPosition = -77 - StatusBarSettings.Height;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 
                closeHeightPosition);
        }

        // Open tutorial immediately
        private void OpenImmediately()
        {
            if (!TutorialInfo.OpenImmediately())
                return;

            visualPart.OpenImmediately();
            navigationPointsElement.SetAnchorImmediately(0);
            tutorialAutoSwipe.Open();
        }
        
        // Method for touch skip button
        private void SkipTouched()
        {
            visualPart.UiSyncElements = mainStats.TutorialStatistics.CurrentAlphaSync();
            skipButton.Reactivate();
            navigationPointsElement.StopColor();
            visualPart.Close();
            TutorialInfo.SkipTutorial();
        }

        // Update skip button view by step
        private void SetupSkipView(int step)
        {
            currentStep = step;
            
            if (!visualPart.Opened)
                return;
            
            skipAlpha.Speed = 0.1f;
            skipAlpha.SetAlphaByDynamic(step == stepsCount ? 0 : 1);
            skipAlpha.Run();
            
            if (step == stepsCount)
                skipButton.Deactivate();
            else
                skipButton.Reactivate();
        }

        private void NextSwipeByTimer()
        {
            if (currentStep < stepsCount)
                mainStats.AddStepForward();
        }
    }
}
