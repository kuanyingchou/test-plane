using UnityEngine;
using System.Collections;
using System;

#if UNITY_ANDROID && !UNITY_EDITOR
[Obsolete("Use KZAndroid instead", false)]
public class KZBattery : MonoBehaviour {
    public static float GetBatteryLevel() {
        return KZAndroid.GetBatteryLevel();
    }

}
#endif
