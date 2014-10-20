using UnityEngine;
using System.Collections;

interface KZIAudio {
    void SetVolume(int volume);
    int GetVolume();
    int GetMaxVolume();
    void VolumeUp();
    void VolumeDown();
}

public class KZAudio : KZIAudio {
    //public KZAudio getInstance() //>>>
    private KZIAudio instance;
    public KZAudio() {
        #if UNITY_ANDORID
        instance = new KZAndroidAudio();
        #else
        instance = new KZDummyAudio();
        #endif
    }
    public void SetVolume(int volume) { instance.SetVolume(volume); }
    public int GetVolume() { return instance.GetVolume(); }
    public int GetMaxVolume() { return instance.GetMaxVolume(); }
    public void VolumeUp() { instance.VolumeUp(); }
    public void VolumeDown() { instance.VolumeDown(); }
}

#if UNITY_ANDROID && !UNITY_EDITOR
class KZAndroidAudio : KZIAudio {
    private AndroidJavaObject obj;
    private int STREAM_TYPE;
    private int ADJUST_LOWER;
    private int ADJUST_RAISE;
    private int maxVolume;

    KZAndroidAudio() {
        InitAndroid();
    }

    private void InitAndroid() {
        //] >>> unity uses music stream?
        ADJUST_RAISE = 1; //cls.GetStatic<int>("ADJUST_RAISE"); 
        ADJUST_LOWER = -1; //cls.GetStatic<AndroidJavaObject>("ADJUST_LOWER"); 
        STREAM_TYPE = 3; //cls.GetStatic<int>("STREAM_MUSIC"); 

        AndroidJavaClass contextCls = 
                new AndroidJavaClass("android.content.Context");
        string audioService = contextCls.GetStatic<string>("AUDIO_SERVICE");
        AndroidJavaObject context = 
                KZAndroid.GetBaseContext(KZAndroid.GetCurrentActivity());
        obj = context.Call<AndroidJavaObject>(
                "getSystemService", audioService);
        maxVolume = GetMaxVolume();
    }

    public void SetVolume(int volume) {
        if(volume > maxVolume) { 
            volume = maxVolume;
        }
        obj.Call("setStreamVolume", STREAM_TYPE, volume, 0);
    }
    public int GetVolume() {
        return obj.Call<int>("getStreamVolume", STREAM_TYPE);
    }
    public int GetMaxVolume() {
        return obj.Call<int>("getStreamMaxVolume", STREAM_TYPE);
    }
    public void VolumeUp() {
        obj.Call("adjustStreamVolume", STREAM_TYPE, ADJUST_RAISE, 0);
    }
    public void VolumeDown() {
        obj.Call("adjustStreamVolume", STREAM_TYPE, ADJUST_LOWER, 0);
    }

}
#endif

class KZDummyAudio : KZIAudio {
    public void SetVolume(int volume) {
        Debug.Log("Not implemented yet!"); 
    }
    public int GetVolume() {
        Debug.Log("Not implemented yet!"); return 0; 
    }
    public int GetMaxVolume() {
        Debug.Log("Not implemented yet!"); return 0; 
    }
    public void VolumeUp() {
        Debug.Log("Not implemented yet!"); 
    }
    public void VolumeDown() {
        Debug.Log("Not implemented yet!"); 
    }
}

