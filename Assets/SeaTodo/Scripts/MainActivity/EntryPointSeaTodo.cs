using System;
using AppSettings.StatusBarSettings;
using Architecture;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Pages;
using Architecture.SettingsArea;
using Architecture.TextHolder;
using HomeTools.HPrefs;
using HomeTools.Source.Calendar;
using HTools;
using MainActivity;
using Theming;
using UnityEngine.EventSystems;

// Entry point of app
public class EntryPointSeaTodo : EntryPoint
{
    protected override void InitComponents()
    {
        LanguageSettings.TryInitialize(); // Initialize language
        TextHolderTime.UpdateByLanguage(); // Update text by language
        CalendarNames.UpdateByLanguage(); // Update calendar names by language
        FlowSymbols.UpdateByLanguage(); // Update tasks measurement by language
        // Create input component
        SyncWithBehaviour.Instance.AddObserver(new InputSyncBehaviour());
        // Create pages system
        SyncWithBehaviour.Instance.AddObserver(PageScroll.Instance);
        // Create component for custom serialization
        MainCanvas.RectTransform.gameObject.AddComponent<HPrefsApplicationLooker>();
        // Initialize application data
        AppData.Initialize();
        // Setup status bar
        StatusBarSettings.Setup(true);
        // Create main systems of app
        MainCanvas.RectTransform.gameObject.AddComponent<MainSystems>();
        // Update modules of event system
        EventSystem.current.UpdateModules();
        // Create component for app device actions
        MainCanvas.RectTransform.gameObject.AddComponent<AppStateInformer>();
        // Colorize other UI elements of app
        AppTheming.UpdateOtherElements();
    }
}





