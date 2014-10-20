using UnityEngine;
using System.Collections;
using System.Text;

public class KZMonitor : MonoBehaviour {

    //[Range(0, 1)]
    public float smoothFactor = .1f;
    public float updateInterval = 1;
    public bool enableFPS = true;
    public bool enableFrequency = true;
    public bool enableMemory = true;
    public bool enableThermal = true;

    private float fps = 0;
    private float deltaTime = 0;
    private Rect rect = new Rect(0, 0, 200, 200);
    private string info;

    public void Start() {
        //Application.targetFrameRate = 30;
        InvokeRepeating("UpdateState", 0, updateInterval);
    }

    public void Update() {
        deltaTime += (Time.deltaTime - deltaTime) * smoothFactor;
        fps = 1.0f / deltaTime;
    }

    public void UpdateState() {
        StringBuilder sb = new StringBuilder();
        if(enableFPS) {
            sb.Append(string.Format("{0:0.0} ms ({1:0.} fps)", 
                    deltaTime * 1000, fps)).Append("\n");
        }

        //[ doesn't work on android
        //string heap = string.Format("{0:0.0} MB", 
        //        Profiler.usedHeapSize / (1024f * 1024));
        //Debug.Log(fpsStr +"; heap: "+heap);
        //]

        #if UNITY_ANDROID && !UNITY_EDITOR
        if(enableMemory) {
            sb.Append(KZAndroid.GetMemoryUsage());
        } 
        if(enableFrequency) {
            sb.Append(KZAndroid.GetCPUFrequency(0)).Append("\n");
        }
        if(enableThermal) {
            sb.Append(KZAndroid.GetCPUTemperature()).Append("\n");
        }
        //KZUtil.Fibonacci(20);
        #endif
        info = sb.ToString();
    }

    public void OnGUI() {
        GUI.contentColor = Color.white;
        GUI.Label(rect, info);
    }
}
