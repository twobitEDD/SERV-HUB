using System;
using System.Collections.Generic;
using Architecture.Data.Components;
using Architecture.TextHolder;

namespace Architecture.Data
{
    // Create data class
    public static class DataBuilder
    {
        // Build data of app
        public static (SerializableData data, bool success) BuildData(string path)
        {
            // Create empty data
            SerializableData data;
            var success = false;
            
            try
            {
                // Try load and deserialize of build temp data
                data = AppDataSerialization.LoadDataFromFile(path) ?? BuildDataTemp();
                success = true;
            }
            catch (Exception e)
            {
                // Construct default data on deserialization error
                data = BuildDataTemp();
                success = false;
#if !UNITY_EDITOR
            AppDataSerialization.BlockSaveToFileActivity = true;
#endif
            }

            // Recheck data
            CheckNullData(data);
            
            return (data, success);
        }

        // Build new empty data package
        public static SerializableData BuildDataTempClear()
        {
            var result = new SerializableData
            {
                FlowGroups = TempData.GetTempData(),
                MainFlowGroupId = 1,
                FlowIdGenerator = 2,
            };
            
            result.FlowGroups[0].Flows = new Flow[0];
            CheckNullData(result);
            
            return result;
        }
        
        // Build temp data when deserialization error
        private static SerializableData BuildDataTemp()
        {
            var result = new SerializableData
            {
                FlowGroups = TempData.GetTempData(),
                MainFlowGroupId = 1,
                FlowIdGenerator = 2,
            };
            
            return result;
        }
        
        // Checking data for null values
        private static void CheckNullData(SerializableData data)
        {
            if (data.PaletteDays == null)
                data.PaletteDays = new Dictionary<int, int>();
            
            if (data.ColorMarkers == null)
            {
                data.ColorMarkers = new List<string>
                {
                    $"{TextLocalization.GetLocalization(TextKeysHolder.MarkerDefaultName1)}",
                    $"{TextLocalization.GetLocalization(TextKeysHolder.MarkerDefaultName2)}",
                    $"{TextLocalization.GetLocalization(TextKeysHolder.MarkerDefaultName3)}",
                    $"{TextLocalization.GetLocalization(TextKeysHolder.MarkerDefaultName4)}",
                    $"{TextLocalization.GetLocalization(TextKeysHolder.MarkerDefaultName5)}",
                };
            }

            if (data.FlowGroups == null)
                data.FlowGroups = new FlowGroup[1] { new FlowGroup() };
            
            if (data.FlowGroups[0].Flows == null)
                data.FlowGroups[0].Flows = new Flow[0];
            
            if (data.FlowGroups[0].ArchivedFlows == null)
                data.FlowGroups[0].ArchivedFlows = new Flow[0];
        }
    }
}
