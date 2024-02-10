using System.Collections;
using Architecture.AboutSeaCalendar;
using Architecture.CalendarModule;
using Architecture.DaysMarkerArea.DaysView.DayMarkerPicker;
using Architecture.DaysMarkerArea.DaysView.MonthMarkersModule;
using Architecture.EditTaskArea;
using Architecture.Elements;
using Architecture.ModuleReminders;
using Architecture.ModuleTrackFlow;
using Architecture.ModuleTrackFlow.ModuleTrackGoal;
using Architecture.Pages;
using Architecture.SettingsArea.LoadBackupModule;
using Architecture.SettingsArea.SinglePageView;
using Architecture.TaskViewArea;
using Architecture.TutorialArea;
using HTools;
using MainActivity.AppBar;
using Modules.Notifications;
using Theming;
using UnityEngine;

namespace Architecture
{
    public class MainSystems : MonoBehaviour
    {
        private void Awake()
        {
            ActivateBase();
            AddMainSystems();
            PostSettings();
            
            StartCoroutine(DaysMarkerAreaOptimization());
            StartCoroutine(StatusBarHierarchyCorrection());
        }

        private static void ActivateBase()
        {
            AppBarMaterial.Reset();
            
            AreasLocator.Instance.AppBar = new AppBar();
            
            AreasLocator.Instance.PageTransition = new PageTransition();
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.PageTransition);
        }

        private void AddMainSystems()
        {
            AreasLocator.Instance.WorkArea = new WorkArea.WorkArea();
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.WorkArea);

            AreasLocator.Instance.TrackArea = new TrackArea.TrackArea(); //-
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.TrackArea);

            AreasLocator.Instance.MenuArea = new MenuArea.MenuArea(); //+

            AreasLocator.Instance.CreateTaskArea = new CreateTaskArea.CreateTaskArea(); //+
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.CreateTaskArea);
            
            AreasLocator.Instance.FlowViewArea = new FlowViewArea(); //-
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.FlowViewArea);
            
            AreasLocator.Instance.EditFlowArea = new EditFlowArea(); //-
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.EditFlowArea);

            AreasLocator.Instance.ModuleTrackGoal = new ModuleTrackGoal(); //+
            
            AreasLocator.Instance.TrackFlowModule = new TrackFlowModule(); //+
            
            AreasLocator.Instance.RemindersModule = new RemindersModule(); //+
            
            AreasLocator.Instance.ModuleTrackTime = new ModuleTrackTime.ModuleTrackTime(); //++
            
            AreasLocator.Instance.CalendarTrackModule = new CalendarTrackModule(); //++
            
            AreasLocator.Instance.ChooseIconModule = new ChooseIconModule.ChooseIconModule(); //++
            
            AreasLocator.Instance.UpdateTitleModule = new EditGroupModule.UpdateTitleModule(); //++
            
            AreasLocator.Instance.DaysMarkerArea = new DaysMarkerArea.DaysMarkerArea(); //+
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.DaysMarkerArea);
            
            AreasLocator.Instance.StatisticsArea = new StatisticsArea.StatisticsArea();
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.StatisticsArea);
            
            AreasLocator.Instance.SettingsArea = new SettingsArea.SettingsArea();
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.SettingsArea);
            
            AreasLocator.Instance.MonthMarkersModule = new MonthMarkersModule(); //++
            
            AreasLocator.Instance.ChooseDayMarkerModule = new ChooseDayMarkerModule(); //++

            AreasLocator.Instance.Tutorial = new Tutorial();
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.Tutorial);
            
            AreasLocator.Instance.ViewAboutSeaCalendar = new ViewAboutSeaCalendar();
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.ViewAboutSeaCalendar);
            
            AreasLocator.Instance.TipModule = new TipModule.TipModule(); //-
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.TipModule);
            
            AreasLocator.Instance.LoadBackupModule = new LoadBackupModule();

            AreasLocator.Instance.SinglePage = new SinglePage();
            SyncWithBehaviour.Instance.AddObserver(AreasLocator.Instance.SinglePage);
            
            AreasLocator.Instance.AcceptModule = new AcceptModule.AcceptModule(); //-
            
            AreasLocator.Instance.IosRequestAuthorization =
                SyncWithBehaviour.Instance.AddObserver(new IosRequestAuthorization());
        }

        private void PostSettings()
        {
            AppTheming.ColorizeThemeItem(AppTheming.AppItem.Other);
            AreasLocator.Instance?.AppBar?.AppBarViewMode?.PrepareToWork();
        }

        private IEnumerator DaysMarkerAreaOptimization()
        {
            yield return new WaitForSeconds(Time.deltaTime * 1);
            AreasLocator.Instance.DaysMarkerArea.OpenByOptimization = true;
            AreasLocator.Instance.DaysMarkerArea.SetupParents();
            AreasLocator.Instance.DaysMarkerArea.SetupToPage();
            yield return new WaitForSeconds(Time.deltaTime * 2);
            AreasLocator.Instance.DaysMarkerArea.SetupFromPage();
            AreasLocator.Instance.DaysMarkerArea.SetupToPool();
            AreasLocator.Instance.DaysMarkerArea.OpenByOptimization = false;
        }

        private IEnumerator StatusBarHierarchyCorrection()
        {
            yield return new WaitForSeconds(Time.deltaTime * 3);
            var childrenInCanvas = MainCanvas.RectTransform.childCount;
            var statusBar = MainCanvas.RectTransform.Find("StatusBar");
            statusBar.transform.SetSiblingIndex(childrenInCanvas - 1);
            var appBar = MainCanvas.RectTransform.Find("AppBarElements");
            appBar.transform.SetSiblingIndex(childrenInCanvas - 2);
            var appBarElements = MainCanvas.RectTransform.Find("AppBar");
            appBarElements.transform.SetSiblingIndex(childrenInCanvas - 3);
            
            AreasLocator.Instance.ViewAboutSeaCalendar.RectTransform.SetSiblingIndex(childrenInCanvas - 2);
            AreasLocator.Instance.Tutorial.RectTransform.SetSiblingIndex(childrenInCanvas - 2);
            AreasLocator.Instance.SinglePage.RectTransform.SetSiblingIndex(childrenInCanvas - 2);
        }
    }
}
