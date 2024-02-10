using System.Collections.Generic;
using Architecture.Data;
using Architecture.Data.Structs;
using Architecture.Statistics;
using Architecture.WorkArea.Activity;

namespace Architecture.StatisticsArea.ArchivedFlows
{
    // Create data for archived tasks view
    public class ArchivedFlowsDataGenerator
    {
        // Get day started and finished of app
        private static HomeDay DateStarted => WeekDataGenerator.DateStarted;
        private static HomeDay TodayDate => WeekDataGenerator.TodayDate;

        // Keep tasks
        private Flow[] cachedFlows = new Flow[0];

        // Update data
        public void UpdateData()
        {
            cachedFlows = AppData.GetCurrentGroup().ArchivedFlows;

            foreach (var cachedFlow in cachedFlows)
            {
                FlowCreator.CheckArchivedFlowFinishDate(cachedFlow);
            }
        }

        // Create data by page number
        public GraphDataStruct CreateStruct(int step)
        {
            // Create data struct
            var dataStruct = new GraphDataStruct
            {
                GraphElementsDescription = new List<int>[1],
                GraphElementActive = new bool[1],
                GraphElementsInfo = new float[1],
                FirstElement = -1,
                Highlighted = -1,
                NoData = cachedFlows.Length == 0 || TodayDate < DateStarted
            };

            // Setup activity of page
            dataStruct.GraphElementActive[0] = step >= 0 && step < cachedFlows.Length;
            
            // Setup id of archived task for page
            if (dataStruct.GraphElementActive[0])
                dataStruct.GraphElementsDescription[0] = new List<int>() {cachedFlows[step].Id};

            return dataStruct;
        }
    }
}
