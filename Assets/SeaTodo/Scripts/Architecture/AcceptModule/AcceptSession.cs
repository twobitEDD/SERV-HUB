using System;
using UnityEngine;

namespace Architecture.AcceptModule
{
    // This class contains information about opened state of accept view
    public class AcceptSession
    {
        private readonly Action closeAction;
        private readonly Action acceptAction;
        public readonly string Title;
        public readonly Vector2 CloseTargetPosition;
        public readonly bool ChangeCloseTargetPosition;
        
        // Create accept session
        public AcceptSession(Action closeAction, Action acceptAction, string title)
        {
            this.closeAction = closeAction;
            this.acceptAction = acceptAction;
            Title = title;
        }
        
        // Create accept session
        public AcceptSession(Action closeAction, Action acceptAction, string title, Vector2 closeTargetPosition)
        {
            this.closeAction = closeAction;
            this.acceptAction = acceptAction;
            this.CloseTargetPosition = closeTargetPosition;
            ChangeCloseTargetPosition = true;
            Title = title;
        }

        // Run when accepted in view
        public void Accepted()
        {
            acceptAction?.Invoke();
        }

        // Run when closed without accept
        public void Closed()
        {
            closeAction?.Invoke();
        }
    }
}
