using UnityEngine;
using UnityEngine.UI;

public static class MainCanvas
{
    public static RectTransform RectTransform { get; }
    public static CanvasScaler CanvasScaler { get; }
    public static readonly float AspectRatio;
    private static readonly float scaleConverter;

    static MainCanvas()
    {
        RectTransform = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();
        CanvasScaler = RectTransform.GetComponent<CanvasScaler>();
        scaleConverter =  CanvasScaler.referenceResolution.x / Screen.width;
        AspectRatio = (float) Screen.height / Screen.width;
    }

    public static Vector2 ScreenToCanvasDelta(Vector2 delta) => scaleConverter * delta;
    public static float ScreenToCanvasDelta(float delta) => scaleConverter * delta;
    
    public static float PercentageToScreenY(float percents) => RectTransform.rect.height * percents;

}
