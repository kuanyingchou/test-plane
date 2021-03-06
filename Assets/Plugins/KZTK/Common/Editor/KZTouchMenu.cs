 #if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;

public class KZTouchMenu : MonoBehaviour {
    [MenuItem ("KZTouch/C# Script")]
    static void AddCSharpMonoBehaviour () {
        Debug.Log ("Creating script for \""
                +Selection.activeGameObject.name+"\"...");
        CreateCSharpMonoBehaviour(Selection.activeGameObject.name);
        Debug.Log ("Done.");
    }
    [MenuItem ("KZTouch/C# Script", true)]
    static bool ValidateAddCSharpMonoBehaviour () {
        return (Selection.activeGameObject != null);
    }


    [MenuItem ("KZTouch/Javascript")]
    static void AddJavascriptMonoBehaviour () {
        Debug.Log ("Creating script for \""
                +Selection.activeGameObject.name+"\"...");
        CreateJavascriptMonoBehaviour(Selection.activeGameObject.name);
        Debug.Log ("Done.");
    }
    [MenuItem ("KZTouch/Javascript", true)]
    static bool ValidateAddJavascriptMonoBehaviour () {
        //Debug.Log ("Add Touch Adapter...");
        return (Selection.activeGameObject != null);
    }


    [MenuItem ("KZTouch/Touch Manager")]
     static void AddTouchManager () {
        Debug.Log ("Adding Touch Manager to \""+Selection.activeGameObject.name+"\" ...");
        Selection.activeGameObject.AddComponent<KZTouchManager>();
        Debug.Log ("Done.");
     }
    [MenuItem ("KZTouch/Touch Manager", true)]
    static bool ValidateAddTouchManager () {
        return Selection.activeGameObject != null; //>>> check # of TouchManager
    }
    
    //[ private
    
    private static void CreateCSharpMonoBehaviour(string objName) {
        string path=KZAsset.GetPath() + "/";
        string name=objName+"Behaviour";
        string ext=".cs";
        
        
        string fullPath=path+name+ext;
        fullPath=AssetDatabase.GenerateUniqueAssetPath(fullPath);
        KZCodeBuilder sb=new KZCodeBuilder();
        sb.pn("using UnityEngine;").pn()
          .pcn("Code generated by KZTouch. Please add it to \""+objName+"\".")  
          .ps("class").ps(System.IO.Path.GetFileNameWithoutExtension(fullPath))
          .ps(":").ps("MonoBehaviour").pn("{")
          .tab().pf("public void OnTouchBegan(KZTouchEvent t)")
          .tab().pf("public void OnTouchStayed(KZTouchEvent t)")
          .tab().pf("public void OnTouchMoved(KZTouchEvent t)")
          .tab().pf("public void OnTouchEnded(KZTouchEvent t)")
//          .tab().pf("public void OnTouchCanceled(KZTouchEvent t)")
          .tab().pf("public void OnTouchEntered(KZTouchEvent t)")
          .tab().pf("public void OnTouchLeaved(KZTouchEvent t)")
          .tab().pf("public void OnTouchClicked(KZTouchEvent t)")
          .pn("}");
                 
        Debug.Log(fullPath);
        KZAsset.WriteTextFile(fullPath, sb.ToString());
        Debug.Log("file \""+path+name+ext+"\" created");
    }

    private static void CreateJavascriptMonoBehaviour(string objName) {
        string path=KZAsset.GetPath() + "/";
        string name=objName+"Behaviour";
        string ext=".js";
        
        
        string fullPath=path+name+ext;
        fullPath=AssetDatabase.GenerateUniqueAssetPath(fullPath);
        KZCodeBuilder sb=new KZCodeBuilder();
        sb.pn("#pragma strict")
          .pcn("Code generated by KZTouch. Please add it to \""+objName+"\".")
          .pf("function OnTouchBegan(t : KZTouchEvent)")
          .pf("function OnTouchStayed(t : KZTouchEvent)")
          .pf("function OnTouchMoved(t : KZTouchEvent)")
          .pf("function OnTouchEnded(t : KZTouchEvent)")
 //         .pf("function OnTouchCanceled(t : KZTouchEvent)")
          .pf("function OnTouchEntered(t : KZTouchEvent)")
          .pf("function OnTouchLeaved(t : KZTouchEvent)")
          .pf("function OnTouchClicked(t : KZTouchEvent)");
                 
        Debug.Log(fullPath);
        KZAsset.WriteTextFile(fullPath, sb.ToString());
        Debug.Log("file \""+path+name+ext+"\" created");
    }

    //private static void CreateText() {
    //    KZAsset.WriteTextFile("hello.txt", "hello, world");
    //    AssetDatabase.Refresh();
    //}
}
#endif
