using System;
using HomeTools.Messenger;
using UnityEngine;

namespace Architecture.MenuArea
{
    // Class for controlling the menu button in a app bar
    public class MenuButton
    {
        // List of animation names
        private const string animationFromMenuToClose = "MenuToClose";
        private const string animationFromCloseToMenu = "CloseToMenu";
        private const string animationFromCloseToMainClose = "CloseToMainClose";
        private const string animationFromMenuToEdit = "MenuToEdit";
        private const string animationFromEditToMenu = "EditToMenu";
        private const string animationFromCloseToEdit = "CloseToEdit";
        private const string animationFromEditToClose = "EditToClose";
        private const string animationFromMenuToEmpty = "MenuToEmpty";
        private const string animationFromEmptyToMenu = "EmptyToMenu";
        private const string animationFromCloseToEmpty = "CloseToEmpty";
        private const string animationFromMenuToSea = "CloseToSea";
        private const string animationFromSeaToMenu = "SeaToMenu";
        
        // Rect object of menu button
        private readonly RectTransform rectTransform;
        private readonly Animation animation;

        private Action lastAction;
        
        // Setup menu button
        public MenuButton(RectTransform rectTransform)
        {
            this.rectTransform = rectTransform;
            animation = rectTransform.GetComponent<Animation>();
            // Add methods to messenger for react menu button
            MainMessenger.AddMember(AnimationFromMenuToClose, AppMessagesConst.MenuButtonToClose);
            MainMessenger.AddMember(AnimationFromCloseToMenu, AppMessagesConst.MenuButtonFromClose);
            MainMessenger.AddMember(AnimationFromCloseToMainClose, AppMessagesConst.CloseButtonToMainClose);
            MainMessenger.AddMember(AnimationFromMenuToEdit, AppMessagesConst.MenuButtonToEdit);
            MainMessenger.AddMember(AnimationFromEditToMenu, AppMessagesConst.MenuButtonFromEdit);
            MainMessenger.AddMember(AnimationEditToClose, AppMessagesConst.EditToClose);
            MainMessenger.AddMember(AnimationCloseToEdit, AppMessagesConst.CloseToEdit);
            MainMessenger.AddMember(AnimationFromMenuToEmpty, AppMessagesConst.MenuButtonToEmpty);
            MainMessenger.AddMember(AnimationFromEmptyToMenu, AppMessagesConst.MenuButtonFromEmpty);
            MainMessenger.AddMember(AnimationCloseToEmpty, AppMessagesConst.CloseButtonToEmpty);
            MainMessenger.AddMember(AnimationCloseToSea, AppMessagesConst.CloseButtonToSea);
            MainMessenger.AddMember(AnimationSeaToMenu, AppMessagesConst.MenuButtonFromSea);
        }

        public Transform GetHandle() => rectTransform.GetChild(0);

        // Each method calls to play a specific animation
        // ->
        
        private void AnimationFromMenuToClose()
        {
            if (lastAction == AnimationFromMenuToClose)
                return;
            
            animation.Play(animationFromMenuToClose);
            lastAction = AnimationFromMenuToClose;
        }

        private void AnimationFromCloseToMenu()
        {
            if (lastAction == AnimationFromCloseToMenu)
                return;
            
            animation.Play(animationFromCloseToMenu);
            lastAction = AnimationFromCloseToMenu;
        }
        
        private void AnimationFromCloseToMainClose()
        {
            if (lastAction == AnimationFromCloseToMainClose)
                return;
            
            animation.Play(animationFromCloseToMainClose);
            lastAction = AnimationFromCloseToMainClose;
        }
        
        private void AnimationFromMenuToEdit()
        {
            if (lastAction == AnimationFromMenuToEdit)
                return;
            
            animation.Play(animationFromMenuToEdit);
            lastAction = AnimationFromMenuToEdit;
        }
        
        private void AnimationCloseToEdit()
        {
            if (lastAction == AnimationCloseToEdit)
                return;
            
            animation.Play(animationFromCloseToEdit);
            lastAction = AnimationCloseToEdit;
        }
        
        private void AnimationEditToClose()
        {
            if (lastAction == AnimationEditToClose)
                return;
            
            animation.Play(animationFromEditToClose);
            lastAction = AnimationEditToClose;
        }
        
        private void AnimationFromEditToMenu()
        {
            if (lastAction == AnimationFromEditToMenu)
                return;
            
            animation.Play(animationFromEditToMenu);
            lastAction = AnimationFromEditToMenu;
        }
        
        private void AnimationFromMenuToEmpty()
        {
            if (lastAction == AnimationFromMenuToEmpty)
                return;
            
            animation.Play(animationFromMenuToEmpty);
            lastAction = AnimationFromMenuToEmpty;
        }
        
        private void AnimationFromEmptyToMenu()
        {
            if (lastAction == AnimationFromEmptyToMenu)
                return;
            
            animation.Play(animationFromEmptyToMenu);
            lastAction = AnimationFromEmptyToMenu;
        }
        
        private void AnimationCloseToEmpty()
        {
            if (lastAction == AnimationCloseToEmpty)
                return;
            
            animation.Play(animationFromCloseToEmpty);
            lastAction = AnimationCloseToEmpty;
        }
        
        private void AnimationCloseToSea()
        {
            if (lastAction == AnimationCloseToSea)
                return;
            
            animation.Play(animationFromMenuToSea);
            lastAction = AnimationCloseToSea;
        }
        
        private void AnimationSeaToMenu()
        {
            if (lastAction == AnimationSeaToMenu)
                return;
            
            animation.Play(animationFromSeaToMenu);
            lastAction = AnimationSeaToMenu;
        }
    }
}
