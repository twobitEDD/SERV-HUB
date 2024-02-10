using System;
using System.Linq;
using Architecture.TextHolder;
using Modules.Localization;

namespace Architecture.SettingsArea
{
    public static class LanguageSettings
    {
        public static string[] LanguagesArray { get; private set; }
        public static string CurrentLanguage => LocalizationManager.CurrentLanguage;
        private static bool initialized;

        public static void TryInitialize()
        {
            if (initialized)
                return;

            var variable = LocalizationObject.Instance;
            var languages = LocalizationManager.Languages.Values;
            LanguagesArray = languages.ToArray();
            initialized = true;
        }

        public static void SetupLanguage(string language)
        {
            TryInitialize();
            
            if (!Array.Exists(LanguagesArray, e=>e == language))
                return;

            LocalizationManager.CurrentLanguage = language;
            TextLocalization.Instance.UpdateElements();
        }
    }
}
