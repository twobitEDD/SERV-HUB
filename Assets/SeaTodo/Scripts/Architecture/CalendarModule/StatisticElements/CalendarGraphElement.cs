using System.Collections.Generic;
using Architecture.Statistics.Interfaces;
using UnityEngine;

namespace Architecture.CalendarModule.StatisticElements
{
    public class CalendarGraphElement : IGraphElement
    {
        
        public void SetupViewPlace(RectTransform viewPlace)
        {
        }

        public void SetToView(float position)
        {
        }

        public void SetToPool()
        {
        }

        public void Start()
        {
        }

        public void Update(float infoGraph, List<int> infoDescription, bool infoActive, bool highlighted)
        {
        }

        public void UpdateDynamic(float infoGraph, bool infoActive, bool highlighted)
        {
        }
        
        // Create element for calendar page
        public IGraphElement CreateClone() => new CalendarGraphElement();
    }
}
