using Architecture.CalendarModule;
using Architecture.Components;
using HomeTools.Source.Design;
using HTools;
using Theming;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView
{
    // Component with year button and list of months in Sea Calendar
    public class MarkersYear
    {
        private readonly RectTransform rectTransform; // Rect of part
        private readonly RectTransform monthExample; //  Example UI package of month

        // Generator of months packages
        private readonly ColorPaletteDescriptor colorPaletteDescriptor;
        // Pool components for day view items
        private readonly DaysPool daysPool;
        // Array of month line components
        private MonthItem[] markersMonths;
        // Text for year view
        private readonly Text yearText;
        // Animation component of year text
        private readonly RectTransformSync yearTextRectSync;

        // Button component of left year arrow
        private readonly MainButtonJob leftArrowButton;
        // Button component of right year arrow
        private readonly MainButtonJob rightArrowButton;
        // Animation component of alpha channel of left arrow UI
        private readonly UIAlphaSync leftArrowAlphaSync;
        // Animation component of alpha channel of right arrow UI
        private readonly UIAlphaSync rightArrowAlphaSync;

        // Alpha of interactive arrow
        private const float arrowAlphaActive = 0.37f;
        // Alpha of non-interactive arrow
        private const float arrowAlphaPassive = 0.17f;
        // Position of upper month line 
        private const float upperAnchorOfMonths = -437;
        // Distance between month lines
        private const float shiftBetweenMonths = 107;
        // Count of months in year
        private const int monthsCount = 12;
        // Current year
        public int CurrentYear;

        // Create main
        public MarkersYear(RectTransform part)
        {
            // Save rect of part
            rectTransform = part;
            // Find pool object
            var pool = part.Find("Pool").GetComponent<RectTransform>();
            // Find month line example
            monthExample = pool.Find("Month").GetComponent<RectTransform>();
            // Component that returns month data packages
            colorPaletteDescriptor = new ColorPaletteDescriptor();
            // Create days pool component
            daysPool = new DaysPool(pool);
            // Setup current year
            CurrentYear = 0;
            
            // Find year text and setup animation component of alpha channel for this text
            yearText = part.Find("Current Year/Year").GetComponent<Text>();
            yearTextRectSync = new RectTransformSync();
            yearTextRectSync.SetRectTransformSync(yearText.rectTransform);
            yearTextRectSync.Speed = 0.047f;
            yearTextRectSync.SpeedMode = RectTransformSync.Mode.Lerp;
            yearTextRectSync.TargetScale = Vector3.one * 1.1f;
            yearTextRectSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(yearTextRectSync, AppSyncAnchors.MarkersAreaDays);

            // Find arrow images
            var leftArrow = part.Find("Current Year/Arrow Left").GetComponent<SVGImage>();
            var rightArrow = part.Find("Current Year/Arrow Right").GetComponent<SVGImage>();
            // Find handles of arrows
            var leftArrowHandle = part.Find("Current Year/Handle Left").gameObject;
            var rightArrowHandle = part.Find("Current Year/Handle Right").gameObject;
            
            // Add arrow images to Color Theming module
            AppTheming.AddElement(leftArrow, ColorTheme.DaysMarkerAreaYearArrow, AppTheming.AppItem.DaysMarkerArea);
            AppTheming.AddElement(rightArrow, ColorTheme.DaysMarkerAreaYearArrow, AppTheming.AppItem.DaysMarkerArea);
            
            // Create button component for left arrow
            leftArrowButton = new MainButtonJob(leftArrow.rectTransform, TouchedYearLeft, leftArrowHandle);
            leftArrowButton.AttachToSyncWithBehaviour(AppSyncAnchors.MarkersAreaDays);
            // Create button component for right arrow
            rightArrowButton = new MainButtonJob(rightArrow.rectTransform, TouchedYearRight, rightArrowHandle);
            rightArrowButton.AttachToSyncWithBehaviour(AppSyncAnchors.MarkersAreaDays);

            // Create component of alpha channel for left arrow
            leftArrowAlphaSync = new UIAlphaSync();
            leftArrowAlphaSync.AddElement(leftArrow);
            leftArrowAlphaSync.Speed = 0.1f;
            leftArrowAlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            leftArrowAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(leftArrowAlphaSync, AppSyncAnchors.MarkersAreaDays);
            
            // Create component of alpha channel for right arrow
            rightArrowAlphaSync = new UIAlphaSync();
            rightArrowAlphaSync.AddElement(rightArrow);
            rightArrowAlphaSync.Speed = 0.1f;
            rightArrowAlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            rightArrowAlphaSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(rightArrowAlphaSync, AppSyncAnchors.MarkersAreaDays);

            // Add year text to Color Theming module
            AppTheming.AddElement(yearText, ColorTheme.DaysMarkerAreaYear, AppTheming.AppItem.DaysMarkerArea);

            // Generate months view
            GenerateMonths();
        }

        // Generate month lines
        private void GenerateMonths()
        {
            // Create arrow of month items
            markersMonths = new MonthItem[monthsCount];
            
            // Setup month items
            for (var i = 0; i < monthsCount; i++)
            {
                // Spawn UI of month line 
                var monthObject = Object.Instantiate(monthExample, rectTransform);
                monthObject.name = $"{monthExample.name} {i + 1}";
                // Setup position of month
                monthObject.anchoredPosition = new Vector2(0, upperAnchorOfMonths - shiftBetweenMonths * i);
                // Create month component
                var month = new MonthItem(monthObject, i + 1, this, daysPool);
                // Add month component to array
                markersMonths[i] = month;
            }
        }

        // Update year and months by current year
        public void DefaultUpdate()
        {
            // Update button states
            leftArrowButton.Reactivate();
            rightArrowButton.Reactivate();
            
            // Setup state of alpha channel of month
            yearTextRectSync.SetT(1);
            yearTextRectSync.SetDefaultT(1);
            yearTextRectSync.Stop();

            // Update year arrows
            UpdateYearArrowsDefault();
            
            // Update year to current
            CurrentYear = Calendar.Today.Year;
            // Get new month data packages
            var data = colorPaletteDescriptor.GetYearDaysPalette(CurrentYear);
            // Setup data to view
            for (var i = 0; i < monthsCount; i++)
                markersMonths[i].SetupDefaultData(data[i]);

            // Update year text
            yearText.text = CurrentYear.ToString();
        }
        
        // Update months by year
        private void UpdateByYear(int year)
        {
            // Check for the year already displayed
            if (CurrentYear == year)
                return;
            
            // Start animation of year text
            yearTextRectSync.SetT(0);
            yearTextRectSync.SetDefaultT(0);
            yearTextRectSync.SetTByDynamic(1);
            yearTextRectSync.Run();
            
            // Update year text
            CurrentYear = year;
            yearText.text = CurrentYear.ToString();
            
            // Get month data packages by new year
            var data = colorPaletteDescriptor.GetYearDaysPalette(CurrentYear);
            // Setup data to view
            for (var i = 0; i < monthsCount; i++)
                markersMonths[i].SetupDataByDynamic(data[i]);
        }

        // The method that is called when the button is pressed to the left
        private void TouchedYearLeft()
        {
            leftArrowButton.Reactivate();
            UpdateByYear(GetNewYear(-1));

            UpdateYearArrowsDynamic();
        }
        
        // The method that is called when the button is pressed to the right
        private void TouchedYearRight()
        {
            rightArrowButton.Reactivate();
            UpdateByYear(GetNewYear(1));
            
            UpdateYearArrowsDynamic();
        }

        // Get new year by arrow direction
        private int GetNewYear(int direction)
        {
            var result = CurrentYear;
            var newYear = CurrentYear + direction;

            return newYear > Calendar.Today.Year ? result : newYear;
        }

        // Return to default state and stop animation of arrows
        private void UpdateYearArrowsDefault()
        {
            leftArrowAlphaSync.SetDefaultAlpha(arrowAlphaActive);
            leftArrowAlphaSync.SetAlpha(arrowAlphaActive);
            leftArrowAlphaSync.Stop();
            
            rightArrowAlphaSync.SetAlpha(arrowAlphaPassive);
            rightArrowAlphaSync.SetDefaultAlpha(arrowAlphaPassive);
            rightArrowAlphaSync.Stop();
        }
        
        // Update state of alpha channel of right arrow when user touches arrows
        private void UpdateYearArrowsDynamic()
        {
            rightArrowAlphaSync.SetAlphaByDynamic(CurrentYear == Calendar.Today.Year ? arrowAlphaPassive : arrowAlphaActive);
            rightArrowAlphaSync.Run();
        }
    }
}
