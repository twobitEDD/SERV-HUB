using Architecture.Elements;
using Architecture.TrackArea;
using HomeTools;
using Modules.Notifications;
using UnityEngine;

namespace MainActivity
{
    // Component for additional device actions 
    public class AppStateInformer : MonoBehaviour
    {
        // Link to app bar
        private AppBar.AppBar AppBar => AreasLocator.Instance.AppBar;
        // Link to track area
        private TrackArea TrackArea => AreasLocator.Instance.TrackArea;
        
        private bool homeTouched; // Home button of device touched
        
        // Application focus updated
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
             //   AppNotifications.UpdateNotifications();
            }
        }

        // Application pause updated
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                 AppNotifications.UpdateNotifications(true);
            }
        }

        // Application quit actions
        private void OnApplicationQuit()
        {
             AppNotifications.UpdateNotifications(true);
        }

        // Check for home button touches
        private void Update()
        {
            if (Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Menu))
            {
                HomeButtonTouched();
            }
            else
            {
                homeTouched = false;
            }
        }

        // Invoke actions when touched home button
        private void HomeButtonTouched()
        {
            if (homeTouched)
                return;
            
            homeTouched = true;
            
            if (AppBar?.RightButtonActionsQueue == null)
                return;

            if (AppBar.RightButtonActionsQueue.HasActions)
            {
                AppBar.RightButtonActionsQueue.InvokeLastAction();
                return;
            }

            if (TrackArea.HasSession)
            {
                TrackArea.Close();
                return;
            }

            if (AppBar.TryCloseMenu())
                return;
            
            if (AppBar.TryMoveBackByAreas())
                return;

            AppToBack();
        }

        private void AppToBack()
        {
            if (AppParameters.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                activity.Call<bool>("moveTaskToBack", true);
            }
        }
    }
}
