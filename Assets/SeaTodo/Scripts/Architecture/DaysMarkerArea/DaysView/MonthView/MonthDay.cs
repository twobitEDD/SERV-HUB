using Architecture.CalendarModule;
using Architecture.Components;
using Architecture.Data.Structs;
using Architecture.DaysMarkerArea.DaysColors;
using Architecture.DaysMarkerArea.DaysView.DayMarkerPicker;
using Architecture.Elements;
using Architecture.TextHolder;
using HomeTools.Other;
using HTools;
using InternalTheming;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.DaysMarkerArea.DaysView.MonthView
{
    // Day item component for month view
    public class MonthDay
    {
        // Link to View for choose characteristic to selected day
        private static ChooseDayMarkerModule ChooseDayMarkerModule => AreasLocator.Instance.ChooseDayMarkerModule;
        
        // Rect of day item
        public readonly RectTransform RectTransform;
        // Button component of item
        private readonly MainButtonJobBehaviour mainButtonJob;
        // Component for lock touches when scroll
        private readonly DeltaScrollLockerOut deltaScrollLocker;
        
        // UI components of day item
        private readonly Text daysInt;
        private readonly Text weekDay;
        private readonly Image dayColor;
        private readonly Text markerName;
        private readonly Image bottomLine;

        // Saved alpha chanel of week
        private readonly float alphaOfWeek;
        
        private HomeDay currentDay; // Date of day item
        private int currentMarkerId; // Characteristic id
        private MonthMarkersPackage currentMarkersPackage; // Month data package
        private bool currentInCalendar; // Flag if it`s current in calendar

        // Create and setup day item
        public MonthDay(RectTransform rectTransform)
        {
            RectTransform = rectTransform;

            // Create button component
            var handle = rectTransform.Find("Handle");
            mainButtonJob = handle.gameObject.AddComponent<MainButtonJobBehaviour>();
            mainButtonJob.Setup(rectTransform, DayTouched, handle.gameObject);
            mainButtonJob.SimulateWaveScale = 1.037f;
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.MonthMarkersModule);
            
            // Create component for lock touches when scroll
            deltaScrollLocker = new DeltaScrollLockerOut(1);
            SyncWithBehaviour.Instance.AddObserver(deltaScrollLocker, AppSyncAnchors.MonthMarkersModule);
            
            // Add actions from locker to button component 
            mainButtonJob.AddTouchDownAction(deltaScrollLocker.StartCheck);
            mainButtonJob.AddTouchUpAction(deltaScrollLocker.EndCheck);

            // Find UI component
            daysInt = rectTransform.Find("Day Int").GetComponent<Text>();
            markerName = rectTransform.Find("Marker Name").GetComponent<Text>();
            weekDay = rectTransform.Find("Day Of Week").GetComponent<Text>();
            dayColor = rectTransform.Find("Day Color").GetComponent<Image>();
            bottomLine = rectTransform.Find("Split Border").GetComponent<Image>();
            AppTheming.AddElement(bottomLine, ColorTheme.DaysMarkerAreaModuleSplitLine, AppTheming.AppItem.DaysMarkerArea);

            // Save alpha of text color of day of week
            alphaOfWeek = weekDay.color.a;
        }

        // Setup parent for day item
        public void SetParent(RectTransform pool)
        {
            RectTransform.SetParent(pool);
            RectTransform.localScale = Vector3.one;
        }

        // Reset day item immediately
        public void ResetByDefault(HomeDay homeDay, int markerId, bool currentDayInCalendar)
        {
            currentDay = homeDay; // Save date
            currentMarkerId = markerId; // Save characteristic id
            currentInCalendar = currentDayInCalendar; // Flag about today date

            // Update text components
            daysInt.text = currentDay.Day.ToString();
            weekDay.text = TextHolderTime.DaysOfWeekShort(currentDay.DayOfWeek);

            // Update colors of UI of day item
            UpdateByColorsByMarker(markerId);
            // Setup button activity
            UpdateButtonActivity();
        }

        // Save month data package
        public void SetMonthPackage(MonthMarkersPackage monthMarkersPackage) =>
            currentMarkersPackage = monthMarkersPackage;

        // Set activity of line border of day item
        public void SetActivateLineBorder(bool active)
        {
            if (bottomLine.enabled != active)
                bottomLine.enabled = active;
        }

        // Setup button activity
        private void UpdateButtonActivity()
        {
            if (currentDay > Calendar.Today)
                mainButtonJob.Deactivate();
            else
                mainButtonJob.Reactivate();
        }

        // Method call on click
        private void DayTouched()
        {
            // Check locker component
            if (!deltaScrollLocker.Access)
                return;
            
            // Create choose characteristic session
            var newSession = new ChooseDaySession(DynamicUpdate,currentMarkerId + 1);
            // Open choose characteristic view
            ChooseDayMarkerModule.Open(OtherHTools.GetWorldAnchoredPosition(markerName.rectTransform), newSession);
        }

        // Update day item after close choose characteristic view
        private void DynamicUpdate(int id)
        {
            // Check for new characteristic id
            if (id == currentMarkerId)
                return;
            
            // Setup id to month data package
            currentMarkersPackage.ShouldUpdate = true;
            currentMarkersPackage.UpdateDay(currentDay, id);
            // Save local id
            currentMarkerId = id;
            // Update colors of UI of day item
            UpdateByColorsByMarker(currentMarkerId);
        }
        
        // Update colors of UI of day item
        private void UpdateByColorsByMarker(int markerId)
        {
            // Create color for day item
            var color = ThemeLoader.GetCurrentTheme().DaysMarkerAreaMonthDayCircleDisabled;
            // Check for color from characteristics
            if (markerId >= 0)
                color = ColorMarkersDescriptor.GetColor(markerId);
            else if (markerId == -1)
                color = ThemeLoader.GetCurrentTheme().DaysMarkerAreaMonthDayCirclePassive;

            var empty = markerId < 0;

            // Set color to left circle
            dayColor.color = currentInCalendar && empty ? ThemeLoader.GetCurrentTheme().SecondaryColor : color;
            // Update color alpha
            color.a = currentInCalendar ? alphaOfWeek * 1.5f : alphaOfWeek;
            // Set color to text of day of week 
            weekDay.color = currentInCalendar && empty ? ThemeLoader.GetCurrentTheme().SecondaryColor : color;
            
            // Update color for day of month text
            color = ThemeLoader.GetCurrentTheme().DaysMarkerAreaModuleDayNameDisabled;
            if (markerId >= 0)
                color = ColorMarkersDescriptor.GetColor(markerId);
            else if (markerId == -1)
                color = ThemeLoader.GetCurrentTheme().DaysMarkerAreaModuleDayNameEmpty;

            // Set color to text of month day
            daysInt.color = currentInCalendar && empty ? ThemeLoader.GetCurrentTheme().SecondaryColor : color;
            
            // Update color for day characteristic text
            color = ThemeLoader.GetCurrentTheme().DaysMarkerAreaModuleMarkerNameDisabled;
            if (markerId >= 0)
                color = ColorMarkersDescriptor.GetColor(markerId);
            else if (markerId == -1)
                color = ThemeLoader.GetCurrentTheme().DaysMarkerAreaModuleMarkerNameEmpty;

            // Set color to text of day characteristic
            markerName.color = color;
            
            // Update characteristic text
            if (markerId >= 0)
                markerName.text = ColorMarkersDescriptor.GetColorName(markerId);
            else
                markerName.text = string.Empty;
        }
    }
}
