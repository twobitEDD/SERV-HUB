using Architecture.CreateTaskArea.ChooseTypePart;
using Architecture.Elements;
using Architecture.Pages;
using Architecture.TaskViewArea.NormalView;
using Architecture.TextHolder;
using HomeTools.Source.Design;
using HTools;
using InternalTheming;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.CreateTaskArea
{
    // Object of choose task type part
    public class ChooseTaskTypePart
    {
        // main part of task type items
        private MainPart mainPart;
        
        // Position parameters of elements
        private const float imagePositionFromTop = 477;
        private const float titlePositionFromTop = 657;
        private const float descriptionPositionFromTop = 768;
        
        // Height of background
        private const float backgroundHeight = 1687;

        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        private readonly GroupBackground groupBackground; // Background of part
        private RectTransformSync groupBackgroundSync; // Animation component for background
        private Text titleText; // Title of part
        private Text description; // Description of part
        
        private IconColorCircles iconColorCircles; // Circles that placed around icon

        public readonly float GetBottomSide; // Bottom position of part in page
        
        // Create
        public ChooseTaskTypePart()
        {
            // Create background for part
            groupBackground = new GroupBackground();
            // Add part to pool
            PageTransition().PagePool.AddContent(groupBackground.RectTransform);
            // Update background height and Y position
            groupBackground.UpdateVisible(backgroundHeight, 170);
            // Calculate bottom side
            GetBottomSide = groupBackground.RectTransform.anchoredPosition.y - groupBackground.RectTransform.sizeDelta.y;
        }
        
        // Initialize base elements to background
        public void SetupBaseElements()
        {
            InitBackground(); // Init background parts
            
            InitTitle(); // Init title of part
            InitDescription(); // Init description of part
            
            // Create component of main elements of part
            mainPart = new MainPart();
            mainPart.SetupToBackground(groupBackground.RectTransform);
        }

        // Initialize image to background
        private void InitBackground()
        {
            var image = SceneResources.Get("CreateFlow Content 1").GetComponent<RectTransform>();
            SyncWithBehaviour.Instance.AddAnchor(image.gameObject, AppSyncAnchors.CreateFlowAreaChooseType);
            groupBackground.AddItemToBackground(image);
            image.anchoredPosition = new Vector2(0, groupBackground.RectTransform.sizeDelta.y / 2 - imagePositionFromTop);
            
            var circle1 = image.Find("Circle 0").GetComponent<Image>();
            var circle2 = image.Find("Circle 1").GetComponent<Image>();
            var circle3 = image.Find("Circle 2").GetComponent<Image>();
            iconColorCircles = new IconColorCircles(circle1, circle2, circle3);
        }

        // Initialize and prepare title to background
        private void InitTitle()
        {
            titleText = SceneResources.GetPreparedCopy("CreateFlow Title").GetComponent<Text>();
            groupBackground.AddItemToBackground(titleText.rectTransform);
            titleText.rectTransform.anchoredPosition = new Vector2(0, 
                groupBackground.RectTransform.sizeDelta.y / 2 - titlePositionFromTop);
            
            TextLocalization.Instance.AddLocalization(titleText, TextKeysHolder.ChooseTaskType);
        }
        
        // Initialize and prepare description to background
        private void InitDescription()
        {
            description = SceneResources.GetPreparedCopy("CreateFlow Description").GetComponent<Text>();
            groupBackground.AddItemToBackground(description.rectTransform);
            description.rectTransform.anchoredPosition = new Vector2(0, 
                groupBackground.RectTransform.sizeDelta.y / 2 - descriptionPositionFromTop);
            
            TextLocalization.Instance.AddLocalization(description, TextKeysHolder.CreateFlowTypeDescription);
        }

        // Create animation component for background part
        public void SetupMovableParameters()
        {
            groupBackgroundSync = new RectTransformSync();
            groupBackgroundSync.SetRectTransformSync(groupBackground.RectTransform);
            groupBackgroundSync.TargetPosition = new Vector3(0, SetupBasePositions(), 0);
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SpeedMode = RectTransformSync.Mode.Lerp;
            groupBackgroundSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(groupBackgroundSync);
        }

        // Setup main position of part in page
        private float SetupBasePositions() => MainCanvas.RectTransform.sizeDelta.y - 
                                              AppElementsInfo.DistanceContentToBottom + backgroundHeight / 2;

        // Play move animation of part in new page when open
        public void SetPartToNewPage()
        {
            groupBackgroundSync.Speed = 0.3f;
            groupBackgroundSync.SetDefaultT(0);
            groupBackgroundSync.TargetPosition = new Vector3(0, SetupBasePositions(), 0);
            groupBackgroundSync.SetTByDynamic(1);
            groupBackgroundSync.Run();
            
            iconColorCircles.UpdateColors(ThemeLoader.GetCurrentTheme().CreateFlowAreaContentCircles);
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

        // Get chosen type of task
        public FlowType GetFlowType() => mainPart.CurrentType;
        
        // Get chosen goal of task
        public int GetFlowGoal() => mainPart.GetGoalOrigin();
        
        // Set active state of  part
        public void SetActive(bool active) => groupBackground.RectTransform.gameObject.SetActive(active);
        
        // Set to part type of task
        public void SetupCurrentType() => mainPart.SetupCountCurrentType();
    }
}
