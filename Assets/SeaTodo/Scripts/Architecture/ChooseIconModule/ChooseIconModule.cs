using Architecture.ChooseIconModule.StatisticIconElements;
using Architecture.Components;
using Architecture.Elements;
using Architecture.Pages;
using HomeTools.Messenger;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ChooseIconModule
{
    // Class that contains main part for view of choose icon 
    public class ChooseIconModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private readonly VisualPart visualPart;  // Class that contains visual part of this page
        public readonly MainStats MainStatsIcons; // Part of view that contains list of icons
        // Navigation points for icons list
        private readonly NavigationPointsElement navigationPointsElementIcons; 
        
        // Part of view that contains list of colors
        public readonly StatisticColorElements.MainStats MainStatsColors;
        // Navigation points for colors list
        private readonly NavigationPointsElement navigationPointsElementColors;
        
        // Current session of view
        public ChooseIconSession ChooseIconSession { get; private set; }

        public ChooseIconModule()
        {
            // Get view and setup order in hierarchy
            var rectTransform = SceneResources.Get("ChooseIcon Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            var layerIndex = MainCanvas.RectTransform.childCount - 5;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.ChooseIconModule);
            
            // Create visual part of view
            visualPart = new VisualPart(rectTransform, FinishOpenedSession, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);

            // Create components of lists - icons and colors
            MainStatsIcons = new MainStats(rectTransform.Find("ChooseIcon Graphic").GetComponent<RectTransform>());
            MainStatsColors = new StatisticColorElements.MainStats
                                    (rectTransform.Find("ChooseColor Graphic").GetComponent<RectTransform>());
            
            // Colorize parts that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            // Initialize visual component
            visualPart.Initialize();
            
            // Add to visual part of icons (for control alpha channel)
            foreach (var sync in MainStatsIcons.ChooseIconStatistics.GetAlphaSyncs())
                visualPart.UiSyncElements.AddSync(sync);
            
            // Add to visual part of color circles (for control alpha channel)
            foreach (var sync in MainStatsColors.ChooseColorStatistics.GetAlphaSyncs())
                visualPart.UiSyncElements.AddSyncGroup(sync);

            // Find and setup navigation points for icons list
            var navColorIcons = new Graphic[2];
            for (var i = 0; i < navColorIcons.Length; i++)
                navColorIcons[i] = rectTransform.Find($"NavItem {i + 1} Icons").GetComponent<Graphic>();
            navigationPointsElementIcons = new NavigationPointsElement(navColorIcons);
            navigationPointsElementIcons.SetupComponents(AppSyncAnchors.ChooseIconModule, ColorTheme.SecondaryColorD1, ColorTheme.EditGroupNavigationPassive, 1.27f);
            MainStatsIcons.ChooseIconDataCreator.SetActionGetStep(navigationPointsElementIcons.SetAnchorDynamic);
            
            // Find and setup navigation points for color circles list
            var navColorPoints = new Graphic[3];
            for (var i = 0; i < navColorPoints.Length; i++)
                navColorPoints[i] = rectTransform.Find($"NavItem {i + 1} Colors").GetComponent<Graphic>();
            navigationPointsElementColors = new NavigationPointsElement(navColorPoints);
            navigationPointsElementColors.SetupComponents(AppSyncAnchors.ChooseIconModule, ColorTheme.SecondaryColorD1, ColorTheme.EditGroupNavigationPassive, 1.27f);
            MainStatsColors.ChooseColorDataCreator.SetActionGetStep(navigationPointsElementColors.SetAnchorDynamic);

            // Add message for close when user touches dark background
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutChooseIconClicked);
        }
        
        // The method which is responsible for open view
        public void Open(Vector2 startPosition, ChooseIconSession trackIconSession, bool autoScroll)
        {
            ChooseIconSession = trackIconSession;

            // Setup navigation for icons list
            var iconStep = MainStatsIcons.ChooseIconDataCreator.GetStepById(ChooseIconSession.SelectedIconLocal);
            navigationPointsElementIcons.SetAnchorImmediately(iconStep);
            
            if (autoScroll)
                iconStep--;
            
            // Setup right position of icons page
            MainStatsIcons.ChooseIconStatistics.UpdateToDefaultStep(iconStep);
            MainStatsIcons.StatisticsScroll.SetScrollBySteps(autoScroll ? -1 : 0);
            
            // Setup navigation for color circles list
            var colorStep = MainStatsColors.ChooseColorDataCreator.GetStepById(ChooseIconSession.SelectedColorLocal);
            navigationPointsElementColors.SetAnchorImmediately(colorStep);
            
            if (autoScroll)
                colorStep++;
            
            // Setup right position of color circles page
            MainStatsColors.ChooseColorStatistics.UpdateToDefaultStep(colorStep);
            MainStatsColors.StatisticsScroll.SetScrollBySteps(autoScroll ? 1 : 0);
            
            // Stop update in session
            trackIconSession.StopItemsInSession();
            // Start visual process of open view
            visualPart.Open(startPosition);
            // Add close action to list for device home button
            AppBarActions().AddAction(Close);
        }

        // The method which is responsible for close of view
        private void Close()
        {
            // Stop update colors in navigations
            navigationPointsElementColors.StopColor();
            navigationPointsElementIcons.StopColor();
            // Start close process of view
            visualPart.Close();
            // Remove action of close from list (device home button)
            AppBarActions().RemoveAction(Close);
            // Turn on scroll
            PageScroll.Instance.Enabled = true;
        }
        
        // Call finish session when view closed
        private void FinishOpenedSession() => ChooseIconSession.FinishSession();
    }
}
