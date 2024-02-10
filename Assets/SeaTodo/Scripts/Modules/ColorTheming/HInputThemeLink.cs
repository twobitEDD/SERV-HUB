using HomeTools.Input;
using Theming;
using UnityEngine;

namespace Modules.ColorTheming
{
    // Component for link input field to App theming module 
    public class HInputThemeLink : MonoBehaviour
    {
        public ColorTheme selectionColor;
        private void Start()
        {
            var input = GetComponent<HInputField>();
            
            if (input == null)
                return;

            input.SelectionColor = selectionColor;
            AppTheming.AddInputField(input);
            
            Destroy(this);
        }
    }
}
