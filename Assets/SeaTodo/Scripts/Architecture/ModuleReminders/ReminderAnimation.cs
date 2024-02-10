using HomeTools.Source.Design;
using HTools;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.ModuleReminders
{
    // Animation component of day reminder item
    public class ReminderAnimation
    {
        // Animation component of alpha channel for reminder circle
        private readonly UIAlphaSync activeAlpha;
        // Animation component of rect for reminder icon
        private readonly RectTransformSync activeSync;
        // Animation component of alpha channel for reminder icon
        private readonly UIAlphaSync doneAlpha;
        // Animation component of alpha channel for reminder time
        private readonly UIAlphaSync timeAlpha;
        // Animation component of item
        private readonly Animation doneAnimation;
        // Animation speed
        private const float animationSpeed = 0.2f;

        // Create and setup
        public ReminderAnimation(RectTransform active, RectTransform done, Text timeText)
        {
            // Create animation component of alpha channel for reminder circle
            activeAlpha = new UIAlphaSync();
            activeAlpha.AddElement(active.GetComponent<SVGImage>());
            activeAlpha.Speed = animationSpeed;
            activeAlpha.SpeedMode = UIAlphaSync.Mode.Lerp;
            activeAlpha.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(activeAlpha, AppSyncAnchors.RemindersModule);
            
            // Create animation component of alpha channel for reminder icon
            doneAlpha = new UIAlphaSync();
            doneAlpha.AddElement(done.GetComponent<SVGImage>());
            doneAlpha.Speed = animationSpeed;
            doneAlpha.SpeedMode = UIAlphaSync.Mode.Lerp;
            doneAlpha.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(doneAlpha, AppSyncAnchors.RemindersModule);
            
            // Create animation component of rect for reminder icon
            activeSync = new RectTransformSync();
            activeSync.SetRectTransformSync(active.GetComponent<RectTransform>());
            activeSync.Speed = animationSpeed * 1.5f;
            activeSync.SpeedMode = RectTransformSync.Mode.Lerp;
            activeSync.TargetScale = Vector3.one * 1.5f;
            activeSync.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(activeSync, AppSyncAnchors.RemindersModule);
            
            // Create animation component of alpha channel for reminder time
            timeAlpha = new UIAlphaSync();
            timeAlpha.AddElement(timeText);
            timeAlpha.Speed = animationSpeed;
            timeAlpha.SpeedMode = UIAlphaSync.Mode.Lerp;
            timeAlpha.PrepareToWork();
            SyncWithBehaviour.Instance.AddObserver(timeAlpha, AppSyncAnchors.RemindersModule);
            
            // Get animation component
            doneAnimation = done.GetComponent<Animation>();
        }

        // Set default state for animation components 
        public void SetDefaultState(bool active)
        {
            activeAlpha.SetAlpha(active ? 1 : 0);
            activeAlpha.SetDefaultAlpha(active ? 1 : 0);
            
            doneAlpha.SetAlpha(active ? 1 : 0);
            doneAlpha.SetDefaultAlpha(active ? 1 : 0);
            
            activeSync.SetT(active ? 1 : 0);
            activeSync.SetDefaultT(active ? 1 : 0);
            
            timeAlpha.SetAlpha(active ? 1 : 0);
            timeAlpha.SetDefaultAlpha(active ? 1 : 0);

            activeAlpha.Stop();
            doneAlpha.Stop();
            activeSync.Stop();
            timeAlpha.Stop();
        }
        
        // Start to active state for animation components 
        public void SetApplied()
        {
            activeAlpha.SetAlphaByDynamic(1);
            activeAlpha.Run();

            activeSync.TargetScale = Vector3.one * 1.5f;
            activeSync.SetTByDynamic(1);
            activeSync.Run();
            
            doneAlpha.SetAlphaByDynamic(1);
            doneAlpha.Run();
            
            timeAlpha.Speed = animationSpeed * 0.7f;
            timeAlpha.SetAlphaByDynamic(1);
            timeAlpha.Run();
            
            doneAnimation.Play(doneAnimation.clip.name);
        }

        // Start animation component for reminder text when closed track time view
        public void UpdateTimeAnimation()
        {
            timeAlpha.SetAlpha(0.5f);
            timeAlpha.SetDefaultAlpha(0.5f);
            timeAlpha.Speed = animationSpeed * 0.2f;
            timeAlpha.SetAlphaByDynamic(1);
            timeAlpha.Run();
        }
        
        // Start to passive state for animation components 
        public void SetToEmpty()
        {
            activeAlpha.SetAlphaByDynamic(0);
            activeAlpha.Run();
            
            activeSync.TargetScale = Vector3.one * 0.5f;
            activeSync.SetTByDynamic(0);
            activeSync.Run();
            
            doneAlpha.SetAlphaByDynamic(0);
            doneAlpha.Run();
            
            timeAlpha.Speed = animationSpeed;
            timeAlpha.SetAlphaByDynamic(0);
            timeAlpha.Run();
        }
    }    
}
