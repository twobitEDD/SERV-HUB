using HomeTools;
using UnityEngine;

namespace HTools
{
    // Base for App entry point
    public abstract class EntryPoint : MonoBehaviour
    {
        private void Awake()
        {
            if (AppParameters.LoadedFromLogo)
            {
                Destroy(this);
                return;
            }
            
            var create = SingletonCenter.Instance;
            InitComponents();
            Destroy(this);
        }

        protected abstract void InitComponents();
    }
}