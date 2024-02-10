using System.Collections.Generic;

namespace HomeTools.Source.Design
{
    // Component for collect Animation Components of alpha channel and control it
    public class UIAlphaSyncGroup
    {
        // Lists of animation components
        private readonly List<UIAlphaSync> uiAlphaSyncs = new List<UIAlphaSync>();
        private readonly List<UIAlphaSyncGroup> uiAlphaSyncGroup = new List<UIAlphaSyncGroup>();

        // Speed
        public float Speed
        {
            set
            {
                foreach (var sync in uiAlphaSyncs)
                    sync.Speed = value;

                foreach (var syncGroup in uiAlphaSyncGroup)
                    syncGroup.Speed = value;
            }
        }

        // Add animation component
        public void AddSync(UIAlphaSync alphaSync)
        {
            if (alphaSync == null)
                return;
            
            uiAlphaSyncs.Add(alphaSync);
        }
        
        // Add animation component group
        public void AddSyncGroup(UIAlphaSyncGroup alphaSync)
        {
            if (alphaSync == null)
                return;
            
            uiAlphaSyncGroup.Add(alphaSync);
        }
        
        // Remove animation component
        public void RemoveSync(UIAlphaSync alphaSync)
        {
            if (alphaSync == null)
                return;
            
            uiAlphaSyncs.Remove(alphaSync);
        }
        
        // Remove animation component group
        public void RemoveSyncGroup(UIAlphaSyncGroup alphaSync)
        {
            if (alphaSync == null)
                return;
            
            uiAlphaSyncGroup.Remove(alphaSync);
        }

        // Run animation components
        public void Run()
        {
            foreach (var sync in uiAlphaSyncs)
                sync.Run();
            
            foreach (var sync in uiAlphaSyncGroup)
                sync.Run();
        }
        
        // Stop animation components
        public void Stop()
        {
            foreach (var sync in uiAlphaSyncs)
                sync.Stop();
            
            foreach (var sync in uiAlphaSyncGroup)
                sync.Stop();
        }

        // Prepare animation components to work
        public void PrepareToWork()
        {
            foreach (var sync in uiAlphaSyncs)
                sync.PrepareToWork();
            
            foreach (var sync in uiAlphaSyncGroup)
                sync.PrepareToWork();
        }

        // Set alpha channel to all components
        public void SetAlpha(float a)
        {
            foreach (var sync in uiAlphaSyncs)
                sync.SetAlpha(a);
            
            foreach (var sync in uiAlphaSyncGroup)
                sync.SetAlpha(a);
        }
        
        // Set default alpha channel to all components
        public void SetDefaultAlpha(float a)
        {
            foreach (var sync in uiAlphaSyncs)
                sync.SetDefaultAlpha(a);
            
            foreach (var sync in uiAlphaSyncGroup)
                sync.SetDefaultAlpha(a);
        }
        
        // Set alpha channel to all components for animation
        public void SetAlphaByDynamic(float a)
        {
            foreach (var sync in uiAlphaSyncs)
                sync.SetAlphaByDynamic(a);
            
            foreach (var sync in uiAlphaSyncGroup)
                sync.SetAlphaByDynamic(a);
        }
    }
}
