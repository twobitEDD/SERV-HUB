using System.Collections.Generic;
using Architecture.Other;
using Architecture.Statistics;

namespace Architecture.TutorialArea.TutorialElements
{
    // Data generator for tutorial pages
    public class TutorialDataGenerator
    {
        // Default step
        public static int DefaultStep => 4;

        // Create list of data by step
        public GraphDataStruct CreateStruct(int step)
        {
            // Create data struct
            var dataStruct = new GraphDataStruct
            {
                GraphElementsDescription = new List<int>[1],
                GraphElementActive = new bool[1],
                GraphElementsInfo = new float[1],
            };

            dataStruct.GraphElementActive[0] = step >= 0 && step < DefaultStep;
            dataStruct.GraphElementsDescription[0] = new List<int>();
            dataStruct.GraphElementsInfo[0] = step;

            return dataStruct;
        }
    }
}
