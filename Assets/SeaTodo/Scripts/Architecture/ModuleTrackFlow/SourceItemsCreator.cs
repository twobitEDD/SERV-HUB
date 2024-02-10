using HomeTools.Source.Design;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleTrackFlow
{
    // Component that creates objects for track time view
    public static class SourceItemsCreator
    {
        // Create text objects
        public static Text[] CreateTextElements(RectTransform pool, int elementsCount)
        {
            var texts = new Text[elementsCount];

            texts[0] = pool.Find("Item").GetComponent<Text>();
            
            for (var i = 1; i < elementsCount; i++)
            {
                var textObject = Object.Instantiate(texts[0], pool, true);
                textObject.gameObject.name = $"Item {i}";
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
    }
}