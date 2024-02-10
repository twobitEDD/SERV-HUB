using System;
using Architecture.CalendarModule;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using HomeTools.Messenger;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TrackArea.TrackModule
{
    // Component for track result part in bottom of track task progress area
    public class TrackResultArea : IDisposable
    {
        // Link to calendar
        private readonly Calendar trackCalendar = AreasLocator.Instance.TrackArea.TrackCalendar;
        
        private readonly Text resultText; // Text of result
        private Flow currentFlow; // Current task
        private int cacheResult; // Saved result 

        // Create
        public TrackResultArea(RectTransform trackPanel)
        {
            resultText = trackPanel.Find("Track Result").GetComponent<Text>();
            MainMessenger.AddMember(UpdateCache, AppMessagesConst.TrackCalendarUpdated);
        }

        // Setup new task
        public void PrepareByNewFlow(Flow flow)
        {
            currentFlow = flow;
            UpdateCache();
        }

        // Update cache of task progress
        private void UpdateCache()
        {
            if (currentFlow == null)
                return;
            
            cacheResult = currentFlow.GetIntProgress() - currentFlow.GetTrackedDay(trackCalendar.CurrentDay);
            UpdateText(currentFlow.GetTrackedDayOrigin(trackCalendar.CurrentDay));
        }

        // Update progress in current calendar day
        public void Update(int linePosition)
        {
            UpdateText(linePosition);
        }

        // Update result text
        private void UpdateText(int linePosition)
        {
            var progress = cacheResult + currentFlow.CountTrackedDay(linePosition);
            resultText.text = $"ˁ {FlowInfoAll.GetWorkProgressFlow(currentFlow.Type, progress, resultText.fontSize)}";
            resultText.CalculateLayoutInputHorizontal();
        }

        public void Dispose()
        {
            MainMessenger.RemoveMember(UpdateCache, AppMessagesConst.TrackCalendarUpdated);
        }
    }
}
