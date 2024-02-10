using System.Collections.Generic;
using System.IO;
using Architecture.Data.Components;
using HomeTools;
using HomeTools.Other;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Android;

namespace Architecture.Data
{
    // Serializing Application Data
    public static class AppDataSerialization
    {
        public static bool BlockSaveToFileActivity;
        
        // Creating a path
        public static readonly string Path = Application.persistentDataPath + "/Data";
        // Default data file name
        private static readonly string DataFileName = "SeaData";
        // Create full path
        public static string FilePath => Path + "/" + DataFileName;

        // Load data by given path
        public static SerializableData LoadDataFromFile(string path)
        {
            CreatePath(Path);
            
            if (!File.Exists(path)) 
                return null;
            
            SerializableData file;
            
            using (var sv = new StreamReader(path))
            {
                var data = sv.ReadToEnd();
                file = JsonConvert.DeserializeObject<SerializableData>(data);
            }

            return file;
        }

        // Save given data to file
        public static void SaveDataToFile(SerializableData data)
        {
            if (BlockSaveToFileActivity)
                return;
            
            CreatePath(Path);
            SaveDataToFile(data, FilePath);
        }

        // Save data to file by given path
        public static void SaveDataToFile(SerializableData data, string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            var serializeObject = JsonConvert.SerializeObject(data);

            using (var sv = new StreamWriter(filePath))
                sv.WriteLine(serializeObject);
        }
        
        // Create path
        public static void CreatePath(string currentPath)
        {
            var exists = Directory.Exists(currentPath);
            if (!exists)
                Directory.CreateDirectory(currentPath);
        }
    }
}
