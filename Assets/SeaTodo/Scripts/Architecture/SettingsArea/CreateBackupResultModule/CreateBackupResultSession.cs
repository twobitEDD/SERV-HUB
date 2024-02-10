using System;

namespace Architecture.SettingsArea.CreateBackupResultModule
{
    // Component of backup result session
    public class CreateBackupResultSession
    {
        public readonly bool Created;
        public readonly string Name;
        public readonly string Path;

        // Create
        public CreateBackupResultSession(bool created, string name, string path)
        {
            Created = created;
            Name = name;
            Path = path;
        }
    }
}
