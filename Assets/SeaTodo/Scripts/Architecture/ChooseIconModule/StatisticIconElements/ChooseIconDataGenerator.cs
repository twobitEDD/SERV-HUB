using System.Collections.Generic;
using Architecture.Other;
using Architecture.Statistics;

namespace Architecture.ChooseIconModule.StatisticIconElements
{
    // Data generator for icons view in Choose Icon Module
    public class ChooseIconDataGenerator
    {
        // Calculate default step
        public static int DefaultStep => FlowIconLoader.FlowIconsCount() / 8;

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
            
            // If page active
            dataStruct.GraphElementActive[0] = step >= 0 && step < DefaultStep;
            // Create empty list of icon ids
            dataStruct.GraphElementsDescription[0] = new List<int>();
            // Set step id
            dataStruct.GraphElementsInfo[0] = step;
            
            // Setup ids of icons
            for (var i = 0; i < 8; i++)
            {
                var id = step * 8 + i;
                if (id < FlowIconLoader.FlowIconsCount())
                    dataStruct.GraphElementsDescription[0].Add(id);
            }
            
            return dataStruct;
        }
    }
}
