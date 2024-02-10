using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TaskViewArea.NormalView
{
    // Component for circle around icons
    public class IconColorCircles
    {
        private readonly Image[] images; // UI circles
        private float minAlpha = 0.37f; // Min alpha of circles
        private float maxAlpha = 0.77f; // Max alpha of circles

        // Create
        public IconColorCircles(params Image[] images) => this.images = images;
        
        // Update colors of circles with random alpha
        public void UpdateColors(Color color)
        {
            foreach (var image in images)
            {
                var colorSet = Color.Lerp(color, Color.white, Random.Range(0f, 0.37f));
                colorSet.a = Random.Range(minAlpha, maxAlpha);
                image.color = colorSet;
            }
        }
    }
}
