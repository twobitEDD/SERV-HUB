using Architecture.Elements;
using Architecture.Pages;
using HTools;
using Theming;

namespace Architecture.DaysMarkerArea
{
    // Main class of Sea Calendar page
    public class DaysMarkerArea : IBehaviorSync
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Link to main page
        private WorkArea.WorkArea WorkArea() => AreasLocator.Instance.WorkArea;

        private readonly DaysViewPart daysPart; // Part with days view
        private readonly ColorsPart colorsPart; // Part with characteristics of days
        
        // Flag for open this page on start of scene for optimization
        public bool OpenByOptimization;
        
        // Created page flag
        private bool started; 

        // Create main components
        public DaysMarkerArea()
        {
            // Create part with days view
            daysPart = new DaysViewPart(); 
            // Create part with days characteristics
            colorsPart = new ColorsPart(daysPart.GetBottomSide); 
        }
        
        // Setup this page to page transitions system
        public void SetupToPage()
        {
            // Start animation of parts of page
            daysPart.SetPartToNewPage();
            colorsPart.SetPartToNewPage();
            
            // Activate parts of page
            daysPart.SetActive(true);
            colorsPart.SetActive(true);
            
            // Prepare page position and page scroll borders
            var newPage = PageTransition().CurrentPage();
            var max = -colorsPart.GetBottomSide - MainCanvas.RectTransform.sizeDelta.y + AppElementsInfo.DistanceContentToBottom;

            if (max < 0) max = 0f;

            newPage.MaxAnchor = max;
            newPage.MinAnchor = 0;

            // If it isn`t temp run of method for optimization than
            // allow open tip "About Sea Calendar"
            if (OpenByOptimization)
                daysPart.WaitToTryOpenTip = false;
        }

        // Setup parts to page object
        public void SetupParents()
        {
            // Get page object
            var newPage = PageTransition().CurrentPage();
            // Set parts to new page
            daysPart.SetToPage(newPage);
            colorsPart.SetToPage(newPage);
            // Deactivate parts of page 
            daysPart.SetActive(false);
            colorsPart.SetActive(false);
        }

        // Setup parts from page 
        public void SetupFromPage()
        {
            // Update scroll borders of main page
            WorkArea().UpdateAnchorsForScroll();
            
            // Play animation - move from screen
            daysPart.SetPartFromPage();
            colorsPart.SetPartFromPage();
        }

        // Setup parts of page to page pool
        public void SetupToPool()
        {
            var newPage = PageTransition().PagePool;
            daysPart.SetToPage(newPage);
            colorsPart.SetToPage(newPage);
        }

        public void Start()
        {
        }

        public void Update()
        {
            LateStart();
        }

        // Initialize parts of page
        private void LateStart()
        {
            if (started)
                return;
            
            daysPart.SetupMovableParameters();
            daysPart.InitializeContent();
            colorsPart.SetupMovableParameters();
            colorsPart.InitializeContent();
            
            // Setup colors of UI in this page
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.DaysMarkerArea);

            started = true;
        }
    }
}
