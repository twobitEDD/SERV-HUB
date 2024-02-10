using HTools;
using UnityEngine;

namespace Architecture.Pages
{
    // Mathematical calculations of smooth page movement
    public static class PageScrollTools
    {
        // Smooth page movement params
        private const float normalDeviation = 27;
        private const float densitySplit = 0.47f;
        private const float maxDeviation = 97;
        private const float maxDeviationClamp = 157;
        private const float densityScrollDividerWhenFree = 5f;
        private const float densityScrollSecondPartMultiplier = 0.57f;
        private const float infinityClampScroll = 0.85f;
        private static float currentMaxDeviation;

        // Page scroll component
        private static readonly PageScroll pageScroll;
        
        static PageScrollTools()
        {
            pageScroll = PageScroll.Instance;
        }

        // Calculate page moving
        public static float MoveCalculation(float delta, float position, float minAnchor, float maxAnchor)
        {
            currentMaxDeviation = maxDeviation;

            FullDeviationControl(delta, ref position, minAnchor, maxAnchor);
            AddDeltaWhenFree(delta, ref position, minAnchor, maxAnchor);
            return position;
        }
        
        // Control of deviation
        private static void FullDeviationControl(float delta, ref float position, float minAnchor, float maxAnchor)
        {
            if (position > maxAnchor && delta > 0)
            {
                if (position > maxAnchor + normalDeviation)
                    delta = DeltaSecondDensity(delta, position, maxAnchor - normalDeviation);
                else
                    delta = DeltaFirstDensity(delta, position, maxAnchor);
            }
            
            if (position < minAnchor && delta < 0)
            {
                if (position < minAnchor - normalDeviation)
                    delta = DeltaSecondDensity(delta, position, minAnchor + normalDeviation);
                else
                    delta = DeltaFirstDensity(delta, position, minAnchor);
            }

            delta = DeviationDeltaClamped(delta, position, minAnchor, maxAnchor);
            
            position += delta;
        }
    
        // Add additional delta when move without touch
        private static void AddDeltaWhenFree(float delta, ref float position, float minAnchor, float maxAnchor)
        {
            if (InputHS.Touched)
                return;
            
            if (position > maxAnchor)
                position -= (position - maxAnchor) * 2 / currentMaxDeviation * (normalDeviation / densityScrollDividerWhenFree);

            if (position < minAnchor)
                position -= (position - minAnchor) * 2 / currentMaxDeviation * (normalDeviation / densityScrollDividerWhenFree);
        }
    
        // Calculate delta when out of borders
        private static float DeltaSecondDensity(float delta, float position, float anchor)
        {
            const float deviationPercentage = densitySplit;
            var percentage = (position - anchor) / currentMaxDeviation;
            percentage = Mathf.Clamp(Mathf.Abs(percentage), 0, infinityClampScroll);
            delta -= delta * (1 - densitySplit) + delta * (deviationPercentage * percentage);
            delta *= densityScrollSecondPartMultiplier;
            return delta;
        }

        // Calculate delta when in borders
        private static float DeltaFirstDensity(float delta, float position, float anchor)
        {
            const float deviationPercentage = 1 - densitySplit;
            var percentage = (position - anchor) / normalDeviation;
            percentage = Mathf.Clamp(Mathf.Abs(percentage), 0, 1);
            delta -= delta * (percentage * deviationPercentage);
            return delta;
        }
        
        // Clamp delta when max borders
        private static float DeviationDeltaClamped(float delta, float position, float minAnchor, float maxAnchor)
        {
            if (position > maxAnchor + maxDeviationClamp && delta > 0)
                return 0;

            if (position < minAnchor - maxDeviationClamp && delta < 0)
                return 0;
            
            return delta;
        }
        
        // Reset inertia when without touch and when out of borders
        public static void ResetInertiaWhenFree(float position, float minAnchor, float maxAnchor)
        {
            if (InputHS.Touched)
                return;

            if (position > maxAnchor + normalDeviation)
                pageScroll.ResetInertia();

            if (position < minAnchor - normalDeviation)
                pageScroll.ResetInertia();
        }
    }
}
