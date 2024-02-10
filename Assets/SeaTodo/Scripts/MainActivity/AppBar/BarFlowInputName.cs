using Architecture.TextHolder;
using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace MainActivity.AppBar
{
    // Component for input task name in app bar
    public class BarFlowInputName
    {
        private readonly InputField flowNameText; // Input field
        private readonly Text placeholder; // Placeholder text component
        private readonly Image inputHandler; // Image of input handler

        // Create and setup
        public BarFlowInputName(Transform bar)
        {
            flowNameText = SceneResources.Get("CreateFlow Name Input").GetComponent<InputField>();
            flowNameText.transform.SetParent(bar);
            placeholder = flowNameText.transform.Find("Placeholder").GetComponent<Text>();
            inputHandler = flowNameText.GetComponent<Image>();
        }

        // Localize placeholder
        public void Setup()
        {
            TextLocalization.Instance.AddLocalization(placeholder, TextKeysHolder.Name);
        }

        public bool HasText() => flowNameText.text.Length > 0;

        public string GetName() => flowNameText.text;

        public void SetupNameText(string text) => flowNameText.text = text;
        
        // Start new input session
        public void StartNewSession()
        {
            flowNameText.text = string.Empty;
            flowNameText.enabled = true;
            inputHandler.enabled = true;
        }
        
        // Finish input session name
        public void FinishSession()
        {
            flowNameText.enabled = false;
            inputHandler.enabled = false;
        }
    }
}
