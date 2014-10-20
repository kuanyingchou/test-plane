using UnityEngine;

public class KZLauncherUtil {
    public static bool enabled = true;
    public static bool IsGlobalCheatingOn() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return false;
        AndroidJavaClass util = new AndroidJavaClass("com.allproducts.kizi.plugins.util.Util"); 
        bool res = false;
        try 
        {
            res =  util.CallStatic<bool>("isGlobalCheatingOn", KZAndroid.GetBaseContext(KZAndroid.GetCurrentActivity()));
        }
        catch(AndroidJavaException e)
        {
            Debug.Log("Couldn't find launcher, quitting...");
            Application.Quit();
        }
        return res;
    #else
        return false;
    #endif
    }
    public static void BackToCategory() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        AndroidJavaClass util = new AndroidJavaClass("com.allproducts.kizi.plugins.util.Util"); 
        util.CallStatic("backToCategory", KZAndroid.GetBaseContext(KZAndroid.GetCurrentActivity()));
    #else
        ;
    #endif
    }
}
