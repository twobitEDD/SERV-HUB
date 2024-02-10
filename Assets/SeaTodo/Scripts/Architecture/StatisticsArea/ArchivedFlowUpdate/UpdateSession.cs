using System;
using UnityEngine;

namespace Architecture.StatisticsArea.ArchivedFlowUpdate
{
    // Component of update archived task session
    public class UpdateSession
    {
        private readonly Action acceptAction; // Invoke action when activate task
        private readonly Action removeAction; // Invoke action when remove task
        // Position of close view
        public readonly Vector2 CloseTargetPosition; 
        public readonly bool ChangeCloseTargetPosition;
        
        public UpdateSession(Action acceptAction, Action removeAction)
        {
            this.acceptAction = acceptAction;
            this.removeAction = removeAction;
        }
        
        public UpdateSession(Action acceptAction, Action removeAction, Vector2 closeTargetPosition) 
            : this(acceptAction, removeAction)
        {
            CloseTargetPosition = closeTargetPosition;
            ChangeCloseTargetPosition = true;
        }

        // Invoke accepted action
        public void Accepted()
        {
            acceptAction?.Invoke();
        }
        
        // Invoke remove action
        public void Removed()
        {
            removeAction?.Invoke();
        }
    }
}
