using Architecture.Elements;
using Architecture.Pages;
using Architecture.SettingsArea.ChooseItems;
using Architecture.TextHolder;
using HomeTools;
using HomeTools.Source.Design;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Data buttons part of settings page
    public class DataPart
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;

        private RectTransform rectTransform; // Rect object of part
        private const float partAnchoredPosition = 0; // Position of part
        private float backgroundHeight = 803; // Height of background
        private bool shortVersion; // Short version of view for iOS
        
        // Background component
        private readonly GroupBackground groupBackground;
        // Animation components for background
        private RectTransformSync groupBackgroundSync;
        // Bottom position of part in page
        public float GetBottomSide { get; private set; }

        // Components of buttons
        private LineExportFile lineExportFile;
        private LineAutoBackup lineAutoBackup;
        private LineCreateBackup lineCreateBackup;
        private LineLoadBackup lineLoadBackup;
        private LineClearData lineClearData;
        
        private CreateBackupResultModule.CreateBackupResultModule createBackupResultModule;
        
        // Create and setup
        public DataPart(float upperSidePosition)
        {
            // Create background
            groupBackground = new GroupBackground();
            // Add part to pool
            PlatformsPartEdition();
            // Update background height and Y position
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Calculate bottom side
            groupBackground.UpdateVisible(backgroundHeight, upperSidePosition + AppElementsInfo.DistanceBetweenBackgrounds);
            // Save bottom side
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Create animation component for background part
        public void SetupMovableParameters()
        {
            groupBackgroundSync = new RectTransformSync();
            groupBackgroundSync.SetRectTransformSync(groupBackground.RectTransform);
            groupBackgroundSync.TargetPosition = new Vector3(0, 0, 0);
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SpeedMode = RectTransformSync.Mode.Lerp;
            groupBackgroundSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(groupBackgroundSync);
        }
        
        // Initialize base elements to background
        public void InitializeContent(ChooseItemsPanel chooseItemsPanel)
        {
            // Initialize rect object of part and setup to background
            var partName = shortVersion ? "Settings Data Part Short" : "Settings Data Part";
            rectTransform = SceneResources.Get(partName).GetComponent<RectTransform>();
            SyncWithBehaviour.Instance.AddAnchor(rectTransform.gameObject, AppSyncAnchors.SettingsData);
            groupBackground.AddItemToBackground(rectTransform);
            rectTransform.anchoredPosition = new Vector2(0, partAnchoredPosition);

            // Find and localize title
            var title = rectTransform.Find("Title").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(title, TextKeysHolder.SettingsDataTitle);
            
            // Initialize settings buttons
            createBackupResultModule = new CreateBackupResultModule.CreateBackupResultModule();
            
            lineExportFile = new LineExportFile(rectTransform.Find("Export Data").GetComponent<RectTransform>(), 
                createBackupResultModule);
            lineClearData = new LineClearData(rectTransform.Find("Clear Data").GetComponent<RectTransform>());
            
            // Do not initialize other buttons if iOS
            if (shortVersion)
                return;
            
            lineAutoBackup = new LineAutoBackup(rectTransform.Find("Auto Backup").GetComponent<RectTransform>(), chooseItemsPanel);
            lineCreateBackup = new LineCreateBackup(rectTransform.Find("Create Data").GetComponent<RectTransform>(), 
                createBackupResultModule);
            lineLoadBackup = new LineLoadBackup(rectTransform.Find("Load Data").GetComponent<RectTransform>());
        }
        
        // Play move animation of part in new page
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, -SetupBasePositions(), 0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            lineExportFile?.Reactivate();
            lineAutoBackup?.Reactivate();
            lineCreateBackup?.Reactivate();
            lineLoadBackup?.Reactivate();
            lineClearData?.Reactivate();
        }
        
        // Play move animation of part in new page when close
        public void SetPartFromPage()
        {
            var centeredPosition = PageTransition().CurrentPage().Page.anchoredPosition.y +
                                   groupBackground.RectTransform.anchoredPosition.y;
            var direction = centeredPosition > 0 ? -1 : 1;

            groupBackgroundSync.Speed = 0.37f;
            groupBackgroundSync.TargetPosition = new Vector3(0, groupBackground.RectTransform.anchoredPosition.y - SetupBasePositions() * direction, 0);
            groupBackgroundSync.SetTByDynamic(0);
            groupBackgroundSync.Run();
        }

        // Set part to page
        public void SetToPage(PageItem pageItem) => pageItem.AddContent(groupBackground.RectTransform);
        
        // Set active state of part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);

        // Update anchored position
        public void UpdateAnchorPosition(float anchor)
        {
            groupBackground.UpdateVisible(backgroundHeight, anchor + AppElementsInfo.DistanceBetweenBackgrounds);
            groupBackgroundSync.PrepareToWork(true);
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Setup main position of part in page
        private float SetupBasePositions() =>  MainCanvas.RectTransform.sizeDelta.y - 
                                               AppElementsInfo.DistanceContentToBottom + backgroundHeight / 2;


        // Check for short view version
        private void PlatformsPartEdition()
        {
            if (!AppParameters.Ios)
                return;

            backgroundHeight -= 372f;
            shortVersion = true;
        }
    }
}
