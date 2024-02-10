using System.Linq;
using Architecture.Data;
using Architecture.Data.Components;
using Architecture.Other;
using Architecture.WorkArea.Activity;
using Architecture.WorkArea.WeeklyActivity;
using HTools;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.StatisticsArea.Frequency
{
    // View of user activity frequency
    public class FrequencyView
    {
        private readonly FrequencyItem[] frequencyItems; // Week day frequency items
        
        // Animation component of text animation
        private readonly TextUpdateAnimation frequencyUpdateAnimation;
        // Rest day of week
        private WeekInfo.DayOfWeek restDay;
        
        // Create and setups
        public FrequencyView(RectTransform rectTransform)
        {
            // Create array of items
            frequencyItems = new FrequencyItem[7];
            for (var i = 0; i < 7; i++)
                frequencyItems[i] = new FrequencyItem(rectTransform.Find($"Frequency {i + 1}").GetComponent<RectTransform>());

            // Setup view of global frequency of activity
            var frequencyPercentage = rectTransform.Find("Percentage").GetComponent<Text>();
            frequencyUpdateAnimation = new TextUpdateAnimation(frequencyPercentage, "{0}%", "{0:0}");
            SyncWithBehaviour.Instance.AddObserver(frequencyUpdateAnimation, AppSyncAnchors.StatisticsArea);
        }

        // Update view of frequency items
        public void SetupViewByTasks()
        {
            var frequencyData = AppData.GroupsAdditionalInfo().RecalculateFrequencyForGroup(AppData.GetCurrentGroup());
            
            InitializeDaysOrder();

            restDay = AppCurrentSettings.DayOff;
            
            foreach (var frequencyItem in frequencyData.Frequency)
            {
                var item = frequencyItems.FirstOrDefault(e => e.DayOfWeek == frequencyItem.Key);
                item?.Update(frequencyItem.Value);
            }
            
            UpdateFullPercentage(frequencyData, false);
        }

        // Update view of frequency items with animation
        public void UpdateDynamic()
        {
            var frequencyData = AppData.GroupsAdditionalInfo().RecalculateFrequencyForGroup(AppData.GetCurrentGroup());
            
            foreach (var frequencyItem in frequencyData.Frequency)
            {
                var item = frequencyItems.FirstOrDefault(e => e.DayOfWeek == frequencyItem.Key);
                item?.UpdateDynamic(frequencyItem.Value);
            }

            UpdateFullPercentage(frequencyData, true);
        }

        // Update global frequency percentage of user activity
        private void UpdateFullPercentage(DataFrequencyInfo.GroupFrequency data, bool dynamic)
        {
            var percentage = data.Frequency.Select(e=>e.Value).Sum();

            percentage *= 100;
            percentage /= 7;
            percentage = (int) Mathf.Clamp(percentage, 0, 100);
            
            if (dynamic)
                frequencyUpdateAnimation.SetupProgress(percentage);
            else
                frequencyUpdateAnimation.SetupProgressImmediately(percentage);
        }

        // Get ordered days of week
        private void InitializeDaysOrder()
        {
            var fromSunday = AppCurrentSettings.DaysFromSunday;

            if (fromSunday)
            {
                frequencyItems[0].DayOfWeek = WeekInfo.DayOfWeek.Sunday;
                frequencyItems[1].DayOfWeek = WeekInfo.DayOfWeek.Monday;
                frequencyItems[2].DayOfWeek = WeekInfo.DayOfWeek.Tuesday;
                frequencyItems[3].DayOfWeek = WeekInfo.DayOfWeek.Wednesday;
                frequencyItems[4].DayOfWeek = WeekInfo.DayOfWeek.Thursday;
                frequencyItems[5].DayOfWeek = WeekInfo.DayOfWeek.Friday;
                frequencyItems[6].DayOfWeek = WeekInfo.DayOfWeek.Saturday;
            }
            else
            {
                frequencyItems[0].DayOfWeek = WeekInfo.DayOfWeek.Monday;
                frequencyItems[1].DayOfWeek = WeekInfo.DayOfWeek.Tuesday;
                frequencyItems[2].DayOfWeek = WeekInfo.DayOfWeek.Wednesday;
                frequencyItems[3].DayOfWeek = WeekInfo.DayOfWeek.Thursday;
                frequencyItems[4].DayOfWeek = WeekInfo.DayOfWeek.Friday;
                frequencyItems[5].DayOfWeek = WeekInfo.DayOfWeek.Saturday;
                frequencyItems[6].DayOfWeek = WeekInfo.DayOfWeek.Sunday;
            }
        }
    }
}
