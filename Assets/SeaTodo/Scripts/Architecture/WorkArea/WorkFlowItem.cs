using System;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Pages;
using Architecture.TaskViewArea;
using HomeTools.Handling;
using HomeTools.Source.Design;
using HTools;
using MainActivity.AppBar;
using MainActivity.MainComponents;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Architecture.WorkArea
{
    // Task item in main page
    public class WorkFlowItem : IBehaviorSync
    {
        // Link to app bar
        private AppBar AppBar() => AreasLocator.Instance.AppBar;
        // Link to task view page
        private FlowViewArea FlowViewArea() => AreasLocator.Instance.FlowViewArea;
        public RectTransform RectTransform { get; } // Rect of item
        public int Id; // Id of item
        // UI elements of item
        private readonly Text flowName;
        private readonly Text flowStats;
        private readonly SVGImage flowImage;
        
        private readonly HandleObject handleObject; // Handle component
        // Animation component of alpha channel
        private readonly UIAlphaSync handleAlpha;
        // Animation component of rect
        private readonly RectTransformSync handleScale;
        // Link to tasks move component
        private readonly WorkFlowsListSet workFlowsListSet;
        // Task track button
        private readonly FlowTrackButton flowTrackButton;
        public Flow FlowData { get; private set; } // Task of item

        // States of ite,
        public bool Touched { get; private set; }
        private bool readyToClick;
        private float touchedProgress;
        // Animation params
        private const float touchedSpeed = 0.03f;
        private const float alphaSpeed = 0.17f;
        private const float alphaVisible = 0.3f;

        // Function of convert screen delta to canvas delta
        private readonly Func<Vector2, Vector2> screenConverter;
        private float virtualPosition;
        private bool joined;

        private bool waitForAccess;

        // Create and setup
        public WorkFlowItem(WorkFlowsListSet workFlowsListSet)
        {
            // Save main component
            this.workFlowsListSet = workFlowsListSet;

            // Setup main UI 
            RectTransform = SceneResources.GetPreparedCopy("WorkArea Flow");
            RectTransform.SetSiblingIndex(RectTransform.parent.childCount - 1);
            // Save UI elements
            flowName = RectTransform.Find("Flow Name").GetComponent<Text>();
            flowStats = RectTransform.Find("Flow Stats").GetComponent<Text>();
            flowImage = RectTransform.Find("Flow Image").GetComponent<SVGImage>();

            // Create and setup handle object
            handleObject = new HandleObject(RectTransform.Find("Flow Handle").gameObject);
            handleObject.AddEvent(EventTriggerType.PointerDown, PointerDown);
            handleObject.AddEvent(EventTriggerType.PointerUp, PointerUp);
            handleObject.AddEvent(EventTriggerType.PointerClick, PointerClick);

            // Create animation component of alpha channel
            handleAlpha = new UIAlphaSync();
            handleAlpha.AddElement(RectTransform.Find("Flow Tapped").GetComponent<Image>());
            handleAlpha.AddElement(RectTransform.Find("Flow Tapped/Shadow").GetComponent<Image>());
            SyncWithBehaviour.Instance.AddObserver(handleAlpha, AppSyncAnchors.WorkAreaYear);
            handleAlpha.PrepareToWork(1f);
            handleAlpha.SetAlpha(0);
            handleAlpha.Speed = alphaSpeed;
            handleAlpha.SpeedMode = UIAlphaSync.Mode.Lerp;
            handleAlpha.UpdateEnableByAlpha = true;
            
            // Create animation component of rect
            handleScale = new RectTransformSync();
            handleScale.SetRectTransformSync(RectTransform);
            SyncWithBehaviour.Instance.AddObserver(handleScale, AppSyncAnchors.WorkAreaYear);
            handleScale.Speed = 0.2f;
            handleScale.SpeedMode = RectTransformSync.Mode.Lerp;
            handleScale.TargetScale = Vector3.one * 1.06f;
            handleScale.SetDefaultT(0);
            handleScale.PrepareToWork();

            // Setup track button component
            var buttonObjects = RectTransform.Find("Flow Track").GetComponent<RectTransform>();
            var buttonArea = RectTransform.Find("Flow Track Area").GetComponent<RectTransform>();
            flowTrackButton = new FlowTrackButton(buttonObjects, buttonArea, this);
            SyncWithBehaviour.Instance.AddObserver(flowTrackButton, AppSyncAnchors.WorkAreaYear);
            
            // Save function of convert deltas
            screenConverter = MainCanvas.ScreenToCanvasDelta;
        }

        // Setup position of item
        public void SetPosition(Vector2 position)
        {
            RectTransform.anchoredPosition = position;
            virtualPosition = position.y;
        }

        // Update view by new task
        public void SetupByFlowData(Flow flow)
        {
            FlowData = flow;
            UpdateName();
            UpdateIcon();
            UpdateColor();
            UpdateStats();
            flowTrackButton.UpdateFlowData(flow);
        }

        // Update view by old task
        public void UpdateMainViewInfo()
        {
            UpdateIcon();
            UpdateColor();
            UpdateName();
        }

        // Update stats view
        public void UpdateStats()
        {
            var type = FlowData.Type;
            var progress = FlowData.GetIntProgress();
            flowStats.text = $"{AppFontCustomization.Rating}  {FlowInfoAll.GetWorkProgressFlow(type, progress, flowStats.fontSize, false)}";
        }

        // Update icon color
        private void UpdateColor() => flowImage.color = FlowColorLoader.LoadColorById(FlowData.Color);

        // Update icon
        private void UpdateIcon() => flowImage.sprite = FlowIconLoader.LoadIconById(FlowData.Icon);

        // Update name
        private void UpdateName() => flowName.text = FlowData.Name;
        // Update activity of item
        public void SetActive(bool state) => RectTransform.gameObject.SetActive(state);
        

        public void Start()
        {
        }

        public void Update()
        {
            PressToEditMode();
            MoveMode();
            JoinToPosition();
            WaitForAccess();
        }

        // Process of enter in edit mode when pressed
        private void PressToEditMode()
        {
            if (!Touched)
                return;

            if (touchedProgress >= 1 || workFlowsListSet.EditMode)
                return;

            touchedProgress += touchedSpeed;

            var alpha = 0f;
            if (touchedProgress > alphaVisible)
            {
                alpha = touchedProgress - alphaVisible;
                alpha *= 1 / (1 - alphaVisible);
            }
            handleAlpha.SetAlpha(alpha);
            
            if (touchedProgress >= 1 && !flowTrackButton.Touched)
            {
                PageScroll.Instance.Enabled = false;
                workFlowsListSet.EditMode = true;
                workFlowsListSet.CurrentFlow = this;
                RectTransform.SetSiblingIndex(RectTransform.parent.childCount - 1);
                handleScale.SetTByDynamic(0);
                handleScale.Run();
                flowTrackButton.SetMoveMode();
                return;
            }

            if (!ScrollHandler.AccessByScroll || flowTrackButton.Touched)
                ResetByScroll();
        }

        // Process of move mode 
        private void MoveMode()
        {
            if (!Touched)
                return;

            if (!workFlowsListSet.EditMode)
                return;
            
            var delta = screenConverter.Invoke(InputHS.DeltaMove);
            virtualPosition += delta.y;
        }

        // Move task to anchored default position after move mode
        private void JoinToPosition()
        {
            if (joined && !Touched)
                return;
            
            RectTransform.anchoredPosition = Vector2.Lerp(RectTransform.anchoredPosition, new Vector2(0, virtualPosition), 0.5f);
            
            if (Mathf.Abs(RectTransform.anchoredPosition.y - virtualPosition) < 1)
            {
                RectTransform.anchoredPosition = new Vector2(0, virtualPosition);
                joined = true;
            }
        }

        // Wait for touchable state
        private void WaitForAccess()
        {
            if (!waitForAccess)
                return;

            if (!workFlowsListSet.HasActivity && !workFlowsListSet.DisabledByScroll)
                PointerDown();
        }

        // Touch down method
        private void PointerDown()
        {
            if (workFlowsListSet.HasActivity && workFlowsListSet.CurrentFlow != this)
            {
                waitForAccess = true;
                return;
            }
            
            Touched = true;
            readyToClick = false;

            handleAlpha.SetDefaultAlpha(1);
            handleAlpha.CheckAlphaByTarget();
            handleAlpha.Stop();
            handleAlpha.PrepareToWork(1);
            handleAlpha.SetDefaultAlpha(0);
            handleAlpha.SetAlpha(0);
            
            handleScale.Stop();

            virtualPosition = RectTransform.anchoredPosition.y;
            workFlowsListSet.HasActivity = true;
            workFlowsListSet.DisabledByScroll = true;
            
            if (!workFlowsListSet.JoinToAnchorsState)
                touchedProgress = 0;
            else
            {
                workFlowsListSet.EditMode = true;
                PageScroll.Instance.Enabled = false;
            }
        }
        
        // Touch up method
        private void PointerUp()
        {
            waitForAccess = false;
            
            if (!Touched)
                return;
            
            Touched = false;
            workFlowsListSet.EditMode = false;
            joined = false;
            PageScroll.Instance.Enabled = true;

            if (touchedProgress < 1)
            {
                readyToClick = true;
                PageScroll.Instance.Enabled = true;
                UnHighlight();
            }
        }

        // Clicked method
        private void PointerClick()
        {
            if (!readyToClick)
                return;
            
            OpenFlowView();
        }

        // Reset mode by scroll
        private void ResetByScroll()
        {
            Touched = false;
            UnHighlight();
            PageScroll.Instance.Enabled = true;
            workFlowsListSet.EditMode = false;
            workFlowsListSet.DisabledByScroll = true;
            waitForAccess = false;
        }

        // Unhighlight task item from movable mode
        public void UnHighlight()
        {
            handleAlpha.Speed = alphaSpeed * 2;
            handleAlpha.SetAlphaByDynamic(0);
            handleAlpha.Run();

            handleScale.Speed = alphaSpeed;
            handleScale.SetTByDynamic(1);
            handleScale.Run();
            
            flowTrackButton.SetDefaultMode();
            
            workFlowsListSet.HasActivity = false;
        }

        // Call when touch task for open task view mode
        private void OpenFlowView()
        {
            if (touchedProgress > 0.7f)
                return;

            var opened = AppBar().CanOpenFlowMode();

            if (opened)
            {
                FlowViewArea().SetupCurrentFlow(FlowData);
                AppBar().OpenViewFlowMode(FlowData.Name);
                PageTransitionTemplates.OpenPageFromWorkArea(FlowViewArea().SetupParents, 
                    FlowViewArea().SetupToPage);
            }
        }
    }
}
