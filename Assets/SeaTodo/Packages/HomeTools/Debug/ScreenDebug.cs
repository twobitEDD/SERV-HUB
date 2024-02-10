using System.Globalization;
using HomeTools;
using UnityEngine;
using UnityEngine.UI;

// Component fro show debug messages by UI on device screen
public static class ScreenDebug
{
    private static Text text;
    
    // Debug on any platforms
    public static void Log(string message) => WriteLine(message);
    public static void Log(int message) => WriteLine(message.ToString());
    public static void Log(float message) => WriteLine(message.ToString(CultureInfo.CurrentCulture));

    // Debug only in editor
    public static void LogEditor(string message) => WriteLineEditor(message);
    public static void LogEditor(int message) => WriteLineEditor(message.ToString());
    public static void LogEditor(float message) => WriteLineEditor(message.ToString(CultureInfo.InvariantCulture));
    
    private static void WriteLine(string message)
    {
        if (text == null)
            InitializeText();
        
        text.text += message + "   |   ";
        Debug.Log(message);
    }
    
    private static void WriteLineEditor(string message)
    {
        if (!AppParameters.Device) Debug.Log(message);
    }

    // Create text component for show messages
    private static void InitializeText()
    {
        text = new GameObject("DEBUG").AddComponent<Text>();
        
        text.transform.SetParent(MainCanvas.RectTransform);
        
        var rect = text.GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0, 0, 0);
        rect.localScale = Vector3.one;
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMax = Vector2.one * -50f;
        rect.offsetMin = Vector2.one * 50f;
        text.fontSize = 30;
        text.color = Color.black;
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 30);
        text.raycastTarget = false;
    }
}
