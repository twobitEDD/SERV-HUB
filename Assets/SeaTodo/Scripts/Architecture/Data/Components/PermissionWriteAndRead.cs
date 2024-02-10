using HomeTools;
using UnityEngine.Android;

namespace Architecture.Data.Components
{
    // Requesting permission to read and create files
    public static class PermissionWriteAndRead
    {
        // Permission check
        public static bool HasPermissions
        {
            get
            {
                if (AppParameters.Android)
                {
                    return Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) &&
                           Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
                }

                return false;
            }
        }

        // Access request
        public static void TryAskPermissions()
        {
            if (HasPermissions)
                return;
            
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
        }
    }
}
