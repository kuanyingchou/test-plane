using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KZPlayerPrefs {
    //private static Dictionary<string, object> pairs = 
    //        new Dictionary<string, object>();

    public static void SetInt(string key, int val) {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //KZSharedPreferences.SetInt(CombineKeys(key), val);
        //#else
        PlayerPrefs.SetInt(CombineKeys(key), val);
        //#endif
        //pairs[key]=val;
    }
    public static void SetFloat(string key, float val) {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //KZSharedPreferences.SetFloat(CombineKeys(key), val);
        //#else
        PlayerPrefs.SetFloat(CombineKeys(key), val);
        //#endif
        //pairs[key]=val;
    }
    public static void SetString(string key, string val) {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //KZSharedPreferences.SetString(CombineKeys(key), val);
        //#else
        PlayerPrefs.SetString(CombineKeys(key), val);
        //#endif
        //pairs[key]=val;
    }

    public static int GetInt(string key) { 
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //return KZSharedPreferences.GetInt(CombineKeys(key)); 
        //#else
        return PlayerPrefs.GetInt(CombineKeys(key)); 
        //#endif
    }
    public static float GetFloat(string key) {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //return KZSharedPreferences.GetFloat(CombineKeys(key)); 
        //#else
        return PlayerPrefs.GetFloat(CombineKeys(key)); 
        //#endif
    }
    public static string GetString(string key) {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //return KZSharedPreferences.GetString(CombineKeys(key)); 
        //#else
        return PlayerPrefs.GetString(CombineKeys(key)); 
        //#endif
    }

    public static bool HasKey(string key) {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //return KZSharedPreferences.HasKey(CombineKeys(key));
        //#else
        return PlayerPrefs.HasKey(CombineKeys(key));
        //#endif
    }
    public static void DeleteKey(string key) {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //KZSharedPreferences.DeleteKey(CombineKeys(key));
        //#else
        PlayerPrefs.DeleteKey(CombineKeys(key));
        //#endif
        //pairs.Remove(key);
    }

    ////[ There is no way to get keyset from Unity API. 
    ////  Since we can't delete other users' keys, so don't use DeleteAll()
    //public static void DeleteAll() {
    //    foreach(string k in pairs.Keys) {
    //        string key = CombineKeys(k);
    //        #if UNITY_ANDROID && !UNITY_EDITOR
    //        if(KZSharedPreferences.HasKey(key)) {
    //            KZSharedPreferences.DeleteKey(key);
    //        }
    //        #else
    //        if(PlayerPrefs.HasKey(key)) {
    //            PlayerPrefs.DeleteKey(key);
    //        }
    //        #endif
    //    }

    //    pairs.Clear();
    //}

    public static void Save() {
        //#if UNITY_ANDROID && !UNITY_EDITOR
        //KZSharedPreferences.Save();
        //#else
        PlayerPrefs.Save();
        //#endif
    }

    //[ private
    private static string CombineKeys(string key) {
        //Debug.Log(KZPlayerManager.GetPrefix());
        return GetPrefix() + key;
    }
    private static string GetPrefix() {
        return KZPlayerManager.GetCurrentPlayerId()+"_";
        //>>> should be at the client side
    }


}
