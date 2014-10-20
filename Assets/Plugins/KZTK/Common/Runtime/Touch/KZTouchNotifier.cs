using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class KZTouchNotifier : MonoBehaviour {
    private List<System.Object> listeners = new List<System.Object>();

    public void AddListener(System.Object o) {
        listeners.Add(o);
    } 
    public void OnTouchBegan(KZTouchEvent t) {
        listeners.ForEach(x => KZUtil.Call(x, "OnTouchBegan", t));
    }
    public void OnTouchStayed(KZTouchEvent t) {
        listeners.ForEach(x => KZUtil.Call(x, "OnTouchStayed", t));
    }
    public void OnTouchMoved(KZTouchEvent t) {
        listeners.ForEach(x => KZUtil.Call(x, "OnTouchMoved", t));
    }
    public void OnTouchEnded(KZTouchEvent t) {
        listeners.ForEach(x => KZUtil.Call(x, "OnTouchEnded", t));
    }
    public void OnTouchEntered(KZTouchEvent t) {
        listeners.ForEach(x => KZUtil.Call(x, "OnTouchEntered", t));
    }
    public void OnTouchLeaved(KZTouchEvent t) {
        listeners.ForEach(x => KZUtil.Call(x, "OnTouchLeaved", t));
    }
    public void OnTouchClicked(KZTouchEvent t) {
        listeners.ForEach(x => KZUtil.Call(x, "OnTouchClicked", t));
    }
}
