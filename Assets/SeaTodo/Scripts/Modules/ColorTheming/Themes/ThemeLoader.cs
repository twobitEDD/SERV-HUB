using Architecture.Data;

namespace InternalTheming
{
    // Component for load theme by settings data
    public static class ThemeLoader
    {
        public enum ThemeType
        {
            White,
            Dark
        }
        
        // Link to themes
        private static readonly ITheme themeWhite;
        private static readonly ITheme themeDark;
        
        // Create app themes
        static ThemeLoader()
        {
            themeWhite = new ColorsThemeWhite();
            themeDark = new ColorsThemeDark();
        }
        
        // Get current app theme
        public static ITheme GetCurrentTheme() => AppCurrentSettings.DarkTheme ? themeDark : themeWhite;

        // Save app theme
        public static void SaveNewTheme(ThemeType theme) => AppCurrentSettings.DarkTheme = theme == ThemeType.Dark;
    }
}
