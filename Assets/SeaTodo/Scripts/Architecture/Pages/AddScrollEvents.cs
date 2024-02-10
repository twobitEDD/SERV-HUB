using UnityEngine;

namespace Architecture.Pages
{
    // MonoBehaviour class for adding scroll events for other objects
    public class AddScrollEvents : MonoBehaviour
    {
        void Start()
        {
            AddScrollEventsCustom.AddEventActions(gameObject);
            Destroy(this);
        }
    }
}
