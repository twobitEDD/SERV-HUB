using System.Collections.Generic;
using System.Linq;
using Architecture.Components;
using Architecture.Elements;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea.ChooseItems
{
    // The class that is responsible for the selection window from various text lists
    public class ChooseItemsPanel
    {
        private readonly RectTransform rectTransform; // Rect of view
        private readonly Text title; // title text of view
        private readonly VisualPart visualPart; // Component for UI visual control
        private readonly ItemsPool itemsPool; // Pool of text items fir view
        // List of active items
        private readonly List<ChooseItem> activeItems = new List<ChooseItem>();

        private ChooseItem currentItem; // Selected item
        private ChooseItemsSession currentSession; // Current session
        
        // For run action when user touches home button on device
        private static ActionsQueue AppBarActions() => AreasLocator.Instance.AppBar.RightButtonActionsQueue;
        
        // Create and setup
        public ChooseItemsPanel()
        {
            // Prepare view from resources
            rectTransform = SceneResources.Get("ChooseItems Panel").GetComponent<RectTransform>();
            rectTransform.SetParent(MainCanvas.RectTransform);
            var layerIndex = MainCanvas.RectTransform.childCount - 5;
            rectTransform.transform.SetSiblingIndex(layerIndex);
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.ChooseItemsPanel);
            
            // Create text items pool
            itemsPool = new ItemsPool(rectTransform.Find("Pool").GetComponent<RectTransform>(), this);
            
            // Create visual part
            visualPart = new VisualPart(rectTransform, itemsPool.UIAlphaSync, FinishSession, layerIndex);
            SyncWithBehaviour.Instance.AddObserver(visualPart);
            visualPart.Initialize();
            
            // Send message to messenger when closed
            MainMessenger.AddMember(Close, AppMessagesConst.BlackoutLanguagePanel);
            // Find title text
            title = rectTransform.Find("Info").GetComponent<Text>();
        }
        
        // The method which is responsible for open view
        public void Open(Vector2 position, ChooseItemsSession chooseItemsSession)
        {
            // Save session
            currentSession = chooseItemsSession;
            // Localize title
            TextLocalization.Instance.AddLocalization(title, chooseItemsSession.TitleKey);
            // Create text items by session
            GenerateItemsBySession();
            // Setup selective item
            SetupCurrentItem();
            // Start visual component and add action of close
            visualPart.Open(position);
            // Add close action to app bar
            AppBarActions().AddAction(Close);
        }

        // Setup current item
        public void SetupCurrentItem(ChooseItem chooseItem)
        {
            currentItem?.Deactivate();
            currentItem = chooseItem;
        }
        
        // The method which is responsible for close view
        private void Close()
        {
            // Prepare items to close
            foreach (var languagesItem in activeItems)
                languagesItem.PrepareToClose();

            // Visual UI close
            visualPart.Close();
            // Remove close action from app bar
            AppBarActions().RemoveAction(Close);
        }

        // Create items with text by session
        private void GenerateItemsBySession()
        {
            // Set all items to pool
            foreach (var languagesItem in activeItems)
                itemsPool.SetToPool(languagesItem);
            
            // Clear active items list
            activeItems.Clear();

            // Calculate position params
            var shift = itemsPool.ItemHeight;
            var upperPosition = (currentSession.Items.Length - 1) * 0.5f * shift;
            
            // Setup items
            for (var i = 0; i < currentSession.Items.Length; i++)
            {
                var item = itemsPool.GetFromPool();
                item.RectTransform.SetParent(rectTransform);
                item.RectTransform.localScale = Vector3.one;
                item.RectTransform.anchoredPosition = new Vector2(0, upperPosition - shift * i);
                item.UpdateName(currentSession.Items[i]);
                item.CurrentOrder = i;
                activeItems.Add(item);
            }

            // Update size of view
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (activeItems.Count + 0.6f) * shift);
        }

        // Setup selective item
        private void SetupCurrentItem()
        {
            currentItem = activeItems.FirstOrDefault(e => e.Name.text == currentSession.CurrentItem);
            
            foreach (var item in activeItems)
                item.UpdateBeforeJob(item == currentItem);
        }

        // Finish session when close view
        private void FinishSession()
        {
            if (currentItem == null)
                return;

            currentSession?.FinishSession(currentItem.CurrentOrder);
        }
    }
}
