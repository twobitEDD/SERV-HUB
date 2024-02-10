using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.ModuleReminders;
using Architecture.Other;
using Architecture.Pages;
using Architecture.TaskViewArea.NormalView;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.CreateTaskArea
{
    // Object of choose reminders part
    public class ChooseRemindersPart
    {
        // Link to reminders module
        private RemindersModule RemindersModule() => AreasLocator.Instance.RemindersModule;
        
        // Position parameters of elements
        private const float imagePositionFromTop = 197;
        private const float titlePositionFromTop = 367;
        private const float descriptionPositionFromTop = 478;
        private const float remindersPosition = -227;
        
        // Height of background
        private const float backgroundHeight = 1350;
        
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        private readonly GroupBackground groupBackground; // Background of part
        private RectTransformSync groupBackgroundSync; // Animation component for background
        private Text createFlowTitleText; // Title of part
        private Text createFlowDescription; // Description of part
        
        private IconColorCircles iconColorCircles; // Circles that placed around icon
        
        public readonly float GetBottomSide; // Bottom position of part in page

        // Create
        public ChooseRemindersPart(float upperSidePosition)
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add part to pool
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background height and Y position
            groupBackground.UpdateVisible(backgroundHeight, upperSidePosition + AppElementsInfo.DistanceBetweenBackgrounds);
            // Calculate bottom side
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }

        // Initialize base elements to background
        public void SetupBaseElements()
        {
            InitBackground(); // Init background parts

            InitTitle(); // Init title of part
            InitDescription(); // Init description of part
        }

        // Initialize image to background
        private void InitBackground()
        {
            var image = SceneResources.Get("CreateFlow Content 2").GetComponent<RectTransform>();
            groupBackground.AddItemToBackground(image);
            image.anchoredPosition = new Vector2(0, groupBackground.RectTransform.sizeDelta.y / 2 - imagePositionFromTop);
            
            var circle1 = image.Find("Circle 0").GetComponent<Image>();
            var circle2 = image.Find("Circle 1").GetComponent<Image>();
            var circle3 = image.Find("Circle 2").GetComponent<Image>();
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);
        }

        // Initialize and prepare title to background
        private void InitTitle()
        {
            createFlowTitleText = SceneResources.GetPreparedCopy("CreateFlow Title").GetComponent<Text>();
            groupBackground.AddItemToBackground(createFlowTitleText.rectTransform);
            createFlowTitleText.rectTransform.anchoredPosition = new Vector2(0, 
                groupBackground.RectTransform.sizeDelta.y / 2 - titlePositionFromTop);

            TextLocalization.Instance.AddLocalization(createFlowTitleText, TextKeysHolder.CreateFlowTitleReminders);
        }
        
        // Initialize and prepare description to background
        private void InitDescription()
        {
            createFlowDescription = SceneResources.GetPreparedCopy("CreateFlow Description").GetComponent<Text>();
            groupBackground.AddItemToBackground(createFlowDescription.rectTransform);
            createFlowDescription.rectTransform.anchoredPosition = new Vector2(0, 
                groupBackground.RectTransform.sizeDelta.y / 2 - descriptionPositionFromTop);
            
            TextLocalization.Instance.AddLocalization(createFlowDescription, TextKeysHolder.CreateFlowRemindersDescription);
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

        // Play move animation of part in new page when open and update reminders
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.TargetPosition = new Vector3(0, SetupBasePositions(), 0);
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            RemindersModule().SetRemindersToPlace(groupBackground.RectTransform, new Vector2(0, remindersPosition));
            
            // Create reminders session for reminders module
            var reminderSession = new ReminderSession();
            reminderSession.SessionInfo[WeekInfo.DayOfWeek.Sunday] = (false, new HomeTime(8,0));
            reminderSession.SessionInfo[WeekInfo.DayOfWeek.Monday] = (false, new HomeTime(8,0));
            reminderSession.SessionInfo[WeekInfo.DayOfWeek.Tuesday] = (false, new HomeTime(8,0));
            reminderSession.SessionInfo[WeekInfo.DayOfWeek.Wednesday] = (false, new HomeTime(8,0));
            reminderSession.SessionInfo[WeekInfo.DayOfWeek.Thursday] = (false, new HomeTime(8,0));
            reminderSession.SessionInfo[WeekInfo.DayOfWeek.Friday] = (false, new HomeTime(8,0));
            reminderSession.SessionInfo[WeekInfo.DayOfWeek.Saturday] = (false, new HomeTime(8,0));
            
            RemindersModule().SetupSession(reminderSession);
            
            iconColorCircles.UpdateColors(ThemeLoader.GetCurrentTheme().CreateFlowAreaContentCircles);
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

        // Set active state of part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);

        // Set part to page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(groupBackground.RectTransform);
    }
}
