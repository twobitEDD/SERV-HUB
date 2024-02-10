using Architecture.ChooseIconModule;
using Architecture.Components;
using Architecture.Data;
using Architecture.EditGroupModule.StatisticEditGroup;
using Architecture.Elements;
using Architecture.TextHolder;
using HomeTools.Input;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.EditGroupModule
{
    // Update title and title icon view 
    public class UpdateTitleModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private readonly VisualPart visualPart; // Component for UI visual control
        // Components with title icons scroll list
        public readonly MainStats MainStatsIcons;
        // Component with navigation points for title icon scroll
        private readonly NavigationPointsElement navigationPointsElement;
        // Input field for title name
        private readonly HInputField inputField;
        public bool Opened; // Opened view flag
        // Session of title editing
        public EditGroupSession EditGroupSession { get; private set; }

        // Create and setup
        public UpdateTitleModule()
        {
            // Get UI view from scene resources and setup parent
            var rectTransform = SceneResources.Get("UpdateTitle Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            // Setup order in hierarchy of view 
            var layerIndex = MainCanvas.RectTransform.childCount - 5;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.EditTitleModule);
            // Create component of visual UI
            visualPart = new VisualPart(rectTransform, ActivateInput, FinishOpenedSession, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            // Create component with scroll of title icons
            MainStatsIcons = new MainStats(rectTransform.Find("ChooseIcon Graphic").GetComponent<RectTransform>());

            // Colorize parts that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            // Initialize component of visual UI
            visualPart.Initialize();
            
            // Create component for navigation points of title icons list
            navigationPointsElement = new NavigationPointsElement(
                rectTransform.Find("NavItem 1").GetComponent<SVGImage>(),
                              rectTransform.Find("NavItem 2").GetComponent<SVGImage>());
            navigationPointsElement.SetupComponents(AppSyncAnchors.EditTitleModule, ColorTheme.SecondaryColorD1, ColorTheme.EditGroupNavigationPassive, 1.27f);
            MainStatsIcons.EditGroupDataCreator.SetActionGetStep(navigationPointsElement.SetAnchorDynamic);

            // Find and setup name input fiend
            inputField = rectTransform.Find("Edit Name Input").GetComponent<HInputField>();
            inputField.AddActionWhenSelected(visualPart.ActivateInput);
            inputField.AddActionWhenDeselected(visualPart.DeactivateInput);
            
            // Find placeholder text and localize
            var placeholder = rectTransform.Find("Edit Name Input/Placeholder").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(placeholder, TextKeysHolder.Name);

            // Add close method to messenger for dark background clicks
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutEditGroupClicked);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, EditGroupSession trackIconSession)
        {
            // Save edit title session
            EditGroupSession = trackIconSession;
            // Setup current title to input field
            UpdateInputField(trackIconSession.FlowGroup);
            // Get page number of title icons list by current icon
            var iconStep = MainStatsIcons.EditGroupDataCreator.GetStepById(EditGroupSession.SelectedIconLocal);
            // Setup title icons list by page number
            MainStatsIcons.EditGroupStatistics.UpdateToDefaultStep(iconStep);
            // Update navigation state by page number
            navigationPointsElement.SetAnchorImmediately(iconStep);
            // Setup animation component of alpha channel from title icons list to visual component
            visualPart.UiSyncElementsIcons = MainStatsIcons.EditGroupStatistics.CurrentAlphaSync();
            // Reset state of session
            trackIconSession.StopItemsInSession();
            // Open of visual UI
            visualPart.Open(startPosition);
            // Add close action to app bar
            AppBarActions().AddAction(Close);

            Opened = true;
        }

        // Update input field by global tasks package name
        private void UpdateInputField(FlowGroup flowGroup)
        {
            inputField.text = flowGroup.Name;
        }

        // Activate input field components
        private void ActivateInput()
        {
            inputField.enabled = true;
        }
        
        // Deactivate input field components
        private void DeactivateInput()
        {
            inputField.enabled = false;
        }
        
        // The method which is responsible for close view
        private void Close()
        {
            // Deactivate input components
            DeactivateInput();
            // Stop process in navigation components
            navigationPointsElement.StopColor();
            // Set animation components of alpha channel from title icons list to visual component
            visualPart.UiSyncElementsIcons = MainStatsIcons.EditGroupStatistics.CurrentAlphaSync();
            // Visual UI close
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);

            Opened = false;
        }
        
        // Finish update title session
        private void FinishOpenedSession() => EditGroupSession.FinishSession(inputField.text);
    }
}
