using HomeTools.Source.Design;
using UnityEngine;

namespace Architecture.TutorialArea.TutorialElements
{
    // Interface for page of tutorial
    public interface ITutorialItem
    {
        RectTransform RectTransform(); // Get rect object of page
        UIAlphaSync UIAlphaSync(); // Get animation component of page
        void ResetToDefault(); // Reset page to default step
        void SetActive(bool active); // Set activity of page
    }
}
