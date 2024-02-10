using HomeTools.Source.Design;
using MainActivity.MainComponents;
using Unity.VectorGraphics;
using UnityEngine;

namespace MainActivity.AppBar
{
    // Component of app bar when create task page
    public class AppBarCreateFlowMode
    {
        // Animation components
        
        private readonly RectTransformSync syncMaterial;
        
        private readonly RectTransformSync syncProjectName;
        private readonly UIAlphaSync syncProjectNameAlpha;
        
        private readonly RectTransformSync syncProjectIcon;
        private readonly UIAlphaSync syncProjectIconAlpha;

        private readonly RectTransformSync syncNameField;
        private readonly UIAlphaSync syncNamePlaceholderAlpha;
        private readonly UIAlphaSync syncNameText;
        
        private readonly RectTransformSync syncLineField;
        private readonly UIAlphaSync syncLineAlpha;
        
        private readonly RectTransformSync syncIcon;
        private readonly UIAlphaSync syncIconAlpha;
        
        private readonly RectTransformSync syncEmptySignal;
        private readonly UIAlphaSync syncEmptySignalAlpha;
        
        // Create and setup
        public AppBarCreateFlowMode(RectTransform elementsRect)
        {
            syncMaterial = AppBarFlowModeCreate.CreateBarPlane();
            syncProjectName = AppBarFlowModeCreate.CreateProjectNameSync(elementsRect);
            syncProjectNameAlpha = AppBarFlowModeCreate.CreateProjectNameAlpha(elementsRect);
            syncProjectIcon = AppBarFlowModeCreate.CreateProjectIconSync(elementsRect);
            syncProjectIconAlpha = AppBarFlowModeCreate.CreateProjectIconAlpha(elementsRect);

            var inputField = elementsRect.Find("CreateFlow Name Input").GetComponent<RectTransform>();
            syncNameField = AppBarFlowModeCreate.CreateNameFieldSync(elementsRect, syncProjectName, inputField);
            syncNamePlaceholderAlpha = AppBarFlowModeCreate.CreateNameFieldPlaceholderAlpha(elementsRect, inputField);
            syncNameText = AppBarFlowModeCreate.CreateNameFieldTextAlpha(elementsRect, inputField);
            
            var inputLine = SceneResources.Get("CreateFlow Name Line").GetComponent<RectTransform>();
            syncLineField = AppBarFlowModeCreate.CreateLineFieldSync(elementsRect, syncProjectName, inputLine);
            syncLineAlpha = AppBarFlowModeCreate.CreateLineFieldAlpha(elementsRect, inputLine);
            
            var flowIcon = elementsRect.Find("CreateFlow Icon").GetComponent<RectTransform>();
            syncIcon = AppBarFlowModeCreate.CreateIconSync(elementsRect, flowIcon);
            syncIconAlpha = AppBarFlowModeCreate.CreateIconAlpha(elementsRect, flowIcon);
            
            var emptyIcon = SceneResources.Get("EmptySignal").GetComponent<SVGImage>();
            syncEmptySignalAlpha = AppBarFlowModeCreate.CreateEmptySignalAlpha(elementsRect, syncNameField, syncProjectIcon, emptyIcon);
            syncEmptySignal = AppBarFlowModeCreate.CreateEmptySignalSync(emptyIcon.rectTransform);
        }

