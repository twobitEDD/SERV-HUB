using System;

namespace Architecture.TipModule
{
    // This class contains information about opened state of tip view
    public class TipSession
    {
        private readonly Action closeAction;
        public readonly string Title;
        public readonly TipsContentContainer.Type Type; // Tip type to view

        // Create accept session
        public TipSession(Action closeAction, string title, TipsContentContainer.Type type)
        {
            this.closeAction = closeAction;
            Title = title;
            Type = type;
        }
        
        // Finish session
        public void Closed()
        {
            closeAction?.Invoke();
        }
    }
}
