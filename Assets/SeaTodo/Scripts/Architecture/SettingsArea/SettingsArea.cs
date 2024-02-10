using Architecture.Elements;
using Architecture.Pages;
using Architecture.SettingsArea.ChooseItems;
using HomeTools.Messenger;
using HTools;
using Theming;

namespace Architecture.SettingsArea
{
    // Class of settings page
    public class SettingsArea : IBehaviorSync
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        // Link to main page
        private WorkArea.WorkArea WorkArea() => AreasLocator.Instance.WorkArea;
        
        private readonly MainPart mainPart; // Main settings part
        private readonly DataPart dataPart; // Data settings part
        private readonly OtherPart otherPart; // Part with additional views 
        // Component of panel for choose text item from list
        private readonly ChooseItemsPanel chooseItemsPanel;

        // Initialized page flag
        private bool started;

        // Create parts
        public SettingsArea()
        {
            // Create panel
            chooseItemsPanel = new ChooseItemsPanel();
            
            // Create parts
            mainPart = new MainPart();
            dataPart = new DataPart(mainPart.GetBottomSide);
            otherPart = new OtherPart(dataPart.GetBottomSide);
        }
        
        // Setup this page to page transitions system
        public void SetupToPage()
        {
            // Update page views
            mainPart.SetPartToNewPage();
            dataPart.SetPartToNewPage();
            otherPart.SetPartToNewPage();

            // Set activity of page views
            mainPart.SetActive(true);
            dataPart.SetActive(true);
            otherPart.SetActive(true);

            // Prepare page position and page scroll borders
            UpdateVisibleAnchors();
        }

        // Setup parts to page object
        public void SetupParents()
        {
            // Get page object
            var newPage = PageTransition().CurrentPage();
            
            // Set parts to new page
            mainPart.SetToPage(newPage);
            dataPart.SetToPage(newPage);
            otherPart.SetToPage(newPage);

            // Deactivate parts of page
            mainPart.SetActive(false);
            dataPart.SetActive(false);
            otherPart.SetActive(false);
        }

        // Setup parts from page 
        public void SetupFromPage()
        {
            // Update scroll borders of main page
            WorkArea().UpdateAnchorsForScroll();
            
            // Play animation - move from screen
            mainPart.SetPartFromPage();
            dataPart.SetPartFromPage();
            otherPart.SetPartFromPage();
        }

        // Setup parts of page to page pool
        public void SetupToPool()
        {
            var newPage = PageTransition().PagePool;
            mainPart.SetToPage(newPage);
            dataPart.SetToPage(newPage);
            otherPart.SetToPage(newPage);
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
            
            mainPart.SetupMovableParameters();
            mainPart.InitializeContent(chooseItemsPanel);
            
            dataPart.SetupMovableParameters();
            dataPart.InitializeContent(chooseItemsPanel);
            
            otherPart.SetupMovableParameters();
            otherPart.InitializeContent();
            
            // Setup colors of UI in this page
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.Settings);
            MainMessenger.AddMember(UpdateVisibleAnchors, AppMessagesConst.PurchaseUpdated);

            started = true;
        }

        // Update positions of parts in page
        private void UpdateVisibleAnchors()
        {
            UpdateBuyAllVisible();
            UpdateAnchorsForScroll();
        }

        // Refresh the page with a buy button on the page
        private void UpdateBuyAllVisible()
        {
            mainPart.UpdateAnchoredPosition();
            dataPart.UpdateAnchorPosition(mainPart.GetBottomSide);
            otherPart.UpdateAnchorPosition(dataPart.GetBottomSide);
        }
        
        // Prepare page position and page scroll borders
        private void UpdateAnchorsForScroll()
        {
            var newPage = PageTransition().CurrentPage();
            var max = -otherPart.GetBottomSide - MainCanvas.RectTransform.sizeDelta.y + AppElementsInfo.DistanceContentToBottom;

            if (max < 0) max = 0f;

            newPage.MaxAnchor = max;
            newPage.MinAnchor = 0;
        }
    }
}
