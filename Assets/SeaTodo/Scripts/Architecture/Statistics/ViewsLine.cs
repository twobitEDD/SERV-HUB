using System;
using System.Linq;
using Architecture.Statistics.Interfaces;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Architecture.Statistics
{
    // Set of statistics pages that organize motions
    public class ViewsLine
    { 
        private const int viewCount = 2; // Count of pages
        private const float viewsHeightPosition = 0; // Y position of pages
        // Multiplier when touched and clamped 
        private const float slowWhenClampedTouched = 0.5f;
        // Max scroll position outside the bounds
        private const float maxDeviationPercentage = 0.3f;

        private readonly ViewItem[] views; // Array of page items
        private StatisticsScroll statisticsScroll; // Scroll component
        private readonly float viewWidth; // Width of page
        
        private bool resetScrollByTouchUp; // Flag - reset scroll when untouched
        private float positionClampDeflection; // Deflection of clamped position
        private bool inMotion; // Flag - if page in motion

        // Create and setup
        public ViewsLine(RectTransform viewExample, Transform mainParent, Type viewAdditional)
        {
            viewWidth = viewExample.sizeDelta.x;
            views = new ViewItem[viewCount];
            GenerateViews(viewExample, mainParent, viewAdditional);
        }

        // Save scroll component 
        public void ConnectWithScroll(StatisticsScroll scroll) => statisticsScroll = scroll;

        // Setup Y position for pages
        public void SetupHeight(float position)
        {
            foreach (var view in views)
                view.SetupHeightPosition(position);
        }
        
        // Setup default line of pages
        public void SetupPositions()
        {
            for (var i = 0; i < viewCount; i++)
            {
                views[i].AddDeltaWidth((viewWidth) * i);
                views[i].GraphItem.SetupStep(i);
                views[i].GraphItem.UpdateStep(0);
            }
        }
        
        // Setup fixed default line of pages 
        public void UpdatePositions()
        {
            for (var i = 0; i < viewCount; i++)
            {
                views[i].AddDeltaWidthFixed((viewWidth) * i);
                views[i].GraphItem.SetupStepFixed(i);
                views[i].GraphItem.UpdateStep(0);
            }
        }
        
        // Add delta move to pages
        public void AddDelta(float delta)
        {
            inMotion = true;
            
            Array.Sort(views);
            delta = ClampedDeltaByViews(delta);
            
            foreach (var view in views)
                view.AddDeltaWidth(delta);

            ViewsLoop(delta);
            
            statisticsScroll.AddDeltaBackToJoin(delta);
        }

        // Get graphics components of pages
        public GraphItem[] GetGraphItems()
        {
            var result = new GraphItem[viewCount];
            for (var i = 0; i < viewCount; i++)
                result[i] = views[i].GraphItem;

            return result;
        }

        // Touch up method
        public void TouchUpReaction()
        {
            if (!resetScrollByTouchUp)
                return;
            
            statisticsScroll.BackToNormalStateWhenClamped(positionClampDeflection);
            resetScrollByTouchUp = false;
            positionClampDeflection = 0;
        }

        // Turn off motion of page
        public void SleepMotion()
        {
            if (!inMotion)
                return;

            var view = views.FirstOrDefault(e => Mathf.Abs(e.Position) < 10);
            view?.GraphItem.MotionSlept();
            
            inMotion = false;
        }

        // Check activity of current motion
        public bool CheckSleepMotion() => !inMotion;

        // Get current statistics page
        public ViewItem GetCenteredView() => views.OrderBy(e => Mathf.Abs(e.Position)).ToArray()[0];

        // Setup loop motion for pages
        private void ViewsLoop(float delta)
        {
            if (Math.Abs(delta) < 0.0001f)
                return;

            var lastView = views[viewCount - 1];
            var firstView = views[0];

            if (delta > 0 && lastView.AnchoredPosition.x > viewWidth)
            {
                lastView.AddDeltaWidthFixed(firstView.AnchoredPosition.x - viewWidth);
                lastView.MoveDataBack();
                return;
            }

            if (delta < 0 && firstView.AnchoredPosition.x < -viewWidth)
            {
                firstView.AddDeltaWidthFixed(lastView.AnchoredPosition.x + viewWidth);
                firstView.MoveDataForward();
            }
        }
        
        // Checking the scroll update when going out of borders by the move delta
        private float ClampedDeltaByViews(float delta)
        {
            if (delta < 0 && views[1].GraphItem.EmptyActivity)
            {
                var pivotView = views[0];
                var maxPosition = -pivotView.Width * maxDeviationPercentage;
                var deltaPosition = pivotView.Width + maxPosition;
                var currentDeltaPosition = Mathf.Abs(pivotView.Position - maxPosition);
                var percentage = Mathf.Clamp(currentDeltaPosition / deltaPosition, 0, 1);
                delta *= percentage * slowWhenClampedTouched;
                
                resetScrollByTouchUp = percentage < 1;
                positionClampDeflection = pivotView.Position;
                
                statisticsScroll.CheckForResetInertiaWhenUntouched();
            }
            
            if (delta > 0 && views[views.Length - 2].GraphItem.EmptyActivity)
            {
                var pivotView = views[views.Length - 1];
                var maxPosition = pivotView.Width * maxDeviationPercentage;
                var deltaPosition = maxPosition;
                var currentDeltaPosition = Mathf.Abs(pivotView.Position - maxPosition);
                var percentage = Mathf.Clamp(currentDeltaPosition / deltaPosition, 0, 1);
                delta *= percentage * slowWhenClampedTouched;
                
                resetScrollByTouchUp = percentage < 1;
                positionClampDeflection = pivotView.Position;
                
                statisticsScroll.CheckForResetInertiaWhenUntouched();
            }

            return delta;
        }

        // Create pages for statistics scroll
        private void GenerateViews(RectTransform viewExample, Transform mainParent, Type viewAdditional)
        {
            for (var i = 0; i < viewCount; i++)
            {
                var newView = Object.Instantiate(viewExample, mainParent);
                var view = new ViewItem(newView, (IViewAdditional)Activator.CreateInstance(viewAdditional));
                view.SetupHeightPosition(viewsHeightPosition);
                views[i] = view;
            }
        }
    }
}
