using UnityEngine;
using System.Collections;

//[ android utility methods intended to be used by libraries
//
//Note: the code should not throw any exceptions when things go wrong.

#if UNITY_ANDROID && !UNITY_EDITOR
public class KZAndroid : MonoBehaviour {
    public static AndroidJavaObject GetCurrentActivity() {
        // from http://answers.unity3d.com/questions/59622/accessing-the-activity-context-in-android-plugin.html
        AndroidJavaClass jc = 
                new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        return jo;
    }

    public static AndroidJavaObject GetApplicationContext(
            AndroidJavaObject activity) {
        if(activity == null) {
            return null;
        } else {
            return activity.Call<AndroidJavaObject>("getApplicationContext");
        }
    }
    public static AndroidJavaObject GetBaseContext(
            AndroidJavaObject activity) {
        if(activity == null) return null;
        else return activity.Call<AndroidJavaObject>("getBaseContext");
    }
    public static AndroidJavaObject GetBaseContext() {
        AndroidJavaObject activity = GetCurrentActivity();
        if(activity == null) return null;
        else return activity.Call<AndroidJavaObject>("getBaseContext");
    }

    public static string GetPackageName() {
        AndroidJavaObject activity = GetCurrentActivity();
        if(activity == null) return "";
        else return GetApplicationContext(activity).Call<string>("getPackageName");
    }

    //>>> TODO
    public static string GetStringExtra(string key) {
        AndroidJavaObject activity = GetCurrentActivity();
        AndroidJavaObject intent = activity.Call<AndroidJavaObject>("getIntent"); 
        return intent.Call<string>("getStringExtra", key);
    }

    public static float GetBatteryLevel() {
        AndroidJavaObject activity = KZAndroid.GetCurrentActivity();
        if(activity == null) return 0;
        AndroidJavaObject context = KZAndroid.GetBaseContext(activity);
        if(context == null) return 0;
        AndroidJavaClass util = new AndroidJavaClass(
                "com.allproducts.kizi.plugins.util.Util");
        if(util == null) return 0;
        return util.CallStatic<float>("getBatteryLevel", context);
    }

    public static string GetMemoryUsage() {
        AndroidJavaObject obj = new AndroidJavaObject(
            "com.allproducts.kizi.plugins.util.CPULoad");
        return obj.Call<string>("getProcessMemory");
    }
    public static string GetCPUTemperature() {
        AndroidJavaObject obj = new AndroidJavaObject(
            "com.allproducts.kizi.plugins.util.CPULoad");
        return obj.Call<string>("getCurrentCpuTremal");
    }
    public static string GetCPUFrequency(int coreIndex) {
        AndroidJavaObject obj = new AndroidJavaObject(
            "com.allproducts.kizi.plugins.util.CPULoad");
        return obj.Call<string>("getCurrentCpuFrequency", coreIndex);
    }

}
#endif
