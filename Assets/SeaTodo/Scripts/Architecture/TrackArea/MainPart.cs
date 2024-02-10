using HomeTools.Other;
using HTools;
using MainActivity.MainComponents;
using UnityEngine;

namespace Architecture.TrackArea
{
    // Component for setup area view
    public class MainPart : IBehaviorSync
    {
        // Position params
        
        private const float heightAspectMain = 1.521f;
        private const int shiftInnerBackground = 167;
        private const int shiftOfHandle = 34;
        private const int shiftFromBottom = 20;
        private const int bottomAreaResultsShift = 140;
        public const float RectOutOfSideDistance = 870f;
        
        private float bottomScreenSide;
        private float bottomRectPosition;
        
        public RectTransform RectTransform { get; }
        private readonly ScreenBlackout screenBlackout;
        
        private readonly Vector2 flowNameSize = new Vector2(500, 150);
        private readonly Vector2 flowNamePositionShift = new Vector2(70, -37);
        
        private readonly Vector2 trackDaySize = new Vector2(300, 70);
        private readonly Vector2 trackDayPositionShift = new Vector2(103, -208);
        
        private readonly Vector2 calendarSize = new Vector2(45, 45);
        private readonly Vector2 calendarPositionShift = new Vector2(79, -207);

        private bool started;
        
        // Setup main rect
        public MainPart(ScreenBlackout screenBlackout)
        {
            this.screenBlackout = screenBlackout;
            
            RectTransform = SceneResources.Get("TrackArea").GetComponent<RectTransform>();
            RectTransform.SetParent(MainCanvas.RectTransform);
            RectTransform.SetSiblingIndex(2);
            RectTransform.anchorMin = new Vector2(0, 1);
            RectTransform.anchorMax = Vector2.one;
            RectTransform.SetRectTransformAnchorHorizontal(0, 0);
            CheckActiveRect(0);
        }

        public void Start()
        {
        }

        public void Update()
        {
            if (started) 
                return;
            
            SetupInnerElements();    
            SetupBasePositions();
            SetupElementsByPositions();
            started = true;
        }
        
        public void SetupPositionByT(float t)
        {
            var newPosition = 
                Mathf.LerpUnclamped(bottomRectPosition, bottomRectPosition + shiftFromBottom + RectOutOfSideDistance, t);
            RectTransform.anchoredPosition = new Vector2(RectTransform.anchoredPosition.x, newPosition);
            screenBlackout.SetState(t);
            CheckActiveRect(t);
        }
        
        private void CheckActiveRect(float t)
        {
            if (t <= 0f && RectTransform.gameObject.activeSelf)
                RectTransform.gameObject.SetActive(false);
            
            if (t > 0f && !RectTransform.gameObject.activeSelf)
                RectTransform.gameObject.SetActive(true);
        }

        private void SetupInnerElements()
        {
            RectTransform.sizeDelta = new Vector2(0, MainCanvas.RectTransform.rect.width * heightAspectMain);
            
            var secondBackground = RectTransform.Find("Background").GetComponent<RectTransform>();
            secondBackground.sizeDelta = new Vector2(secondBackground.sizeDelta.x, RectTransform.sizeDelta.y - shiftInnerBackground * 2);
            
            var splitLine1 = RectTransform.Find("Split Line 1").GetComponent<RectTransform>();
            splitLine1.anchoredPosition = new Vector2(0, -shiftInnerBackground);

            var handleLine = RectTransform.Find("Handle Line").GetComponent<RectTransform>();
            handleLine.anchoredPosition = new Vector2(0, -shiftOfHandle);

            var name = RectTransform.Find("Flow Name").GetComponent<RectTransform>();
            name.sizeDelta = flowNameSize;
            name.anchoredPosition = new Vector2(flowNameSize.x / 2, - flowNameSize.y / 2) + flowNamePositionShift;
            
            var day = RectTransform.Find("Day In Track").GetComponent<RectTransform>();
            day.sizeDelta = trackDaySize;
            day.anchoredPosition = new Vector2(trackDaySize.x / 2, -trackDaySize.y / 2) + trackDayPositionShift + new Vector2(calendarSize.x, 0);
            
            var calendar = RectTransform.Find("Calendar Button").GetComponent<RectTransform>();
            calendar.sizeDelta = calendarSize;
            calendar.anchoredPosition = new Vector2(calendarSize.x / 2, -trackDaySize.y / 2) + calendarPositionShift;
        }

        private void SetupBasePositions()
        {
            bottomScreenSide = -MainCanvas.RectTransform.sizeDelta.y;
            bottomRectPosition = bottomScreenSide - shiftFromBottom - RectTransform.sizeDelta.y / 2;
            SetupPositionByT(0);
        }

        private void SetupElementsByPositions()
        {
            var position = RectOutOfSideDistance - bottomAreaResultsShift;
            
            var splitLine2 = RectTransform.Find("Split Line 2").GetComponent<RectTransform>();
            splitLine2.anchoredPosition = new Vector2(0, -position);
            
            var handleArea = RectTransform.Find("Handle Area").GetComponent<RectTransform>();
            handleArea.sizeDelta = new Vector2(handleArea.sizeDelta.x, shiftInnerBackground + 100);
            handleArea.anchoredPosition = new Vector2(0, -shiftInnerBackground / 2f + 40);
            
            var scrollHandleArea = RectTransform.Find("Scroll Handle Area").GetComponent<RectTransform>();

            var size = RectOutOfSideDistance - bottomAreaResultsShift - shiftInnerBackground;
            scrollHandleArea.sizeDelta = new Vector2(scrollHandleArea.sizeDelta.x, size);
            scrollHandleArea.anchoredPosition = Vector2.zero + new Vector2(0, -RectOutOfSideDistance / 2 - AppElementsInfo.BackgroundShadowCorrection);
            
            var blockHandleArea = RectTransform.Find("Block Handle Area").GetComponent<RectTransform>();
            var sizeOfBlock = bottomAreaResultsShift + RectOutOfSideDistance * 0.12f;
            blockHandleArea.sizeDelta = new Vector2(blockHandleArea.sizeDelta.x, sizeOfBlock);
            blockHandleArea.anchoredPosition = Vector2.zero + new Vector2(0, -RectOutOfSideDistance + bottomAreaResultsShift - sizeOfBlock / 2);
            
            var trackerArea = RectTransform.Find("Tracker").GetComponent<RectTransform>();
            trackerArea.anchoredPosition = Vector2.zero + new Vector2(0, -RectOutOfSideDistance / 2 - AppElementsInfo.BackgroundShadowCorrection);
            
            var trackResult = RectTransform.Find("Track Result").GetComponent<RectTransform>();
            trackResult.anchoredPosition = new Vector2(0, -position - bottomAreaResultsShift / 2f);
        }
    }
}
