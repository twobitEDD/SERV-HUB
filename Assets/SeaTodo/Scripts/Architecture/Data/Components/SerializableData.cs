using System.Collections.Generic;

namespace Architecture.Data.Components
{
    // Class with information for serialization
    public class SerializableData
    {
        public FlowGroup[] FlowGroups; // Global packages
        public int MainFlowGroupId; // Id of selected global package
        public int FlowIdGenerator; // generator of uniq id

        public Dictionary<int, int> PaletteDays; // Data for marked days of Sea Calendar
        public List<string> ColorMarkers; // Day characteristic names for the Sea Calendar
    }
}
