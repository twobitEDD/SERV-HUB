using System;
using System.Collections.Generic;
using InternalTheming;
using Theming;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HomeTools.Input
{
    // Additional layer for unity input field
    public class HInputField : InputField
    {
        // Selection color marker
        public ColorTheme SelectionColor { get; set; }
        // Selected/deselected input field actions
        private readonly List<Action> selectActions = new List<Action>();
        private readonly List<Action> deselectActions = new List<Action>();

        // Add actions
        public void AddActionWhenSelected(Action action) => selectActions.Add(action);
        public void AddActionWhenDeselected(Action action) => deselectActions.Add(action);
        
        // Override select method - add additional actions invoke
        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);

            foreach (var selectAction in selectActions)
                selectAction.Invoke();
        }

        // Override deselect method - add additional actions invoke
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);

            foreach (var deselectAction in deselectActions)
                deselectAction.Invoke();
        }

        // Update selective color by marker
        public void UpdateSelectionColor() =>
            selectionColor = ColorConverter.GetColor(ThemeLoader.GetCurrentTheme(), SelectionColor);
    }
}
