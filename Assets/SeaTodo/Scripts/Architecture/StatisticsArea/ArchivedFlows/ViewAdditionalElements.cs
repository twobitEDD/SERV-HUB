using System.Linq;
using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Elements;
using Architecture.MenuArea;
using Architecture.Other;
using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using Architecture.StatisticsArea.ArchivedFlowUpdate;
using Architecture.TextHolder;
using Architecture.TipModule;
using HomeTools.Messenger;
using HomeTools.Other;
using HomeTools.Source.Calendar;
using HTools;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.StatisticsArea.ArchivedFlows
{
    // Page with archived flow
    public class ViewAdditionalElements : IViewAdditional
    {
        // Link to main page
        private WorkArea.WorkArea WorkArea() => AreasLocator.Instance.WorkArea;
        // Link to tips view
        private TipModule.TipModule TipModule() => AreasLocator.Instance.TipModule;
        // Link to activity page
        private StatisticsArea StatisticsArea => AreasLocator.Instance.StatisticsArea;
        // Link to update archived task view
        private ArchivedUpdateModule ArchivedUpdateModule => AreasLocator.Instance.StatisticsArea.ArchivedUpdateModule;
        // Update archived task session
        private UpdateSession updateSession;
        
        // UI elements of archived task
        private RectTransform viewRect;
        private SVGImage image;
        private SVGImage icon;
        private Text name;
        private Text done;
        private Text days;
        private Text started;
        private Text finished;
        // Touch task handler
        private TaskHandler touchHandler;
        // Current task
        private Flow flow;

        // Initialize page
        public void Setup(RectTransform view)
        {
            viewRect = view;
            // Find UI elements
            image = viewRect.Find("Image").GetComponent<SVGImage>();
            icon = viewRect.Find("Image/Icon").GetComponent<SVGImage>();
            name = viewRect.Find("Name").GetComponent<Text>();
            done = viewRect.Find("Done").GetComponent<Text>();
            days = viewRect.Find("Days").GetComponent<Text>();
            started = viewRect.Find("Date Started").GetComponent<Text>();
            finished = viewRect.Find("Date Finished").GetComponent<Text>();
            // Create touch task handler
            touchHandler = new TaskHandler(viewRect.Find("Task Handler").gameObject, image.rectTransform, TouchTaskAction);
            SyncWithBehaviour.Instance.AddObserver(touchHandler, AppSyncAnchors.StatisticsArea);
        }

        // Setup scroll component for each color item
        public void SetupScroll(StatisticsScroll scroll)
        {
            touchHandler.AddStatisticsEvents(scroll);
        }

        // Update page
        public void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            UpdateActions(current);

        // Update page
        public void Update(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            UpdateActions(current);

        // Update task view by new data
        private void UpdateActions(GraphDataStruct current)
        {
            // Check if page is empty
            if (current.EmptyActivity())
            {
                viewRect.gameObject.SetActive(false);
                return;
            }

            // Get task by id
            var flowId = current.GraphElementsDescription[0][0];
            flow = AppData.GetCurrentGroup().ArchivedFlows.FirstOrDefault(e => e.Id == flowId);

            // Check task
            if (flow == null)
            {
                viewRect.gameObject.SetActive(false);
                return;
            }
            
            // Activate page object
            viewRect.gameObject.SetActive(true);
            
            // Setup UI 
            image.color = FlowColorLoader.LoadColorById(flow.Color);
            icon.sprite = FlowIconLoader.LoadIconById(flow.Icon);
            name.text = flow.Name;
            
            // Setup task stats view 
            
            var progressInt = flow.GetIntProgress();
            done.text = FlowInfoAll.GetWorkProgressFlow(flow.Type, progressInt, done.fontSize, false);
            var dateStarted = flow.GetStartedDay();
            var finishedDate = FlowExtensions.IntToHomeDay(flow.DateFinished);

            started.text = $"{dateStarted.Day} {CalendarNames.GetMonthShortName(dateStarted.Month)} {dateStarted.Year}";
            finished.text = $"{finishedDate.Day} {CalendarNames.GetMonthShortName(finishedDate.Month)} {finishedDate.Year}";

            var daysLength = 0;
            if (finishedDate >= dateStarted)
                daysLength = OtherHTools.DaysDistance(finishedDate, dateStarted);

            var prefix = FlowSymbols.DaysFull;

            days.text = $"{daysLength} {prefix}";
        }

        // Clicked task method
        private void TouchTaskAction()
        {
            updateSession = new UpdateSession(AcceptAction, RemoveAction, Vector2.zero);
            ArchivedUpdateModule.Open(GetWorldAnchoredPosition(), updateSession);
        }

        // Activate task method
        private void AcceptAction()
        {
            var tasksCount = AppData.GetCurrentGroup().Flows.Length;
            var tasksLimit = MainPart.MaxCountOfFlows;
            if (tasksCount < tasksLimit)
            {
                FlowCreator.BackToProgressFromArchive(flow);
                FlowCreator.UpdateRemindersWhenStatusChanged(flow);
                StatisticsArea.UpdateArchivedPart();
                WorkArea().UpdateFlows();
                MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
            }
            else
            {
                var title = TextLocalization.GetLocalization(TextKeysHolder.TasksLimit);
                var session = new TipSession(null, title, TipsContentContainer.Type.TaskLimit);
                TipModule().Open(Vector2.zero, session, 700);
            }
        }
        
        // Remove task method
        private void RemoveAction()
        {
            FlowCreator.RemoveFlowFromArchive(flow);
            StatisticsArea.UpdateArchivedPart();
            MainMessenger.SendMessage(AppMessagesConst.UpdateWorkAreaFlowsViewTrack);
        }

        // Get position for open update archived task view
        private Vector2 GetWorldAnchoredPosition()
        {
            var parent = image.transform.parent;

            image.transform.SetParent(MainCanvas.RectTransform);
            var result = image.rectTransform.anchoredPosition + new Vector2(0, 50);
            image.transform.SetParent(parent);

            return result;
        }
    }
}
