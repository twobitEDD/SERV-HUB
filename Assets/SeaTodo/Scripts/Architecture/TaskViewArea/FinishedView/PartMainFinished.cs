using System;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using Architecture.Other;
using Architecture.Pages;
using Architecture.TextHolder;
using HTools;
using MainActivity.MainComponents;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.FinishedView
{
    // Part of UI of finished task view
    public class PartMainFinished
    {
        // Link to pages system
        private PageTransition PageTransition() => AreasLocator.Instance.PageTransition;
        
        // Position params for UI elements
        private const float finishedImageP = 0.57f;
        private const float titleTextFromImage = 0f + 57f;
        private const float infoTextFromImage = 117 + 57f;
        private const float descriptionTextFromImage = 227 + 57f;

        // Animation component for part
        private PartMainFinishedAnimation partMainFinishedAnimation;

        // UI of part
        private Image finishedImage;
        private Text titleText;
        private Text descriptionText;
        private Text infoText;
        
        // Link to main component of view area
        private readonly FlowViewArea flowViewArea;
        // Image position
        private float imagePosition;

        // Create
        public PartMainFinished(FlowViewArea flowViewArea)
        {
            this.flowViewArea = flowViewArea;
        }
        
        // Initialize base elements
        public void SetupBaseElements()
        {
            InitImage();
            InitTitle();
            InitDescription();
            InitMainInfo();
            
            partMainFinishedAnimation = new PartMainFinishedAnimation(finishedImage, titleText, descriptionText, infoText);
            SyncWithBehaviour.Instance.AddObserver(partMainFinishedAnimation, AppSyncAnchors.FlowViewFinished);
        }

        // Initialize image of page
        private void InitImage()
        {
            finishedImage = SceneResources.Get("ViewFlow Content Finished").GetComponent<Image>();
            AppTheming.AddElement(finishedImage, ColorTheme.ImagesColor, AppTheming.AppItem.FlowViewArea);
            PageTransition().PagePool.AddContent(finishedImage.rectTransform);
            SyncWithBehaviour.Instance.AddAnchor(finishedImage.gameObject, AppSyncAnchors.FlowViewFinished);
            
            var sizeDeltaY = MainCanvas.RectTransform.sizeDelta.y / 2;
            imagePosition = -sizeDeltaY + sizeDeltaY * finishedImageP;
            finishedImage.rectTransform.anchoredPosition = new Vector2(0, imagePosition);
        }
        
        // Initialize title of page
        private void InitTitle()
        {
            titleText = SceneResources.Get("ViewFlow Finished Title").GetComponent<Text>();
            PageTransition().PagePool.AddContent(titleText.rectTransform);
            TextLocalization.Instance.AddLocalization(titleText, TextKeysHolder.ViewFlowTitleCurrentProgress);
            
            var imageRt = finishedImage.rectTransform;
            titleText.rectTransform.anchoredPosition = 
                new Vector2(0, imageRt.anchoredPosition.y - imageRt.sizeDelta.y / 2 - titleTextFromImage);
        }
        
        // Initialize description of page
        private void InitDescription()
        {
            descriptionText = SceneResources.GetPreparedCopy("ViewFlow Finished Description").GetComponent<Text>();
            PageTransition().PagePool.AddContent(descriptionText.rectTransform);
            AppTheming.AddElement(descriptionText, ColorTheme.ViewFlowAreaDescription, AppTheming.AppItem.FlowViewArea);
            
            var imageRt = finishedImage.rectTransform;
            descriptionText.rectTransform.anchoredPosition = 
                new Vector2(0, imageRt.anchoredPosition.y - imageRt.sizeDelta.y / 2 - descriptionTextFromImage);
        }
        
        // Initialize additional info about task
        private void InitMainInfo()
        {
            infoText = SceneResources.GetPreparedCopy("ViewFlow Finished Info").GetComponent<Text>();
            PageTransition().PagePool.AddContent(infoText.rectTransform);
            
            var imageRt = finishedImage.rectTransform;
            infoText.rectTransform.anchoredPosition = 
                new Vector2(0, imageRt.anchoredPosition.y - imageRt.sizeDelta.y / 2 - infoTextFromImage);
        }

        // Show UI of page
        public void SetPartToNewPage()
        {
            partMainFinishedAnimation.Show();
        }

        // Setup parts from page
        public void SetPartFromPage()
        {
            partMainFinishedAnimation.Hide();
        }

        // Update page view by task
        public void UpdateInfo()
        {
            var flow = flowViewArea.CurrentFlow;
            var color = FlowColorLoader.LoadColorById(flow.Color);
            
            var progressInt = flow.GetIntProgress();
            var inGoalInt = FlowInfoAll.GetGoalByOriginFlowInt(flow.Type, flow.GoalInt);
            var percentage = $"{(((float)progressInt / (float)inGoalInt) * 100):0.##}%";
            
            titleText.color = new Color(color.r, color.g, color.b, titleText.color.a);
            
            var localizedTitle = TextLocalization.GetLocalization(TextKeysHolder.ViewFlowCompleted);
            titleText.text = localizedTitle?.Length > 0 
                    ? string.Format(localizedTitle, percentage) 
                    : string.Empty;

            var startedDay = flow.GetStartedDay();
            var startedDayTime = new DateTime(startedDay.Year, startedDay.Month, startedDay.Day);
            var startedDayText = $"{startedDayTime.Day}.{startedDayTime.Month}.{startedDayTime:yyyy}";
            
            var count = flowViewArea.CurrentFlow.GoalData.Count;

            var localizedDescription = TextLocalization.GetLocalization(TextKeysHolder.ViewFlowCompletedDescription);
            descriptionText.text = localizedDescription?.Length > 0
                    ? string.Format(localizedDescription, count, startedDayText)
                    : string.Empty;

            var inProgress = progressInt <= 0 ? "—" : $"{FlowInfoAll.GetWorkProgressFlow(flow.Type, progressInt, infoText.fontSize)}";
            
            infoText.color = new Color(color.r, color.g, color.b, infoText.color.a);
            var text = TextLocalization.GetLocalization(TextKeysHolder.FlowAreaFinishedInfo);
            infoText.text = string.Format(text, inProgress);
        }
        
        // Setup this part to page
        public void SetToPage(PageItem pageItem)
        {
            pageItem.AddContent(finishedImage.rectTransform);
            pageItem.AddContent(titleText.rectTransform);
            pageItem.AddContent(descriptionText.rectTransform);
            pageItem.AddContent(infoText.rectTransform);
            
            finishedImage.rectTransform.anchoredPosition = new Vector2(0, imagePosition);
        }

        // Set activity of page elements
        public void SetActive(bool active)
        {
            finishedImage.rectTransform.gameObject.SetActive(active);
            titleText.rectTransform.gameObject.SetActive(active);
            descriptionText.rectTransform.gameObject.SetActive(active);
            infoText.rectTransform.gameObject.SetActive(active);
        }
    }
}
