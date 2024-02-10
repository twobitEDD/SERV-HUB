using UnityEngine;

namespace HTools
{
    // Component for create singleton object to DontDestroyOnLoad
    public class SingletonCenter : MonoBehaviour
    {
        private static SingletonCenter instance;

        public static SingletonCenter Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("_instances");
                    go.AddComponent<SingletonCenter>();
                }

                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                if (this != instance)
                    Destroy(gameObject);
            }
        }
    }
}