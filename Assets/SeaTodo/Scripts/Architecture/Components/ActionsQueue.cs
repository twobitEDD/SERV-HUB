using System;
using System.Collections.Generic;

namespace Architecture.Components
{
    // Component for queue of actions (Can use for home device button when need close pages)
    public class ActionsQueue
    {
        // List of actions
        private readonly List<Action> actionsQueue = new List<Action>();

        // Add action to list
        public void AddAction(Action action) => actionsQueue.Add(action);
        
        // Remove action from list
        public void RemoveAction(Action action)
        {
            if (actionsQueue.Contains(action))
                actionsQueue.Remove(action);
        }

        // Check if queue contains actions
        public bool HasActions => actionsQueue.Count > 0;

        // Invoke last action from list
        public void InvokeLastAction()
        {
            // Check if has actions
            if (actionsQueue.Count == 0)
                return;

            // Get action
            var last = actionsQueue[actionsQueue.Count - 1];
            // Invoke action
            last.Invoke();
            // Remove action from list
            actionsQueue.Remove(last);
        }
    }
}
