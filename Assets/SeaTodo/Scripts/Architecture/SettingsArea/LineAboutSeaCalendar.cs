using Architecture.AboutSeaCalendar;
using Architecture.Components;
using Architecture.Elements;
using Architecture.TextHolder;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.SettingsArea
{
    // Button component for settings of page about Sea Calendar
    public class LineAboutSeaCalendar
    {
        // Link to Sea Calendar tutorial object
        private static ViewAboutSeaCalendar ViewAboutSeaCalendar => AreasLocator.Instance.ViewAboutSeaCalendar;
        private readonly RectTransform rectTransform; // Rect of button
        private readonly MainButtonJob mainButtonJob; // Button component

        // Create and setup
        public LineAboutSeaCalendar(RectTransform rectTransform)
        {
            // Save components
            this.rectTransform = rectTransform;

            // Create button components
            var handler = rectTransform.Find("Handler");
            mainButtonJob = new MainButtonJob(rectTransform, Touched, handler.gameObject);
            mainButtonJob.AttachToSyncWithBehaviour(AppSyncAnchors.SettingsMain);
            mainButtonJob.SimulateWaveScale = 1.057f;

            // Find name and localize
            var name = rectTransform.Find("Name").GetComponent<Text>();
            TextLocalization.Instance.AddLocalization(name, TextKeysHolder.SettingsButtonAboutSeaCalendar);
        }
        
        // Reset button
        public void Reactivate() => mainButtonJob.Reactivate();
        
        // Call when touched 
        private void Touched()
        {
            // Reset state of button component
            mainButtonJob.Reactivate();
            // Open page about Sea Calendar
            ViewAboutSeaCalendar.Open();
        }
    }
}
