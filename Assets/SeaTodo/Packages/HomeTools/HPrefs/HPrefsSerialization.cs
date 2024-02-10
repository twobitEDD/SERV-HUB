using System.Collections.Generic;
using System.IO;
using HomeTools.Other;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace HomeTools.HPrefs
{
    // Class for serialization fields
    public static class HPrefsSerialization
    {
        // Path and file name
        private static readonly string path = Application.persistentDataPath + "/Data";
        private const string dataFileName = "data";

        // Data for serialization
        public static Dictionary<string, string> StringData = new Dictionary<string, string>();
        public static Dictionary<string, int> IntData = new Dictionary<string, int>();
        public static Dictionary<string, float> FloatData = new Dictionary<string, float>();

        // Full path
        private static string FilePath => path + "/" + dataFileName;

        // Load data from file
        public static void LoadDataFromFile(string filePath = "default", string folderPath = "default")
        {
            if (filePath == "default")
            {
                filePath = FilePath;
                CreatePath(path);
            }
            else
            {
                CreatePath(folderPath);
            }
            
            if (File.Exists(filePath))
            {
                using (var sv = new StreamReader(filePath))
                {
                    var data = OtherHTools.DecodingFromBase64(sv.ReadToEnd());
                    var readToEndString = data.Split('*');
                    StringData = JsonConvert.DeserializeObject<Dictionary<string, string>>(readToEndString[0]);
                    IntData = JsonConvert.DeserializeObject<Dictionary<string, int>>(readToEndString[1]);
                    FloatData = JsonConvert.DeserializeObject<Dictionary<string, float>>(readToEndString[2]);
                }
            }

            if (StringData.Count == 0)
            {
                StringData = new Dictionary<string, string>();
            }

            if (IntData.Count == 0)
            {
                IntData = new Dictionary<string, int>();
            }

            if (FloatData.Count == 0)
            {
                FloatData = new Dictionary<string, float>();
            }
        }

        // Save data to file
        public static void SaveDataToFile(string filePath = "default", string folderPath = "default")
        {
            if (filePath == "default")
            {
                filePath = FilePath;
                CreatePath(path);
            }
            else
            {
                CreatePath(folderPath);
            }
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var serializeObject = JsonConvert.SerializeObject(StringData)
                                  + "*" + JsonConvert.SerializeObject(IntData)
                                  + "*" + JsonConvert.SerializeObject(FloatData);

            using (var sv = new StreamWriter(filePath))
            {

                sv.WriteLine(OtherHTools.CodingToBase64(serializeObject));
            }
        }

        // Try to create directory for file
        private static void CreatePath(string path)
        {
            bool exists = Directory.Exists(path);
            if (!exists)
                Directory.CreateDirectory(path);
        }
    }
}
