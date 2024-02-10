using Architecture;
using Architecture.Components;
using Architecture.CreateTaskArea;
using Architecture.DaysMarkerArea;
using Architecture.EditGroupModule;
using Architecture.EditTaskArea;
using Architecture.Elements;
using Architecture.MenuArea;
using Architecture.Pages;
using Architecture.SettingsArea;
using Architecture.StatisticsArea;
using Architecture.TaskViewArea;
using HomeTools.Handling;
using HomeTools.Messenger;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainActivity.AppBar
{
    // Main component of app bar
    public class AppBar
    {
        // App bar states
        private enum MenuButtonState
        {
            Normal,
            CreateFlow,
            ViewFlow,
            EditFlow,
            Statistics,
            Calendar,
            Settings,
        }
        
        // Links to app pages
        private static MenuArea MenuArea() => AreasLocator.Instance.MenuArea;
        private static CreateTaskArea CreateFlowArea() => AreasLocator.Instance.CreateTaskArea;
        private static FlowViewArea FlowViewArea() => AreasLocator.Instance.FlowViewArea;
        private static EditFlowArea EditFlowArea() => AreasLocator.Instance.EditFlowArea;
        private static StatisticsArea StatisticsArea() => AreasLocator.Instance.StatisticsArea;
        private static DaysMarkerArea CalendarArea() => AreasLocator.Instance.DaysMarkerArea;
        private static SettingsArea SettingsArea() => AreasLocator.Instance.SettingsArea;
        
        // Title of main page
        public readonly GroupTitle GroupTitle;
        // Create task mode of app bar
        private readonly AppBarCreateFlowMode appBarCreateFlowMode;
        // View task mode of app bar
        public readonly AppBarViewFlowMode AppBarViewMode;
        // Edit task mode of app bar
        private readonly AppBarEditFlowMode appBarEditFlowMode;
        // Edit title of app bar component
        private readonly AppBarEditTitle appBarEditTitle;
        // Current app bar state
        private MenuButtonState menuButtonState;

        // Input name of app bar
        public readonly BarFlowInputName BarFlowInputName;
        // Icon of app bar
        public readonly BarFlowIcon BarFlowIcon;
        // Menu button component
        private readonly MenuButton menuButton;
        // Handle button component
        private readonly HandleObject menuHandle;
        // For run action when user touches home button on device
        public readonly ActionsQueue RightButtonActionsQueue;

        // Create and setup
        public AppBar()
        {
            // Setup elements and create menu button
            var rectTransform = GameObject.Find("AppBarElements").GetComponent<RectTransform>();
            menuButton = new MenuButton(rectTransform.Find("MenuButton").GetComponent<RectTransform>());

            // Setup app bar position and size in canvas
            var appBar = AppBarMaterial.RectTransform;
            var position = new Vector3(0, -appBar.sizeDelta.y / 2 + appBar.anchoredPosition.y + AppBarMaterial.BarHeight / 2, 0);
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,  AppBarMaterial.BarHeight);

            // Create app bar modes components
            BarFlowInputName = new BarFlowInputName(rectTransform);
            BarFlowIcon = new BarFlowIcon(rectTransform);
            appBarCreateFlowMode = new AppBarCreateFlowMode(rectTransform);
            AppBarViewMode = new AppBarViewFlowMode(rectTransform, CloseViewMode);
            appBarEditFlowMode = new AppBarEditFlowMode(appBarCreateFlowMode, AppBarViewMode, rectTransform);

            // Create component for collect actions for device button
            RightButtonActionsQueue = new ActionsQueue();
            
            // Create edit title button
            var buttonEditGroup = rectTransform.Find("Edit Title Button").GetComponent<Image>();
            appBarEditTitle = new AppBarEditTitle(buttonEditGroup, RightButtonActionsQueue);
            
            // Create title component for main page
            var barTitle = rectTransform.Find("BarTitle").GetComponent<Text>();
            var barImage = rectTransform.Find("Image").GetComponent<SVGImage>();
            GroupTitle = new GroupTitle(barTitle, barImage);
            
            // Setup input name component 
            BarFlowInputName.Setup();
            appBarEditTitle.Active = true;
            
            // Create handle object for menu button
            menuHandle = new HandleObject(menuButton.GetHandle().gameObject);
            menuHandle.AddEvent(EventTriggerType.PointerClick, MenuAction);
        }

        // Update title immediately
        public void UpdateProjectTitleImmediately() => GroupTitle.UpdateImmediately();
        // Update title with animation
        public void UpdateProjectTitleByNewInfo() => GroupTitle.UpdateByNewInfo();
        
        // Touched menu logic
        private void MenuAction()
        {
            // If has opened view
            if (RightButtonActionsQueue.HasActions)
            {
                RightButtonActionsQueue.InvokeLastAction();
                return;
            }
            
            // Try invoke actions when normal button (open/close menu)
            if (NormalButtonAction())
                return;
            
            // Try invoke actions when close create task mode
            if (CreateFlowButtonAction())
                return;
            
            // Try invoke actions when enter to edit task mode
            if (EnterToEditFlowMode())
                return;

            // Try invoke actions when close edit task mode
            CloseEditFlowMode();
        }
        
        // Try invoke actions when normal button (open/close menu)
        private bool NormalButtonAction()
        {
            if (menuButtonState != MenuButtonState.Normal)
                return false;
            
            if (MenuArea().MenuAreaScroll.CurrentState == MenuAreaScroll.ScrollState.hidden ||
                MenuArea().MenuAreaScroll.CurrentState == MenuAreaScroll.ScrollState.toHide)
            {
                OpenAction();
                return true;
            }

            if (MenuArea().MenuAreaScroll.CurrentState == MenuAreaScroll.ScrollState.shown ||
                MenuArea().MenuAreaScroll.CurrentState == MenuAreaScroll.ScrollState.toShow)
            {
                CloseAction();
            }
            
            return true;
        }
        
        // Open menu
        private void OpenAction()
        {
            MenuArea().Open();
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonToClose);
        }

        // Close menu
        private void CloseAction()
        {
            MenuArea().Close();
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonFromClose);
        }
        
        // Try invoke actions when close create task mode
        private bool CreateFlowButtonAction()
        {
            if (menuButtonState != MenuButtonState.CreateFlow)
                return false;

            CloseCreateFlowMode();
            return true;
        }
        
        // Close create task mode
        public void CloseCreateFlowMode()
        {
            appBarCreateFlowMode.CloseFlowMode();
            menuButtonState = MenuButtonState.Normal;
            BarFlowInputName.FinishSession();
            BarFlowIcon.SetActive(false);
            appBarEditTitle.Active = true;
            
            PageTransitionTemplates.ClosePageToWorkArea(CreateFlowArea().SetupFromPage, 5, CreateFlowArea().SetupToPool);
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonFromClose);
        }
        
        
        
        
        
        
        // Open create task page
        public void OpenCreateFlowMode()
        {
            appBarCreateFlowMode.OpenFlowMode();
            menuButtonState = MenuButtonState.CreateFlow;
            BarFlowInputName.StartNewSession();
            BarFlowIcon.UpdateImmediately(0,0);
            BarFlowIcon.SetToAutoScroll(true);
            BarFlowIcon.SetActive(true);
            appBarEditTitle.Active = false;
        }

        // Check if can open task view page
        public bool CanOpenFlowMode() => menuButtonState == MenuButtonState.Normal;
        // Open task view page
        public void OpenViewFlowMode(string key)
        {
            GroupTitle.StopUiAlpha();
            AppBarViewMode.SetName(key);
            AppBarViewMode.OpenFlowView();
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonToEdit);
            menuButtonState = MenuButtonState.ViewFlow;
            appBarEditTitle.Active = false;
        }

        // Start close view page
        public void CloseViewModeState()
        {
            menuButtonState = MenuButtonState.Normal;
            AppBarViewMode.CloseFlowView();
            appBarEditTitle.Active = true;
        }
        
        // Close view page
        private void CloseViewMode()
        {
            // If has opened view
            if (RightButtonActionsQueue.HasActions)
            {
                RightButtonActionsQueue.InvokeLastAction();
                return;
            }
            
            // Invoke close from task view page
            if (menuButtonState == MenuButtonState.ViewFlow)
                CloseFromFlowViewArea();
            
            // Invoke close activity view page
            if (menuButtonState == MenuButtonState.Statistics)
                CloseFromStatisticsArea();
            
            // Invoke close calendar view page
            if (menuButtonState == MenuButtonState.Calendar)
                CloseFromCalendarArea();
            
            // Invoke close settings view page
            if (menuButtonState == MenuButtonState.Settings)
                CloseFromSettingsArea();

            // Start close view page
            CloseViewModeState();
        }

        // Close activity view page
        private static void CloseFromStatisticsArea()
        {
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonFromEmpty);
            PageTransitionTemplates.ClosePageToWorkArea(StatisticsArea().SetupFromPage, 5, StatisticsArea().SetupToPool);
        }

        // Close task view page
        private static void CloseFromFlowViewArea()
        {
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonFromEdit);
            PageTransitionTemplates.ClosePageToWorkArea(FlowViewArea().SetupFromPage, 5, FlowViewArea().SetupToPool);
        }
        
        // Close calendar view page
        private void CloseFromCalendarArea()
        {
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonFromEmpty);
            GroupTitle.UpdateImmediately();
            PageTransitionTemplates.ClosePageToWorkArea(CalendarArea().SetupFromPage, 5, CalendarArea().SetupToPool);
        }
        
        // Close settings view page
        private void CloseFromSettingsArea()
        {
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonFromEmpty);
            GroupTitle.UpdateImmediately();
            PageTransitionTemplates.ClosePageToWorkArea(SettingsArea().SetupFromPage, 5, SettingsArea().SetupToPool);
        }

        // Enter to edit task mode
        private bool EnterToEditFlowMode()
        {
            if (menuButtonState != MenuButtonState.ViewFlow)
                return false;
            
            MainMessenger.SendMessage(AppMessagesConst.EditToClose);
            menuButtonState = MenuButtonState.EditFlow;

            AppBarViewMode.SetActiveHandle(false);
            appBarEditFlowMode.OpenFlowMode();
            EditFlowArea().SetupCurrentFlow(FlowViewArea().CurrentFlow);
            PageTransitionTemplates.OpenPageInsideSecond(FlowViewArea().SetupFromPage, 5, FlowViewArea().SetupToPool,
                EditFlowArea().SetupParents, EditFlowArea().SetupToPage);
        
            BarFlowInputName.StartNewSession();
            BarFlowInputName.SetupNameText(EditFlowArea().CurrentFlow.Name);
            BarFlowIcon.UpdateImmediately(FlowViewArea().CurrentFlow.Icon, FlowViewArea().CurrentFlow.Color);
            BarFlowIcon.SetToAutoScroll(false);
            BarFlowIcon.SetActive(true);

            return true;
        }
        
        // Close edit task mode
        private bool CloseEditFlowMode()
        {
            if (menuButtonState != MenuButtonState.EditFlow)
                return false;
            
            MainMessenger.SendMessage(AppMessagesConst.CloseToEdit);
            menuButtonState = MenuButtonState.ViewFlow;

            BarFlowIcon.SetActive(false);
            AppBarViewMode.SetActiveHandle(true);
            appBarEditFlowMode.CloseFlowMode();
            BarFlowInputName.FinishSession();
            EditFlowArea().UpdateFlow();
            FlowViewArea().UpdateByCurrentFlow();
            PageTransitionTemplates.OpenPageInsideSecond(EditFlowArea().SetupFromPage, 5, EditFlowArea().SetupToPool,
                FlowViewArea().SetupParents, FlowViewArea().SetupToPage);
            
            return true;
        }

        // Try close menu view
        public bool TryCloseMenu()
        {
            if (menuButtonState != MenuButtonState.Normal)
                return false;

            if (MenuArea().MenuAreaScroll.CurrentState != MenuAreaScroll.ScrollState.shown &&
                MenuArea().MenuAreaScroll.CurrentState != MenuAreaScroll.ScrollState.toShow) 
                return false;
            
            CloseAction();
            
            return true;
        }

        // Try move back by device button
        public bool TryMoveBackByAreas()
        {
            // Has default state
            if (menuButtonState == MenuButtonState.Normal)
                return false;

            // Close activity page
            if (menuButtonState == MenuButtonState.Statistics)
            {
                CloseFromStatisticsArea();
                CloseViewModeState();
                return true;
            }
            
            // Close view task page
            if (menuButtonState == MenuButtonState.ViewFlow)
            {
                CloseFromFlowViewArea();
                CloseViewModeState();
                return true;
            }
            
            // Close edit task page
            if (menuButtonState == MenuButtonState.EditFlow)
            {
                CloseEditFlowMode();
                return true;
            }
            
            // Close create task page
            if (menuButtonState == MenuButtonState.CreateFlow)
            {
                CloseCreateFlowMode();
                return true;
            }
            
            // Close calendar view
            if (menuButtonState == MenuButtonState.Calendar)
            {
                CloseFromCalendarArea();
                CloseViewModeState();
                return true;
            }
            
            // Close settings page
            if (menuButtonState == MenuButtonState.Settings)
            {
                CloseFromSettingsArea();
                CloseViewModeState();
                return true;
            }

            return false;
        }
        
        // Open activity page
        public void OpenStatisticsMode(string key)
        {
            AppBarViewMode.SetNameByKey(key);
            GroupTitle.UpdateImmediately();
            GroupTitle.StopUiAlpha();
            AppBarViewMode.OpenFlowView();
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonToEmpty);
            menuButtonState = MenuButtonState.Statistics;
            appBarEditTitle.Active = false;
        }
        
        // Open Sea Calendar page
        public void OpenCalendarMode(string key)
        {
            GroupTitle.StopUiAlpha();
            AppBarViewMode.SetNameByKey(key);
            AppBarViewMode.OpenFlowView();
            MainMessenger.SendMessage(AppMessagesConst.CloseButtonToEmpty);
            menuButtonState = MenuButtonState.Calendar;
            appBarEditTitle.Active = false;
        }
        
        // Open settings page
        public void OpenSettingsMode(string key)
        {
            GroupTitle.StopUiAlpha();
            AppBarViewMode.SetNameByKey(key);
            AppBarViewMode.OpenFlowView();
            MainMessenger.SendMessage(AppMessagesConst.CloseButtonToEmpty);
            menuButtonState = MenuButtonState.Settings;
            appBarEditTitle.Active = false;
        }

        // Animate empty name state in edit or create task page
        public bool CheckEnteredNameOrSignal()
        {
            var result = BarFlowInputName.HasText();
            if (result)
                return true;

            appBarCreateFlowMode.EmptySignal();
            return false;
        }
    }
}
