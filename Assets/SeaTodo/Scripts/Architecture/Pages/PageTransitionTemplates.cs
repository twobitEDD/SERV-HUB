using System;
using Architecture.Elements;

namespace Architecture.Pages
{
    /// <summary>
    /// A class for adding a state queue
    /// for organizing a complete animation of the transition between pages
    /// </summary>
    public static class PageTransitionTemplates 
    {
        private static PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        
        // Open page from page 2 actions
        public static void OpenPageInsideSecond(Action actionOfCurrentPage, int delay, Action actionDeactivateCurrentPage, 
                                                                        Action actionBeforeNewPage, Action actionOfNewPage)
        {
            PageTransition().AddDisableFade();
            PageTransition().AddAction(actionOfCurrentPage);
            PageTransition().AddTimer(delay);
            PageTransition().AddToFade();
            PageTransition().AddTimer(5);
            PageTransition().AddAction(actionDeactivateCurrentPage);
            PageTransition().AddAction(actionBeforeNewPage);
            PageTransition().AddDisableFade();
            PageTransition().AddAction(actionOfNewPage);
        }

        // Open page from main page (page 1) actions
        public static void OpenPageFromWorkArea(Action actionBeforeNewPage, Action actionOfNewPage)
        {
            PageTransition().AddSwitchPages();
            PageTransition().AddDisableFade();
            PageTransition().AddAction(actionBeforeNewPage);
            PageTransition().AddToFade();
            PageTransition().AddTimer(5);
            PageTransition().AddDisableOldPage();
            PageTransition().AddEnableNewPage();
            PageTransition().AddDisableFade();
            PageTransition().AddAction(actionOfNewPage);
        }
        
        // Close page from page 2 to main page actions
        public static void ClosePageToWorkArea(Action actionOfCurrentPage, int delay, Action setupToPoolOld)
        {
            PageTransition().AddToFade();
            PageTransition().AddTimer(2);
            PageTransition().AddAction(actionOfCurrentPage);
            PageTransition().AddTimer(3);
            PageTransition().AddSwitchPages();
            PageTransition().AddTimer(delay);
            PageTransition().AddEnableFade();
            PageTransition().AddDisableOldPage();
            PageTransition().AddAction(setupToPoolOld);
            PageTransition().AddEnableNewPage();
            PageTransition().AddFromFade();
        }
    }
}
