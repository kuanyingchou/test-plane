using UnityEngine;
using System.Collections;
using System;

//Attach it to any gameobject in scene to print revision number

//2013.3.15  ken  initial version
public class KZRevisionPrinter : MonoBehaviour {
    private KZRevision revision;
    private static Color fontColor = Color.white;

    private float borderWidth = 10;
    private GUIStyle style;
    private Rect area;
    private float screenWidth;
    private float screenHeight;
    private Texture2D shade;

    public void Awake() {
        revision=KZRevision.Load();
        KZTexture tex = new KZTexture(1, 1);
        tex.SetPixel(0, 0, new Color(0, 0, 0, .2f));
        shade = tex.ToTexture2D();
    }

    public void OnGUI() {
        if (screenWidth != Screen.width || screenHeight != Screen.height)
        {
            UpdateDrawingArea();
        }
        if (style == null)
        {
            style = new GUIStyle("label");
        }

        if(revision == null) {
            //Write("Version N/A. Kizi Lab Inc.");
        } else {
            if (Debug.isDebugBuild)
            {
                Write(revision.ToHTML());
            }
            else 
            {
                Write("v"+revision.commitCount+" - "+revision.hash);
            }
        }
    }

    private void UpdateDrawingArea()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        area = new Rect(
                borderWidth,
                borderWidth,
                screenWidth - 2 * borderWidth,
                screenHeight - 2 * borderWidth - (Debug.isDebugBuild ? 20 : 0));    
    }

    private void Write(string text) {
        //// isn't compatiple with "flexiblespace"
        //if (style == null)
        //{
        //    style = new GUIStyle(GUI.skin.GetStyle("label"));
        //    style.fontSize = fontSize;
        //}

        style.normal.background = shade;

        GUILayout.BeginArea(area);
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        GUI.contentColor = fontColor;
        GUILayout.Label(text, style);

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
