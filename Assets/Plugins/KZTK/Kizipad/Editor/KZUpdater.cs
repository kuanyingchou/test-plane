#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Net;

public class KZUpdater {
    //public static string packageName = "kztk.unitypackage";
    public static string infoFile = "info.txt";

    public static string remote = 
            "http://192.168.0.92:8080/view/System/job/kztk-unity4/ws/out/";
    public static string local = 
            Application.dataPath + "/Plugins/";

    [MenuItem("KZTK/Update KZTK")]
    public static void Update() {
        using (WebClient client = new WebClient()) {
            /*
            client.DownloadProgressChanged += (sender, e) => {
                Debug.Log(e.ProgressPercentage + "%");
            };

            client.DownloadFileCompleted += (sender, e) => {
                if(e.Cancelled || e.Error != null) {
                    Debug.Log("something's wrong!");
                }
            };
            */


            string packageName = client.DownloadString(remote + infoFile).Trim();
            Debug.Log("downloading " + packageName + " from " + remote + " ...");

            client.DownloadFile(remote + packageName, local + packageName);

            Debug.Log("download completed.");
            AssetDatabase.ImportPackage(local + packageName, interactive: true);
        }
    }

}
#endif
