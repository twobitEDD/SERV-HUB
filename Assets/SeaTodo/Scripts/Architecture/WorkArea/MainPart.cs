using System;
using Architecture.CalendarModule;
using Architecture.CalendarModule.StatisticElements;
using Architecture.Components;
using Architecture.Data.Structs;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TextHolder;
using HomeTools.Messenger;
using HomeTools.Other;
using HomeTools.Source.Calendar;
using HomeTools.Source.Design;
using HTools;
using MainActivity.AppBar;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.WorkArea
{
    // Component of main part of main page
    public class MainPart
    {
        // Link to app bar
        private AppBar AppBar => AreasLocator.Instance.AppBar;
        // Link to create task page
        private CreateTaskArea.CreateTaskArea CreateFlowArea() => AreasLocator.Instance.CreateTaskArea;
        public Calendar WorkCalendar { get; private set; } // Calendar
        private HomeDay lastWorkDay; // Day of tasks list
        
        private GroupBackground groupBackground; // Component of background
        private YearStats yearStats; // Year stats
        private RectTransform backgroundImage; // UI image
        private RectTransform part1; // Rect of Ui part
        private WorkFlowsList flowsList; // list of flows
        private Text dayByCalendar; // Text of day
        private UIAlphaSync calendarDaySync; // Animation component of day text

        private MainButtonJob emptyCreateFlow; // Button component of area when no tasks
        // UI of view empty tasks list
        private GameObject emptyObject;
        private Text emptyText;

        // Additional height when no tasks in view
        private const float additionalDistanceWhenEmpty = 77f;
        // Bottom side of part
        public float BottomSidePosition { get; private set; }
        
        // Setup view
        public void SpawnElements()
        {
            // Create year view
            yearStats = new YearStats();
            SyncWithBehaviour.Instance.AddAnchor(yearStats.RectTransform.gameObject, AppSyncAnchors.WorkAreaYear);
            // Create background
            groupBackground = SpawnBackground();
            // Setup background image
            backgroundImage = SceneResources.Get("WorkArea Background").GetComponent<RectTransform>();
            // Get part from resources
            part1 = SceneResources.Get("WorkArea Main").GetComponent<RectTransform>();
            // Create component for view tasks
            flowsList = new WorkFlowsList();

            // Setup calendar
            var calendarIcon = part1.Find("Calendar Button").GetComponent<RectTransform>();
            WorkCalendar = new Calendar(calendarIcon, CalendarUpdated, UpdateCalendarGeneratorAction) {CurrentDay = OtherHTools.GetDayBySystem(DateTime.Today)};

            // Setup UI for view empty tasks list
            emptyObject = part1.Find("Empty Flow").gameObject;
            emptyText = emptyObject.transform.Find("Name").GetComponent<Text>();
            emptyCreateFlow = new MainButtonJob(emptyObject.GetComponent<RectTransform>(), 
                OpenCreateFlowFromEmpty, emptyObject.transform.Find("Handle").gameObject);
            emptyCreateFlow.AttachToSyncWithBehaviour(AppSyncAnchors.WorkAreaYear);
            emptyCreateFlow.Reactivate();
            TextLocalization.Instance.AddLocalization(emptyText, TextKeysHolder.Empty);
            
            // Setup day of calendar text
            dayByCalendar = part1.Find("Day By Calendar").GetComponent<Text>();
            UpdateDayByCalendar();
            MainMessenger.AddMember(UpdateDayByCalendar, AppMessagesConst.UpdateWorkAreaByNewDayImmediately);
            MainMessenger.AddMember(UpdateDayByCalendar, AppMessagesConst.LanguageUpdated);
            
            // Create animation component of alpha channel of text of day of calendar
            calendarDaySync = new UIAlphaSync();
            calendarDaySync.AddElement(dayByCalendar);
            calendarDaySync.Speed = 0.05f;
            calendarDaySync.SpeedMode = UIAlphaSync.Mode.Lerp;
            calendarDaySync.PrepareToWork(1);
            SyncWithBehaviour.Instance.AddObserver(calendarDaySync, AppSyncAnchors.WorkAreaYear);
        }

        // Initialize tasks
        public void FirstUpdateAfterAll()
        {
            flowsList.FirstUpdateAfterAll();
        }
        
        // Setup all element to page
        public void SetupElementsToPage(PageItem pageItem)
        {
            SetupBackgroundImageToPage(pageItem);
            SetupYearToPage(pageItem);
            SetupBackgroundToPage(pageItem);
            SetupPart1ToPage(pageItem);
            flowsList.InitializeByCurrentGroup();
            flowsList.SetupFlowsToPage(pageItem);
            emptyObject.SetActive(flowsList.Empty);
            var bottomBorder = flowsList.BottomFlowsBorder - (flowsList.Empty ? additionalDistanceWhenEmpty : 0);
            UpdateBackground(bottomBorder);
        }

        // Update tasks
        public void UpdateFlows(PageItem pageItem)
        {
            flowsList.InitializeByCurrentGroup();
            flowsList.SetupFlowsToPage(pageItem);
            emptyObject.SetActive(flowsList.Empty);
            var bottomBorder = flowsList.BottomFlowsBorder - (flowsList.Empty ? additionalDistanceWhenEmpty : 0);
            UpdateBackground(bottomBorder);
        }

        // Setup year view to page
        private void SetupYearToPage(PageItem pageItem)
        {
            pageItem.AddContent(yearStats.RectTransform);
            yearStats.SetupAnchoredPosition();
        }
        
        // Setup background to page
        private void SetupBackgroundToPage(PageItem pageItem)
        {
            pageItem.AddContent(groupBackground.RectTransform);
            UpdateBackground(2000);
        }
        
        // Setup background image to page
        private void SetupBackgroundImageToPage(PageItem pageItem)
        {
            pageItem.AddContent(backgroundImage, new Vector2(0, -160));
        }
        
        // Setup part to page
        private void SetupPart1ToPage(PageItem pageItem) =>
            pageItem.AddContent(part1, new Vector2(0, -537));

        // Update background
        private void UpdateBackground(float position)
        {
            const int upAnchor = 294;
            const float shadowCorrection = AppElementsInfo.BackgroundShadowCorrection;
            groupBackground.UpdateVisible(upAnchor + Mathf.Abs(position) + shadowCorrection, upAnchor);
            BottomSidePosition = -Mathf.Abs(position) - shadowCorrection;
            groupBackground.RectTransform.SetSiblingIndex(0);
        }

        // Update day view by calendar
        private void UpdateDayByCalendar()
        {
            lastWorkDay = WorkCalendar.CurrentDay;
            
            if (WorkCalendar.TodayIsCurrent)
            {
                dayByCalendar.text = TextLocalization.GetLocalization(TextKeysHolder.Today);
                return;
            }
            
            if (Calendar.Today - WorkCalendar.CurrentDay == 1)
            {
                dayByCalendar.text = TextLocalization.GetLocalization(TextKeysHolder.Yesterday);
                return;
            }
            
            dayByCalendar.text = $"{WorkCalendar.CurrentDay.Day} {CalendarNames.GetMonthShortName(WorkCalendar.CurrentDay.Month)}";
        }

        // Calendar updated animation
        private void CalendarUpdated()
        {
            if (lastWorkDay != WorkCalendar.CurrentDay)
            {
                calendarDaySync.SetAlpha(0.3f);
                calendarDaySync.SetDefaultAlpha(0.3f);
                calendarDaySync.SetAlphaByDynamic(1);
                calendarDaySync.Run();
            }
            
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaByNewDayImmediately);
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
        }

        private void UpdateCalendarGeneratorAction() => CalendarDataGenerator.UpdateDates();

        // Open create task page when click on Empty task list
        private void OpenCreateFlowFromEmpty()
        {
            PageTransitionTemplates.OpenPageFromWorkArea(CreateFlowArea().SetupParents, CreateFlowArea().SetupToPage);
            AppBar.OpenCreateFlowMode();
            MainMessenger.SendMessage(AppMessagesConst.MenuButtonToClose);
            emptyCreateFlow.Reactivate();
        }

        // Spawn background and create background component
        private static GroupBackground SpawnBackground()
        {
            var background = new GroupBackground();
            background.Image.sprite = null;
            background.Image.color = new Color(0,0,0,0);
            background.Shadow.sprite = null;
            background.Shadow.color = new Color(0,0,0,0);
            AppTheming.RemoveElement(background.Image, ColorTheme.WorkAreaBackground, AppTheming.AppItem.WorkArea);
            AppTheming.RemoveElement(background.Shadow, ColorTheme.WorkAreaBackgroundShadow, AppTheming.AppItem.WorkArea);

            return background;
        }
    }
}
