using System.Collections.Generic;
using Architecture.Other;
using Architecture.Statistics;
using UnityEngine;

namespace Architecture.ChooseIconModule.StatisticColorElements
{
    // Data generator for color circles view in Choose Icon Module
    public class ChooseColorDataGenerator
    {
        // Calculate default step
        public static int DefaultStep => FlowColorLoader.GetPaletteCount() / 5;

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
            // Create empty list of color ids
            dataStruct.GraphElementsDescription[0] = new List<int>();
            // Set step id
            dataStruct.GraphElementsInfo[0] = step;
            
            // Setup ids of colors
            for (var i = 0; i < 5; i++)
            {
                var id = step * 5 + i;
                if (id < FlowColorLoader.GetPaletteCount())
                    dataStruct.GraphElementsDescription[0].Add(id);
            }
            
            return dataStruct;
        }
    }
}
