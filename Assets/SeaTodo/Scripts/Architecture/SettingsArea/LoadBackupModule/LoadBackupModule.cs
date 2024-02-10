using System.Collections.Generic;
using Architecture.Components;
using Architecture.Data.Components;
using Architecture.Elements;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea.LoadBackupModule
{
    // The class that is responsible for the load backup window
    public class LoadBackupModule
    {
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;

        private readonly RectTransform rectTransform; // Rect object of view
        private readonly VisualPart visualPart; // Component for visual update of UI
        private readonly RectTransform contentContainer; // Container object for content
        private readonly BackupsContent backupsContent; // View of backup files list
        
        private readonly RectTransform scrollVerticalRect; // Rect object of view
        private readonly float defaultRectHeight; // Default height of scroll rect
        private readonly float defaultModuleHeight; // Default height of rect
        private const float sizePerUnit = 5.57f; // Converter size of rect per unit 
        
        private readonly Text emptyTitle; // Title that visible when no backups
        private readonly Text descriprion; // Description of view
        private readonly SVGImage emptyIcon; // Icon that visible when no backups
        
        private LoadBackupSession loadBackupSession; // Empty session

        // Create and setup
        public LoadBackupModule()
        {
            // Setup rect of view
            rectTransform = SceneResources.Get("Load Backup Module").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            
            // Setup order in hierarchy
            var layerIndex = MainCanvas.RectTransform.childCount - 5;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.LoadBackupModule);
            
            // Create visual part
            visualPart = new VisualPart(rectTransform, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            visualPart.Initialize();
            
            // Send message to messenger when closed
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutLoadBackupModuleClicked);
            
            // Find and localize other text components
            var textInfo = rectTransform.Find("Info").GetComponent<Text>();
            descriprion = rectTransform.Find("Description").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(textInfo, TextKeysHolder.LoadBackupModuleTitle);
            TextLocalization.Instance.AddLocalization(descriprion, TextKeysHolder.LoadBackupModuleDescription);
            // Find scroll objects
            contentContainer = rectTransform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
            scrollVerticalRect = rectTransform.Find("Scroll View/Scrollbar Vertical").GetComponent<RectTransform>();
            
            // Save default height of rect
            defaultModuleHeight = rectTransform.sizeDelta.y;
            // Save default height of scroll rect
            defaultRectHeight = scrollVerticalRect.sizeDelta.y;

            // Find and localize title and find icon that shows when no backups
            emptyTitle = rectTransform.Find("Empty Title").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(emptyTitle, TextKeysHolder.Empty);
            emptyIcon = rectTransform.Find("Empty Icon").GetComponent<SVGImage>();
            
            // Create backups list components
            backupsContent = new BackupsContent(contentContainer.Find("Month Marker Content").GetComponent<RectTransform>());
            MainMessenger.AddMember(UpdateContent, AppMessagesConst.UpdateLoadBackupContent);
        }

        // The method which is responsible for open view
        public void Open(Vector2 startPosition, LoadBackupSession session)
        {
            // Save session
            loadBackupSession = session;

            // Update backups list
            UpdateContent();
            // Start visual component and add action of close
            visualPart.Open(startPosition);
            // Add close action to app bar
            AppBarActions().AddAction(Close);
            // Update module height by backups list
            UpdateModuleHeight(GetModuleHeightByContent());

            // Setup activity of UI when no backups
            emptyIcon.enabled = !backupsContent.HasBackups();
            emptyTitle.enabled = !backupsContent.HasBackups();
        }

        // Update list of backups
        private void UpdateContent()
        {
            // Update backups
            backupsContent.Setup(GetBackupsList());
            // Update content size by backups list
            PrepareView();
        }

        // Update module height
        private void UpdateModuleHeight(float height)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
            var newHeight = defaultRectHeight - (defaultModuleHeight - height) * sizePerUnit;
            scrollVerticalRect.sizeDelta = new Vector2(scrollVerticalRect.sizeDelta.x, newHeight);
        }

        // The method which is responsible for close view
        public void Close()
        {
            // Visual UI close
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
        }

        // Update content size by backups list
        private void PrepareView()
        {
            contentContainer.sizeDelta = new Vector2(contentContainer.sizeDelta.x, backupsContent.RectTransform.sizeDelta.y);
            contentContainer.anchoredPosition = Vector3.zero;
        }

        // Get backups list from data
        private List<string> GetBackupsList() =>
            AppBackupSerialization.LoadBackups();

        // Get module height by backups list
        private float GetModuleHeightByContent()
        {
            const float min = 170 + BackupsContent.ShiftBetweenItems * 2.5f;
            const int max = 770;
            var current = 170f + backupsContent.RectTransform.sizeDelta.y;
            return Mathf.Clamp(current, min, max);
        }
    }
}
