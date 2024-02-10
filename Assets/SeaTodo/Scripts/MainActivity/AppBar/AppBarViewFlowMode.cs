using System;
using Architecture.TextHolder;
using HomeTools.Handling;
using HomeTools.Source.Design;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MainActivity.AppBar
{
    // Component for create animation components for view task page
    public class AppBarViewFlowMode
    {
        // Animation components
        
        private readonly UIAlphaSync syncProjectNameAlpha;

        public readonly Text FlowName;
        private readonly UIAlphaSync syncFlowNameAlpha;

        private readonly UIAlphaSync syncIconAlpha;

        private readonly UIAlphaSync syncBackAlpha;
        private readonly RectTransformSync syncBack;

        private readonly HandleObject handleObjectBack;
        private readonly GameObject handleGameObject;
        
        // Create and setup
        public AppBarViewFlowMode(RectTransform elementsRect, Action closeAction)
        {
            var projectName = elementsRect.Find("BarTitle").GetComponent<Text>();
            syncProjectNameAlpha = AppBarViewFlowModeCreate.CreateProjectNameAlpha(elementsRect, projectName);

            FlowName = AppBarViewFlowModeCreate.CreateFlowName(projectName);
            syncFlowNameAlpha = AppBarViewFlowModeCreate.CreateFlowNameAlpha(FlowName);

            var projectIcon = elementsRect.Find("Image").GetComponent<SVGImage>();
            syncIconAlpha = AppBarViewFlowModeCreate.CreateIconAlpha(projectIcon);

            var backArrow = elementsRect.Find("ArrowButton");

            var backIcon = backArrow.Find("Icon").GetComponent<SVGImage>();
            syncBackAlpha = AppBarViewFlowModeCreate.CreateBackAlpha(backIcon);
            syncBack = AppBarViewFlowModeCreate.CreateBackSync(backIcon.rectTransform);

            handleGameObject = backArrow.Find("Handle").gameObject;
            handleObjectBack = new HandleObject(handleGameObject);
            handleObjectBack.AddEvent(EventTriggerType.PointerClick, closeAction);
        }

        // Prepare to work
        public void PrepareToWork()
        {
            var color = FlowName.color;
            color.a = 0;
            FlowName.color = color;
        }
        
        // Set name key for localization
        public void SetNameByKey(string key) => TextLocalization.Instance.AddLocalization(FlowName, key);
        
        // Set name
        public void SetName(string name) => FlowName.text = name;

        // Animate app bar when open view task page
        public void OpenFlowView()
        {
            syncProjectNameAlpha.Speed = 0.4f;
            syncProjectNameAlpha.SetAlphaByDynamic(0f);
            syncProjectNameAlpha.Run();
            
            syncFlowNameAlpha.Speed = 0.1f;
            syncFlowNameAlpha.SetAlphaByDynamic(1);
            syncFlowNameAlpha.Run();
            
            syncIconAlpha.Speed = 0.4f;
            syncIconAlpha.SetAlphaByDynamic(0);
            syncIconAlpha.Run();
            
            syncBackAlpha.EnableByAlpha(1);
            syncBackAlpha.Speed = 0.1f;
            syncBackAlpha.SetAlphaByDynamic(1);
            syncBackAlpha.Run();
            
            syncBack.Speed = 0.4f;
            syncBack.SetT(0);
            syncBack.SetDefaultT(0);
            syncBack.SetTByDynamic(1);
            syncBack.Run();
            
            handleObjectBack.GameObject.SetActive(true);
            SetActiveHandle(true);
        }

        // Animate app bar when close view task page
        public void CloseFlowView()
        {
            syncProjectNameAlpha.Speed = 0.1f;
            syncProjectNameAlpha.SetAlphaByDynamic(1f);
            syncProjectNameAlpha.Run();
            
            syncFlowNameAlpha.Speed = 0.4f;
            syncFlowNameAlpha.SetAlphaByDynamic(0);
            syncFlowNameAlpha.Run();
            
            syncIconAlpha.Speed = 0.1f;
            syncIconAlpha.SetAlphaByDynamic(1f);
            syncIconAlpha.Run();
            
            syncBackAlpha.Speed = 0.4f;
            syncBackAlpha.SetAlphaByDynamic(0);
            syncBackAlpha.Run();
            syncBackAlpha.UpdateEnableByAlpha = true;
            
            syncBack.Speed = 0.4f;
            syncBack.SetTByDynamic(0);
            syncBack.Run();
            
            handleObjectBack.GameObject.SetActive(false);
        }

        public (UIAlphaSync alphaSync, RectTransformSync rectSync) GetBackComponents() => (syncBackAlpha, syncBack);
        
        public (UIAlphaSync syncFlowNameAlpha, Text text) GetFlowNameComponent() => (syncFlowNameAlpha, FlowName);

        public void SetActiveHandle(bool status) => handleGameObject.SetActive(status);
    }
}
