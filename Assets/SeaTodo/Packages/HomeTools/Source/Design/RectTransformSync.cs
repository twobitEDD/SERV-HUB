using HTools;
using UnityEngine;

namespace HomeTools.Source.Design
{
    // Animation component of Rect Transform
    public class RectTransformSync : IBehaviorSync
    {
        // Animation speed mode
        public enum Mode
        {
            Lerp,
            Sum
        }
        
        // Component for save target state
        private readonly TransformKeeper transformKeeper = new TransformKeeper();
        // Rect object
        public RectTransform RectTransform { get; private set; }

        public float LocalT => localT; // Get local interpolation position

        // Saved transform params
        private Vector3 anchoredPosition;
        private Vector2 rectSize;
        private Vector3 scale;
        private Vector3 angle;
        
        // Animation state params
        private float localT;
        private float targetT;
        private bool active;
        
        public float Speed { private get; set; } // Animation speed
        public Mode SpeedMode { private get; set; } // Animation speed mode
        
        // Setup target positions
        public Vector3 TargetPosition { 
            set => transformKeeper.AnchoredPositionTarget = value;
            get => transformKeeper.AnchoredPositionTarget;
        }

        // Setup target delta size
        public Vector3 TargetSizeDelta
        {
            set => transformKeeper.RectSizeTarget = value;
            get => transformKeeper.RectSizeTarget;
        }
        
        // Setup target angle
        public Vector3 TargetAngle
        {
            set => transformKeeper.AngleTarget = value;
            get => transformKeeper.AngleTarget;
        }

        // Setup target scale
        public Vector3 TargetScale
        {
            set => transformKeeper.ScaleTarget = value;
            get => transformKeeper.ScaleTarget;
        }

        //Enable debug of animation
        public bool EnableDebug { private get; set; }
        public bool EnableDebugJob { private get; set; }
        
        // Setup Rect Transform for animation
        public void SetRectTransformSync(RectTransform rect) => RectTransform = rect;
        
        // Prepare component to animate
        public void PrepareToWork(bool keepState = false)
        {
            if (RectTransform == null)
                return;
            
            anchoredPosition = RectTransform.anchoredPosition;
            rectSize = RectTransform.sizeDelta;
            scale = RectTransform.localScale;
            angle = RectTransform.localEulerAngles;
            
            if (keepState)
                return;
            
            localT = 1;
        }

        // Set default interpolation position
        public void SetDefaultT(float t) => localT = t;

        // Set target interpolation position
        public void SetTByDynamic(float t) => targetT = t;

        // Stop animate
        public void Stop() => active = false;

        // Start animate
        public void Run() => active = true;
        
        // Set from other places rect state by interpolation position
        public void SetT(float t) => JobSetT(t);
        
        // Set rect state by interpolation position
        private void JobSetT(float t)
        {
            if (RectTransform == null)
                return;
            
            if (transformKeeper.AnchoredPositionUse)
                RectTransform.anchoredPosition = Vector3.Lerp(transformKeeper.AnchoredPositionTarget, anchoredPosition, t);
            if (transformKeeper.RectSizeUse)
                RectTransform.sizeDelta = Vector3.Lerp(transformKeeper.RectSizeTarget, rectSize, t);
            if (transformKeeper.ScaleUse)
                RectTransform.localScale = Vector3.Lerp(transformKeeper.ScaleTarget, scale, t);
            if (transformKeeper.AngleUse)
                RectTransform.localEulerAngles = Vector3.Lerp(transformKeeper.AngleTarget, angle, t);

            DebugLogJob(t);
        }
        
        public void Start() { }

        // Updates each frame
        public void Update()
        {
            if (!active)
                return;

            DebugLog();
                
            if (Mathf.Abs(localT - targetT) < 0.001f)
                return;

            RunT();
            JobSetT(localT);
        }

        // Update interpolation position
        private void RunT()
        {
            switch (SpeedMode)
            {
                case Mode.Lerp:
                    localT = Mathf.Lerp(localT, targetT, Speed);
                    break;
                case Mode.Sum:
                    localT = (Mathf.Abs(localT - targetT)) > Speed ? 
                        localT > targetT ? localT - Speed : localT + Speed
                        : localT = targetT;
                    break;
            }
        }

        public void Dispose() => SyncWithBehaviour.Instance.RemoveObserver(this);

        private void DebugLog()
        {
            if (EnableDebug)
                Debug.Log("localT: " + localT + "; targetT: " + targetT);
        }
        
        public void DebugLogState() => Debug.Log("localT: " + localT + "; targetT: " + targetT);

        private void DebugLogJob(float t)
        {
            if (EnableDebugJob)
                Debug.Log("t: " + t);
        }

        // Set activity of rect object
        public void Enable(bool enable) => RectTransform.gameObject.SetActive(enable);
    }

    // Component for save target properties of Rect Transform
    public class TransformKeeper
    {
        private Vector3 anchoredPositionTarget;

        public bool AnchoredPositionUse { get; private set; }
        public Vector3 AnchoredPositionTarget
        {
            set
            {
                AnchoredPositionUse = true;
                anchoredPositionTarget = value;
            }
            get => anchoredPositionTarget;
        }

        private Vector3 rectSizeTarget;
        public bool RectSizeUse {get; private set;}
        public Vector3 RectSizeTarget 
        {
            set
            {
                RectSizeUse = true;
                rectSizeTarget = value;
            }
            get => rectSizeTarget;
        }
        
        
        private Vector3 scaleTarget;
        public bool ScaleUse {get; private set;}
        public Vector3 ScaleTarget
        {
            set
            {
                ScaleUse = true;
                scaleTarget = value;
            }
            get => scaleTarget;
        }
        
        private Vector3 angleTarget;
        public bool AngleUse {get; private set;}
        public Vector3 AngleTarget 
        {
            set
            {
                AngleUse = true;
                angleTarget = value;
            }
            get => angleTarget;
        }
    }
}
