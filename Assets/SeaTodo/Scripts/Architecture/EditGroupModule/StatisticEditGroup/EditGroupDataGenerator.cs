using System.Collections.Generic;
using Architecture.Other;
using Architecture.Statistics;
using UnityEngine;

namespace Architecture.EditGroupModule.StatisticEditGroup
{
    // Data generator for title icons view in Update Title view
    public class EditGroupDataGenerator
    {
        // Calculate default step
        public static int DefaultStep => GroupIconLoader.IconsCount() / 4;

        // Create data by step page
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
            // Create empty list of icons ids
            dataStruct.GraphElementsDescription[0] = new List<int>();
            // Set step id
            dataStruct.GraphElementsInfo[0] = step;
            
            // Setup ids of icons
            for (var i = 0; i < 4; i++)
            {
                var id = step * 4 + i;
                if (id < GroupIconLoader.IconsCount())
                    dataStruct.GraphElementsDescription[0].Add(id + 1);
            }

            return dataStruct;
        }
    }
}
