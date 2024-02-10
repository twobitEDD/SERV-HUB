using MainActivity.MainComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Architecture.TextHolder
{
    // Component for add text to localization
    [RequireComponent(typeof(Text))]
    public class TextLocalizationLink : MonoBehaviour
    {
        public string LocalizeTerm;
        
        private void Awake()
        {
            TextLocalization.Instance.AddLocalization(GetComponent<Text>(), LocalizeTerm);
            AwakeDestroy();
        }
        
        private void AwakeDestroy()
        {
            if (SceneResources.GetGameObjectPath(gameObject).Contains(SceneResources.ResourcesObjectName)) 
                return;
        
            Destroy(this);
        }
    }
}
