using UnityEngine;
using System.Collections;

public class KZSimpleMarker : MonoBehaviour {
    public string text;
    public int fontSize;
    private Rect rect;
    private GUIStyle labelStyle;

    public void Start() {
    }

    public void OnGUI() {
        labelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
        labelStyle.fontSize = fontSize;
        float width = text.Length * fontSize * .53f;
        float height = fontSize * 1.5f;
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        rect = new Rect(
                pos.x - width * .5f, 
                pos.y - height * .5f, 
                width, 
                height);
        GUI.Label(rect, text, labelStyle);
    }
}
