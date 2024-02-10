using UnityEngine;

namespace Architecture.Other
{
    // Class for loading main page title icons
    public static class GroupIconLoader
    {
        private const string resourcesPathFolder = "GroupIcons";
        private const int groupLength = 8;

        public static Sprite LoadIconById(int id)
        {
            id = Mathf.Clamp(id, 0, IconsCount());
            return Object.Instantiate(Resources.Load<Sprite>($"{resourcesPathFolder}/icon {id}"));
        }

        public static int IconsCount() => groupLength;
    }
}
