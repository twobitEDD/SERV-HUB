using Architecture.Pages;

namespace Architecture.TaskViewArea
{
    // Interface for task view modes
    public interface IFlowView
    {
        void SetupParents(PageItem newPage); // Setup parts to page object
        void SetupToPage(); // Setup this page to page transitions system
        void SetupFromPage(); // Setup parts from page 
        void SetupToPool(PageItem newPage); // Setup parts of page to page pool
        void LateStart(); // Initialize parts of page
        float GetBottomSide(); // Bottom position of part in page
    }
}
