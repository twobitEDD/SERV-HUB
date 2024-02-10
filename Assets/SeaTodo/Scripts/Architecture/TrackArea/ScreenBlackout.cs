using System;
using HomeTools.Handling;
using HomeTools.Messenger;
using HomeTools.Other;
using HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Architecture.TrackArea
{
    // Background screen image
    public class ScreenBlackout : IDisposable
    {
        // Handle object of screen
        public readonly HandleObject HandleObject;
        
        // Animation component of alpha channel
        private UIAlphaSync uiAlphaSync;
        public readonly Image Image;
        private bool tapped;

        // Message that sends when background clicked
        public string MessageBlackoutClicked;
        
        // Create and setup
        public ScreenBlackout(string name, int orderIndex = 1)
        {
            // Create background object
            var blackoutObject = new GameObject(name);
            blackoutObject.transform.SetParent(MainCanvas.RectTransform);
            Image = blackoutObject.gameObject.AddComponent<Image>();
            
            // Setup rect of background
            var rectTransform = blackoutObject.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.SetRectTransformAnchorHorizontal(0, 0);
            rectTransform.SetRectTransformAnchorVertical(0, 0);
            rectTransform.SetSiblingIndex(orderIndex);

            // Create handle component of background
            HandleObject = new HandleObject(rectTransform.gameObject);
            HandleObject.AddEvent(EventTriggerType.PointerDown, PointerDown);
            HandleObject.AddEvent(EventTriggerType.PointerUp, PointerUp);
            HandleObject.AddEvent(EventTriggerType.PointerExit, PointerExit);
        }

        // Prepare background to work
        public void PrepareBlackout()
        {
            uiAlphaSync = new UIAlphaSync();
            uiAlphaSync.AddElement(Image);
            
            uiAlphaSync.PrepareToWork();

            SetState(0);
        }

        // Setup alpha channel amount to background
        public void SetState(float t)
        {
            uiAlphaSync.SetAlpha(t);
            Image.enabled = t > 0.01f;
        }

        private void PointerDown()
        {
            tapped = true;
        }

        private void PointerUp()
        {
            if (!tapped)
                return;

            tapped = false;
            MainMessenger.SendMessage(MessageBlackoutClicked);
        }
        
        private void PointerExit()
        {
            if (tapped)
                tapped = false;
        }

        public void Dispose()
        {
            uiAlphaSync?.Dispose();
            HandleObject?.Dispose();
        }
    }
}
