using UnityEngine;

namespace Architecture.Statistics.Interfaces
{
    // Component for view of additional info on graphic
    public interface IViewAdditional
    {
        // Setup rect to component
        void Setup(RectTransform view);
        // Setup scroll to component
        void SetupScroll(StatisticsScroll statisticsScroll);
        // Update view by new data
        void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next);
        // Update view by corrected data
        void Update(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next);
    }
}
