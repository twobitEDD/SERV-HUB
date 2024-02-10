using Architecture.Components;
using Architecture.Statistics;
using UnityEngine;

namespace Architecture.TaskViewArea.NormalView.Statistics
{
    // Component of 
    public class MonthCalendarIcon
    {
        // Animation names
        private const string animationActivityTo = "CalendarTo";
        private const string animationActivityFrom = "CalendarFrom";
        private const string animationActivityDefault = "CalendarDefault";
        // Animation component of icon
        private readonly Animation animation;
        // Component button
        private readonly MainButtonJob handleObject;
        // Data generator
        private readonly FlowDataCreator flowDataCreator;
        // Component of statistics scroll
        private readonly StatisticsScroll statisticsScroll;
        // Component of statistics
        private readonly FlowStatistics flowStatistics;
        // Step of page with current month
        private int TodayStep => flowDataCreator.DefaultStep();
        private bool animationActivity; // Activity of animation
        private int currentStep; // Current step

        // Create and setup
        public MonthCalendarIcon(RectTransform rectTransform, FlowDataCreator flowDataCreator, 
                            StatisticsScroll statisticsScroll, FlowStatistics flowStatistics)
        {
            // Save components
            this.flowDataCreator = flowDataCreator;
            this.statisticsScroll = statisticsScroll;
            this.flowStatistics = flowStatistics;
            // Find animation component
            animation = rectTransform.GetComponent<Animation>();
            // Setup method to data generator
            flowDataCreator.SetActionGetStep(StepUpdated);
            animationActivity = true;
            
            // Create handle component
            var handle = rectTransform.Find("Handle");
            handleObject = new MainButtonJob(rectTransform, TouchClick, handle.gameObject);
            handleObject.Reactivate();
            handleObject.AttachToSyncWithBehaviour();
        }

        // Start move to default step
        public void StartMoveToDefaultStep(int steps)
        {
            statisticsScroll.SetScrollBySteps(-steps);
            UpdateViewToDefault();
        }

        // Update statistics view by step
        private void StepUpdated(int newStep)
        {
            var activityView = TodayStep == newStep;
            currentStep = newStep;
            
            if (activityView != animationActivity)
            {
                animationActivity = activityView;
                UpdateViewByAnimation();
            }
        }

        // Method when touched
        private void TouchClick()
        {
            if (animationActivity)
                return;

            var deltaStep = TodayStep - currentStep;
            
            if (deltaStep > 2)
            {
                var correct = deltaStep - 2;
                flowStatistics.AddStepDelta(correct);
                deltaStep = 2;
            }
            
            statisticsScroll.SetScrollBySteps(-deltaStep);
            handleObject.SimulateWave();
        }

        // Update animation
        private void UpdateViewByAnimation() =>
            animation.CrossFade(animationActivity ? animationActivityFrom : animationActivityTo);

        // Set icon to default animation state
        private void UpdateViewToDefault()
        {
            animationActivity = true;
            animation.Play(animationActivityDefault);
        }
    }
}
