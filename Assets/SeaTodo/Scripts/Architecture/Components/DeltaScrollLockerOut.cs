using HomeTools.Handling;
using HTools;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Architecture.Components
{
    // Class that give activity bool by sum of delta touched
    public class DeltaScrollLockerOut : IBehaviorSync
    {
        private readonly float deltaLimit; // Limit of delta move
        private float collectedDelta; // Collected delta move
        private bool active; // active check
        public bool Access; // Give access if delta move less than limit

        // Object to check touched
        private readonly HandleObject handleObject;

        // Create and save delta limit
        public DeltaScrollLockerOut(float deltaLimit)
        {
            this.deltaLimit = deltaLimit;
        }
        
        public void Start() { }

        // Start check moving of finger
        public void StartCheck()
        {
            active = true;
            Access = true;
            collectedDelta = 0;
        }

        // Stop check process
        public void EndCheck() => active = false;

        public void Update()
        {
            if (!active)
                return;
            
            // Collect delta of moving
            if (collectedDelta < deltaLimit)
            {
                collectedDelta += Mathf.Abs(InputHS.DeltaMove.x);
                collectedDelta += Mathf.Abs(InputHS.DeltaMove.y);
            }
            
            // Check collected delta by limit
            if (collectedDelta < deltaLimit)
                return;

            // Update access state
            Access = false;
            active = false;
        }
    }
}
