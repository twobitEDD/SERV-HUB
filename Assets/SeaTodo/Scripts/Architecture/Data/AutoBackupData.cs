using Architecture.CalendarModule;
using Architecture.Data.Components;

namespace Architecture.Data
{
    // Class for automatically saving application data
    public static class AutoBackupData
    {
        public const string BackupFileName = "SeaBackup"; // Save file prefix
        
        // Try create backup
        public static void TryCreateAutoBackup()
        {
            // Check permissions to save file
            if (!PermissionWriteAndRead.HasPermissions)
                return;

            // Check if auto backup turned on
            if (!AppCurrentSettings.AutoBackupOn)
                return;
            
            // Get today date
            var today = Calendar.Today;
            
            // Check if should to create backup file
            if (today.DayOfWeek != AppCurrentSettings.AutoBackup)
                return;

            // Build backup file name
            var backupName = $"{BackupFileName} {today.Day}.{today.Month}.{today.Year}{AppBackupSerialization.BackupFormat}";
            // Get old backup file name
            var oldBackupName = AppCurrentSettings.LastAutoBackup;
            
            // Check names
            if (backupName == oldBackupName)
                return;
            
            // Try to remove old auto backup file 
            if (oldBackupName != string.Empty && AppBackupSerialization.FileExists(oldBackupName))
                AppBackupSerialization.RemoveFile(oldBackupName);

            // Create new backup file
            AppBackupSerialization.CreateFile(backupName);
            
            // Build csv file for share
            var backupNameCsv = $"{BackupFileName} {today.Day}.{today.Month}.{today.Year}.csv";
            AppBackupSerialization.CreateCsv(backupNameCsv);
            
            // Save of last auto backup file name
            AppCurrentSettings.LastAutoBackup = backupName;
        }
    }
}
