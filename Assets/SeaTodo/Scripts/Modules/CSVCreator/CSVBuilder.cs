using System.IO;
using Architecture.Data;

namespace Modules.CSVCreator
{
    // Main component for generate CSV data file
    public static class CsvBuilder
    {
        // Data file name
        public static readonly string DataFileName = "SeaTodo.csv";
        // Path for file
        public static string FilePath => AppDataSerialization.Path + "/" + DataFileName;
        
        // Generate new file by default path
        public static void GenerateNewCsv()
        {
            AppDataSerialization.CreatePath(AppDataSerialization.Path);
            GenerateNewCsvToPath(FilePath);
        }

        // Generate new file by path
        public static void GenerateNewCsvToPath(string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            var serializeObject = CsvFormer.CreateCsvResources();

            using (var sv = new StreamWriter(path))
            {
                foreach (var line in serializeObject)
                    sv.WriteLine(line);
            }
        }
    }
}
