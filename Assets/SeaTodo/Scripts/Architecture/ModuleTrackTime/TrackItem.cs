using HTools;
using UnityEngine;

namespace Architecture.ModuleTrackTime
{
    // Component that is responsible for a separate scrollbar
    public class TrackItem
    {
        public readonly RectTransform RectTransform; // Rect of scrollbar
        private readonly TrackScroll trackScroll; // Scroll component
        
        // Synchronize track items with scroll
        public bool SyncPosition { set => trackScroll.SyncPosition = value; }

        // Create and setup
        public TrackItem(RectTransform rectTransform, ITrackLine lineTrackSources)
        {
            // Save rect
            RectTransform = rectTransform;
            // Create scroll component
            trackScroll = new TrackScroll(RectTransform.gameObject, RectTransform, lineTrackSources);
            SyncWithBehaviour.Instance.AddObserver(trackScroll, AppSyncAnchors.TrackTimeModule);
        }

        // Prepare scrollbar to new track time session
        public void PrepareToSession(int position)
        {
            trackScroll.SetScrollToDefault(position);
        }
    }
}
