using System;

namespace Architecture.SettingsArea.CreateBackupModule
{
    // Component of create backup session
    public class CreateBackupSession
    {
        private readonly Action<CreateBackupSession> updateAction; // Update backup action
        public bool Created; // Created flag
        public string FileName; // Backup name
        public string FilePath; // Backup path

        // Create
        public CreateBackupSession(Action<CreateBackupSession> updateAction)
        {
            this.updateAction = updateAction;
        }
        
        // Finish session
        public void UpdateAction()
        {
            updateAction?.Invoke(this);
        }
    }
}
