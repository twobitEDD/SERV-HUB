using System;
using System.Collections.Generic;
using Architecture.DaysMarkerArea.DaysView;
using UnityEngine;

namespace Architecture.SettingsArea.LoadBackupModule
{
    // Empty session
    public class LoadBackupSession
    {
        private readonly Action closeAction;

        public LoadBackupSession(Action closeAction)
        {
            this.closeAction = closeAction;
        }
    }
}
