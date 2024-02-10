using System;
using UnityEngine;

namespace Architecture.Other
{
    // Class for loading task icons
    public static class FlowIconLoader
    {
        private const string resourcesPath = "FlowIcons/";
        private const int groupLength = 4;
        
        // The names are the same as the names of the folders in the hierarchy
        private enum FlowName
        {
            Part1 = 1,
            Part2,
            Part3,
            Part4,
        }
        
        public static Sprite LoadIconById(int id)
        {
            id = Mathf.Clamp(id, 0, FlowIconsCount());
            var groupId = id / groupLength;
            var iconId = id - groupLength * groupId + 1;
            var flowGroup = (FlowName) groupId + 1;
            return UnityEngine.Object.Instantiate(Resources.Load<Sprite>($"{resourcesPath}{flowGroup}/icon {iconId}"));
        }

        public static int FlowIconsCount() => Enum.GetNames(typeof(FlowName)).Length * groupLength;
    }
}
