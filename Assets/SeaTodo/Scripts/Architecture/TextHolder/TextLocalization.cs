using System.Collections.Generic;
using UnityEngine.UI;

namespace Architecture.TextHolder
{
    // Module for text localization that works with I2 Languages package
    public class TextLocalization
    {
        // Dictionary of text components and localization keys
        private readonly Dictionary<Text, string> localizationList = new Dictionary<Text, string>();
        
        // Singleton
        private static TextLocalization instance;
        public static TextLocalization Instance
        {
            get
            {
                if (instance == null)
                    return instance = new TextLocalization();

                return instance;
            }
        }

        // Add text and localization key to module and localize
        public void AddLocalization(Text text, string key)
        {
            if (localizationList.ContainsKey(text))
                localizationList[text] = key;
            else
                localizationList.Add(text, key);
            
            UpdateLocalizationText(text, key);
        }
        
        // Get localized text by key
        public static string GetLocalization(string key) => LocalizationManager.GetTranslation(key);

        // Update all texts with localization
        public void UpdateElements()
        {
            foreach (var item in localizationList)
                UpdateLocalizationText(item.Key, item.Value);
        }

        // Update localization of text
        private void UpdateLocalizationText(Text text, string key)
        {
            text.text = LocalizationManager.GetTranslation(key);
            TextLocalizationColorize.ColorizeFilter(text);
        }
    }
}
