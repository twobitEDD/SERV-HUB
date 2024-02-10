using System;
using Architecture.Data;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TextHolder;
using Architecture.TipModule;
using Architecture.TrackArea;
using Architecture.TutorialArea;
using HomeTools.Handling;
using HomeTools.Other;
using HomeTools.Source.Design;
using HTools;
using MainActivity.AppBar;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace Architecture.MenuArea
{
    // Menu component for buttons control
    public class MainPart : IDisposable
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Link to main app bar component
        private AppBar AppBar() => AreasLocator.Instance.AppBar;
        // Link to create task page
        private CreateTaskArea.CreateTaskArea CreateFlowArea() => AreasLocator.Instance.CreateTaskArea;
        // Link to Sea Calendar page
        private DaysMarkerArea.DaysMarkerArea SeaCalendarArea() => AreasLocator.Instance.DaysMarkerArea;
        // Link to Settings page
        private SettingsArea.SettingsArea SettingsArea() => AreasLocator.Instance.SettingsArea;
        // Link menu scroll component
        private MenuAreaScroll MenuAreaScroll() => AreasLocator.Instance.MenuArea.MenuAreaScroll;
        // Link tips view
        private TipModule.TipModule TipModule() => AreasLocator.Instance.TipModule;
        // Link to tutorial view
        private Tutorial Tutorial() => AreasLocator.Instance.Tutorial;
        // Position of menu out of screen
        public const float RectOutOfScreenDistance = 450f;
        // Max count of tasks
        public const int MaxCountOfFlows = 7;
        // Shift buttons on the screen
        private readonly Vector2 buttonsShift = new Vector2(90, 17);
        // Rect object of menu
        private readonly RectTransform rectTransform;
        // Handles of buttons
        private readonly HandleObject handleAddFlow;
        private readonly HandleObject handleDaysMarker;
        private readonly HandleObject handleSettings;
        private readonly HandleObject handleOther;
        // Animation components for buttons 
        private readonly RectTransformSync buttonOther;
        private readonly RectTransformSync buttonMain;
        private readonly RectTransformSync buttonSettings;
        // Link to screen blackout component
        private readonly ScreenBlackout screenBlackout;
        // Button items
        private ButtonItem[] buttonItems;
        // Text components of buttons
        private readonly Text addFlowText;
        private readonly Text daysMarkerText;
        private readonly Text settingsText;
        private readonly Text otherText;
        // Additional UI for Add Task button
        private readonly Text addFlowCount;
        private readonly SVGImage addFlowIcon;
        // Flag responsible for the ability to create a task
        private bool addFlowAccess;
        // Flag responsible for the ability to press buttons when they are not in motion
        private bool positionAccess; 

        // Create and setup
        public MainPart(ScreenBlackout screenBlackout)
        {
            // Save screen component
            this.screenBlackout = screenBlackout;
            // Get menu object from scene resources and setup it in hierarchy
            rectTransform = SceneResources.Get("MenuArea").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            rectTransform.SetSiblingIndex(2);
            
            // Create handle components for main buttons
            handleAddFlow = new HandleObject(rectTransform.Find("Button Main/Handle Add Flow").gameObject);
            handleAddFlow.AddEvent(EventTriggerType.PointerClick, TouchedToAddFlow);
            handleDaysMarker = new HandleObject(rectTransform.Find("Button Main/Handle Days Marker").gameObject);
            handleDaysMarker.AddEvent(EventTriggerType.PointerClick, TouchedToSeaCalendar);
            // Setup animation component for main buttons
            buttonMain = SyncWithBehaviour.Instance.AddObserver(new RectTransformSync());
            SetupScaleMechanism(handleAddFlow.GameObject.transform.parent.GetComponent<RectTransform>(), buttonMain);
            
            // Create handle components for button for other
            handleOther = new HandleObject(rectTransform.Find("Button Other/Handle Other").gameObject);
            handleOther.AddEvent(EventTriggerType.PointerClick, TouchedOther);
            // Setup animation component for button for other
            buttonOther = SyncWithBehaviour.Instance.AddObserver(new RectTransformSync());
            SetupScaleMechanism(handleOther.GameObject.transform.parent.GetComponent<RectTransform>(), buttonOther);
            
            // Create handle components for settings button
            handleSettings = new HandleObject(rectTransform.Find("Button Settings/Handle Settings").gameObject);
            handleSettings.AddEvent(EventTriggerType.PointerClick, TouchedToSettings);
            // Setup animation component for settings button
            buttonSettings = SyncWithBehaviour.Instance.AddObserver(new RectTransformSync());
            SetupScaleMechanism(handleSettings.GameObject.transform.parent.GetComponent<RectTransform>(), buttonSettings);

            // Get text components of buttons
            addFlowText = rectTransform.Find("Button Main/Name 1").GetComponent<Text>();
            daysMarkerText = rectTransform.Find("Button Main/Name 2").GetComponent<Text>();
            settingsText = rectTransform.Find("Button Settings/Name").GetComponent<Text>();
            otherText = rectTransform.Find("Button Other/Name").GetComponent<Text>();

            // Get additional UI components of buttons
            addFlowCount = rectTransform.Find("Button Main/Icon 1/Count").GetComponent<Text>();
            addFlowIcon = rectTransform.Find("Button Main/Icon 1").GetComponent<SVGImage>();
            
            InitializeMenuItems(); // Initialize buttons components
            SetupMainParameters(); // Setup anchored position of menu
            RenameButtons(); // Setup localized button names 
            SetupOtherButton(); // Setup activity of button for other
            AddAdditionalEventsToButtons(); // Add additional events to buttons
            SetupButtonsDelay(1, 0.5f, 0.25f); // Setup delays for buttons animation
            CheckActiveRect(0); // Setup activity of menu by float process
        }

        // Initialize buttons components
        private void InitializeMenuItems()
        {
            // Create components
            buttonItems = new ButtonItem[3];
            // Setup main shift for menu
            var mainShift = rectTransform.sizeDelta.x + buttonsShift.x;
            
            // Create button components and setup parameters
            
            buttonItems[0] = new ButtonItem(rectTransform.Find("Button Main").GetComponent<RectTransform>());
            buttonItems[0].SetupPositionParameters(new Vector3(mainShift, 0, 0));
            buttonItems[0].SetupAnimationParameters(new Vector3(0, 180, 0));
            
            buttonItems[1] = new ButtonItem(rectTransform.Find("Button Settings").GetComponent<RectTransform>());
            buttonItems[1].SetupPositionParameters(new Vector3(mainShift, 0, 0));
            buttonItems[1].SetupAnimationParameters(new Vector3(0, 0, 0));
            
            buttonItems[2] = new ButtonItem(rectTransform.Find("Button Other").GetComponent<RectTransform>());
            buttonItems[2].SetupPositionParameters(new Vector3(mainShift, 0, 0));
            buttonItems[2].SetupAnimationParameters(new Vector3(0, 0, 0));
        }

        // Add additional events to button components
        private void AddAdditionalEventsToButtons()
        {
            SetupScaleMechanismEvents(handleAddFlow, buttonMain, buttonItems[0]);
            SetupScaleMechanismEvents(handleDaysMarker, buttonMain, buttonItems[0]);
            SetupScaleMechanismEvents(handleOther, buttonOther, buttonItems[1]);
            SetupScaleMechanismEvents(handleSettings, buttonSettings, buttonItems[2]);
            
            screenBlackout.HandleObject.AddEvent(EventTriggerType.PointerDown, () => SetByBlackout(true));
            screenBlackout.HandleObject.AddEvent(EventTriggerType.PointerUp, () => SetByBlackout(false));
        }
        
        // Setup anchored position of menu
        private void SetupMainParameters()
        {
            var sizeDelta = rectTransform.sizeDelta;
            
            rectTransform.anchoredPosition = new Vector2(sizeDelta.x / 2 + 70, 
                AppBarMaterial.RectTransform.anchoredPosition.y - AppBarMaterial.RectTransform.sizeDelta.y / 2 - 
                sizeDelta.y / 2 - buttonsShift.y);
        }

        // Get array of buttons handles
        public HandleObject[] GetHandleObjects() => 
            new[] {handleOther, handleAddFlow, handleDaysMarker, handleSettings};


        // Reset state of buttons
        public void ActualizeState()
        {
            // Update task count in button Create task
            var flowsCount = AppData.GetCurrentGroup().Flows.Length;
            addFlowCount.text = $"{flowsCount}/{MaxCountOfFlows}";

            // Update color of text and icon of Create task button
            
            var textColor = addFlowText.color;
            textColor.a = flowsCount < MaxCountOfFlows ? 1 : 0.5f;
            addFlowText.color = textColor;
            
            var iconColor = addFlowIcon.color;
            iconColor.a = flowsCount < MaxCountOfFlows ? 1 : 0.5f;
            addFlowIcon.color = iconColor;
            
            // Update the ability to create a new task
            addFlowAccess = flowsCount < MaxCountOfFlows;
        }
        
        // Setup position of menu on screen by interpolate
        public void SetupPositionByT(float t)
        {
            // Refresh dark screen opacity
            screenBlackout.SetState(t);

            // Refresh position of buttons
            foreach (var buttonItem in buttonItems)
                buttonItem.SetMarkerPosition(t);
            
            // Update button clickability
            positionAccess = t > 0.99f && t < 1.01f;
            
            // Refresh menu activity state
            CheckActiveRect(t);
        }

        // Check menu activity state
        private void CheckActiveRect(float t)
        {
            if (t <= 0f && rectTransform.gameObject.activeSelf)
                rectTransform.gameObject.SetActive(false);
            
            if (t > 0f && !rectTransform.gameObject.activeSelf)
                rectTransform.gameObject.SetActive(true);
        }

        // Setup localized button names 
        private void RenameButtons()
        {
            TextLocalization.Instance.AddLocalization(addFlowText, TextKeysHolder.AddTask);
            TextLocalization.Instance.AddLocalization(daysMarkerText, TextKeysHolder.SeaCalendar);
            TextLocalization.Instance.AddLocalization(settingsText, TextKeysHolder.SettingsName);
            TextLocalization.Instance.AddLocalization(otherText, TextKeysHolder.Tutorial);
        }
        
        // Setup activity of button for other
        private void SetupOtherButton()
        {
            buttonItems[2].SetActive(TutorialInfo.ShowInMenu());
        }

        // Method call when clicking on open create task page
        private void TouchedToAddFlow()
        {
            if (!positionAccess)
                return;

            if (!addFlowAccess)
            {
                var title = TextLocalization.GetLocalization(TextKeysHolder.TasksLimit);
                var session = new TipSession(null, title, TipsContentContainer.Type.TaskLimit);
                TipModule().Open(OtherHTools.GetWorldAnchoredPosition(addFlowText.rectTransform), session, 700);
                return;
            }

            PageTransitionTemplates.OpenPageFromWorkArea(CreateFlowArea().SetupParents, CreateFlowArea().SetupToPage);
            PageTransition().AddAction(AppBar().OpenCreateFlowMode);
            MenuAreaScroll().AutoCloseWithoutButton();
        }

        // Method call when clicking on open tutorial
        private void TouchedOther()
        {
            if (!positionAccess)
                return;
            
            Tutorial().Open();
            MenuAreaScroll().AutoClose();
        }

        // Method call when clicking on open Sea Calendar
        private void TouchedToSeaCalendar()
        {
            if (!positionAccess)
                return;
            
            AppBar().OpenCalendarMode(TextKeysHolder.SeaCalendar);
            PageTransitionTemplates.OpenPageFromWorkArea(SeaCalendarArea().SetupParents, 
                SeaCalendarArea().SetupToPage);

            MenuAreaScroll().AutoCloseWithoutAnimation();
        }

        // Method call when clicking on open Settings page
        private void TouchedToSettings()
        {
            if (!positionAccess)
                return;
            
            AppBar().OpenSettingsMode(TextKeysHolder.SettingsName);
            PageTransitionTemplates.OpenPageFromWorkArea(SettingsArea().SetupParents, 
                SettingsArea().SetupToPage);

            MenuAreaScroll().AutoCloseWithoutAnimation();
        }

        // Setup animation component of button
        private void SetupScaleMechanism(RectTransform rect, RectTransformSync rectTransformSync)
        {
            rectTransformSync.SetRectTransformSync(rect);
            rectTransformSync.TargetScale = Vector3.one * 1.05f;
            rectTransformSync.Speed = 0.2f;
            rectTransformSync.SpeedMode = RectTransformSync.Mode.Lerp;
            rectTransformSync.PrepareToWork();
        }
        
        // Setup delays for animation of buttons
        private void SetupButtonsDelay(params float[] delays)
        {
            for (var i = 0; i < delays.Length; i++)
            {
                if (i < buttonItems.Length)
                    buttonItems[i].SetupMarkerDelay(delays[i]);
            }
        }

        // Setup events for buttons handles
        private void SetupScaleMechanismEvents(HandleObject handle, RectTransformSync rectTransformSync, ButtonItem buttonItem)
        {
            handle.AddEvent(EventTriggerType.PointerDown, ()=>
            {
                rectTransformSync.SetTByDynamic(0);
                rectTransformSync.Run();
                buttonItem.SetupTouched(true);
            });
            
            handle.AddEvent(EventTriggerType.PointerUp, ()=>
            {
                rectTransformSync.SetTByDynamic(1);
                rectTransformSync.Run();
                buttonItem.SetupTouched(false);
            });
        }

        // Update opacity of dark screen under menu by interpolation 
        private void SetByBlackout(bool touched)
        {
            foreach (var buttonItem in buttonItems)
                buttonItem.SetupBlackoutTouched(touched);
        }

        public void Dispose()
        {
            handleOther?.Dispose();
            handleAddFlow?.Dispose();
            handleDaysMarker?.Dispose();
            handleSettings?.Dispose();
            buttonOther?.Dispose();
            buttonMain?.Dispose();
            buttonSettings?.Dispose();
        }
    }
}
