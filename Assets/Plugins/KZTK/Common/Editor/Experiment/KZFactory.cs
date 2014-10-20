 #if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using System.Collections;

public class KZFactory : MonoBehaviour {
    [MenuItem("KZTK/Development/Create ScriptableObject")]
    public static void CreateScriptableObject() {
        A a = new A();
        AssetDatabase.CreateAsset(a, "Assets/a.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = a;
    }

    public class A : ScriptableObject {
        string id = "hello";
        public void SayHi() { Debug.Log(id);  }
    }
}
#endif
