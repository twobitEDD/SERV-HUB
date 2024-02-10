using Architecture.ChooseIconModule;
using MainActivity.MainComponents;
using UnityEngine;

namespace MainActivity.AppBar
{
    // Layer for choose icon button
    public class BarFlowIcon
    {
        // Component of choose icon button
        private readonly ChooseIconButton chooseIconButton;

        // Create and setup
        public BarFlowIcon(RectTransform bar)
        {
            var rectTransform = SceneResources.Get("CreateFlow Icon").GetComponent<RectTransform>();
            rectTransform.SetParent(bar);
            
            chooseIconButton = new ChooseIconButton(rectTransform);
            chooseIconButton.SetActiveHandlerButton(false);
        }

        public int GetIconId() => chooseIconButton.IconIdLocal;
        
        public int GetColorId() => chooseIconButton.ColorIdLocal;
        
        public void UpdateImmediately(int icon, int color) => chooseIconButton.UpdateImmediately(icon, color);

        public void SetToAutoScroll(bool active) => chooseIconButton.AutoScroll = active;

        public void SetActive(bool active) => chooseIconButton.SetActiveHandlerButton(active);
    }
}
