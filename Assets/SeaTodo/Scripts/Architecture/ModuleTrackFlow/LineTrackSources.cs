using System;
using Architecture.Data;
using Modules.ColorPackage;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleTrackFlow
{
    // Component for storing sets with text for various types of tasks for scroll
    public class LineTrackSources
    {
        private readonly LineTrackSourceCount lineTrackSourceCount;
        private readonly LineTrackSourceSymbol lineTrackSourceSymbol;
        private readonly LineTrackSourceDone lineTrackSourceDone;
        private readonly LineTrackSourceStars lineTrackSourceStars;
        private readonly LineTrackSourceTimeHours lineTrackSourceTimeHours;
        private readonly LineTrackSourceTimeMinutes lineTrackSourceTimeMinutes;
        private readonly LineTrackSourceTimeSeconds lineTrackSourceTimeSeconds;
        
        private readonly LineTrackSourceCountGoal lineTrackSourceCountGoal;
        private readonly LineTrackSourceSymbolGoal lineTrackSourceSymbolGoal;
        private readonly LineTrackSourceDoneGoal lineTrackSourceDoneGoal;
        private readonly LineTrackSourceStarsGoal lineTrackSourceStarsGoal;
        private readonly LineTrackSourceTimeHoursGoal lineTrackSourceTimeHoursGoal;
        private readonly LineTrackSourceTimeMinutesGoal lineTrackSourceTimeMinutesGoal;
        private readonly LineTrackSourceTimeSecondsGoal lineTrackSourceTimeSecondsGoal;

        private readonly Text[] textItems; // Text items

        private Flow currentFlow; // Current task
        private TrackFlowModule.Mode currentMode; // Mode of scroll view

        public TrackPoint[] Points;
        public readonly ColorPackage ColorPackage;
        public Action<int> SetCenterOrigin { set; private get; }
        
        public LineTrackSources(RectTransform pool, TrackFlowModule trackFlowModule)
        {
            ColorPackage = new ColorPackage();
            
            textItems = SourceItemsCreator.CreateTextElements(pool, TrackPositions.PositionsCount);
            var rectTransforms = SourceItemsCreator.GetRectElements(textItems);
            var alphaSyncs = SourceItemsCreator.GetSyncsByText(textItems);
            ColorPackage.AddItems(textItems);
            
            lineTrackSourceCount = new LineTrackSourceCount(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceSymbol = new LineTrackSourceSymbol(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceDone = new LineTrackSourceDone(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceStars = new LineTrackSourceStars(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceTimeHours = new LineTrackSourceTimeHours(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceTimeMinutes = new LineTrackSourceTimeMinutes(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceTimeSeconds = new LineTrackSourceTimeSeconds(pool, textItems, rectTransforms, alphaSyncs);
            
            lineTrackSourceCountGoal = new LineTrackSourceCountGoal(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceSymbolGoal = new LineTrackSourceSymbolGoal(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceDoneGoal = new LineTrackSourceDoneGoal(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceStarsGoal = new LineTrackSourceStarsGoal(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceTimeHoursGoal = new LineTrackSourceTimeHoursGoal(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceTimeMinutesGoal = new LineTrackSourceTimeMinutesGoal(pool, textItems, rectTransforms, alphaSyncs);
            lineTrackSourceTimeSecondsGoal = new LineTrackSourceTimeSecondsGoal(pool, textItems, rectTransforms, alphaSyncs);
        }

        // Setup task and mode
        public void PutFlow(Flow flow, TrackFlowModule.Mode mode)
        {
            CleanFromOldFlow();
            currentFlow = flow;
            currentMode = mode;
            SetupElementsToPoints();
            UpdateTextItems();
        }

        // Get source by task and mode
        public ITrackLine GetActualSource()
        {
            if (currentFlow == null)
                return null;

            switch (currentMode)
            {
                case TrackFlowModule.Mode.track:
                    return GetActualSourceTrack();
                case TrackFlowModule.Mode.goal:
                    return GetActualSourceGoal();
                default:
                    return null;
            }
        }
        
        // Get source by task type when track mode
        private ITrackLine GetActualSourceTrack()
        {
            switch (currentFlow.Type)
            {
                case FlowType.count:
                    return lineTrackSourceCount;
                case FlowType.symbol:
                    return lineTrackSourceSymbol;
                case FlowType.done:
                    return lineTrackSourceDone;
                case  FlowType.stars:
                    return lineTrackSourceStars;
                case FlowType.timeS:
                    return lineTrackSourceTimeSeconds;
                case FlowType.timeM:
                    return lineTrackSourceTimeMinutes;
                case FlowType.timeH:
                    return lineTrackSourceTimeHours;
                default:
                    return null;
            }
        }
        
        // Get source by task type when track goal mode
        private ITrackLine GetActualSourceGoal()
        {
            switch (currentFlow.Type)
            {
                case FlowType.count:
                    return lineTrackSourceCountGoal;
                case FlowType.symbol:
                    return lineTrackSourceSymbolGoal;
                case FlowType.done:
                    return lineTrackSourceDoneGoal;
                case  FlowType.stars:
                    return lineTrackSourceStarsGoal;
                case FlowType.timeS:
                    return lineTrackSourceTimeSecondsGoal;
                case FlowType.timeM:
                    return lineTrackSourceTimeMinutesGoal;
                case FlowType.timeH:
                    return lineTrackSourceTimeHoursGoal;
                default:
                    return null;
            }
        }
        
        public void SetCenterOriginPosition(int position) => SetCenterOrigin?.Invoke(position);

        public Text[] GetTextItems() => textItems;
        
        private void CleanFromOldFlow()
        {
            if (currentFlow == null)
                return;
            
            GetActualSource().CleanWork();
        }

        // Update size of text components by task type and mode
        private void UpdateTextItems()
        {
            if (currentFlow == null)
                return;

            switch (currentMode)
            {
                case TrackFlowModule.Mode.track:
                    UpdateTextItemsTrack();
                    break;
                case TrackFlowModule.Mode.goal:
                    UpdateTextItemsGoal();
                    break;
            }
        }
        
        private void UpdateTextItemsTrack()
        {
            switch (currentFlow.Type)
            {
                case FlowType.count:
                    SetupSizeToTextItems(57);
                    break;
                case FlowType.symbol:
                    SetupSizeToTextItems(87);
                    break;
                case FlowType.done:
                    SetupSizeToTextItems(87);
                    break;
                case FlowType.stars:
                    SetupSizeToTextItems(47);
                    break;
                case FlowType.timeS:
                    SetupSizeToTextItems(47);
                    break;
                case FlowType.timeM:
                    SetupSizeToTextItems(47);
                    break;
                case FlowType.timeH:
                    SetupSizeToTextItems(47);
                    break;
            }
        }
        
        private void UpdateTextItemsGoal()
        {
            switch (currentFlow.Type)
            {
                case FlowType.count:
                    SetupSizeToTextItems(57);
                    break;
                case FlowType.symbol:
                    SetupSizeToTextItems(57);
                    break;
                case FlowType.done:
                    SetupSizeToTextItems(57);
                    break;
                case FlowType.stars:
                    SetupSizeToTextItems(47);
                    break;
                case FlowType.timeS:
                    SetupSizeToTextItems(47);
                    break;
                case FlowType.timeM:
                    SetupSizeToTextItems(47);
                    break;
                case FlowType.timeH:
                    SetupSizeToTextItems(47);
                    break;
            }
        }

        private void SetupSizeToTextItems(int size)
        {
            foreach (var item in textItems)
                item.fontSize = size;
        }

        private void SetupElementsToPoints()
        {
            var source = GetActualSource();

            for (var i = 0; i < source.ElementsCount; i++)
            {
                source.LinkPoint(Points[i]);
            }
        }
    }
}
