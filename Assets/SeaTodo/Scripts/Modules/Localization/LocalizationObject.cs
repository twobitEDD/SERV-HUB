using System;
using System.Linq;
using UnityEngine;

namespace Modules.Localization
{
    public class LocalizationObject : ScriptableObject
    {
        [Serializable]
        public class LocalizationItem
        {
            public Language Language;
            public string Translate;
        }
    
        [Serializable]
        public class LocalizationTerm
        {
            public string Key;
            public LocalizationItem[] Items;
        }
    
        public enum Language
        {
            English,
            Russian,
            French,
            German,
            Italian,
            Portuguese,
            Spanish,
            Turkish
        }

        [SerializeField]
        public LocalizationTerm[] data;

        private static LocalizationObject instance;
        public static LocalizationObject Instance
        {
            get
            {
                if (!instance)
                    instance = Resources.FindObjectsOfTypeAll<LocalizationObject>().FirstOrDefault();
                
                return instance;
            }
        }

        public static void SetupInstanceCustom(LocalizationObject variable)
        {
            Debug.Log("setup");
            instance = variable;
        }
    }
}


