using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class ResourcesHolder
{
    private static Scene _scene;
    public static GameObject CreateYearStats()
    {
        return LoadGameObject("Year stats");
    }

    
    private static GameObject LoadGameObject(string path)
    {
        var gameObject = Object.Instantiate(Resources.Load<GameObject>(path));
        gameObject.name = gameObject.name.Replace("(Clone)",string.Empty);
        return gameObject;
    }
}
