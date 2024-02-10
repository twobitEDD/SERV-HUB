using System.Collections.Generic;
using System.Linq;
using Architecture.Components;
using Architecture.Data;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Pages;
using Architecture.TextHolder;
using Architecture.TipModule;
using HomeTools.Input;
using HomeTools.Messenger;
using HomeTools.Other;
using HTools;
using MainActivity.MainComponents;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysColors.UpdateModule
{
    // View of rename days characteristics
    public class UpdateMarkerModule
    {
        // Link to Tips view
        private TipModule.TipModule TipModule() => AreasLocator.Instance.TipModule;
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        private readonly VisualPart visualPart; // Component with visual elements
        private readonly MainButtonJob mainButtonJob; // Button component
        private readonly RectTransform buttonApplyHandle; // Rect of apply handle
        private readonly ApplyDelayView applyDelayView; // Component for delay when apply
        private readonly Text textDefault; // default characteristic name text
        private readonly Text textDescription; // description of day characteristic
        private readonly MainButtonJob setDefaultName; // Button area for set default name

        private readonly HInputField inputField; // Input name field

        private UpdateMarkerSession updateMarkerSession; // Update name session
        private bool apply; // Apply view state flag

        // Create and setup main of view
        public UpdateMarkerModule()
        {
            // Find view and setup parent
            var rectTransform = SceneResources.Get("UpdateMarker Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            
            // Setup order position of view in hierarchy
            var layerIndex = MainCanvas.RectTransform.childCount - 6;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.UpdateCalendarMarkerModule);
            
            // Create component for visual UI elements 
            visualPart = new VisualPart(rectTransform, ActivateInput, TryBuildNewMarker, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            
            // Colorize parts that marked as TimeTrackModule
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.TimeTrackModule);
            // Initialize UI visual component
            visualPart.Initialize();

            // Add close method when user touches background under view
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutUpdateMarkerModuleClicked);
            
            // Find title of view and localize
            var textInfo = rectTransform.Find("Info").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textInfo, TextKeysHolder.TitleUpdateMarkerModule);

            // Find day characteristic description
            textDescription = rectTransform.Find("Description").GetComponent<Text>();
            // Setup accept button
            var buttonRect = rectTransform.Find("Accept").GetComponent<RectTransform>();
            buttonApplyHandle = buttonRect.Find("Handle").GetComponent<RectTransform>();
            mainButtonJob = new MainButtonJob(buttonRect, ApplyAction, buttonApplyHandle.gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.UpdateCalendarMarkerModule);

            // Setup input name field and placeholder of input field
            inputField = rectTransform.Find("Edit Name Input").GetComponent<HInputField>();
            var placeholder = rectTransform.Find("Edit Name Input/Placeholder").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(placeholder, TextKeysHolder.Name);
            
            // Find default name text of day characteristic
            textDefault = rectTransform.Find("Default Name").GetComponent<Text>();

            // Setup actions for moving view when activate keyboard
            inputField.AddActionWhenSelected(visualPart.ActivateInput);
            inputField.AddActionWhenDeselected(visualPart.DeactivateInput);
            
            // Find UI for delay line around apply button
            var loadRound = buttonRect.Find("Load Round").GetComponent<Image>();
            var circleRound = buttonRect.Find("Load Circle").GetComponent<SVGImage>();
            var circleRoundStart = buttonRect.Find("Load Circle Start").GetComponent<SVGImage>();
            // Setup component delay when touched apply
            applyDelayView = new ApplyDelayView(loadRound, circleRound, circleRoundStart, StartApply);
            SyncWithBehaviour.Instance.AddObserver(applyDelayView, AppSyncAnchors.UpdateCalendarMarkerModule);
            
            // Setup button for set default name to input field
            var handlerDefaultName = rectTransform.Find("Handler Set Default").gameObject;
            setDefaultName = new MainButtonJob(textDefault.rectTransform, SetDefaultName, handlerDefaultName);
            setDefaultName.AttachToSyncWithBehaviour(AppSyncAnchors.UpdateCalendarMarkerModule);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, UpdateMarkerSession session)
        {
            // Save session component
            updateMarkerSession = session;
            
            // Add close action to app bar
            AppBarActions().AddAction(Close);
            // Reset states of buttons in view
            mainButtonJob.Reactivate();
            setDefaultName.Reactivate();

            // Set name of day characteristic to input
            inputField.text = ColorMarkersDescriptor.GetColorName(session.ColorMarker);
            
            // Setup color of day in view
            var markerColor = ColorMarkersDescriptor.GetColor(session.ColorMarker);
            textDefault.color = markerColor;
            
            // Localize title and description of days
            textDefault.text = $"{TextLocalization.GetLocalization(GetMarkerDefaultKey(session.ColorMarker))}";
            textDescription.text = $"{TextLocalization.GetLocalization(GetMarkerDescriptionKey(session.ColorMarker))}";

            // Method of open UI of view
            visualPart.Open(startPosition);

            // Reset apply delay component
            applyDelayView.Reset();
            apply = false;
        }
        
        // The method which is responsible for close view
        private void Close()
        {
            // Lock if wait for close
            if (apply)
                return;
            
            // Deactivate input 
            DeactivateInput();
            // Method of close UI of view
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
        }

        // Activate input component
        private void ActivateInput() => inputField.enabled = true;

        // Deactivate input component
        private void DeactivateInput() => inputField.enabled = false;

        // Start delay to close
        private void ApplyAction()
        {
            if (apply)
                return;
            
            // If other markers have the same name than open tip
            if (TryFindSimilarMarker())
            {
                // Get title text for tip
                var title = TextLocalization.GetLocalization(TextKeysHolder.HasMarker);
                // Create session of tip
                var session = new TipSession(null, title, TipsContentContainer.Type.SimilarMarker);
                // Open tip module
                TipModule().Open(OtherHTools.GetWorldAnchoredPosition(buttonApplyHandle), session, 200);
                return;
            }
            
            // Start delay to close 
            applyDelayView.StartView();
            apply = true;
        }

        // Close view when applied
        private void StartApply()
        {
            // Method of close UI of view
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
        }

        // Try apply new name to day characteristic item
        private void TryBuildNewMarker()
        {
            if (!apply)
                return;
            
            // Check name to old version
            var defaultName = ColorMarkersDescriptor.GetColorName(updateMarkerSession.ColorMarker);
            var updated = defaultName != inputField.text && inputField.text != string.Empty;

            if (!updated)
                return;
            
            // Write new name to data
            AppData.ColorMarkers[updateMarkerSession.ColorMarker] = inputField.text;
            // Finish session
            updateMarkerSession.Finish();
        }

        // Trying to find a name that already exists
        private bool TryFindSimilarMarker()
        {
            var orders = new List<int>() {0, 1, 2, 3, 4};
            orders.Remove(updateMarkerSession.ColorMarker);

            return orders.Any(order => inputField.text == ColorMarkersDescriptor.GetColorName(order));
        }

        // Set default name to input
        private void SetDefaultName()
        {
            setDefaultName.Reactivate();
            inputField.text = textDefault.text;
        }

        // Return day characteristic default name 
        private string GetMarkerDefaultKey(int markerId)
        {
            // Checking for an array out of bounds
            if (markerId < 0)
                markerId = 0;

            // Checking for an array out of bounds
            if (markerId >= ColorMarkersDescriptor.MarkersCount)
                markerId = ColorMarkersDescriptor.MarkersCount - 1;

            // Return default name by order in list
            switch (markerId)
            {
                case 0:
                    return TextKeysHolder.MarkerDefaultName1;
                case 1:
                    return TextKeysHolder.MarkerDefaultName2;
                case 2:
                    return TextKeysHolder.MarkerDefaultName3;
                case 3:
                    return TextKeysHolder.MarkerDefaultName4;
                case 4:
                    return TextKeysHolder.MarkerDefaultName5;
                default:
                    return string.Empty;
            }
        }
        
        // Return description of day characteristic 
        public static string GetMarkerDescriptionKey(int markerId)
        {
            // Checking for an array out of bounds
            if (markerId < 0)
                markerId = 0;

            // Checking for an array out of bounds
            if (markerId >= ColorMarkersDescriptor.MarkersCount)
                markerId = ColorMarkersDescriptor.MarkersCount - 1;

            // Return default description by order in list
            switch (markerId)
            {
                case 0:
                    return TextKeysHolder.MarkerDescription1;
                case 1:
                    return TextKeysHolder.MarkerDescription2;
                case 2:
                    return TextKeysHolder.MarkerDescription3;
                case 3:
                    return TextKeysHolder.MarkerDescription4;
                case 4:
                    return TextKeysHolder.MarkerDescription5;
                default:
                    return string.Empty;
            }
        }
    }
}
