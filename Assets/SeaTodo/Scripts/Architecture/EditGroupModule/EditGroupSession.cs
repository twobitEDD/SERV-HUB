using System;
using Architecture.Data;
using Architecture.EditGroupModule.StatisticEditGroup;

namespace Architecture.EditGroupModule
{
    // Component of edit title session
    public class EditGroupSession
    {
        public readonly FlowGroup FlowGroup; // Global package of tasks 
        public int SelectedIconLocal; // Id of icon
        
        private readonly Action closedAction; // Action that is called when the session ends
        private EditGroupItem editGroupItemLocal; // Selected icon item in list of icons

        // Create and setup
        public EditGroupSession(FlowGroup flowGroup, Action closedAction)
        {
            this.closedAction = closedAction;
            FlowGroup = flowGroup;
            SelectedIconLocal = flowGroup.Icon;
        }

        // Call when new icon selected and start animation process for old selected icon
        public void UpdateSelectedIcon(int selectedIcon, EditGroupItem editGroupItem)
        {
            // Deactivate old selected icon
            if (editGroupItemLocal != null && editGroupItemLocal != editGroupItem)
            {
                editGroupItemLocal.DeselectColor();
                
                editGroupItemLocal.RectTransformSync.Speed = 0.27f;
                editGroupItemLocal.RectTransformSync.SetTByDynamic(1);
                editGroupItemLocal.RectTransformSync.Run();

                editGroupItemLocal.LocalActive = false;
            }
            // Save new chosen icon
            editGroupItemLocal = editGroupItem;
            SelectedIconLocal = selectedIcon;
        }

        // Stop animation of icon. Needed when the view closes
        public void StopItemsInSession()
        {
            editGroupItemLocal?.UIColorSync.Stop();
            editGroupItemLocal?.UIColorSync.SetDefaultMarker(1);
            editGroupItemLocal?.RectTransformSync.Stop();
            editGroupItemLocal?.RectTransformSync.SetT(0);
            editGroupItemLocal?.RectTransformSync.SetDefaultT(0);
        }

        // Call action when session finished
        public void FinishSession(string editedName)
        {
            FlowGroup.Name = editedName != string.Empty ? editedName : FlowGroup.Name;
            FlowGroup.Icon = SelectedIconLocal;
            closedAction.Invoke();
        }
    }
}
