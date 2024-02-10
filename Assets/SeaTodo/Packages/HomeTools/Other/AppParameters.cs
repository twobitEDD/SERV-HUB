using UnityEngine;

namespace HomeTools
{
    // Class for check device type
    public static class AppParameters
    {
        public static bool Device
        {
            get
            {
#if UNITY_EDITOR
                return false;
#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
                return true;
#else
                return false;
#endif
            }
        }

        public static bool Android => Application.platform == RuntimePlatform.Android;
        
        public static bool Ios
        {
            get
            {
#if UNITY_IOS || UNITY_IPHONE
                return true;
#endif
                return false;
            }
        }
 
        public static bool LoadedFromLogo;
    }
}
