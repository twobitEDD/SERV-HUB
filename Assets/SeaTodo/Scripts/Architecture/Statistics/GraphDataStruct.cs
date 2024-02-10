using System;
using System.Collections.Generic;

namespace Architecture.Statistics
{
    // Struct with data for statistics page
    public struct GraphDataStruct
    {
        // Array with active graphics elements
        public bool[] GraphElementActive;
        // Info of active graphics elements
        public float[] GraphElementsInfo;
        // Description each for graphics element
        public List<int>[] GraphElementsDescription;
        // Id of first graphics active element
        public int FirstElement;
        // Flag if page highlighted or current or etc.
        public int Highlighted;
        // Flag if page does not contains data
        public bool NoData;

        // Check if page visible
        public bool EmptyActivity() => !Array.Exists(GraphElementActive, e => e.Equals(true));
        
        // Check if data without errors
        public bool Actual() => GraphElementsDescription != null && GraphElementsDescription.Length == GraphElementsInfo.Length && 
                                GraphElementsDescription.Length == GraphElementActive.Length;
    }
}
