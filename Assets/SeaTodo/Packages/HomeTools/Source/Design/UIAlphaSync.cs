using System.Collections.Generic;
using HomeTools.Other;
using HTools;
using UnityEngine;
using UnityEngine.UI;

namespace HomeTools.Source.Design
{
    // Animation component of Alpha channel of UI graphic
    public class UIAlphaSync : IBehaviorSync
    {
        // Animation speed mode
        public enum Mode
        {
            Lerp,
            Sum
        }
        
        // List of UI elements and array of alpha channels
        private readonly List<Graphic> groupOfGraphics = new List<Graphic>();
        private float[] alphaOfGraphics;
        
        // List of Materials and array of alpha channels
        private readonly List<Material> groupOfMaterials = new List<Material>();
        private float[] alphaOfMaterials;
        
        public float Speed { private get; set; }  // Animation speed
        public Mode SpeedMode { private get; set; }  // Animation speed mode

        public float LocalAlpha => localAlpha; // Get local interpolation position
        
        // Animation state params
        private float localAlpha;
        private float targetAlpha;
        private float addedAlpha;
        private bool active;
        public bool UpdateEnableByAlpha;
        public bool DisableWithRays = false;
        
        //Enable debug of animation
        public bool EnableDebug;
        public bool EnableDebugJob;

        // Clear animation component
        public void Clear() => groupOfGraphics.Clear();

        // Add material to animation component
        public void AddMaterialElement(Material element)
        {
            if (!groupOfMaterials.Contains(element))
                groupOfMaterials.Add(element);
        }
        
        // Add UI element to animation component
        public void AddElement(Graphic element)
        {
            if (!groupOfGraphics.Contains(element))
                groupOfGraphics.Add(element);
        }
        
        // Add UI elements to animation component
        public void AddElements(IEnumerable<Graphic> elements)
        {
            foreach (var element in elements)
                groupOfGraphics.Add(element);
        }
        
        // Add UI elements to animation component
        public void AddElements(params Graphic[] elements)
        {
            foreach (var element in elements)
                groupOfGraphics.Add(element);
        }
        
        // Prepare component to animation
        public void PrepareToWork(float alpha = -1)
        {
            if (groupOfGraphics.Count > 0)
            {
                alphaOfGraphics = new float[groupOfGraphics.Count];
                for (var i = 0; i < groupOfGraphics.Count; i++)
                    alphaOfGraphics[i] = alpha == -1 ? groupOfGraphics[i].color.a : alpha;
            }
            
            if (groupOfMaterials.Count > 0)
            {
                alphaOfMaterials = new float[groupOfMaterials.Count];
                for (var i = 0; i < groupOfMaterials.Count; i++)
                    alphaOfMaterials[i] = alpha == -1 ? groupOfMaterials[i].color.a : alpha;
            }

            localAlpha = alpha == -1 ? 1 : alpha;
        }

        // Set target alpha for interpolation
        public void SetAlphaByDynamic(float alpha) => targetAlpha = alpha;

        // Stop animate
        public void Stop() => active = false;

        // Start animate
        public void Run() => active = true;
        // Update activity of UI by local alpha
        public void CheckAlphaByTarget() => EnableByAlpha(localAlpha);
        // Set default alpha channel
        public void SetDefaultAlpha(float alpha) => localAlpha = alpha;
        // Set from other places alpha channel state by interpolation position
        public void SetAlpha(float alpha) => JobSetAlpha(alpha);

        // Set alpha channel by interpolation position
        private void JobSetAlpha(float alpha)
        {
            if (groupOfGraphics.Count > 0)
            {
                for (var i = 0; i < groupOfGraphics.Count; i++)
                {
                    var color = groupOfGraphics[i].color;
                    color.a = Mathf.Lerp(0, alphaOfGraphics[i], alpha);
                    groupOfGraphics[i].color = color;
                    DebugLogJobState(color.a, alphaOfGraphics[i]);
                }
            }

            if (groupOfMaterials.Count > 0)
            {
                for (var i = 0; i < groupOfMaterials.Count; i++)
                {
                    var color = groupOfMaterials[i].color;
                    color.a = Mathf.Lerp(0, alphaOfMaterials[i], alpha);
                    groupOfMaterials[i].color = color;
                    DebugLogJobState(color.a, alphaOfMaterials[i]);
                }
            }
            
            addedAlpha = alpha;
        }

        // Update activity of UI elements by alpha channel
        public void EnableByAlpha(float alpha)
        {
            if (groupOfGraphics.Count > 0)
            {
                for (var i = 0; i < groupOfGraphics.Count; i++)
                {
                    if (groupOfGraphics[i].raycastTarget && !DisableWithRays)
                        continue;
                    
                    if (alpha <= 0 && groupOfGraphics[i].enabled)
                        groupOfGraphics[i].enabled = false;
                    
                    if (alpha > 0 && !groupOfGraphics[i].enabled)
                        groupOfGraphics[i].enabled = true;
                }
            }
        }

        // Set activity of UI elements
        public void Enable(bool enable)
        {
            if (groupOfGraphics.Count <= 0) 
                return;
            
            foreach (var graphic in groupOfGraphics)
                graphic.enabled = enable;
        }

        public string LocalAlphaDebug() => localAlpha.ToString();

        public void Start() { }

        // Updates each frame
        public void Update()
        {
            if (!active)
                return;

            DebugLogState();

            if (Mathf.Abs(localAlpha - targetAlpha) < 0.001f && Mathf.Abs(addedAlpha - targetAlpha) < 0.001f)
            {
                active = false;
                
                JobSetAlpha(targetAlpha);
                
                if (UpdateEnableByAlpha)
                    EnableByAlpha(targetAlpha);
                
                return;
            }

            RunAlpha();
            JobSetAlpha(localAlpha);
        }

        // Update alpha channel by interpolate to target amount
        private void RunAlpha()
        {
            switch (SpeedMode)
            {
                case Mode.Lerp:
                    localAlpha = Mathf.Lerp(localAlpha, targetAlpha, Speed);
                    break;
                case Mode.Sum:
                    localAlpha = (Mathf.Abs(localAlpha - targetAlpha)) > Speed ? 
                        localAlpha > targetAlpha ? localAlpha - Speed : localAlpha + Speed
                        : localAlpha = targetAlpha;
                    break;
            }
        }

        private void DebugLogState()
        {
            if (!EnableDebug)
                return;
                
            Debug.Log($"localAlpha: {localAlpha}; targetAlpha: {targetAlpha}; addedAlpha: {addedAlpha}");
        }
        
        private void DebugLogJobState(float alpha, float mainAlpha)
        {
            if (!EnableDebugJob)
                return;
            
            Debug.Log($"type: Graphics; alpha: {alpha}; main Alpha: {mainAlpha}");
        }

        public void Dispose() => SyncWithBehaviour.Instance.RemoveObserver(this);
    }
}
