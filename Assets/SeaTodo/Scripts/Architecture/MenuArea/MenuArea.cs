using System;
using Architecture.TrackArea;
using HomeTools.Messenger;
using HTools;
using Theming;

namespace Architecture.MenuArea
{
    // The main component responsible for the menu
    public class MenuArea : IDisposable
    {
        private readonly MainPart mainPart; // Component with buttons
        public readonly MenuAreaScroll MenuAreaScroll; // Scroll component of menu
        private readonly ScreenBlackout screenBlackout; // Dark screen under menu
        
        // Create and setup
        public MenuArea()
        {
            // Create dark screen UI
            screenBlackout = new ScreenBlackout("Menu Blackout");
            // Setup color to dark screen UI
            AppTheming.AddElement(screenBlackout.Image, ColorTheme.TrackAreaBlackout, AppTheming.AppItem.MenuArea);
            // Setting the message sent when user touch the screen
            screenBlackout.MessageBlackoutClicked = AppMessagesConst.BlackoutMenuClicked;
            // Create component with menu buttons
            mainPart = new MainPart(screenBlackout);
            // Colorize parts that marked as MenuArea
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.MenuArea);
            // Initialize dark screen object
            screenBlackout.PrepareBlackout();
            // Create menu scroll component
            MenuAreaScroll = new MenuAreaScroll(screenBlackout, mainPart);
            SyncWithBehaviour.Instance.AddObserver(MenuAreaScroll);
            // Setup menu scroll component
            MenuAreaScroll.SetHandleObjects(mainPart.GetHandleObjects());
            MenuAreaScroll.SetHandleBlackout(screenBlackout.HandleObject);
        }

        // Called to open a menu
        public void Open()
        {
            // Reset state for menu
            mainPart.ActualizeState();
            // Start auto open menu process
            MenuAreaScroll.AutoOpen();
            // Send messages
            MainMessenger.SendMessage(AppMessagesConst.BlackoutTrackClicked);
            MainMessenger.SendMessage(AppMessagesConst.MenuStartedShow);
        }
        
        // Called to close a menu
        public void Close()
        {
            // Start auto close menu process
            MenuAreaScroll.AutoClose();
        }
        
        public void Dispose()
        {
            mainPart.Dispose();
            screenBlackout.Dispose();
        }
    }
}
