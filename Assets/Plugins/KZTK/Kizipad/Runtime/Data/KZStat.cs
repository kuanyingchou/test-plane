using UnityEngine;
using System.Collections;

public class KZStat {
    public static bool DEBUG=true;
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
    private static bool enabled = true;
    #endif

    public class ButtonType {
        public static int 
                Record = 0, 
                PlayRecording = 1, 
                Interaction = 2, 
                Question = 3, 
                Tutorial = 4;
    }

    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
    private static AndroidJavaObject stat = null;
    #endif

    public KZStat() {}

    private static void Init() {
//KZDebug.Log("before init()");
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.Init()");
        try 
        {
            stat = new AndroidJavaObject(
                    "com.allproducts.kizi.plugins.data.StatisticsManager",
                    KZAndroid.GetBaseContext(KZAndroid.GetCurrentActivity())); 
        }
        catch(AndroidJavaException e)
        {
            Debug.Log("Couldn't find statistics manager, quitting...");
            Application.Quit();
        }
    #endif
//KZDebug.Log("after init()");
    }

    public static void StartTerm() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.StartTerm()");
        if(stat == null) Init();
        stat.Call("startTerm");
    #endif
    }
    public static void FinishTerm() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.FinishTerm()");
        if(!isInitialized()) return;
        stat.Call("finishTerm");
    #endif
    }
    public static void PutData(string key, System.Object val) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.PutData()");
        if(!isInitialized()) return;
        stat.Call("putData", key, val.ToString());
    #endif
    }
    
    public static long GetElapsedGameTime() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return -1;
        if(DEBUG) Debug.Log("KZStat.GetElapsedGameTime()");
        if(!isInitialized()) return -1;
        return stat.Call<long>("getElapsedGameTime");
    #else
        return -1;
    #endif
    }

    //Note: need to change image import settings:
    //  Texture Type: Advanced
    //  Read/Write Enabled: True
    //  Override for Android: True
    //  Format: RGB 24 bit or RGBA 32 bit
    public static void SendNotification(
            string title, byte[] image, string meta) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.SendNotification()");
        if(!isInitialized()) return;
        stat.Call("sendNotification", title, image, meta);
    #endif
    }

    public static void StartLevel(int stage, int level) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.StartLevel()");
        if(!isInitialized()) return; 
        stat.Call("startLevel", stage, level);
    #endif
    }
    public static void StartGame() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.StartGame()");
        if(!isInitialized()) return;
        stat.Call("startGame");
    #endif
    }
    public static void FinishLevel(int numberOfStars) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.FinishLevel()");
        if(!isInitialized()) return;
        stat.Call("finishLevel", numberOfStars);
    #endif
    }
    public static int AskQuestion(string question) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return -1;
        if(DEBUG) Debug.Log("KZStat.AskQuestion()");
        if(!isInitialized()) return -1;
        return stat.Call<int>("askQuestion", question);
    #else
        return -1;
    #endif
    }
    public static void AnswerQuestion(string answer, bool correct) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.AnswerQuestion()");
        if(!isInitialized()) return;
        stat.Call("answerQuestion", answer, correct);
    #endif
    }
    public static void AnswerQuestion(int id, string choice, bool correct) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.AnswerQuestion()");
        if(!isInitialized()) return;
        stat.Call("answerQuestion", id, choice, correct);
    #endif
    }
    public static void ClickButton(int type, string code) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.ClickButton()");
        if(!isInitialized()) return;
        stat.Call("clickButton", type, code);
    #endif
    }
    public static void UpdateAptitude(int aptitude) {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.UpdateAptitude()");
        if(!isInitialized()) return;
        stat.Call("updateAptitude", aptitude);
    #endif
    }
    public static void PauseClock() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.PauseClock()");
        if(!isInitialized()) return;
        stat.Call("pauseClock");
    #endif
    }
    public static void ResumeClock() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return;
        if(DEBUG) Debug.Log("KZStat.ResumeClock()");
        if(!isInitialized()) return;
        stat.Call("resumeClock");
    #endif
    }
    private static bool isInitialized() {
    #if UNITY_ANDROID && !UNITY_EDITOR && KIZIPAD
        if(!enabled) return false;
        if(stat != null) {
            return true;
        } else {
            KZDebug.LogError("Need to initialize first!");
            return false;
        }
    #else
        return false;
    #endif
    }
}
