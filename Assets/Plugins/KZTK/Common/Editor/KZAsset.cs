 #if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using System.Text;
using System.Linq;
using System.Collections.Generic;

//Asset related helper functions
//2013.3.14  ken  initial version
public class KZAsset {
    public static string GetPath() {
        return Application.dataPath;
    }
    public static void WriteTextFile(string path, string content) { //>>> what if file exists?
        
        //Debug.Log ("before writing to \""+path+"\"");
        System.IO.File.WriteAllText(path, content);
        
        AssetDatabase.ImportAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //Debug.Log ("done writing to \""+path+"\"");
    }
    public static string ReadTextFile(string path) {
        return System.IO.File.ReadAllText(path);
    }

    public static void Open(string path) {
        AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(path));
    }
    public static void Select(string path) {
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(path);
    }
    public static void Highlight(string path) {
        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));
    }


    /*
    public static string[] FindAllInProject(
            string query, 
            bool ignoreCase = true,
            params string[] extensions) {
        Debug.Log("searching \"" + query + "\" in " +
                extensions.Aggregate((a, b) => a + ", " + b) + " ...");
        List<string> paths = new List<string>();
        if(extensions.Length > 0) {
            extensions.ToList().ForEach(
                x => paths.AddRange(FindInProject(query, x, ignoreCase)));
        } else {
            paths.AddRange(FindInProject(query, "", ignoreCase));
        }
        Debug.Log(string.Format("Found {0} file{1} in {2}.", 
                paths.Count, 
                (paths.Count>1?"s":""), 
                extensions.Aggregate((a, b) => a + ", " + b)));
        paths.Sort();
        return paths.ToArray();
    }


    //[ modified from http://forum.unity3d.com/threads/how-can-i-get-a-list-of-all-scripts-in-a-project.84835/
    public static string[] FindInProject(
            string query, string extension, bool ignoreCase=true) {
        //[ starts with "Assets/..."
        string[] assetPaths = AssetDatabase.GetAllAssetPaths(); 
        List<string> results = new List<string>();
        query = query.Trim();
        query = string.IsNullOrEmpty(query)?"*":query;
        string ext = "";
        if(!string.IsNullOrEmpty(extension)) {
            ext = "."+extension.Trim();
        }

        assetPaths.ToList().Where(
                        x => x.EndsWith(ext))
                  .ToList().Where(
                        x => 
                        query.Equals("*") ?
                        true :
                        KZUtil.Contains(x, query, ignoreCase:ignoreCase))
                  .ToList().ForEach(x => results.Add(x));

        results.ForEach(x => Debug.Log(x));
        results.Sort();
        return results.ToArray();
    }
    */

    public static List<string> GetAllAssetPaths() {
        return AssetDatabase.GetAllAssetPaths().ToList();
    }

    /*
    //[ tests
    [MenuItem ("KZTK/Find All Scripts in Project") ]
    public static void FindAllScriptFilesInProject() {
        FindAllInProject("*", "cs", "js", "boo");
    }

    [MenuItem ("KZTK/Find All Medias in Project") ]
    public static void FindAllMediaFilesInProject() {
        FindAllInProject(
                "*", "ogg", "mp3", "wav", "aiff", "aif", "mpeg", "mpg");
    }

    [MenuItem ("KZTK/Identify Media Files") ]
    public static void IdentifyUnusedMediaFiles() {
        string[] files = FindAllInProject(
                "*", "ogg", "mp3", "wav", "aiff", "aif", "mpeg", "mpg");
        if(files.Length > 0) {
            Select(files[0]);
            Highlight(files[0]);
        }
    }
    */
}
#endif
