using System.Collections.Generic;
using AppSettings.StatusBarSettings;
using Architecture.Components;
using Architecture.Elements;
using Architecture.TextHolder;
using HomeTools.Other;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea.SinglePageView
{
    // Class for displaying content on the whole page
    public class SinglePage : IBehaviorSync
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private bool started; // Initialized page flag
        public RectTransform RectTransform; // Rect object of page
        private VisualPart visualPart; // Component for animation of UI
        private MainButtonJob closeButton; // Close button component
        private RectTransform contentContainer; // Container for content
        private Text title; // Title text of page
        private SVGImage icon; // Icon of page
        private RectTransform readContent; // Current content in page
        
        private RectTransform scrollVerticalRect; // Scrollbar rect
        private RectTransform scrollView; // Scroll parent rect
        
        // Multiplier for scrollbar rect
        private const float verticalRectMultiplier = 0.85f;

        public void Start() => started = false;

        public void Update()
        {
            if (started)
                return;

            Initialize();
            Setup();

            started = true;
        }

        // Initialize page
        private void Initialize()
        {
            // Find in scene resources and setup rect page
            RectTransform = SceneResources.Get("Single Page View").GetComponent<RectTransform>();
            RectTransform.SetParent(MainCanvas.RectTransform);
            RectTransform.anchoredPosition = Vector2.zero;
            RectTransform.SetRectTransformAnchorHorizontal(0, 0);
            RectTransform.SetRectTransformAnchorVertical(0, 0);
            SyncWithBehaviour.Instance.AddAnchor(RectTransform.gameObject, AppSyncAnchors.SinglePageView);
            
            // Find scroll objects and page element
            contentContainer = RectTransform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            scrollVerticalRect = RectTransform.Find("Scroll View/Scrollbar Vertical").GetComponent<RectTransform>();
            scrollView = RectTransform.Find("Scroll View").GetComponent<RectTransform>();
            title = RectTransform.Find("Title").GetComponent<Text>();
            icon = RectTransform.Find("Icon").GetComponent<SVGImage>();
        }
        
        // The method which is responsible for open view
        public void Open(string name, RectTransform content, List<UIAlphaSync> contentAlphaSync, Sprite iconSprite)
        {
            // Update title and icon
            title.text = name;
            icon.sprite = iconSprite;
            
            // Set to scene resources old content
            if (readContent != null)
                SceneResources.Set(readContent.gameObject);

            if (content == null || contentAlphaSync == null)
                return;
             
            // Setup new content object
            readContent = content;
            readContent.SetParent(contentContainer);
            readContent.anchoredPosition = Vector2.zero;
            contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, readContent.sizeDelta.y);

            // Update component for animation of UI and start open animation
            visualPart.ContentView = content;
            visualPart.ContentAlphaSync = contentAlphaSync;
            visualPart.Open();
            
            // Set  position of content container
            contentContainer.anchoredPosition = new Vector2(0, -300);
            // Add close action to list for device home button
            AppBarActions().AddAction(CloseTouched);
        }

        // Setup components of page
        private void Setup()
        {
            // Create component for UI animations
            visualPart = new VisualPart(RectTransform);
            SyncWithBehaviour.Instance.AddObserver(visualPart, AppSyncAnchors.SinglePageView);
            visualPart.Initialize();

            // Setup close button
            var close = RectTransform.Find("Close").GetComponent<Text>();
            SetupClosePosition(close.rectTransform);
            closeButton = new MainButtonJob(close.rectTransform, CloseTouched, close.gameObject);
            closeButton.Reactivate();
            closeButton.AttachToSyncWithBehaviour(AppSyncAnchors.SinglePageView);
            TextLocalization.Instance.AddLocalization(close, TextKeysHolder.CloseAboutSeaCalendar);

            // Update height of rect
            var height = scrollView.rect.height;
            var newHeight = height / scrollVerticalRect.localScale.y * verticalRectMultiplier;
            scrollVerticalRect.sizeDelta = new Vector2(scrollVerticalRect.sizeDelta.x, newHeight);
            
            // Colorize UI elements
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.AboutSeaCalendar);
            // Setup closed state for page
            visualPart.CloseImmediately();
        }
        
        // Set position for close button
        private static void SetupClosePosition(RectTransform rectTransform)
        {
            var closeHeightPosition = -77 - StatusBarSettings.Height;
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 
                closeHeightPosition);
        }
        
        // Call when close button touched
        private void CloseTouched()
        {
            // Reset state of button component
            closeButton.Reactivate();
            // Start close process of view
            visualPart.Close();
            // Remove action of close from list (device home button)
            AppBarActions().RemoveAction(CloseTouched);
        }
    }
}
