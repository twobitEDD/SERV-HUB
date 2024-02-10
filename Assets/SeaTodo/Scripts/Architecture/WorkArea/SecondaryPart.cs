using Architecture.Components;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TaskViewArea.NormalView;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HTools;
using InternalTheming;
using MainActivity.AppBar;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;
using MainStats = Architecture.WorkArea.WeeklyActivity.MainStats;

namespace Architecture.WorkArea
{
    // Component of second part of page with weekly activity
    public class SecondaryPart
    {
        // Link to main component of app bar
        private static AppBar AppBar() => AreasLocator.Instance.AppBar;
        // Link to activity page
        private static StatisticsArea.StatisticsArea StatisticsArea() => AreasLocator.Instance.StatisticsArea;
        // Link to weekly activity
        private readonly MainStats mainStats;
        // Background component
        private readonly GroupBackground groupBackground;
        // Rect object of additional UI above weekly activity
        private readonly RectTransform part1;
        private Text title; // Text of title of part
        // Component of circles of icon of page
        private readonly IconColorCircles iconColorCircles;
        // Button component for activity
        private readonly MainButtonJob statisticsButton;
        // Background height
        private const float backgroundHeight = 977;
        
        public float AnchoredPosition { set; private get; } // Position of part in page
        public float BottomSidePosition { get; private set; } // Bottom position of part

        // Create and setup
        public SecondaryPart()
        {
            // Setup main components
            var statsPart = SceneResources.Get("WorkArea Graphic").GetComponent<RectTransform>();
            SyncWithBehaviour.Instance.AddAnchor(statsPart.gameObject, AppSyncAnchors.WorkAreaWeeklyActivity);
            mainStats = new MainStats(statsPart);
            groupBackground = SpawnBackground();
            part1 = SceneResources.Get("WorkArea Second").GetComponent<RectTransform>();
            title = part1.Find("Title").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.WeeklyActivity);

            // Find circles of icon and create icon component
            var circle1 = part1.Find("Content/Circle 0").GetComponent<Image>();
            var circle2 = part1.Find("Content/Circle 1").GetComponent<Image>();
            var circle3 = part1.Find("Content/Circle 2").GetComponent<Image>();
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);
            UpdateCircles();

            // Create activity icon under weekly activity view
            var statisticIcon = statsPart.Find("Graphic/Statistics").GetComponent<RectTransform>();
            var statisticsButtonHandle = statisticIcon.Find("Handle").gameObject;
            statisticsButton = new MainButtonJob(statisticIcon, OpenStatisticsArea, statisticsButtonHandle);
            statisticsButton.Reactivate();
            statisticsButton.AttachToSyncWithBehaviour();
            
            // Add update icon circles to messenger
            MainMessenger.AddMember(UpdateCircles, 
                string.Format(AppMessagesConst.ColorizedArea, AppTheming.AppItem.WorkArea));
        }

        // Setup elements to page
        public void SetupElementsToPage(PageItem pageItem)
        {
            SetElementsOfBackground(pageItem);

            pageItem.AddContent(part1, new Vector2(0, AnchoredPosition - 37));
            pageItem.AddContent(mainStats.RectTransform, new Vector2(0, AnchoredPosition - 497));

            SetElementToBackground();
        }

        // Setup elements to background
        private void SetElementToBackground()
        {
            groupBackground.AddItemToBackground(part1);
            groupBackground.AddItemToBackground(mainStats.RectTransform);
        }
        
        // Update elements to page and update background
        private void SetElementsOfBackground(PageItem pageItem)
        {
            pageItem.AddContent(part1);
            pageItem.AddContent(mainStats.RectTransform);
            pageItem.AddContent(groupBackground.RectTransform);
            groupBackground.UpdateVisible(backgroundHeight, AnchoredPosition);
            BottomSidePosition = AnchoredPosition - backgroundHeight;
        }

        // Open activity area
        private void OpenStatisticsArea()
        {
            AppBar().OpenStatisticsMode(TextKeysHolder.Activity);
            PageTransitionTemplates.OpenPageFromWorkArea(StatisticsArea().SetupParents, 
                StatisticsArea().SetupToPage);
        }
        
        // Update circles of icon
        private void UpdateCircles() => iconColorCircles.UpdateColors(ThemeLoader.GetCurrentTheme().CreateFlowAreaContentCircles);
        
        // Create and setup background of part of page
        private static GroupBackground SpawnBackground()
        {
            var background = new GroupBackground();
            background.Image.sprite = null;
            background.Image.color = new Color(1, 1, 1, 0);
            background.Shadow.sprite = null;
            background.Shadow.color = new Color(1, 1, 1, 0);
            AppTheming.RemoveElement(background.Image, ColorTheme.WorkAreaBackground, AppTheming.AppItem.WorkArea);
            AppTheming.RemoveElement(background.Shadow, ColorTheme.WorkAreaBackgroundShadow, AppTheming.AppItem.WorkArea);

            return background;
        }
    }
}
