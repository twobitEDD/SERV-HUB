using System;
using System.Collections.Generic;
using System.Linq;
using Architecture;
using Architecture.Data;
using HomeTools.Input;
using HomeTools.Messenger;
using InternalTheming;
using Modules.ColorTheming.ThemeItems;
using UnityEngine;
using UnityEngine.UI;

namespace Theming
{
    // Main component of app theming module
    public static class AppTheming
    {
        // UI elements in packages
        private static readonly Dictionary<AppItem, IThemeItem> themeItems = new Dictionary<AppItem,IThemeItem>();
        // Link to current app theme colors
        private static ITheme theme;
        // Package of UI for additional elements and components
        private static readonly GraphicsPackage graphicsPackage;
        // List of input fields for colorize
        private static readonly List<HInputField> inputFields = new List<HInputField>();

        // Check if dark theme
        public static bool DarkTheme => AppCurrentSettings.DarkTheme;

        // Marker for UI group
        public enum AppItem
        {
            WorkArea = 0,
            TrackArea = 1,
            MenuArea = 2,
            CreateFlowArea = 3,
            Other = 4,
            RemindersModule = 5,
            TimeTrackModule = 6,
            FlowViewArea = 7,
            StatisticsArea = 8,
            DaysMarkerArea = 9,
            Tutorial = 10,
            Settings = 11,
            AboutSeaCalendar = 12,
        }

        // Create module
        static AppTheming()
        {
            foreach (AppItem item in Enum.GetValues(typeof(AppItem)))
                themeItems.Add(item, new ThemeItem());
            
            graphicsPackage = new GraphicsPackage();
        }

        // Add additional graphics element
        public static void AddGraphics(GraphicImageItem imageItem) => graphicsPackage.AddImage(imageItem);
        // Add additional graphics element
        public static void AddGraphics(GraphicImageMaterialItem imageItem) => graphicsPackage.AddImage(imageItem);
        // Add input field
        public static void AddInputField(HInputField hInputField)
        {
            if (inputFields.Contains(hInputField))
                return;
            
            inputFields.Add(hInputField);
            hInputField.UpdateSelectionColor();
        }

        // Update colors of UI group
        public static void ColorizeThemeItem(AppItem appItem)
        {
            SetupTheme();
            
            var themeItem = themeItems[appItem];
            foreach (var (graphic, themeColor) in themeItem.GetGraphicsItems())
                graphic.color = ColorConverter.GetColor(theme, themeColor);
            
            MainMessenger.SendMessage(string.Format(AppMessagesConst.ColorizedArea, appItem));
        }
        
        // Update colors of UI group and keep alpha channels without update
        private static void ColorizeThemeItemSaveAlpha(AppItem appItem)
        {
            SetupTheme();

            var themeItem = themeItems[appItem];
            foreach (var (graphic, themeColor) in themeItem.GetGraphicsItems())
            {
                var alpha = graphic.color.a;
                var color = ColorConverter.GetColor(theme, themeColor);
                color.a = alpha;
                graphic.color = color;
            }

            MainMessenger.SendMessage(string.Format(AppMessagesConst.ColorizedArea, appItem));
        }
        
        // Setup color of UI element
        public static void ColorizeElement(Graphic element, ColorTheme colorTheme)
        {
            SetupTheme();
            element.color = ColorConverter.GetColor(theme, colorTheme);
        }

        // Add UI element to module
        public static void AddElement(Graphic element, ColorTheme colorTheme, AppItem appItem) =>
            themeItems[appItem].AddItem(element, colorTheme);
        
        // Remove UI element from module
        public static void RemoveElement(Graphic element, ColorTheme colorTheme, AppItem appItem) =>
            themeItems[appItem].RemoveItem(element, colorTheme);
        
        // Setup color of navigation bar
        public static void SetUpNavigationBar()
        {
            SetupTheme();
            ApplicationChrome.navigationBarColor = ApplicationChrome.ToUint(theme.DefaultBackgroundColor);
        }

        // Update app theme and colorize
        public static void UpdateTheme(ThemeLoader.ThemeType theme)
        {
            ThemeLoader.SaveNewTheme(theme);
            AppTheming.theme = ThemeLoader.GetCurrentTheme();

            foreach (AppItem item in Enum.GetValues(typeof(AppItem)))
                ColorizeThemeItemSaveAlpha(item);

            SetUpNavigationBar();
            UpdateOtherElements();
            graphicsPackage.UpdateGraphics();

            foreach (var inputField in inputFields.Where(inputField => inputField != null))
                inputField.UpdateSelectionColor();
        }

        // Setup actual app theme
        private static void SetupTheme()
        {
            if (theme == null)
                theme = ThemeLoader.GetCurrentTheme();
        }

        // Update other elements (cameras background and etc.)
        public static void UpdateOtherElements()
        {
            SetupTheme();

            foreach (var camera in Camera.allCameras)
                camera.backgroundColor = theme.DefaultBackgroundColor;
        }
    }
}
