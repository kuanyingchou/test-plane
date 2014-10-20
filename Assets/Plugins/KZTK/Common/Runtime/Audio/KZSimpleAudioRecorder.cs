using UnityEngine;
using System.Collections;
using System;

//Note: require adtools.jar in Plugins/Android
//
//2013.2.24  ken  Move code to Java side. This class acts only as a C# wrapper.
//2013.2.24  ken  Rename from KZAudioRecorder to KZSimpleAudioRecorder.
//                Add SetOutputPath(), SetOutputFormat().
//2013.2.23  ken  Initial version, uses com.kuanying.adtools as backend.
    
/*
//>>> no one uses this anymore

public class KZSimpleAudioRecorder {
    public static int WAV=0;
    public static int MPEG4=1;
    
    private static string RECORDER_CLASSNAME=
            "com.allproducts.kizi.plugins.audio.SimpleAudioRecorder";
    
    private AndroidJavaObject recorder;
    
    public KZSimpleAudioRecorder() {
        recorder=new AndroidJavaObject(RECORDER_CLASSNAME);
    }
    
    public KZSimpleAudioRecorder(string path, int format) {
        recorder=new AndroidJavaObject(RECORDER_CLASSNAME, path, format);
    }
    
    public void StartRecording() {
        recorder.Call ("startRecording");
    }

    public void StopRecording() {
        recorder.Call ("stopRecording");
    }
    
    public void StartPlaying() {
        recorder.Call ("startPlaying");
    }
    public void PausePlaying() {
        recorder.Call ("pausePlaying");
    }
    public void StopPlaying() {
        recorder.Call ("stopPlaying");
    }
    public void StopAll() {
        if(IsRecording()) StopRecording();
        if(IsPlaying()) StopPlaying();
    }
    
    public bool IsPlaying() { return recorder.Call<bool> ("isPlaying"); }
    
    public bool IsRecording() { return recorder.Call<bool> ("isRecording"); }
    
    public bool IsPaused() { return recorder.Call<bool> ("isPaused"); }
        
}


*/
