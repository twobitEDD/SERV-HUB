using System.Collections.Generic;
using HTools;
using UnityEngine;
using UnityEngine.UI;

namespace Packages.HomeTools.Source.Design
{
    // Animation component of color of UI graphic
    public class UIColorSync : IBehaviorSync
    {
        // Animation speed mode
        public enum Mode { Lerp, Sum }
        
        // List of UI elements and array of colors
        private readonly List<Graphic> groupOfGraphics = new List<Graphic>();
        private readonly List<(Color main, Color target)> colorsOfGraphics = new List<(Color, Color)>();

        public float Speed { private get; set; } // Animation speed
        public Mode SpeedMode { private get; set; } // Animation speed mode
        // Animation state params
        
        private float localMarker;
        private float targetMarker;
        private float addedMarker;
        private bool active;
        
        //Enable debug of animation
        public bool EnableDebug;
        public bool EnableDebugJob;

        // Clear animation component
        public void Clear() => groupOfGraphics.Clear();

        // Add UI element to animation component
        public void AddElement(Graphic item, Color targetColor)
        {
            if (groupOfGraphics.Contains(item))
            {
                colorsOfGraphics[groupOfGraphics.IndexOf(item)] = (item.color, targetColor);
                return;
            }

            groupOfGraphics.Add(item);
            colorsOfGraphics.Add((item.color, targetColor));
        }

        // Prepare component to animation
        public void PrepareToWork()
        {
            if (groupOfGraphics.Count > 0)
                for (var i = 0; i < groupOfGraphics.Count; i++)
                    colorsOfGraphics[i] = (groupOfGraphics[i].color, colorsOfGraphics[i].target);
            
            localMarker = 0;
        }

        // Set target color interpolation position
        public void SetTargetByDynamic(float target) => targetMarker = target;

        // Stop animate
        public void Stop() => active = false;

        // Start animate
        public void Run() => active = true;
        
        // Set default color position
        public void SetDefaultMarker(float marker) => localMarker = marker;
        // Set from other places color by interpolation position
        public void SetColor(float marker) => JobSetColor(marker);

        // Set color by interpolation position
        private void JobSetColor(float marker)
        {
            if (groupOfGraphics.Count > 0)
            {
                for (var i = 0; i < groupOfGraphics.Count; i++)
                {
                    groupOfGraphics[i].color = Color.Lerp(colorsOfGraphics[i].main, colorsOfGraphics[i].target, marker);
                    DebugLogJobState(marker);
                }
            }

            addedMarker = marker;
        }

        public void Start() { }

        // Updates each frame
        public void Update()
        {
            if (!active)
                return;

            DebugLogState();
            
            if (Mathf.Abs(localMarker - targetMarker) < 0.01f && Mathf.Abs(addedMarker - targetMarker) < 0.01f)
                return;

            RunColorMarker();
            JobSetColor(localMarker);
        }

        // Update color marker by interpolate to target amount
        private void RunColorMarker()
        {
            switch (SpeedMode)
            {
                case Mode.Lerp:
                    localMarker = Mathf.Lerp(localMarker, targetMarker, Speed);
                    break;
                case Mode.Sum:
                    localMarker = (Mathf.Abs(localMarker - targetMarker)) > Speed ? 
                        localMarker > targetMarker ? localMarker - Speed : localMarker + Speed
                        : localMarker = targetMarker;
                    break;
            }
        }

        private void DebugLogState()
        {
            if (!EnableDebug)
                return;
                
            Debug.Log($"localMarker: {localMarker}; targetMarker: {targetMarker}; addedMarker: {addedMarker}");
        }
        
        private void DebugLogJobState(float marker)
        {
            if (!EnableDebugJob)
                return;
                
            Debug.Log($"marker: {marker};");
        }
    }
}
