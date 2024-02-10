using Modules.Localization;
using UnityEditor;
using UnityEngine;

public class LocalizationEditor : EditorWindow
{
    private LocalizationObject target;
    private bool loaded;
    
    [MenuItem("Tools/Localization Serialize")]
    static void Init()
    {
        CreateWindow<LocalizationEditor>("Localization Tools");
    }

    void OnGUI()
    {
        // Setup view of fields
        target = (LocalizationObject) EditorGUILayout.ObjectField("Source", target, typeof(LocalizationObject));
        
        if (GUILayout.Button("Serialize"))
        {
            SerializeData();
        }
    }

    private void SerializeData()
    {
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
