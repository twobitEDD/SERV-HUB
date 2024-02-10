using Architecture.Pages;

namespace Architecture.TaskViewArea.FinishedView
{
    // Part of finished task view
    public class ViewFinished : IFlowView
    {
        // Parts of view page
        private readonly PartMainFinished partMainFinished; // Main UI view part 
        private readonly ToProgressButton toProgressButton; // Button of move task to progress
        private readonly ArchiveButton archiveButton; // Button of archive task
        // Main component of task view
        private readonly FlowViewArea flowViewArea;

        // Create and setup
        public ViewFinished(FlowViewArea flowViewArea)
        {
            // Save main component
            this.flowViewArea = flowViewArea;
            // Create parts of page
            partMainFinished = new PartMainFinished(flowViewArea);
            toProgressButton = new ToProgressButton(SetupFromPage, flowViewArea.MarkedAsInProgress);
            archiveButton = new ArchiveButton(flowViewArea.ArchiveTask);
        }

        // Setup parts to page object
        public void SetupParents(PageItem newPage)
        {
            partMainFinished.SetToPage(newPage);
            partMainFinished.SetActive(false);

            toProgressButton.SetToPage(newPage);
            toProgressButton.SetActive(false);
            
            archiveButton.SetToPage(newPage);
        }

        // Setup this page to page transitions system
        public void SetupToPage()
        {
            partMainFinished.SetPartToNewPage();
            partMainFinished.UpdateInfo();
            partMainFinished.SetActive(true);

            if (!flowViewArea.ProgressCompleted)
            {
                toProgressButton.SetPartToNewPage(); 
                toProgressButton.SetActive(true);
            }
            
            archiveButton.SetPartToNewPage();
        }
        
        // Setup parts from page 
        public void SetupFromPage()
        {
            partMainFinished.SetPartFromPage();
            toProgressButton.SetPartFromPage();
            archiveButton.SetPartFromPage();
        }
        
        // Setup parts of page to page pool
        public void SetupToPool(PageItem newPage)
        {
            partMainFinished.SetToPage(newPage);
            toProgressButton.SetToPage(newPage);
            archiveButton.SetToPage(newPage);
        }
        
        // Initialize parts of page
        public void LateStart()
        {
            partMainFinished.SetupBaseElements();
            toProgressButton.SetupMovableParameters();
            archiveButton.SetupMovableParameters();
        }
        
        // Bottom position of part in page
        public float GetBottomSide() => -MainCanvas.RectTransform.sizeDelta.y * 1.5f;
    }
}
