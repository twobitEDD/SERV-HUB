using Architecture.Elements;
using Architecture.Pages;
using Architecture.SettingsArea.ChooseItems;
using Architecture.TaskViewArea.NormalView;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Main buttons part of settings page
    public class MainPart
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;

        private RectTransform rectTransform; // Rect object of part
        private const int partAnchoredPosition = -387; // Position of part
        private const int backgroundHeight = 1471; // Height of background
        
        // Background component
        private readonly GroupBackground groupBackground;
        // Animation components for background
        private RectTransformSync groupBackgroundSync;
        // Component for image circles
        private IconColorCircles iconColorCircles;

        // Bottom position of part in page
        public float GetBottomSide { get; private set; }
        private readonly float defaultBottomSide;

        // Components of buttons
        private LineLanguage lineLanguage;
        private LineDayOff lineDayOff;
        private LineFirstDay lineFirstDay;
        private LineHourFormat lineHourFormat;
        private LineDarkTheme lineDarkTheme;
        private LineTutorial lineTutorial;
        private LineAboutSeaCalendar lineAboutSeaCalendar;

        // Create and setup
        public MainPart()
        {
            // Create background
            groupBackground = new GroupBackground();
            // Add part to pool
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background height and Y position
            groupBackground.UpdateVisible(backgroundHeight, 107);
            // Calculate bottom side
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
            // Save bottom side
            defaultBottomSide = GetBottomSide;
        }
        
        // Create animation component for background part
        public void SetupMovableParameters()
        {
            groupBackgroundSync = new RectTransformSync();
            groupBackgroundSync.SetRectTransformSync(groupBackground.RectTransform);
            groupBackgroundSync.TargetPosition = new Vector3(0, 0, 0);
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SpeedMode = RectTransformSync.Mode.Lerp;
            groupBackgroundSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(groupBackgroundSync);
        }
        
        // Initialize base elements to background
        public void InitializeContent(ChooseItemsPanel chooseItemsPanel)
        {
            // Initialize rect object of part and setup to background
            rectTransform = SceneResources.Get("Settings Main Part").GetComponent<RectTransform>();
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.SettingsMain);
            groupBackground.AddItemToBackground(rectTransform);
            rectTransform.anchoredPosition = new Vector2(0, partAnchoredPosition);

            // Find and localize title
            var title = rectTransform.Find("Title").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.SettingsMainTitle);

            // Initialize settings buttons
            lineLanguage = new LineLanguage(rectTransform.Find("Language").GetComponent<RectTransform>(), chooseItemsPanel);
            lineDayOff = new LineDayOff(rectTransform.Find("Day Off").GetComponent<RectTransform>(), chooseItemsPanel);
            lineFirstDay = new LineFirstDay(rectTransform.Find("First Day").GetComponent<RectTransform>(), chooseItemsPanel);
            lineHourFormat = new LineHourFormat(rectTransform.Find("Hour Format").GetComponent<RectTransform>());
            lineDarkTheme = new LineDarkTheme(rectTransform.Find("Dark Theme").GetComponent<RectTransform>());
            lineTutorial = new LineTutorial(rectTransform.Find("Tutorial").GetComponent<RectTransform>());
            lineAboutSeaCalendar = new LineAboutSeaCalendar(rectTransform.Find("About Calendar").GetComponent<RectTransform>());

            // Initialize image to background
            var circle1 = rectTransform.Find("Content/Circle 0").GetComponent<Image>();
            var circle2 = rectTransform.Find("Content/Circle 1").GetComponent<Image>();
            var circle3 = rectTransform.Find("Content/Circle 2").GetComponent<Image>();
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);

            // Add method of update icon to messenger
            MainMessenger.AddMember(UpdateCircles, 
                string.Format(AppMessagesConst.ColorizedArea, AppTheming.AppItem.Settings));
        }
        
        // Play move animation of part in new page
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, SetupBasePositions() * 3, 0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            UpdateCircles();
            
            lineLanguage.Reactivate();
            lineDayOff.Reactivate();
            lineFirstDay.Reactivate();
            lineHourFormat.Reactivate();
            lineDarkTheme.Reactivate();
            lineTutorial.Reactivate();
            lineAboutSeaCalendar.Reactivate();
        }
        
        // Play move animation of part in new page when close
        public void SetPartFromPage()
        {
            var centeredPosition = PageTransition().CurrentPage().Page.anchoredPosition.y +
                                   groupBackground.RectTransform.anchoredPosition.y;
            var direction = centeredPosition > 0 ? -1 : 1;

            groupBackgroundSync.Speed = 0.37f;
            groupBackgroundSync.TargetPosition = new Vector3(0, groupBackground.RectTransform.anchoredPosition.y - SetupBasePositions() * direction, 0);
            groupBackgroundSync.SetTByDynamic(0);
            groupBackgroundSync.Run();
        }

        // Set part to page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(groupBackground.RectTransform);
        
        // Set active state of part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);
        
        // Update visible of buy button
        public void UpdateAnchoredPosition()
        {
            var newHeight = backgroundHeight;
            groupBackground.UpdateVisible(newHeight, 107);
            GetBottomSide = defaultBottomSide;
        }

        // Setup main position of part in page
        private float SetupBasePositions() =>  MainCanvas.RectTransform.sizeDelta.y - 
                                               AppElementsInfo.DistanceContentToBottom + backgroundHeight / 2;

        // Update icon circles color
        private void UpdateCircles() => iconColorCircles.UpdateColors(ThemeLoader.GetCurrentTheme().CreateFlowAreaContentCircles);
    }
}
