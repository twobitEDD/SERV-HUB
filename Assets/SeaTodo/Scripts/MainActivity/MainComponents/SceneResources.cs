using UnityEngine;

namespace MainActivity.MainComponents
{
    // Component for organize "Resources" for UI in scene
    public static class SceneResources
    {
        // Container for UI elements
        private static readonly Transform resourcesObject;
        // Name of resources object
        public static string ResourcesObjectName { get; }
        // Resources object
        public static Transform Container => resourcesObject;
        
        // Create and setup
        static SceneResources()
        {
            resourcesObject = GameObject.Find("Resources").transform; // Find object
            ResourcesObjectName = resourcesObject.name; // Save name
            resourcesObject.gameObject.SetActive(false); // Deactivate
        }

        // Get object origin from resources by name
        public static GameObject Get(string objectName) => resourcesObject.Find(objectName).gameObject;

        // Move resources object back
        public static void Set(GameObject gameObject) => gameObject.transform.SetParent(resourcesObject);

        // Get copy of origin object by name
        public static RectTransform GetPreparedCopy(string objectName)
        {
            var origin = resourcesObject.Find(objectName).GetComponent<RectTransform>();
            var copy = Object.Instantiate(origin).GetComponent<RectTransform>();
            copy.SetParent(MainCanvas.RectTransform);
            copy.anchoredPosition3D = origin.anchoredPosition3D;
            copy.localScale = origin.localScale;

            return copy;
        }

        // Get full path of object in hierarchy
        public static string GetGameObjectPath(GameObject obj)
        {
            var path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            
            return path;
        }
    }
}
