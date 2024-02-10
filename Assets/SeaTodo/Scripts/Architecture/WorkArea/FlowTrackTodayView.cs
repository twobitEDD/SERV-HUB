using Architecture.Data;
using Architecture.Data.FlowTrackInfo;
using Architecture.Data.Structs;
using Architecture.Elements;
using HomeTools.Source.Design;
using HTools;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.WorkArea
{
    // Component for setup progress of task view
    public class FlowTrackTodayView
    {
        // Animation component of alpha channel of view
        public UIAlphaSync AlphaSync { get; }
        private Flow flowData; // Current task

        // UI elements for each type of task
        private readonly Text count;
        private readonly Text time;
        private readonly Text starsText;
        private readonly SVGImage stars;
        private readonly SVGImage done;
        private readonly SVGImage up;
        private readonly SVGImage down;

        private const float processSpeed = 0.1f;
        
        public FlowTrackTodayView(Transform container)
        {
            // Find UI elements
            count = container.Find("count").GetComponent<Text>();
            time = container.Find("time").GetComponent<Text>();
            starsText = container.Find("stars text").GetComponent<Text>();
            stars = container.Find("stars").GetComponent<SVGImage>();
            done = container.Find("done").GetComponent<SVGImage>();
            up = container.Find("up").GetComponent<SVGImage>();
            down = container.Find("down").GetComponent<SVGImage>();
            
            // Create animation component
            AlphaSync = new UIAlphaSync();
            AlphaSync.AddElement(count);
            AlphaSync.AddElement(time);
            AlphaSync.AddElement(starsText);
            AlphaSync.AddElement(stars);
            AlphaSync.AddElement(done);
            AlphaSync.AddElement(up);
            AlphaSync.AddElement(down);
            AlphaSync.Speed = processSpeed * 8;
            AlphaSync.SpeedMode = UIAlphaSync.Mode.Lerp;
            SyncWithBehaviour.Instance.AddObserver(AlphaSync, AppSyncAnchors.WorkAreaYear);
        }

        // Update view by task
        public void SetupByFlow(Flow flow)
        {
            flowData = flow;
            SetupElements();
        }

        // Update activity of view
        public void SetTracked(bool tracked)
        {
            switch (flowData.Type)
            {
                case FlowType.count:
                    count.gameObject.SetActive(tracked);
                    break;
                case FlowType.done:
                    done.gameObject.SetActive(tracked);
                    break;
                case FlowType.symbol:
                    up.gameObject.SetActive(tracked);
                    down.gameObject.SetActive(!tracked);
                    break;
                case FlowType.stars:
                    starsText.gameObject.SetActive(tracked);
                    stars.gameObject.SetActive(tracked);
                    break;
                case FlowType.timeS:
                    time.gameObject.SetActive(tracked);
                    break;
                case FlowType.timeM:
                    time.gameObject.SetActive(tracked);
                    break;
                case FlowType.timeH:
                    time.gameObject.SetActive(tracked);
                    break;
            }
        }

        // Update view by current task progress
        public void UpdateByDay()
        {
            var active = flowData.GetTrackedDay(GetCurrentHomeDay()) == 1;

            switch (flowData.Type)
            {
                case FlowType.count:
                    count.text = flowData.GetTrackedDay(GetCurrentHomeDay()).ToString();
                    break;
                case FlowType.done:
                    done.gameObject.SetActive(active);
                    break;
                case FlowType.symbol:
                    up.gameObject.SetActive(active);
                    down.gameObject.SetActive(!active);
                    break;
                case FlowType.stars:
                    starsText.text = flowData.GetTrackedDay(GetCurrentHomeDay()).ToString();
                    break;
                case FlowType.timeS:
                    time.text = FlowInfoTimeSeconds.ConvertToView(flowData.GetTrackedDayOrigin(GetCurrentHomeDay()), time.fontSize);
                    break;
                case FlowType.timeM:
                    time.text = FlowInfoTimeMinutes.ConvertToView(flowData.GetTrackedDayOrigin(GetCurrentHomeDay()), time.fontSize);
                    break;
                case FlowType.timeH:
                    time.text = FlowInfoTimeHours.ConvertToView(flowData.GetTrackedDayOrigin(GetCurrentHomeDay()), time.fontSize);
                    break;
            }
        }

        // Get current day by calendar
        private HomeDay GetCurrentHomeDay() => AreasLocator.Instance.WorkArea.WorkCalendar.CurrentDay;

        // Deactivate all UI elements
        private void SetupElements()
        {
            count.gameObject.SetActive(false);
            time.gameObject.SetActive(false);
            starsText.gameObject.SetActive(false);
            stars.gameObject.SetActive(false);
            done.gameObject.SetActive(false);
            up.gameObject.SetActive(false);
            down.gameObject.SetActive(false);
        }
    }
}
