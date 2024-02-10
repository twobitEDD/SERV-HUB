using Architecture.Statistics;
using Architecture.Statistics.Interfaces;
using HomeTools.Source.Calendar;
using InternalTheming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.NormalView.Statistics
{
    // Additional part for statistics
    public class ViewAdditionalElements : IViewAdditional
    {
        private Text month; // Month text

        // Setup
        public void Setup(RectTransform view)
        {
            // Find month
            month = view.Find("Month").GetComponent<Text>();
        }

        public void SetupScroll(StatisticsScroll statisticsScroll)
        {
        }

        // Update month text by data
        public void FullUpdate(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next)
        {
            month.enabled = true;

            if (current.EmptyActivity() && !current.NoData)
            {
                month.enabled = false;
            }
            
            UpdateMonthText(current);
        }

        // Update month text by data
        public void Update(GraphDataStruct preview, GraphDataStruct current, GraphDataStruct next) =>
            FullUpdate(preview, current, next);

        // Update month text
        private void UpdateMonthText(GraphDataStruct graphDataStruct)
        {
            var color = graphDataStruct.Highlighted == -1
                ? ThemeLoader.GetCurrentTheme().ViewFlowAreaDescription
                : ThemeLoader.GetCurrentTheme().SecondaryColor;
            month.color = color;
            
            month.text = $"{CalendarNames.GetMonthFullName(graphDataStruct.GraphElementsDescription[0][1])} " +
                         $"{graphDataStruct.GraphElementsDescription[0][0]}";
        }
    }
}
