using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Modules.CSVCreator;
using UnityEngine;

namespace Architecture.Data.Components
{
    // Serializing Application Data Backup
    public static class AppBackupSerialization
    {
        // Creating a path (for the android platform)
        private static string LocalPath => Application.persistentDataPath.Substring(0,
            Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));
        
        // Save file extension
        public const string BackupFormat = ".sea"; 
        // Folder for files with saving
        private const string folderNameBackups = "SeaTodo/Backups";
        // File folder with saving for export
        private const string folderNameExport = "SeaTodo/Export";
        // Constructing the full path of the save folder
        private static string BackupsPath =>  $"{LocalPath}/{folderNameBackups}";
        
        // Checking for file existence
        public static bool FileExists(string fileName)
        {
            var fullPathToBackup = $"{LocalPath}/{folderNameBackups}/{fileName}";
            AppDataSerialization.CreatePath(BackupsPath);
            
            return File.Exists(fullPathToBackup);
        }
        
        // Attempting to delete a file at the given path
        public static void RemoveFile(string fileName)
        {
            var fullPathToBackup = $"{BackupsPath}/{fileName}";
            
            if (FileExists(fileName))
                File.Delete(fullPathToBackup);
        }
        
        // File creation
        public static void CreateFile(string fileName)
        {
            var fullPathToBackup = $"{BackupsPath}/{fileName}";
            AppDataSerialization.CreatePath(BackupsPath);
            AppData.Save(fullPathToBackup);
        }

        // Get full path with file
        public static string GetPathToFile(string fileName) =>
            $"{LocalPath}/{folderNameBackups}/{fileName}";

        // Create csv data file
        public static void CreateCsv(string fileName)
        {
            var exportPath = $"{LocalPath}/{folderNameExport}";
            var fullPathToCsv = $"{exportPath}/{fileName}";
            AppDataSerialization.CreatePath(exportPath);
            CsvBuilder.GenerateNewCsvToPath(fullPathToCsv);
        }

        // Load backups files paths
        public static List<string> LoadBackups()
        {
            var result = new List<string>();
            
            PermissionWriteAndRead.TryAskPermissions();
            
            if (!PermissionWriteAndRead.HasPermissions)
                return result;
            
            AppDataSerialization.CreatePath(BackupsPath);
            
            var backupDirectory = new DirectoryInfo(BackupsPath);
            var fileInfo = backupDirectory.GetFiles();
            result.AddRange(fileInfo.Select(info => info.Name));

            return result;
        }
    }
}
