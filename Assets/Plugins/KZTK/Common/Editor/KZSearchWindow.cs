 #if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;

//2014.6.16  ken  initial version
//todo:
//- bug: directories can't be opened
//- move to a destination
//- export to text file
//- file size

public class KZSearchWindow : EditorWindow {

    private string text = "";
    private string extensions = "mp3;wav;aif;ogg";
    private string directories = "Assets;";
    private string[] results;
    private bool[] resultToggles;
    private bool ignoreCase = true;
    private Vector2 scrollPos;
    private static char SEP=';';
    private static string DOTDOTDOT = new string('.', 1000);

    [MenuItem ("KZTK/Search in Project...")]
    static void Init () {
        KZSearchWindow window = (KZSearchWindow)EditorWindow.GetWindow(
                typeof (KZSearchWindow));
        window.autoRepaintOnSceneChange = false;
    }
    

    private void ShowResultCount() {
        int resCount = (results == null)? 0: results.Length;
        GUILayout.Label (
                string.Format("{0} Result{1}:", 
                        resCount, resCount>1?"s":""), 
                EditorStyles.boldLabel);
    }

    void OnGUI () {
        GUILayout.Label ("Search in Project", EditorStyles.boldLabel);

        text = EditorGUILayout.TextField("Text", text);

        ignoreCase = EditorGUILayout.Toggle("Ignore Case", ignoreCase);

        extensions = EditorGUILayout.TextField("Types", extensions);

        directories = EditorGUILayout.TextField("Directories", directories);

        if(GUILayout.Button("Search")) { Search(); }

        EditorGUILayout.BeginHorizontal();
        ShowResultCount();
        if(GUILayout.Button("Copy All", GUILayout.ExpandWidth(false))) {
            CopyAllToClipboard();
        }
        EditorGUILayout.EndHorizontal();

        if(results!=null && results.Length > 0) { ShowResults(); }
    }

    private void ShowResults() {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginVertical();
        for(int i = 0; i<results.Length; i++) {
            ShowResultRow(i);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private void ShowResultRow(int i) {
        //EditorGUILayout.SelectableLabel(r);
        EditorGUILayout.BeginHorizontal();
        string r = results[i];

        resultToggles[i] = GUILayout.Toggle(
                resultToggles[i], 
                "", 
                GUILayout.ExpandWidth(false));

        if(GUILayout.Button(i.ToString()+". "+r, 
                GUILayout.ExpandWidth(false))) {
            KZAsset.Select(r);
            KZAsset.Highlight(r);
        }

        GUILayout.Label(DOTDOTDOT,
                GUILayout.ExpandWidth(true),
                GUILayout.MinWidth(0));

        if(GUILayout.Button("Copy Path", 
                GUILayout.ExpandWidth(false))) {
            EditorGUIUtility.systemCopyBuffer = r;
        }

        if(GUILayout.Button("Open", 
                GUILayout.ExpandWidth(false))) {
            KZAsset.Open(r);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void Search() {
        /*
        results = KZAsset.FindAllInProject(
                text, ignoreCase, extensions.Split(new char[] {','}));
        */
        Regex extReg = new Regex(GetExtensionRegex(extensions));
        Regex dirReg = new Regex(GetDirectoryRegex(directories));

        List<string> res = KZAsset.GetAllAssetPaths()
                     .FindAll((x) => dirReg.IsMatch(x))
                     .FindAll((x) => KZUtil.Contains(x, text, ignoreCase))
                     .FindAll((x) => extReg.IsMatch(x));
        res.Sort(); //TODO: support other sorting methods
        results = res.ToArray();
        resultToggles = new bool[results.Length];            
    }

    public void CopyAllToClipboard() {
        if(results == null || results.Length == 0) return;
        string res = KZUtil.Join(results, "\n");
        EditorGUIUtility.systemCopyBuffer = res;
    }

    private static string GetExtensionRegex(string extensions) {
        return KZUtil.Join(Split(extensions), (x)=>"."+x+"$", "|");
    }
    private static string GetDirectoryRegex(string directories) {
        return KZUtil.Join(Split(directories), (x)=>"^"+x, "|");
    }
    private static string[] Split(string field) {
        return field.Split(
                new char[] {SEP}, 
                StringSplitOptions.RemoveEmptyEntries);
    }
}
#endif
