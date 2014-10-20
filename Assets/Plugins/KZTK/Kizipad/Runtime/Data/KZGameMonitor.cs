using UnityEngine;
using System.Collections;

public class KZGameMonitor : MonoBehaviour {
    private System.DateTime last = System.DateTime.MaxValue;
    private System.TimeSpan total = new System.TimeSpan();

    public void OnApplicationFocus(bool hasFocus) {
Debug.Log("OnApplicationFocus");
        System.DateTime now = System.DateTime.Now;
        KZDebug.Log((hasFocus?"in focus":"out of focus") + " at " + now);
        KZDebug.Log("total absent time: " + total);

        if(hasFocus) {
            if(last < now) {
                total += (now - last);
            }
        } else {
            last = now;
        }
    }

    public System.TimeSpan GetTotalAbsentTime() {
        return total;
    }
}
