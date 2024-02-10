using Architecture.DaysMarkerArea.DaysColors.UpdateModule;
using HomeTools.Messenger;
using UnityEngine;

namespace Architecture.DaysMarkerArea.DaysColors
{
    // Component that contains list of day characteristics
    public class DaysPalette
    {
        // Rect of list
        private readonly RectTransform rectTransform;
        // Component for using colors of day characteristics
        private readonly ColorMarkersDescriptor colorMarkersDescriptor;
        // View for update name of characteristics
        private readonly UpdateMarkerModule updateMarkerModule;
        // Array of characteristic items 
        private PaletteItem[] paletteItems;
        
        // Count of characteristic items
        private const int paletteCount = 5;
        // Distance between characteristic items
        private const float paletteItemsDistance = 112f;
        
        // Create main components
        public DaysPalette(RectTransform rectTransform)
        {
            // Save rect of day characteristics part
            this.rectTransform = rectTransform;
            // Create color descriptor
            colorMarkersDescriptor = new ColorMarkersDescriptor();
            // Create view for rename characteristics
            updateMarkerModule = new UpdateMarkerModule();
            // Create view of list of day characteristics
            GenerateItems();
            // Add to messenger method of update characteristic items
            MainMessenger.AddMember(Update, AppMessagesConst.ShouldUpdateColorsCountInCalendar);
        }

        // Update characteristic items immediately
        public void Update()
        {
            foreach (var paletteItem in paletteItems)
                paletteItem.UpdateImmediately();
        }

        // Create visual view of items
        private void GenerateItems()
        {
            paletteItems = new PaletteItem[paletteCount];
            // Find example of item inside part
            var example = rectTransform.Find("DayMarker Item").GetComponent<RectTransform>();

            // Create items and setup
            for (var i = 0; i < paletteCount; i++)
            {
                var itemObject = Object.Instantiate(example, rectTransform);
                itemObject.gameObject.SetActive(true);
                itemObject.anchoredPosition = new Vector2(0, example.anchoredPosition.y - paletteItemsDistance * i);
                
                var item = new PaletteItem(itemObject, colorMarkersDescriptor, updateMarkerModule, i);
                item.UpdateImmediately();
                paletteItems[i] = item;
            }
        }
    }
}
