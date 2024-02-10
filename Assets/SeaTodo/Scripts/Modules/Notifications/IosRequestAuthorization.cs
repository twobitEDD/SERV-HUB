using System.Collections;
using HTools;
using UnityEngine;
#if UNITY_IOS || UNITY_IPHONE
using Unity.Notifications.iOS;
#endif

namespace Modules.Notifications
{
    // Class for ask permissions for send notifications on iOS
    public class IosRequestAuthorization : IBehaviorSync
    {
        private bool inProcess;
        private IEnumerator process;

        public void Start() { }

        public void Update()
        {
            if (inProcess)
                process.MoveNext();
        }

        public void Ask()
        {
            inProcess = true;
            process = RequestAuthorization().GetEnumerator();
        }

        private IEnumerable RequestAuthorization()
        {
#if UNITY_IOS || UNITY_IPHONE
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            
            using (var req = new AuthorizationRequest(authorizationOption, true))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                };

                var res = "\n RequestAuthorization:";
                res += "\n finished: " + req.IsFinished;
                res += "\n granted :  " + req.Granted;
                res += "\n error:  " + req.Error;
                res += "\n deviceToken:  " + req.DeviceToken;
                Debug.Log(res);

                inProcess = false;
            }
#endif
            yield return null;
            Debug.Log("asked success");
            inProcess = false;
        }
    }
}