using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Localization;
using UnityEngine;

public static class LocalizationManager
{
    private static LocalizationObject Data => LocalizationObject.Instance;
    
    public static readonly Dictionary<LocalizationObject.Language, string> Languages = new Dictionary<LocalizationObject.Language, string>()
    {
        {LocalizationObject.Language.English, "English"},
        {LocalizationObject.Language.French, "Français"},
        {LocalizationObject.Language.German, "Deutsche"},
        {LocalizationObject.Language.Italian, "Italiano"},
        {LocalizationObject.Language.Portuguese, "Português"},
        {LocalizationObject.Language.Russian, "Русский"},
        {LocalizationObject.Language.Spanish, "Español"},
        {LocalizationObject.Language.Turkish, "Türk"},
    };
    
    public static string GetTranslation(string key)
    {
        // if (Data.data == null)
        // {
        //     Debug.Log($"NO LOCALIZATION AT KEY: {key}");
        //     return string.Empty;
        // }

        if (!Array.Exists(Data.data, e => e.Key == key))
        {
            Debug.Log($"NO LOCALIZATION AT KEY: {key}");
            return string.Empty;
        }
        
        var item = Array.Find(Data.data, e => e.Key == key);

        if (!Array.Exists(item.Items, e => e.Language == language))
        {
            Debug.Log($"NO LOCALIZATION LANGUAGE AT KEY: {language}");
            return string.Empty;
        }

        var result = Array.Find(item.Items, e => e.Language == language);

        return result.Translate;
    }

    private static LocalizationObject.Language language;

    public static string CurrentLanguage
    {
        get => Languages[language];
        set
        {
            if (!Languages.ContainsValue(value))
                return;
            
            language = Languages.FirstOrDefault(x => x.Value == value).Key;
            PlayerPrefs.SetString(LocalizationInitialize.languageKey, language.ToString());
        }
    }

    public static void SetupCurrentLanguage(string englishLanguageVersion)
    {
        language = (LocalizationObject.Language) Enum.Parse(typeof(LocalizationObject.Language), englishLanguageVersion, true);
    }
}
