using System;
using System.Collections.Generic;
using Architecture.CalendarModule;
using Architecture.Data.Components;
using Architecture.Data.Structs;
using Architecture.TextHolder;

namespace Architecture.Data
{
    // Data manipulation class
    public static class AppData
    {
        // Serialization data package
        private static SerializableData data;
        /// <summary>
        /// Object with a calculated frequency of activity.
        /// Calculated at startup due to performance load
        /// </summary>
        private static DataFrequencyInfo frequencyInfo;
        // Initialized data flag
        private static bool initialized;
        
        // Data initialization for application operation
        public static void Initialize()
        {
            if (initialized)
                return;
            
            // Data initialization
            var loadData = DataBuilder.BuildData(AppDataSerialization.FilePath);
            data = loadData.data;
            
            // Creating an object with the calculation of user activity
            frequencyInfo = new DataFrequencyInfo();

            // Save the date of the first launch of the application
            if (AppCurrentSettings.AppInstalledDate == 0)
            {
                AppCurrentSettings.AppInstalledDate = Calendar.Today.HomeDayToInt();
                AppCurrentSettings.SetDefaultSettings();
            }

            initialized = true;
        }

        // Loading a data package at a given path
        public static void LoadNewData(string path)
        {
            var (serializableData, success) = DataBuilder.BuildData(path);
            
            if (success)
                data = serializableData;
        }
        
        // Clear app data
        public static void ClearData() => data = DataBuilder.BuildDataTempClear();

        // Save application data
        public static void Save() => AppDataSerialization.SaveDataToFile(data);
        
        // Save application data to file by given path
        public static void Save(string path) => AppDataSerialization.SaveDataToFile(data, path);
        
        // Get Package of tasks
        public static FlowGroup GetCurrentGroup() => Array.Find(data.FlowGroups, e => e.Id == data.MainFlowGroupId);
        
        // Get packages of tasks
        public static FlowGroup[] GetGroups => data.FlowGroups;
        
        // Create a new unique ID
        public static int GenerateNewId() => ++data.FlowIdGenerator;
        
        // Return an object with a calculated activity rate
        public static DataFrequencyInfo GroupsAdditionalInfo() => frequencyInfo;
       
        // Return the date when the app was first launched
        public static HomeDay AppInstalledDate => FlowExtensions.IntToHomeDay(AppCurrentSettings.AppInstalledDate);

        // Return the name of the day characteristics for the Sea Calendar
        public static List<string> ColorMarkers => data.ColorMarkers;

        // Return a package of days with a characteristic for the Sea Calendar
        public static Dictionary<int, int> PaletteDays => data.PaletteDays;
    }
}
