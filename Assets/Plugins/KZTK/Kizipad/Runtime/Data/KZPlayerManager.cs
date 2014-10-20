using UnityEngine;
using System.Collections;
using System.Linq;
using System;
using System.IO;

public class KZPlayerManager {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
    private static string PLAYER_MANAGER = 
            "com.allproducts.kizi.plugins.data.PlayerManager";
    private static int playerId = int.MinValue;
    public static bool enabled = true;
    #endif
//    public static string PLAYER_ID_KEY = "kz.playerid"; 
//    //] id leaved in players' PlayerPref in order to determine
//    //  its owner.

//    private static string[] dirs; 
//    //] use lazy initialization since Unity doesn't allow static 
//    //  initialization here.

    // >>> doesn't allow special characters in filepath
    public static string GetFilePath(string filepath) {
        int s = filepath.LastIndexOf("/");
        int b = filepath.LastIndexOf("\\");
        int sep = Math.Max(s, b);
        string dir = filepath.Substring(0, sep); 
        string name = filepath.Substring(sep + 1);
        Debug.Log("dir: "+dir +", name: "+name);
        return dir+"/"+GetCurrentPlayerId()+"_"+name;
    }
    /*
    public static string GetPrefix() {
        return GetCurrentPlayerId()+"_";
    }
    */

/*
    public static void Load() {
        Commit();
        Checkout();
    }
*/

    public static KZPlayer GetCurrentPlayer() {
        #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return null;
        AndroidJavaObject activity = KZAndroid.GetCurrentActivity();
        AndroidJavaObject context = KZAndroid.GetBaseContext(activity);
        AndroidJavaClass cls = new AndroidJavaClass(PLAYER_MANAGER); //>>>
        AndroidJavaObject jp = cls.CallStatic<AndroidJavaObject>(
                "getCurrentPlayer", context);
        //Debug.Log("jp = " + jp + ", "+);
        if(jp == null) { 
            return null;
        } else {
            KZPlayer p = null;
            try {
                int id = jp.Get<int>("id");
                string nickname = jp.Get<string>("nickname");
                int gender = jp.Get<int>("gender");
                long birthday = jp.Get<long>("birthday");
                long lastLogin = jp.Get<long>("lastLogin");
                string photoPath = jp.Get<string>("photoPath");
                //Debug.Log(KZUtil.Join(KZUtil.Array(id, nickname, gender, birthday, lastLogin, photoPath)));
                p = new KZPlayer(id, nickname, gender, birthday, 
                        lastLogin, photoPath);
            } catch(Exception) {
                //Debug.LogWarning(e);
                return null;
            }
            return p;
        }
        #else
        return null;
        #endif
    }

    public static int GetCurrentPlayerId() {
        #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return 0;
        if(playerId > int.MinValue) {
            return playerId;
        } else {
            AndroidJavaObject activity = KZAndroid.GetCurrentActivity();
            AndroidJavaObject context = KZAndroid.GetBaseContext(activity);
            AndroidJavaClass cls = new AndroidJavaClass(PLAYER_MANAGER); //>>>
            playerId = cls.CallStatic<int>("getCurrentPlayerId", context);
            return playerId;
        }
        #else
        return 0;
        #endif
    }

//    public static void Commit() {
//        //Debug.Log("commit");
//        /*
//        if(HasLastPlayer()) {
//            int playerId = GetLastPlayerId();
//            Commit(playerId);
//        }
//        */
//        AndroidJavaClass cls = new AndroidJavaClass(PLAYER_MANAGER);
//        cls.CallStatic("commit", GetDirs());
//    }
//    public static void Checkout() {
//        //Debug.Log("checkout");
//        int playerId = GetCurrentPlayerId();
//        Checkout(playerId);
//        /*
//        if(! PlayerPrefs.HasKey(PLAYER_ID_KEY)) {
//            Debug.Log("setting player id: "+playerId);
//            PlayerPrefs.SetInt(PLAYER_ID_KEY, playerId);
//            PlayerPrefs.Save();
//        }
//        */
//    }
//
//
///*
//    private static void Commit(int playerId) {
//        AndroidJavaClass cls = new AndroidJavaClass(PLAYER_MANAGER);
//        //cls.CallStatic("commit", playerId, GetDirs());
//        cls.CallStatic("commit", GetDirs());
//    }
//*/
//
//    private static void Checkout(int playerId) {
//        AndroidJavaClass cls = new AndroidJavaClass(PLAYER_MANAGER);
//        cls.CallStatic("checkout", playerId, GetDirs());
//    }
//
//    public static bool HasLastPlayer() {
//        return PlayerPrefs.HasKey(PLAYER_ID_KEY);
//    }
//    public static int GetLastPlayerId() {
//        if(HasLastPlayer()) {
//            return PlayerPrefs.GetInt(PLAYER_ID_KEY);
//        }  else {
//            return -1;
//        }
//    }
//
//    private static string GetDirs() {
//        if(dirs == null) {
//            dirs = new string[] {
//                Application.persistentDataPath,
//                "/data/data/" + KZAndroid.GetPackageName() + "/shared_prefs" 
//            };
//        } 
//        return KZUtil.Join(dirs, ";");
//    }

}
