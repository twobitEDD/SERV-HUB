using System;
using System.Collections.Generic;
using Architecture.TrackArea;
using HTools;
using MainActivity.AppBar;
using Theming;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Architecture.Pages
{
    // The class that is responsible for the operation of the application pages
    public class PageTransition : IBehaviorSync
    {
        // States of current transition
        private enum TransitionState
        {
            toFade,
            enableFade,
            disableFade,
            fromFade,
            timer,
            action,
            switchPages,
            disableOldPage,
            enableNewPage,
            empty,
        }

        private PageItem[] pageItems; // Pages for UI
        private ScreenBlackout screenBlackout; // White background
        public PageItem PagePool; // Disabled page with UI as pool 
        
        // Fade speed
        private const float fadeSpeed = 0.2f;
        // Queue of states
        private readonly Queue<TransitionState> transitionStates = new Queue<TransitionState>();
        // Queue of timers
        private readonly Queue<int> timers = new Queue<int>();
        // Queue of actions
        private readonly Queue<Action> actions = new Queue<Action>();

        private TransitionState currentState; // Current state in animation
        private TransitionState currentStateParallel; // Parallel state in animation
        private Action currentAction; // Current await action
        private float fadeMarker; // Current fade transparency
        private float currentTimer; // Current timer
        
        private int currentPageIndex; // Current page index
        private int lastPageIndex; // Preview page index
        
        // Delay to disable page pool
        private int started = 3;

        public PageTransition() => CreatePages();

        // Create and setup
        private void CreatePages()
        {
            //Create pages
            var pages = GameObject.Find("Pages").transform;
            pageItems = new PageItem[pages.childCount];
            for (var i = 0; i < pageItems.Length; i++)
            {
                pageItems[i] = new PageItem(pages.GetChild(i).GetComponent<RectTransform>());
            }

            // Setup position of pages rect
            pages.GetComponent<RectTransform>().anchoredPosition +=
                Vector2.down * AppBarMaterial.RectTransform.sizeDelta.y / 2;

            // Setup page pool
            PagePool = new PageItem(Object.Instantiate(pageItems[0].Page.gameObject, pages, true).GetComponent<RectTransform>());
            PagePool.Page.gameObject.name = "Page Pool";
            Object.Destroy(PagePool.Page.GetComponent<GraphicRaycaster>());
            
            // Setup screen fade background for page transitions
            screenBlackout = new ScreenBlackout(AppMessagesConst.Empty);
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.DefaultBackgroundColor, AppTheming.AppItem.Other);
            screenBlackout.PrepareBlackout();
            screenBlackout.SetState(0);

            // Update states
            currentState = TransitionState.empty;
            currentStateParallel = TransitionState.empty;
        }

        // Get current page with content
        public PageItem CurrentPage() => pageItems[currentPageIndex];

        // Add action to transitions
        public void AddAction(Action action)
        {
            transitionStates.Enqueue(TransitionState.action);
            actions.Enqueue(action);
        }

        // Add timer to transitions
        public void AddTimer(int delay)
        {
            transitionStates.Enqueue(TransitionState.timer);
            timers.Enqueue(delay);
        }
        
        // Add disable page action
        public void AddDisableOldPage()
            => transitionStates.Enqueue(TransitionState.disableOldPage);
        
        // Add enable page action
        public void AddEnableNewPage()
            => transitionStates.Enqueue(TransitionState.enableNewPage);
        
        // Add switch pages action
        public void AddSwitchPages()
            => transitionStates.Enqueue(TransitionState.switchPages);
        
        // Add enable fade background action
        public void AddEnableFade()
            => transitionStates.Enqueue(TransitionState.enableFade);
        
        // Add disable fade background action
        public void AddDisableFade()
            => transitionStates.Enqueue(TransitionState.disableFade);
        
        // Add action of enable to fade to opaque state of background
        public void AddToFade()
            => transitionStates.Enqueue(TransitionState.toFade);
        
        // Add action of enable to fade to transparency state of background
        public void AddFromFade()
            => transitionStates.Enqueue(TransitionState.fromFade);
        
        public void Start()
        {
        }

        // Update processes
        public void Update()
        {
            LateStart();
            
            UpdateStates();

            StateSwitchPages();
            StateAction();
            StateTimer();
            StateDisableOldPage();
            StateEnableNewPage();
            StateEnableFade();
            StateDisableFade();
            EnableToFade();
            EnableFromFade();
            StateToFade();
            StateFromFade();
        }

        // States switcher
        private void UpdateStates()
        {
            if (transitionStates.Count == 0)
                return;
            
            if (currentState != TransitionState.empty)
                return;

            currentState = transitionStates.Dequeue();
            
            switch (currentState)
            {
                case TransitionState.action:
                    currentAction = actions.Dequeue();
                    break;
                case TransitionState.timer:
                    currentTimer = timers.Dequeue();
                    break;
            }
        }
        
        // Process of switch activity of page 1 and page 2
        private void StateSwitchPages()
        {
            if (currentState != TransitionState.switchPages)
                return;
            
            lastPageIndex = currentPageIndex;
            currentPageIndex++;

            if (currentPageIndex >= pageItems.Length)
                currentPageIndex = 0;

            currentState = TransitionState.empty;
        }
        
        // Process of invoke action in queue
        private void StateAction()
        {
            if (currentState != TransitionState.action)
                return;
            
            currentAction.Invoke();
            currentState = TransitionState.empty;
        }
        
        // Process of wait for timer
        private void StateTimer()
        {
            if (currentState != TransitionState.timer)
                return;

            if (currentTimer > 0)
            {
                currentTimer--;
                return;
            }

            currentState = TransitionState.empty;
        }
        
        // State of disable old page
        private void StateDisableOldPage()
        {
            if (currentState != TransitionState.disableOldPage)
                return;

            pageItems[lastPageIndex].Enable = false;
            currentState = TransitionState.empty;
        }
        
        // State of enable new page
        private void StateEnableNewPage()
        {
            if (currentState != TransitionState.enableNewPage)
                return;

            pageItems[currentPageIndex].Page.anchoredPosition = new Vector2(pageItems[currentPageIndex].Page.anchoredPosition.x, 0);
            pageItems[currentPageIndex].Enable = true;
            currentState = TransitionState.empty;
        }
        
        // State of enable fade background
        private void StateEnableFade()
        {
            if (currentState != TransitionState.enableFade)
                return;

            screenBlackout.SetState(1);
            fadeMarker = 1;
            currentState = TransitionState.empty;
        }

        // State of disable fade background
        private void StateDisableFade()
        {
            if (currentState != TransitionState.disableFade)
                return;

            screenBlackout.SetState(0);
            fadeMarker = 0;
            currentState = TransitionState.empty;
        }
        
        // State of enable fade background animation to opaque 
        private void EnableToFade()
        {
            if (currentState != TransitionState.toFade)
                return;

            currentStateParallel = TransitionState.toFade;
            currentState = TransitionState.empty;
        }
        
        // State of enable fade background animation to transparency
        private void EnableFromFade()
        {
            if (currentState != TransitionState.fromFade)
                return;

            currentStateParallel = TransitionState.fromFade;
            currentState = TransitionState.empty;
        }
        
        // Update fade background by state
        private void StateToFade()
        {
            if (currentStateParallel != TransitionState.toFade)
                return;

            if (fadeMarker < 1)
                fadeMarker += fadeSpeed;
            
            screenBlackout.SetState(fadeMarker);
            
            if (fadeMarker < 1)
                return;
            
            currentStateParallel = TransitionState.empty;
        }
        
        // Update fade background by state
        private void StateFromFade()
        {
            if (currentStateParallel != TransitionState.fromFade)
                return;

            if (fadeMarker > 0)
                fadeMarker -= fadeSpeed;
            
            screenBlackout.SetState(fadeMarker);
            
            if (fadeMarker > 0)
                return;
            
            currentStateParallel = TransitionState.empty;
        }

        // Start with timer
        private void LateStart()
        {
            if(started < 0)
                return;

            if (started > 0)
            {
                started--;
                return;
            }
            
            PagePool.Enable = false;
            started = -1;
        }
    }
}
