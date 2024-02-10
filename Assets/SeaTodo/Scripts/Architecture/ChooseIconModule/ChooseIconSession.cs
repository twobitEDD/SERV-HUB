using System;
using Architecture.ChooseIconModule.StatisticColorElements;
using Architecture.ChooseIconModule.StatisticIconElements;
using Architecture.Data.Structs;
using HomeTools.Source.Design;
using Packages.HomeTools.Source.Design;
using UnityEngine;

namespace Architecture.ChooseIconModule
{
    // Session object for view of choose icon
    public class ChooseIconSession
    {
        // Close actions when close view
        private readonly Action<(int, int)> closedAction;

        // Current ids of icon and color
        public int SelectedIconLocal;
        public int SelectedColorLocal;

        // Current items - icon and color
        private ChooseIconItem chooseIconItemLocal;
        private ChooseColorItem chooseColorItemLocal;

        // Create session
        public ChooseIconSession(Action<(int, int)> closedAction)
        {
            this.closedAction = closedAction;

            SelectedIconLocal = 0;
            SelectedColorLocal = 0;
        }

        // Call when new icon selected and start animation process for old selected icon 
        public void UpdateSelectedIcon(int selectedIcon, ChooseIconItem chooseIconItem)
        {
            // Deactivate old selected icon
            if (chooseIconItemLocal != null)
            {
                chooseIconItemLocal.DeselectColor();
                
                chooseIconItemLocal.RectTransformSync.Speed = 0.27f;
                chooseIconItemLocal.RectTransformSync.SetTByDynamic(1);
                chooseIconItemLocal.RectTransformSync.Run();

                chooseIconItemLocal.LocalActive = false;
            }

            // Save new chosen icon
            chooseIconItemLocal = chooseIconItem;
            SelectedIconLocal = selectedIcon;
        }
        
        // Call when new color selected and start animation process for old selected color
        public void UpdateSelectedColor(int selectedColor, ChooseColorItem chooseColorItem)
        {
            // Deactivate old selected color
            if (chooseColorItemLocal != null)
            {
                chooseColorItemLocal.UIAlphaSync.Speed = 0.27f;
                chooseColorItemLocal.UIAlphaSync.SetAlphaByDynamic(0.37f);
                chooseColorItemLocal.UIAlphaSync.Run();
                
                chooseColorItemLocal.RectTransformSync.Speed = 0.27f;
                chooseColorItemLocal.RectTransformSync.SetTByDynamic(1);
                chooseColorItemLocal.RectTransformSync.Run();
                
                chooseColorItemLocal.LocalActive = false;
            }

            // Save new chosen color circle
            chooseIconItemLocal?.UpdateColor(selectedColor);
            chooseColorItemLocal = chooseColorItem;
            SelectedColorLocal = selectedColor;
        }

        // Stop animation of icon and color circle. Needed when the view closes
        public void StopItemsInSession()
        {
            chooseColorItemLocal?.UIAlphaSync.Stop();
            chooseColorItemLocal?.UIAlphaSync.SetDefaultAlpha(1);
            chooseColorItemLocal?.RectTransformSync.Stop();
            chooseColorItemLocal?.RectTransformSync.SetT(0);
            chooseColorItemLocal?.RectTransformSync.SetDefaultT(0);

            chooseIconItemLocal?.UIColorSync.Stop();
            chooseIconItemLocal?.UIColorSync.SetDefaultMarker(1);
            chooseIconItemLocal?.RectTransformSync.Stop();
            chooseIconItemLocal?.RectTransformSync.SetT(0);
            chooseIconItemLocal?.RectTransformSync.SetDefaultT(0);
        }

        // Call actions when session finished
        public void FinishSession() => closedAction.Invoke((SelectedIconLocal, SelectedColorLocal));
    }
}