        // Animate app bar when open create task page
        public void OpenFlowMode()
        {
            syncMaterial.TargetPosition = new Vector3(0, -AppBarMaterial.RectTransform.sizeDelta.y / 4, 0);
            syncMaterial.SetTByDynamic(0);
            syncMaterial.Run();
            
            syncProjectName.SetTByDynamic(0);
            syncProjectName.Run();
            
            syncProjectIcon.SetTByDynamic(0);
            syncProjectIcon.Run();

            syncProjectIconAlpha.SetAlphaByDynamic(0f);
            syncProjectIconAlpha.Run();
            
            syncProjectNameAlpha.SetAlphaByDynamic(0.27f);
            syncProjectNameAlpha.Run();

            syncNamePlaceholderAlpha.Speed = 0.4f;
            syncNamePlaceholderAlpha.SetAlphaByDynamic(1f);
            syncNamePlaceholderAlpha.Run();
            
            syncNameField.SetT(0);
            syncNameField.SetDefaultT(0);
            syncNameField.SetTByDynamic(1);
            syncNameField.Run();
            
            syncNameText.Speed = 0.4f;
            syncNameText.SetAlphaByDynamic(1f);
            syncNameText.Run();
            
            syncLineField.SetT(0);
            syncLineField.SetDefaultT(0);
            syncLineField.SetTByDynamic(1);
            syncLineField.Run();
            
            syncLineAlpha.SetAlphaByDynamic(1f);
            syncLineAlpha.Run();
            
            syncIcon.SetT(0);
            syncIcon.SetDefaultT(0);
            syncIcon.SetTByDynamic(1);
            syncIcon.Run();
            
            syncIconAlpha.SetAlphaByDynamic(1f);
            syncIconAlpha.Run();
            
            syncEmptySignalAlpha.SetAlpha(0);
            syncEmptySignalAlpha.SetDefaultAlpha(0);
            syncEmptySignalAlpha.Stop();
            
            syncEmptySignal.SetT(0);
            syncEmptySignal.SetDefaultT(0);
            syncEmptySignal.Stop();
        }
        
        // Animate app bar when close create task page
        public void CloseFlowMode()
        {
            syncMaterial.TargetPosition = new Vector3(0, -AppBarMaterial.RectTransform.sizeDelta.y / 4, 0);
            syncMaterial.SetTByDynamic(1);
            syncMaterial.Run();
            
            syncProjectName.SetTByDynamic(1);
            syncProjectName.Run();
            
            syncProjectIcon.SetTByDynamic(1);
            syncProjectIcon.Run();
            
            syncProjectIconAlpha.SetAlphaByDynamic(1);
            syncProjectIconAlpha.Run();
            
            syncProjectNameAlpha.SetAlphaByDynamic(1);
            syncProjectNameAlpha.Run();
            
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

        // Animate empty entered name state
        public void EmptySignal()
        {
            syncNamePlaceholderAlpha.Speed = 0.03f;
            syncNamePlaceholderAlpha.SetAlpha(0f);
            syncNamePlaceholderAlpha.SetDefaultAlpha(0f);
            syncNamePlaceholderAlpha.SetAlphaByDynamic(1f);
            syncNamePlaceholderAlpha.Run();
            
            syncEmptySignalAlpha.Speed = 0.05f;
            syncEmptySignalAlpha.SetAlpha(1);
            syncEmptySignalAlpha.SetDefaultAlpha(1);
            syncEmptySignalAlpha.SetAlphaByDynamic(0);
            syncEmptySignalAlpha.Run();
            
            syncEmptySignal.SetT(1);
            syncEmptySignal.SetDefaultT(1);
            syncEmptySignal.SetTByDynamic(0);
            syncEmptySignal.Run();
        }

        public RectTransformSync GetMaterialComponent() => syncMaterial;
        
        public (RectTransformSync syncNameField, UIAlphaSync syncNamePlaceholderAlpha, UIAlphaSync syncNameText) GetNameInputComponents() 
                                                                        => (syncNameField, syncNamePlaceholderAlpha, syncNameText);
        
        public (RectTransformSync syncLineField, UIAlphaSync syncLineAlpha) GetLineComponents() => (syncLineField, syncLineAlpha);
        
        public (RectTransformSync syncIcon, UIAlphaSync syncIconAlpha) GetIconComponents() => (syncIcon, syncIconAlpha);
        
        public (RectTransformSync syncEmptySignal, UIAlphaSync syncEmptySignalAlpha) GetEmptySignalComponents() => (syncEmptySignal, syncEmptySignalAlpha);
    }
}
