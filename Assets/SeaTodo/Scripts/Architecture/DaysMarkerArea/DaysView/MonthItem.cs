using System.Linq;
using Architecture.Components;
using Architecture.DaysMarkerArea.DaysView.MonthMarkersModule;
using Architecture.Elements;
using Architecture.Pages;
using HomeTools.Handling;
using HomeTools.Messenger;
using HomeTools.Other;
using HomeTools.Source.Calendar;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView
{
    // View of month with days circles
    public class MonthItem
    {
        // Link to view of days of month
        private static MonthMarkersModule.MonthMarkersModule MonthMarkersModule() =>
            AreasLocator.Instance.MonthMarkersModule;
        
        private readonly RectTransform rectTransform; // Rect of month object
        private readonly DaysSet daysSet; // Line of day circles
        private readonly int month; // Month number in year

        private readonly Text monthName; // Month name
        private readonly Text monthNameBold; // Month name current
        // Animation component of color of month name
        private readonly UIColorSync monthNameColorSync;
        // Handle component of month
        private readonly HandleObject handle;
        // Animation component of underline of days
        private readonly UIColorSync underlineColorSync;
        private readonly Image underline; // underline for day circles
        private readonly float underlineHeight; // Height of underline image
        private readonly float underlineLength; // Width of underline image
        private readonly Text monthDaysCount; // Days count text
        private readonly MarkersYear markersYear; // Link to package with months and year view

        private readonly MainButtonJob mainButtonJob; // Button component of month
        private MonthMarkersPackage currentPackage; // Keep month data package

        // Create main
        public MonthItem(RectTransform rectTransform, int month, MarkersYear markersYear, DaysPool daysPool)
        {
            // Save components
            this.rectTransform = rectTransform;
            this.markersYear = markersYear;
            this.month = month;

            // Create handle of month and add scroll events
            var handleRect = rectTransform.Find("Handle");
            AddScrollEventsCustom.AddEventActions(handleRect.gameObject);
            
            // Find texts of month name
            monthName = rectTransform.Find("Name").GetComponent<Text>();
            monthNameBold = rectTransform.Find("NameBold").GetComponent<Text>();
            
            // Create animation component of color for month name
            monthNameColorSync = new UIColorSync();
            monthNameColorSync.AddElement(monthName, ThemeLoader.GetCurrentTheme().SecondaryColorP1);
            monthNameColorSync.AddElement(monthNameBold, ThemeLoader.GetCurrentTheme().SecondaryColorP1);
            monthNameColorSync.Speed = 0.2f;
            monthNameColorSync.SpeedMode = UIColorSync.Mode.Lerp;
            monthNameColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(monthNameColorSync, AppSyncAnchors.MarkersAreaDays);
            
            // Create line of day circles component
            daysSet = new DaysSet(rectTransform, daysPool);
            SyncWithBehaviour.Instance.AddObserver(daysSet, AppSyncAnchors.MarkersAreaDays);
            
            // Setup underline image and rect params
            underline = rectTransform.Find("Underline").GetComponent<Image>();
            var underlineRectTransform = underline.rectTransform;
            underlineHeight = underlineRectTransform.sizeDelta.y;
            underlineLength = underlineRectTransform.sizeDelta.x;

            // Find text of days count in month
            monthDaysCount = rectTransform.Find("Month days").GetComponent<Text>();
            // Create animation component for underline and days count
            underlineColorSync = new UIColorSync();
            underlineColorSync.AddElement(underline, ThemeLoader.GetCurrentTheme().SecondaryColorD4A50);
            underlineColorSync.AddElement(monthDaysCount, ThemeLoader.GetCurrentTheme().SecondaryColorD4A50);
            underlineColorSync.Speed = 0.1f;
            underlineColorSync.SpeedMode = UIColorSync.Mode.Lerp;
            underlineColorSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(underlineColorSync, AppSyncAnchors.MarkersAreaDays);

            // Create month button component
            mainButtonJob = new MainButtonJob(rectTransform, MonthTouched, handleRect.gameObject) { SimulateWaveScale = 1.037f };
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.MarkersAreaDays);
        }

        // Setup month data package to days view
        public void SetupDefaultData(MonthMarkersPackage markersPackage)
        {
            // Save month data package
            currentPackage = markersPackage;
            // Update month name text
            monthName.text = CalendarNames.GetMonthShortName(month);
            monthNameBold.text = CalendarNames.GetMonthShortName(month);
            
            // Update color of month name and pause animation component
            UpdateNameTargetColor();
            monthNameColorSync.SetColor(1);
            monthNameColorSync.SetDefaultMarker(1);
            monthNameColorSync.Stop();

            // Update day circles by data
            daysSet.SetupByDefault(markersPackage);
            // Update underline animation component
            UpdateLineDefault();
            // Update underline rect
            UpdateLineFill();
            // Reset state of month button
            mainButtonJob.Reactivate();
        }

        // Setup month data package to days view with animation
        public void SetupDataByDynamic(MonthMarkersPackage markersPackage)
        {
            currentPackage = markersPackage;
            // Update day circles by data with animation
            daysSet.SetupByDynamic(markersPackage);
            // Start animation of month text color
            UpdateMonthColorDynamic();
            // Start animation of underline
            UpdateLineDynamic();
            // Update underline rect
            UpdateLineFill();
        }

        // Update color of month name and prepare animation component
        private void UpdateNameTargetColor()
        {
            // Save color of month name
            var color = monthName.color;
            // Reset state of animation component
            monthNameColorSync.SetDefaultMarker(0);
            monthNameColorSync.SetColor(0);
            monthNameColorSync.Stop();
            
            // Get new color
            var targetColor = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            if (currentPackage.Passed) targetColor = ThemeLoader.GetCurrentTheme().SecondaryColor;
            if (currentPackage.Current) targetColor = ThemeLoader.GetCurrentTheme().SecondaryColorP1;
            
            // Setup old color 
            monthName.color = color;
            monthNameBold.color = color;
            // Prepare animation component by new color
            monthNameColorSync.AddElement(monthName, targetColor);
            monthNameColorSync.AddElement(monthNameBold, targetColor);

            // Set activity month name texts
            monthName.enabled = !currentPackage.Current;
            monthNameBold.enabled = currentPackage.Current;
        }
        
        // Update month name color with animation
        private void UpdateMonthColorDynamic()
        {
            // Update color target of text name
            UpdateNameTargetColor();
            // Start animation component
            monthNameColorSync.SetTargetByDynamic(1);
            monthNameColorSync.Run();
        }

        // Update underline color and animation component
        private void UpdateLineDefault()
        {
            // Get target underline color
            var targetColor = ThemeLoader.GetCurrentTheme().SecondaryColorD4A50;
            if (currentPackage.Passed || currentPackage.Current) 
                targetColor = ThemeLoader.GetCurrentTheme().SecondaryColorD4;
            
            // Get target days count color
            var targetColorCount = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            if (currentPackage.Passed) 
                targetColorCount = ThemeLoader.GetCurrentTheme().SecondaryColorD2;
            if (currentPackage.Current) 
                targetColorCount = ThemeLoader.GetCurrentTheme().SecondaryColorD1;
            
            // Reset animation component
            underlineColorSync.SetDefaultMarker(0);
            underlineColorSync.SetColor(0);
            underlineColorSync.Stop();
            
            // Set color
            underline.color = targetColor;
            monthDaysCount.color = targetColorCount;
            // Prepare animation component by new colors
            underlineColorSync.AddElement(underline, targetColor);
            underlineColorSync.AddElement(monthDaysCount, targetColorCount);
        }

        // Start animation of underline and days count
        private void UpdateLineDynamic()
        {
            // Save colors
            var currentColor = underline.color;
            var currentColorCount = monthDaysCount.color;
            // animation component
            UpdateLineDefault();
            // Save new colors to temp
            var newColor = underline.color;
            var newColorCount = monthDaysCount.color;
            // Setup old colors
            underline.color = currentColor;
            monthDaysCount.color = currentColorCount;
            // Prepare animation component by new colors
            underlineColorSync.AddElement(underline, newColor);
            underlineColorSync.AddElement(monthDaysCount, newColorCount);
            // Start animation component
            underlineColorSync.SetTargetByDynamic(1);
            underlineColorSync.Run();
        }

        // Update underline rect
        private void UpdateLineFill()
        {
            // Get days count in month
            monthDaysCount.text = currentPackage.Days.Count.ToString();
            // Calculate width of line by active days
            var width = (float) (currentPackage.Days.Count(e => e.Value != -2))
                                                    / currentPackage.Days.Count;
            
            // Update rect size
            underline.rectTransform.sizeDelta = new Vector2(
                Mathf.Lerp(underlineHeight, underlineLength, width), underlineHeight);

            // Check activity of underline
            underline.enabled = width > 0.01f;
            
            // Create int for first and last active days
            var firstDay = -1;
            var lastDay = -1;

            // Find first and last active days
            for (var i = 0; i < currentPackage.Days.Count; i++)
            {
                var day = currentPackage.Days.ElementAt(i);
                
                if (day.Value == -2) 
                    continue;
                
                if (firstDay == -1)
                    firstDay = i;

                lastDay = i;
            }
            
            // Check if no active days
            if (firstDay == -1 || firstDay >= daysSet.Days.Count)
                return;

            // Get start and finish position of line by days
            var startPosition = daysSet.Days[firstDay].TargetPosition;
            var endPosition = daysSet.Days[lastDay].TargetPosition;
            // Calculate center position of line
            var position = (endPosition + startPosition) / 2;
            // Setup new position of underline
            underline.rectTransform.anchoredPosition = new Vector2(position, underline.rectTransform.anchoredPosition.y);
        }

        // This method is called when the month is clicked
        private void MonthTouched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            
            // Create month days session
            var session = new MonthMarkersSession(ClosedMonthLayer, 
                $"{CalendarNames.GetMonthFullName(month)} {markersYear.CurrentYear}", currentPackage);
            
            // Open view with days of month
            MonthMarkersModule().Open(OtherHTools.GetWorldAnchoredPosition(rectTransform), session);
        }

        // Update month when month days view closed
        private void ClosedMonthLayer()
        {
            // Check if month day updated
            if (!currentPackage.ShouldUpdate) 
                return;
            
            // Update days with animation
            daysSet.SetupByDynamic(currentPackage);
            // Setup flag for data update
            currentPackage.ShouldUpdate = false;
            // Send message for update days count in characteristic days list
            MainMessenger.SendMessage(AppMessagesConst.ShouldUpdateColorsCountInCalendar);
        }
    }
}
