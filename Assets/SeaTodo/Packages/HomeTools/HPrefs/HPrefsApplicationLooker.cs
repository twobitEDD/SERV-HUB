using Architecture.Data;
using UnityEngine;

namespace HomeTools.HPrefs
{
    // Class that calls app serialization when state of app updates
    public class HPrefsApplicationLooker : MonoBehaviour
    {
        void Awake()
        {
            HPrefsSerialization.LoadDataFromFile();
            HPrefsSerialization.SaveDataToFile();
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                UpdateData();
        }

        void OnApplicationPause(bool pauseStatus) 
        {
            if (pauseStatus)
                UpdateData();
        }

        private void OnApplicationQuit() => UpdateData();

        private static void UpdateData()
        {
            AppData.Save();
            AutoBackupData.TryCreateAutoBackup();
            HPrefsSerialization.SaveDataToFile();
        }
    }
}
