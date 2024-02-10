using System.Linq;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.ModuleReminders;
using Architecture.Other;
using Architecture.Pages;
using Architecture.TaskViewArea.NormalView;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.EditTaskArea
{
    // Class of update task view part
    public class UpdateFlowPart
    {
        // Link to reminders module
        private RemindersModule RemindersModule() => AreasLocator.Instance.RemindersModule;
        
        // Position parameters of elements
        private const float imagePositionFromTop = 477;
        private const float goalPositionFromTop = 177;
        private const float remindersPosition = -395;

        // Height of background
        private const float backgroundHeight = 1687;
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Background of part
        private readonly GroupBackground groupBackground;
        // Animation component for background
        private RectTransformSync groupBackgroundSync;
        // Choose goal component
        private ChooseGoalPartJob chooseGoalPartJob;
        // Title of part
        private Text createFlowTitleText;
        // Circles that placed around icon
        private IconColorCircles iconColorCircles;
        // Bottom position of part in page
        public readonly float GetBottomSide;
        
        // Create
        public UpdateFlowPart()
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add part to pool
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background height and Y position
            groupBackground.UpdateVisible(backgroundHeight, 170);
            // Calculate bottom side
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Initialize base elements to background
        public void SetupBaseElements()
        {
            // Init background parts
            var image = InitBackground();
            // Create choose goal view component
            CreateChooseGoalView(image);
        }

        // Initialize image to background
        private RectTransform InitBackground()
        {
            var image = SceneResources.Get("EditFlow Content").GetComponent<RectTransform>();
            groupBackground.AddItemToBackground(image);
            image.anchoredPosition = new Vector2(0, groupBackground.RectTransform.sizeDelta.y / 2 - imagePositionFromTop);

            var circle1 = image.Find("Circle 0").GetComponent<Image>();
            var circle2 = image.Find("Circle 1").GetComponent<Image>();
            var circle3 = image.Find("Circle 2").GetComponent<Image>();
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);
            
            return image;
        }

        // Create component that is responsible for choose goal UI
        private void CreateChooseGoalView(RectTransform image)
        {
            chooseGoalPartJob = new ChooseGoalPartJob();
            chooseGoalPartJob.SetupToPlace(groupBackground.RectTransform, image.anchoredPosition.y -
                                                                          image.sizeDelta.y / 2 - goalPositionFromTop);
        }
        
        // Create animation component for background part
        public void SetupMovableParameters()
        {
            groupBackgroundSync = new RectTransformSync();
            groupBackgroundSync.SetRectTransformSync(groupBackground.RectTransform);
            groupBackgroundSync.TargetPosition = new Vector3(0, SetupBasePositions(), 0);
            groupBackgroundSync.Speed = 0.17f;
            groupBackgroundSync.SpeedMode = RectTransformSync.Mode.Lerp;
            groupBackgroundSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(groupBackgroundSync);
        }
        
        // Setup main position of part in page
        private float SetupBasePositions() => -MainCanvas.RectTransform.sizeDelta.y - AppElementsInfo.DistanceContentToBottom;

        // Play move animation of part in new page when open
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.TargetPosition = new Vector3(0, -SetupBasePositions() * 2, 0);
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            RemindersModule().SetRemindersToPlace(groupBackground.RectTransform, new Vector2(0, remindersPosition));
            iconColorCircles.UpdateColors(ThemeLoader.GetCurrentTheme().CreateFlowAreaContentCircles);
        }

        // Update UI view of choose task goal
        public void SetupTrackFlow(Flow flow)
        {
            chooseGoalPartJob.PrepareTrackGoalPanel();
            chooseGoalPartJob.UpdateTrackType(flow);
        }

        // Play move animation of part in new page when close
        public void SetPartFromPage()
        {
            var centeredPosition = PageTransition().CurrentPage().Page.anchoredPosition.y +
                                   groupBackground.RectTransform.anchoredPosition.y;
            var direction = centeredPosition > 0 ? -1 : 1;
  
            groupBackgroundSync.Speed = 0.37f;
            groupBackgroundSync.TargetPosition = new Vector3(0, groupBackground.RectTransform.anchoredPosition.y - MainCanvas.RectTransform.sizeDelta.y * direction, 0);
            groupBackgroundSync.SetTByDynamic(0);
            groupBackgroundSync.Run();
        }

        // Set part to page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(groupBackground.RectTransform);

        // Get chosen goal of task
        public int GetGoal() => chooseGoalPartJob.GetGoal();
        
        // Set active state of  part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);
        
        // Setup reminders module by task
        public void CreateRemindersSession(Flow flow)
        {
            // Create session with default time
            var reminderSession = new ReminderSession
            {
                SessionInfo =
                {
                    [WeekInfo.DayOfWeek.Sunday] = (false, new HomeTime(8,0)),
                    [WeekInfo.DayOfWeek.Monday] = (false, new HomeTime(8,0)),
                    [WeekInfo.DayOfWeek.Tuesday] = (false, new HomeTime(8,0)),
                    [WeekInfo.DayOfWeek.Wednesday] = (false, new HomeTime(8,0)),
                    [WeekInfo.DayOfWeek.Thursday] = (false, new HomeTime(8,0)),
                    [WeekInfo.DayOfWeek.Friday] = (false, new HomeTime(8,0)),
                    [WeekInfo.DayOfWeek.Saturday] = (false, new HomeTime(8,0))
                }
            };

            // Update reminders view by task reminders
            if (flow.Reminders != null)
            {
                for (var i = 0; i < flow.Reminders.Count; i++)
                {
                    var elementReminders = flow.Reminders.ElementAt(i);
                    reminderSession.SessionInfo[(WeekInfo.DayOfWeek)(elementReminders.Key)] = (true, elementReminders.Value);
                }
            }
            
            // Update reminders view by new session
            RemindersModule().SetupSession(reminderSession);
        }
    }
}
