using Architecture.Pages;
using HTools;

namespace Architecture.TaskViewArea.NormalView
{
    // Part of normal task view
    public class ViewNormal : IFlowView
    {
        // Parts of view page
        private readonly PartProgress partProgress;
        private readonly PartStatistics partStatistics;
        private readonly PartEstimate partEstimate;
        private readonly PartReminders partReminders;
        private readonly DoneButton doneButton;
        private readonly RemoveButton removeButton;

        // Create and setup
        public ViewNormal(FlowViewArea flowViewArea)
        {
            // Create parts of page
            partProgress = new PartProgress();
            partEstimate = new PartEstimate(partProgress.GetBottomSide);
            partStatistics = new PartStatistics(partEstimate.GetBottomSide);
            partReminders = new PartReminders(partStatistics.GetBottomSide);
            
            doneButton = new DoneButton(partReminders.GetBottomSide, SetupFromPage, flowViewArea.MarkedAsCompleted);
            SyncWithBehaviour.Instance.AddObserver(doneButton);
            
            removeButton = new RemoveButton(doneButton.GetBottomSide, SetupFromPage);
            SyncWithBehaviour.Instance.AddObserver(removeButton);
        }

        // Setup parts to page object
        public void SetupParents(PageItem newPage)
        {
            partProgress.SetToPage(newPage);
            partProgress.SetActive(false);
            
            partStatistics.SetToPage(newPage);
            partStatistics.SetActive(false);
            
            partEstimate.SetToPage(newPage);
            partEstimate.SetActive(false);

            partReminders.SetToPage(newPage);
            partReminders.SetActive(false);
            
            doneButton.SetToPage(newPage);
            removeButton.SetToPage(newPage);
        }
        
        // Setup this page to page transitions system
        public void SetupToPage()
        {
            partProgress.SetPartToNewPage();
            partProgress.UpdateInfo();
            partProgress.SetActive(true);
            
            partStatistics.SetPartToNewPage();
            partStatistics.UpdateInfo();
            partStatistics.SetActive(true);

            partEstimate.SetPartToNewPage();
            partEstimate.UpdateInfo();
            partEstimate.SetActive(true);
            
            partReminders.SetPartToNewPage();
            partReminders.UpdateInfo();
            partReminders.SetActive(true);
            
            doneButton.SetPartToNewPage();
            removeButton.SetPartToNewPage();
        }
        
        // Setup parts from page 
        public void SetupFromPage()
        {
            partProgress.SetPartFromPage();
            partStatistics.SetPartFromPage();
            partEstimate.SetPartFromPage();
            partReminders.SetPartFromPage();
            doneButton.SetPartFromPage();
            removeButton.SetPartFromPage();
        }
        
        // Setup parts of page to page pool
        public void SetupToPool(PageItem newPage)
        {
            partProgress.SetToPage(newPage);
            partStatistics.SetToPage(newPage);
            partEstimate.SetToPage(newPage);
            partReminders.SetToPage(newPage);
            doneButton.SetToPage(newPage);
            removeButton.SetToPage(newPage);
        }
        
        // Initialize parts of page
        public void LateStart()
        {
            partProgress.SetupMovableParameters();
            partProgress.InitializeContent();
            
            partStatistics.SetupMovableParameters();
            partStatistics.InitializeContent();

            partEstimate.SetupMovableParameters();
            partEstimate.InitializeContent();
            
            partReminders.SetupMovableParameters();
            partReminders.InitializeContent();
            
            doneButton.SetupMovableParameters();
            removeButton.SetupMovableParameters();
        }

        // Bottom position of part in page
        public float GetBottomSide() => removeButton.GetBottomSide;
    }
}
