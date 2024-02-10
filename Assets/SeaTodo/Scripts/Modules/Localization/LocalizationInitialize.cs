using Modules.Localization;
using UnityEngine;

public class LocalizationInitialize : MonoBehaviour
{
    public const string languageKey = "LanguageFlow";

    private static bool activated;
    private void Awake()
    {
        if (activated)
            return;

        if (!PlayerPrefs.HasKey(languageKey))
            AutoDetect();

        LocalizationManager.SetupCurrentLanguage(PlayerPrefs.GetString(languageKey));
        
        activated = true;
    }

    // Detect language by device
    private void AutoDetect()
    {
        if (Application.systemLanguage == SystemLanguage.English)
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.English.ToString());
        
        if (Application.systemLanguage == SystemLanguage.Russian)
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.Russian.ToString());
        
        if (Application.systemLanguage == SystemLanguage.French)
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.French.ToString());
        
        if (Application.systemLanguage == SystemLanguage.German)
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.German.ToString());
        
        if (Application.systemLanguage == SystemLanguage.Italian)
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.Italian.ToString());
        
        if (Application.systemLanguage == SystemLanguage.Portuguese)
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.Portuguese.ToString());
        
        if (Application.systemLanguage == SystemLanguage.Spanish)
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.Spanish.ToString());
        
        if (Application.systemLanguage == SystemLanguage.Turkish)
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.Turkish.ToString());
        
        if (!PlayerPrefs.HasKey(languageKey))
            PlayerPrefs.SetString(languageKey, LocalizationObject.Language.English.ToString());
    }
}
