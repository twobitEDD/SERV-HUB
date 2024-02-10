using UnityEngine;

namespace AppSettings.StatusBarSettings
{
    // Tools for android navigation bar
    public static class AndroidBarSettings
    {
        public static int GetAndroidBarHeight()
        {
            var id = 0;
            var result = 0;

            using (var unityPlayer = new AndroidJavaClass("com.android.internal.R$dimen"))
                id = unityPlayer.GetStatic<int>("status_bar_height");

            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (var window = activity.Call<AndroidJavaObject>("getResources"))
                result = window.Call<int>("getDimensionPixelSize", id);

            return result;
        }
    }
}
