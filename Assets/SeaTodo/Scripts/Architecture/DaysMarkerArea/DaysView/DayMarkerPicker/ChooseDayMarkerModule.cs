using Architecture.Components;
using Architecture.Elements;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView.DayMarkerPicker
{
    // View for choose characteristic to selected day
    public class ChooseDayMarkerModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;

        private readonly RectTransform rectTransform; // Rect of view
        private readonly VisualPart visualPart; // Visual component for UI
        private readonly DayPicker dayPicker; // Component with characteristics
        private readonly Text title; // Title of view

        private ChooseDaySession chooseDaySession; // Keep open view session

        // Create view
        public ChooseDayMarkerModule()
        {
            // Get view from resources and setup to parent
            rectTransform = SceneResources.Get("ChooseDay Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.ChooseDayMarkerModule);
            
            // Find title text and localize
            title = rectTransform.Find("Info").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.TitleChooseDayMarker);

            // Setup order of view in hierarchy
            var layerIndex = MainCanvas.RectTransform.childCount - 5;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            
            // Create visual component for UI
            visualPart = new VisualPart(rectTransform, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            
            // Colorize parts that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            // Initialize visual component
            visualPart.Initialize();
            
            // Create characteristics picker
            dayPicker = new DayPicker(rectTransform);
            // Add alpha chanel animation component to visual component
            visualPart.AlphaSyncOut = dayPicker.UIAlphaSync;
            
            // Add close method for clicks on dark background under view
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutChooseDayMarkerModuleClicked);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, ChooseDaySession session)
        {
            // Save session
            chooseDaySession = session;
            // Update characteristics picker
            dayPicker.PrepareView(session);
            dayPicker.UIAlphaSync.PrepareToWork();
            // Open view
            visualPart.Open(startPosition);
            // Add close action to list for device home button
            AppBarActions().AddAction(Close);
            // Update color of title
            title.color = ThemeLoader.GetCurrentTheme().DaysMarkerAreaMarkerMonthTitle;
        }

        // The method which is responsible for close of view
        private void Close()
        {
            // Start close process of view
            visualPart.Close();
            // Remove action of close from list (device home button)
            AppBarActions().RemoveAction(Close);
            // Finish session
            chooseDaySession?.Closed();
        }
    }
}
