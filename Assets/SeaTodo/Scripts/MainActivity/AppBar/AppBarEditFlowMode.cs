using HomeTools.Source.Design;
using UnityEngine;

namespace MainActivity.AppBar
{
    // Component of app bar when edit task page
    public class AppBarEditFlowMode
    {
        // Animation components
        
        private readonly RectTransformSync syncMaterial;
        
        private readonly RectTransformSync syncNameField;
        private readonly UIAlphaSync syncNamePlaceholderAlpha;
        private readonly UIAlphaSync syncNameText;
        
        private readonly RectTransformSync syncLineField;
        private readonly UIAlphaSync syncLineAlpha;

        private readonly RectTransformSync syncIcon;
        private readonly UIAlphaSync syncIconAlpha;
        
        private readonly RectTransformSync syncEmptySignal;
        private readonly UIAlphaSync syncEmptySignalAlpha;
        
        private readonly UIAlphaSync syncBackAlpha;
        private readonly RectTransformSync syncBack;
        
        private readonly UIAlphaSync syncFlowNameAlpha;
        private readonly RectTransformSync syncFlowName;

        // Create and setup
        public AppBarEditFlowMode(AppBarCreateFlowMode appBarCreateFlowMode, AppBarViewFlowMode appBarViewFlowMode, RectTransform mainElements)
        {
            var backComponents = appBarViewFlowMode.GetBackComponents();
            syncBackAlpha = backComponents.alphaSync;
            syncBack = backComponents.rectSync;
            
            syncMaterial = appBarCreateFlowMode.GetMaterialComponent();
            
            var inputNameComponents = appBarCreateFlowMode.GetNameInputComponents();
            syncNameField = inputNameComponents.syncNameField;
            syncNamePlaceholderAlpha = inputNameComponents.syncNamePlaceholderAlpha;
            syncNameText = inputNameComponents.syncNameText;

            var flowNameComponents = appBarViewFlowMode.GetFlowNameComponent();
            syncFlowNameAlpha = flowNameComponents.syncFlowNameAlpha;
            syncFlowName = AppBarEditFlowModeCreate.CreateNameSync(flowNameComponents.text.rectTransform, syncNameField);
            
            var lineComponents = appBarCreateFlowMode.GetLineComponents();
            syncLineField = lineComponents.syncLineField;
            syncLineAlpha = lineComponents.syncLineAlpha;

            var iconComponents = appBarCreateFlowMode.GetIconComponents();
            syncIcon = iconComponents.syncIcon;
            syncIconAlpha = iconComponents.syncIconAlpha;

            var emptySignalComponents = appBarCreateFlowMode.GetEmptySignalComponents();
            syncEmptySignal = emptySignalComponents.syncEmptySignal;
            syncEmptySignalAlpha = emptySignalComponents.syncEmptySignalAlpha;
        }

        // Animate app bar when open edit task page
        public void OpenFlowMode()
        {
            syncMaterial.SetTByDynamic(0);
            syncMaterial.Run();
            
            syncFlowNameAlpha.Speed = 0.3f;
            syncFlowNameAlpha.SetAlphaByDynamic(0);
            syncFlowNameAlpha.Run();
            
            syncBackAlpha.Speed = 0.4f;
            syncBackAlpha.SetAlphaByDynamic(0);
            syncBackAlpha.Run();
            syncBackAlpha.UpdateEnableByAlpha = true;
            
            syncBack.Speed = 0.4f;
            syncBack.SetTByDynamic(0);
            syncBack.Run();
            
            syncNamePlaceholderAlpha.Speed = 0.4f;
            syncNamePlaceholderAlpha.SetAlphaByDynamic(1f);
            syncNamePlaceholderAlpha.Run();
            
            syncNameField.SetT(0);
            syncNameField.SetDefaultT(0);
            syncNameField.SetTByDynamic(1);
            syncNameField.Run();

            syncNameText.SetAlpha(0);
            syncNameText.SetDefaultAlpha(0);
            syncNameText.Speed = 0.1f;
            syncNameText.SetAlphaByDynamic(1f);
            syncNameText.Run();
            
            syncLineField.SetT(0);
            syncLineField.SetDefaultT(0);
            syncLineField.SetTByDynamic(1);
            syncLineField.Run();

            syncLineAlpha.SetAlphaByDynamic(1f);
            syncLineAlpha.Run();

            syncIcon.SetDefaultT(0);
            syncIcon.SetT(0);
            syncIcon.SetTByDynamic(1f);
            syncIcon.Run();

            syncIconAlpha.SetAlpha(0);
            syncIconAlpha.SetDefaultAlpha(0);
            syncIconAlpha.SetAlphaByDynamic(1f);
            syncIconAlpha.Run();
            
            syncEmptySignalAlpha.SetAlpha(0);
            syncEmptySignalAlpha.SetDefaultAlpha(0);
            syncEmptySignalAlpha.Stop();
            
            syncEmptySignal.SetT(0);
            syncEmptySignal.SetDefaultT(0);
            syncEmptySignal.Stop();
        }
        
        // Animate app bar when close ecit task page
        public void CloseFlowMode()
        {
            syncMaterial.SetTByDynamic(1);
            syncMaterial.Run();
            
            syncFlowNameAlpha.Speed = 0.2f;
            syncFlowNameAlpha.SetAlphaByDynamic(1);
            syncFlowNameAlpha.Run();
            
            syncBackAlpha.EnableByAlpha(1);
            syncBackAlpha.Speed = 0.1f;
            syncBackAlpha.SetAlphaByDynamic(1);
            syncBackAlpha.Run();
            
            syncBack.Speed = 0.4f;
            syncBack.SetT(0);
            syncBack.SetDefaultT(0);
            syncBack.SetTByDynamic(1);
            syncBack.Run();
            
            syncNamePlaceholderAlpha.Speed = 0.4f;
            syncNamePlaceholderAlpha.SetAlphaByDynamic(0);
            syncNamePlaceholderAlpha.Run();
            
            syncNameField.SetTByDynamic(0);
            syncNameField.Run();
            
            syncNameText.Speed = 0.4f;
            syncNameText.SetAlphaByDynamic(0);
            syncNameText.Run();
            
            syncLineField.SetTByDynamic(0f);
            syncLineField.Run();
            
            syncLineAlpha.SetAlphaByDynamic(0f);
            syncLineAlpha.Run();
            
            syncIcon.SetTByDynamic(0);
            syncIcon.Run();
            
            syncIconAlpha.SetAlphaByDynamic(0f);
            syncIconAlpha.Run();
            
            syncEmptySignalAlpha.Speed = 0.4f;
        }
    }
}
