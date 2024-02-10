using Architecture;
using Architecture.Components;
using Architecture.Data;
using Architecture.EditGroupModule;
using Architecture.Elements;
using Architecture.MenuArea;
using HomeTools.Messenger;
using UnityEngine;
using UnityEngine.UI;

namespace MainActivity.AppBar
{
    // Component of title of main page
    public class AppBarEditTitle
    {
        // Link to edit title view
        private UpdateTitleModule EditGroupModule() => AreasLocator.Instance.UpdateTitleModule;
        // Link to app bar component
        private AppBar AppBar() => AreasLocator.Instance.AppBar;
        // Link to menu view
        private MenuArea MenuArea() => AreasLocator.Instance.MenuArea;
        
        private readonly Image handlerImage; // Handle object
        private readonly MainButtonJob mainButtonJob; // Button component
        // For run action when user touches home button on device
        private readonly ActionsQueue actionsQueue;

        // States of title
        private bool opened;
        private bool active;

        public bool Active
        {
            get => active;
            set
            {
                mainButtonJob.HandleObject.GameObject.SetActive(value);
                active = value;
            }
        }
        
        // Create and setup
        public AppBarEditTitle(Image handler, ActionsQueue actionsQueue)
        {
            handlerImage = handler;
            this.actionsQueue = actionsQueue;
            
            mainButtonJob = new MainButtonJob(handlerImage.rectTransform, Touched, handlerImage.gameObject);
            mainButtonJob.Reactivate();
            mainButtonJob.AttachToSyncWithBehaviour();
        }

        // Touched title
        private void Touched()
        {
            if (!Active)
                return;
            
            if (actionsQueue.HasActions)
            {
                actionsQueue.InvokeLastAction();
                return;
            }
            
            opened = EditGroupModule().Opened;
            
            MenuArea().Close();
            if (opened) ClosePanel();
            else OpenPanel();
        }

        // Open edit title view
        private void OpenPanel()
        {
            opened = true;
            EditGroupModule().Open(GetWorldAnchoredPosition(), new EditGroupSession(AppData.GetCurrentGroup(), ClosePanel));
        }

        // Close edit title view
        private void ClosePanel()
        {
            opened = false;
            mainButtonJob.Reactivate();
            AppBar().UpdateProjectTitleByNewInfo();
            MainMessenger.SendMessage(AppMessagesConst.BlackoutEditGroupClicked);
        }
        
        // Position for edit title view when open
        private Vector2 GetWorldAnchoredPosition()
        {
            var rt = AppBar().GroupTitle.Icon.rectTransform;
            var parent = rt.transform.parent;

            rt.transform.SetParent(MainCanvas.RectTransform);
            var result = new Vector2(-300, rt.anchoredPosition.y);
            rt.transform.SetParent(parent);

            return result;
        }
    }
}
