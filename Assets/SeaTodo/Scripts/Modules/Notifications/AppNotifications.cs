using Architecture.Elements;
using HomeTools;

namespace Modules.Notifications
{
    // Component for invoke update notifications
    public static class AppNotifications
    {
        public static bool ShouldUpdate;

        // Update notifications
        public static void UpdateNotifications(bool overlay = false)
        {
            if (!ShouldUpdate && !(overlay && AppParameters.Ios))
                return;

#if UNITY_ANDROID
            AndroidNotificationsBuilder.RebuildNotifications();
#elif UNITY_IOS || UNITY_IPHONE
            IosNotificationsBuilder.RebuildNotifications();            
#endif
            ShouldUpdate = false;
        }
    }
}
