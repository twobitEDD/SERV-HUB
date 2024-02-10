using Architecture.Components;
using Architecture.Other;
using Architecture.Pages;
using Architecture.Statistics;
using Architecture.TextHolder;
using HTools;
using InternalTheming;
using Packages.HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VectorGraphics;

namespace Architecture.WorkArea.WeeklyActivity
{
    // Component for view day
    public class DayPreview
    {
        public readonly RectTransform RectTransform; // Rect of view
        private readonly Transform pool; // Pool object
        
        // UI components
        private readonly SVGImage circle;
        private readonly SVGImage roundCircle;
        private readonly SVGImage done;
        private readonly CirclePercentage circlePercentage;
        private readonly Text dayOfMonth;
        private readonly Text dayName;
        
        // Animation component of colors
        private readonly UIColorSync uiColorSyncCircles;
        private float localPercentage; // Completed tasks percentage
        private bool localActive; // Active flag
        private bool localToday; // Today flag
        private readonly MainButtonJob mainButtonJob; // Button component
        
        public WeekInfo.DayOfWeek DayOfWeek; // Day of view
        
        // Create and setup
        public DayPreview(RectTransform origin, StatisticsScroll statisticsScroll)
        {
            // Save pool object
            pool = origin.parent;
            
            // Find UI elements
            RectTransform = Object.Instantiate(origin, pool).GetComponent<RectTransform>();
            var infoParent = RectTransform.Find("Day Info").GetComponent<RectTransform>();
            circle = infoParent.Find("Circle").GetComponent<SVGImage>();
            roundCircle = infoParent.Find("Round").GetComponent<SVGImage>();
            dayOfMonth = RectTransform.Find("Day Of Month").GetComponent<Text>();
            dayName = RectTransform.Find("Day Name").GetComponent<Text>();
            done = infoParent.Find("Done Icon").GetComponent<SVGImage>();
            // Create component for circle percentage
            circlePercentage = new CirclePercentage(infoParent);
            SyncWithBehaviour.Instance.AddObserver(circlePercentage, AppSyncAnchors.WorkAreaWeeklyActivity);

            // Create button component
            var handler = RectTransform.Find("Handler").gameObject;
            AddScrollEventsCustom.AddEventActions(handler);
            mainButtonJob = new MainButtonJob(infoParent, Touched, handler);
            mainButtonJob.HandleObject.AddEvent(EventTriggerType.PointerDown, statisticsScroll.TouchDown);
            mainButtonJob.HandleObject.AddEvent(EventTriggerType.PointerUp, statisticsScroll.TouchUp);
            mainButtonJob.SimulateWaveScale = 1.2f;
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.WorkAreaWeeklyActivity);

            // Create animation component of color
            uiColorSyncCircles = new UIColorSync();
            uiColorSyncCircles.AddElement(circle, ThemeLoader.GetCurrentTheme().SecondaryColor);
            uiColorSyncCircles.AddElement(circlePercentage.RoundPercentage, ThemeLoader.GetCurrentTheme().SecondaryColor);
            uiColorSyncCircles.AddElement(circlePercentage.EndLineCircle, ThemeLoader.GetCurrentTheme().SecondaryColor);
            uiColorSyncCircles.AddElement(circlePercentage.StartLineCircle, ThemeLoader.GetCurrentTheme().SecondaryColor);
            uiColorSyncCircles.AddElement(done, ThemeLoader.GetCurrentTheme().ImagesColor);
            uiColorSyncCircles.Speed = 0.1f;
            uiColorSyncCircles.SpeedMode = UIColorSync.Mode.Lerp;
            uiColorSyncCircles.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(uiColorSyncCircles, AppSyncAnchors.WorkAreaWeeklyActivity);
        }
        
        // Update view by new info
        public void UpdateView(int dayOfMoth, float percentage, bool active, bool today)
        {
            dayName.text = TextHolderTime.DaysOfWeekShort(DayOfWeek);
            dayOfMonth.text = dayOfMoth.ToString();
            localPercentage = percentage;
            
            if (!active) percentage = 0f;
            circlePercentage.SetupPercentage(percentage, true);
            
            done.enabled = active;
            roundCircle.enabled = active;

            localActive = active;
            localToday = today;
            
            UpdateOtherColors();

            UpdateSyncsColor();
            MoveCircleColor(percentage > 0.001f, true);
        }

        // Update view by new info with animation
        public void UpdateViewDynamic(int dayOfMoth, float percentage, bool active, bool today)
        {
            dayName.text = TextHolderTime.DaysOfWeekShort(DayOfWeek);
            dayOfMonth.text = dayOfMoth.ToString();
            localPercentage = percentage;

            if (!active) percentage = 0f;
            circlePercentage.SetupPercentage(percentage, false);
            
            done.enabled = active;
            roundCircle.enabled = active;
            
            localActive = active;
            localToday = today;
            
            UpdateOtherColors();
            MoveCircleColor(percentage > 0.001f, false);
        }

        // Update colors
        public void UpdateColors()
        {
            UpdateSyncsColor();
            UpdateOtherColors();
            MoveCircleColor(localPercentage > 0.01f, true);
        }

        // Reset colors of UI and setup animation component again
        private void UpdateSyncsColor()
        {
            uiColorSyncCircles.Stop();
            
            uiColorSyncCircles.SetColor(0);
            uiColorSyncCircles.SetDefaultMarker(0);
            
            var noActiveColor = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            circle.color = noActiveColor;
            circlePercentage.RoundPercentage.color = noActiveColor;
            circlePercentage.EndLineCircle.color = noActiveColor;
            circlePercentage.StartLineCircle.color = noActiveColor;
            done.color = ThemeLoader.GetCurrentTheme().ActivityDayPreviewDonePassive;
  
            var activeColor = ThemeLoader.GetCurrentTheme().SecondaryColor;
            uiColorSyncCircles.AddElement(circle, activeColor);
            uiColorSyncCircles.AddElement(circlePercentage.RoundPercentage, activeColor);
            uiColorSyncCircles.AddElement(circlePercentage.EndLineCircle, activeColor);
            uiColorSyncCircles.AddElement(circlePercentage.StartLineCircle, activeColor);
            uiColorSyncCircles.AddElement(done, ThemeLoader.GetCurrentTheme().ImagesColor);
            uiColorSyncCircles.PrepareToWork();
        }

        // Update percentage circle
        private void MoveCircleColor(bool active, bool immediately)
        {
            if (immediately)
            {
                uiColorSyncCircles.SetColor(active ? 1 : 0);
                uiColorSyncCircles.SetDefaultMarker(active ? 1 : 0);
                
                return;
            }
            
            uiColorSyncCircles.SetTargetByDynamic(active ? 1 : 0);
            uiColorSyncCircles.Run();
        }
        
        // Update colors of UI texts
        private void UpdateOtherColors()
        {
            if (!localActive)
            {
                dayName.color = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
                dayOfMonth.color = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
            }
            else
            {
                dayOfMonth.color = ThemeLoader.GetCurrentTheme().SecondaryColorD2;

                dayName.color = localToday
                    ? ThemeLoader.GetCurrentTheme().ActivityWeekDayActive
                    : ThemeLoader.GetCurrentTheme().ActivityWeekDayDefault;
            }

            roundCircle.color = ThemeLoader.GetCurrentTheme().SecondaryColorD3;
        }

        private void Touched()
        {
         //  return to work area and setup day
        }
    }
}
