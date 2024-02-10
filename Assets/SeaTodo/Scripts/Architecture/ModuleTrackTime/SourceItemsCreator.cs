using System.Collections.Generic;
using HomeTools.Source;
using HomeTools.Source.Design;
using Packages.HomeTools.Source.Design;
using Theming;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleTrackTime
{
    // Component that creates objects for track time view
    public static class SourceItemsCreator
    {
        // Create text objects
        public static Text[] CreateTextElements(RectTransform pool, int elementsCount, string itemName)
        {
            var texts = new Text[elementsCount];

            texts[0] = pool.Find(itemName).GetComponent<Text>();
            
            for (var i = 1; i < elementsCount; i++)
            {
                var textObject = Object.Instantiate(texts[0], pool, true);
                textObject.gameObject.name = $"{itemName} {i}";
                texts[i] = textObject;
            }

            return texts;
        }

        // Get rect components from text
        public static RectTransform[] GetRectElements(Text[] elements)
        {
            var rectElements = new RectTransform[elements.Length];

            for (var i = 0; i < elements.Length; i++)
                rectElements[i] = elements[i].GetComponent<RectTransform>();

            return rectElements;
        }

        // Create animation components of alpha channel for texts
        public static UIAlphaSync[] GetSyncsByText(Text[] texts)
        {
            var result = new UIAlphaSync[texts.Length];
            
            for (var i = 0; i < texts.Length; i++)
            {
                result[i] = new UIAlphaSync();
                result[i].AddElement(texts[i]);
                result[i].PrepareToWork();
            }

            return result;
        }

        // Add text items to Color Theming module
        public static void SetItemsToTheme(IEnumerable<Text> texts)
        {
            foreach (var text in texts)
                AppTheming.AddElement(text, ColorTheme.SecondaryColor, AppTheming.AppItem.TimeTrackModule);
        }
        
    }
}