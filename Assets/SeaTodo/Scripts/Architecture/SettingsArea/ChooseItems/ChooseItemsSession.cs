using System;

namespace Architecture.SettingsArea.ChooseItems
{
    // Component of choose text item session
    public class ChooseItemsSession
    {
        private readonly Action<int> closeAction; // Close action
        public readonly string[] Items; // Text items for list
        public readonly string CurrentItem; // Selected text item
        public readonly string TitleKey; // Key of localization text for view title

        // Create
        public ChooseItemsSession(Action<int> closeAction, string[] items, string currentItem, string titleKey)
        {
            this.closeAction = closeAction;
            TitleKey = titleKey;
            Items = items;
            CurrentItem = currentItem;
        }

        // Finish session
        public void FinishSession(int order) => closeAction?.Invoke(order);
    }
}
