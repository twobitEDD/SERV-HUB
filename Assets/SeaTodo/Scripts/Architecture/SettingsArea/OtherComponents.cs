using Architecture.TextHolder;
using HomeTools;
using UnityEngine;
#if UNITY_IOS || UNITY_IPHONE
using UnityEngine.iOS;
#endif

namespace Architecture.SettingsArea
{
    public static class OtherComponents
    {
        private static string LinkToGooglePlay => PublicLinks.Instance.linkToGooglePlay;
        private static readonly string HomeSite = PublicLinks.Instance.linkToConnections;
        
        public static void ReviewApp()
        {
            if (AppParameters.Android)
            {
                Application.OpenURL(LinkToGooglePlay);
            }
            else if (AppParameters.Ios)
            {
#if UNITY_IOS || UNITY_IPHONE
                Device.RequestStoreReview();
#endif
            }
        }

        public static void OpenSite() => Application.OpenURL(HomeSite);
    }
}
