using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/*
Getting touch inputs and dispatching them to touched game objects.

A TouchManager dispatches touch events to effected GameObjects. Our goal is to separate the check whether a GameObject is effected by a gesture and what the GameObject should do about it.

The Manager will call the appropriate methods on the effected GameObject upon receiving touch events. The methods include:

    void OnTouchBegan(KZTouchEvent);
    void OnTouchEnded(KZTouchEvent);
    void OnTouchMoved(KZTouchEvent);
    void OnTouchStayed(KZTouchEvent);
    void OnTouchEntered(KZTouchEvent);
    void OnTouchLeaved(KZTouchEvent);
    void OnTouchClicked(KZTouchEvent);

Note: In order for the dispatching to work, it needs to be attached to any GameObject in a scene. There can exist only one instance of this class in a scene.
*/

//2013.3.8   ken  extract dispatch strategy to KZTouchStrategy
//2013.3.4   ken  reimplement dispatch
//2013.2.27  ken  1. fix hover bug
//                2. ignore extra touches to a single object.
//2013.2.26  ken  initial version

/*
TODO
test cases:
  one finger:
    1 began, 1 ended
    1 began, 1 moved*, 1 ended
  2 fingers:
    1 began, 2 began, 2 ended, 1 ended
    1 began, 2 began, 1 ended, 2 ended
    1 began, 2 began, 1 ended, 1 began
    1 began, 2 began, 1 moved, 2 moved, 1 ended, 2 ended
*/

public class KZTouchManager : MonoBehaviour {
    
    public static KZTouchManager instance=null;
    private static readonly RaycastHit NULL_RAYCAST_HIT = new RaycastHit();
    private static readonly int IGNORE_RAYCAST_MASK = ~(1 << 2); 
    //] Since LayerMask.NameToLayer() can only 
    //  be called from the main thread, we use magic number here.
    //  '2' refers to layer 'ignore raycast'.

    public int maxConcurrentTouch=5; 
    public float raycastDistance=Mathf.Infinity; 
    //] seems like the performance penalty of infinity is trivial.
    public bool enableDebugMode=false;
    //private KZTouchDispatcher touchDispatcher;
    private KZTouch[] touches=null;

    private Dictionary<int, TouchRecord> records;
    private float clickTolerance = 100.0f;

    private Texture2D redCircle;
    private Texture2D greenCircle;
    private Texture2D blueCircle;
    private Texture2D yellowCircle;
    //private Texture2D whiteCircle;
    private Color red = new Color(255, 0, 0, 128);
    private Color green = new Color(0, 255, 0, 128);
    private Color blue = new Color(0, 0, 255, 128);
    private Color yellow = new Color(255, 255, 0, 128);
    //private Color white = new Color(255, 255, 255, 128);

    private Color halfTransparent = new Color(1, 1, 1, .5f);

    void Start() {
        //>>> we need something in the scene in order to get the power for
        //    the event dispatcher to work, but this mechanism is 
        //    tedious.
        if(instance!=null) { 
            KZTouchManager[] managers=
                    GameObject.FindObjectsOfType(typeof(KZTouchManager)) 
                    as KZTouchManager[];
            Debug.LogError(
                "Only one instance of KZTouchManager is allowed, found " + 
                managers.Length+".");
        }
        instance=this;
        
        //touchDispatcher=new KZDefaultTouchDispatcher(maxConcurrentTouch);
        
        // An Android device can connect to a mouse, and a PC may come 
        // with a touch screen, so we don't use this:
        //    if(Application.platform==RuntimePlatform.Android) ...     

        records=new Dictionary<int, TouchRecord>();

        CreateCircles();
    }

    private void CreateCircles() {
        float dpi = Screen.dpi;
        if(dpi == 0) {
            dpi = 70;
        }
        float sizeInch = .5f;
        int circleSize = Mathf.RoundToInt(sizeInch * dpi);
        redCircle = KZTexture.GetCircle(circleSize, red).ToTexture2D();
        greenCircle = KZTexture.GetCircle(circleSize, green).ToTexture2D();
        blueCircle = KZTexture.GetCircle(circleSize, blue).ToTexture2D();
        //whiteCircle = KZTexture.GetCircle(circleSize, white).ToTexture2D();
        yellowCircle = KZTexture.GetCircle(circleSize, yellow).ToTexture2D();
    }

    private Camera[] GetAllEnabledCameras() {
        return Camera.allCameras;
    }
    
    void Update () {
        
        //Debug.Log ("Length: "+Input.touches.Length);
        KZInput.UpdateTouches();
        
        touches = KZInput.GetTouches();
        
        if(touches.Length == 0) {
            resetRecords();
        } else {
            int processed = 0; //int max = Mathf.Min(touches.Length, maxConcurrentTouch); 
            Camera[] cameras = GetAllEnabledCameras(); 
            System.Array.Sort(cameras, cameraDepthComparer);
            //System.Array.Sort(touches, fingerIdComparer); //necessary?
            //Debug.Log(KZUtil.Join(cameras, ", "));

            for (int ti = 0; ti < touches.Length; ti++) {
                if(touches[ti] == null) {
                    if(enableDebugMode) Debug.LogWarning("got null touch!");
                    continue;
                }
                if(processed < maxConcurrentTouch) {
                    Detect(touches[ti], cameras);
                }

                processed++;
            }
        }
        
    }

    private void Detect(KZTouch touch, Camera[] cameras) {
        Component touchedComponent = null;
        for(int ci=0; ci<cameras.Length; ci++) {
            //Debug.Log(cameras[ci].name);

            RaycastHit hit;
            touchedComponent=GetTouchedComponent(
                    touch.position, 
                    cameras[ci],
                    out hit);

            Dispatch(touch, 
                    (touchedComponent==null)?
                        null:
                        touchedComponent.gameObject, 
                    cameras[ci],
                    hit);
        }
    }

    private Component GetTouchedComponent(
            Vector2 touchPosition, Camera cam, out RaycastHit hit) {
        Component touchedComponent=null;
        //first check GUI Layer
        touchedComponent=GetTouchedGUIElement(touchPosition, cam);
        
        //then check normal game objects
        if(touchedComponent==null) {
            Ray ray = cam.ScreenPointToRay (touchPosition); 
            
            //Debug.Log (cam.cullingMask);
            if (Physics.Raycast(ray, out hit, raycastDistance, 
                    cam.cullingMask & IGNORE_RAYCAST_MASK)) {
                if(enableDebugMode) {
                    Debug.DrawRay (
                        ray.origin, 
                        ray.direction * hit.distance, 
                        Color.green, 
                        2);
                }
                //e.g. 
                //        layer: 2 (could be 0 - 31)
                //            1: 0000 0000 0000 0000 0000 0000 0000 0001
                //   1 << layer: 0000 0000 0000 0000 0000 0000 0000 0100
                // culling mask: 0000 0000 0000 0000 0000 0000 0000 1110
                // (1 << layer) & culling mask: 
                //               0000 0000 0000 0000 0000 0000 0000 0100

                touchedComponent=hit.transform;
            }
        } else {
            hit = NULL_RAYCAST_HIT;
        }   
        return touchedComponent;
    }
    private Component GetTouchedGUIElement(Vector2 pos, Camera cam) {
        Component touchedComponent=null;
        GUILayer cameraGuiLayer=cam.GetComponent<GUILayer>();
        if(cameraGuiLayer != null) {
            touchedComponent=cameraGuiLayer.HitTest(pos);
        }
        return touchedComponent;
    }
    
    /*
    public KZTouchDispatcher GetTouchDispatcher() {
        return touchDispatcher;
    }
    //should be set before any touch events, move to ctor?
    public void SetTouchDispatcher(KZTouchDispatcher s) {
        touchDispatcher=s;
    }
    */
    
    //[ private
    private static CameraDepthComparer cameraDepthComparer=
            new CameraDepthComparer();
    private class CameraDepthComparer : IComparer<Camera> {
        public int Compare(Camera x, Camera y) {
            float diff=x.depth - y.depth;
            if(diff==0) return 0;
            else if(diff<0) return 1;
            else return -1;
        }
    }

    /*
    private static FingerIdComparer fingerIdComparer=new FingerIdComparer();
    private class FingerIdComparer : IComparer<KZTouch> {
        public int Compare(KZTouch x, KZTouch y) {
            return x.fingerId - y.fingerId;
        }
    }
    */

    public void Dispatch(KZTouch touch, GameObject current, 
            Camera cam, RaycastHit hit) {
        //Debug.Log ("dispatching "+touch+" on "+current+" from "+cam);
        switch(touch.phase) {
        case TouchPhase.Began:
            TouchBegan(touch, current, cam, hit);
            break;
        case TouchPhase.Stationary:
            TouchStayed(touch, current, cam, hit);
            break;
        case TouchPhase.Moved:
            TouchMoved(touch, current, cam, hit);
            break;
        case TouchPhase.Ended:
            TouchEnded(touch, current, cam, hit);
            break;
        case TouchPhase.Canceled:
            //TouchCanceled(touch, current, cam);
            TouchEnded(touch, current, cam, hit); 
            //replace Canceled with Ended to simplify clients' work
            break;
        }
    }
    //[ private
    private void TouchBegan(
            KZTouch touch, GameObject current, Camera cam, RaycastHit hit) {
        if(current == null) return;
        if (IsTouchedByOtherFinger(current)) {
            return; //>>> multi touch
        }

        if( records.ContainsKey(touch.fingerId)) {
            if(touch.isProcessed) return;
            else records.Remove(touch.fingerId);
        } 
        if(records.Count >= maxConcurrentTouch) return;
        touch.isProcessed = true;
        records.Add(touch.fingerId, new TouchRecord(touch, current, cam));
        SendTouchMessage(current, "OnTouchBegan", 
                new KZTouchEvent(touch, current, null, current, cam, hit));
    }
    private void TouchStayed(KZTouch touch, GameObject current, 
            Camera cam, RaycastHit hit) {
        if(!records.ContainsKey(touch.fingerId)) return;
        TouchRecord r = records[touch.fingerId];
        if(r.cam != cam) return;
        if(r.first == null) return;

        SendTouchMessage(r.first, "OnTouchStayed", 
                new KZTouchEvent(touch, r.first, r.first, r.first, cam, hit));
        r.last = current;
    }
    private void TouchMoved(KZTouch touch, GameObject current, Camera cam, 
            RaycastHit hit) {
        if(!records.ContainsKey(touch.fingerId)) return;
        TouchRecord r = records[touch.fingerId];
        if(r.cam != cam) return;
        if(r.first == null) return;

        SendTouchMessage(r.first, "OnTouchMoved", 
                new KZTouchEvent(touch, r.first, r.last, current, cam, hit));
            
        if(current != r.last) {
            if(r.last!=null) {
                SendTouchMessage(r.first, "OnTouchLeaved", 
                        new KZTouchEvent(touch, r.first, r.last, current, cam, hit));
                //Debug.Log ("leave");
            }                   
            if(current != null) {
                SendTouchMessage(r.first, "OnTouchEntered", 
                        new KZTouchEvent(touch, r.first, r.last, current, cam, hit));
                //Debug.Log ("enter");
            }
        } 
        r.last = current;
    }
    private void TouchEnded(KZTouch touch, GameObject current, Camera cam, 
            RaycastHit hit) {
        if(!records.ContainsKey(touch.fingerId)) return;
        TouchRecord r = records[touch.fingerId];
        if(r.cam != cam) return;
        if(r.first != null) {
            SendTouchMessage(r.first, "OnTouchEnded", 
                    new KZTouchEvent(touch, r.first, current, null, cam, hit));
            if(current != null && current == r.first) {
                if((touch.position - r.firstTouch.position).sqrMagnitude <= clickTolerance) {
                    SendTouchMessage(r.first, "OnTouchClicked", 
                            new KZTouchEvent(touch, r.first, current, null, cam, hit));
                }
            }
        }
        records.Remove(touch.fingerId);
    }

    private void SendTouchMessage(GameObject target, string msg, 
            KZTouchEvent e) {
        //Debug.Log("sending "+msg);
        if(target != null) {
            target.SendMessage(msg, e, SendMessageOptions.DontRequireReceiver);
        }
    }
    private bool IsTouchedByOtherFinger(GameObject t) {
        foreach(var pair in records) {
            TouchRecord r = pair.Value;
            if(r != null && 
               r.first == t && 
               r.cam && 
               r.cam.enabled) return true;
        }
        return false;
    }

    public void resetRecords()
    {
        //if(enableDebugMode) Debug.Log("reset records");
        var keys = new List<int>(records.Keys);
        foreach (int k in keys)
        {
            TouchRecord r = records[k];
            if (r != null && r.first != null &&
                    r.firstTouch != null)
            {
                KZTouch dummyEnd = new KZTouch(
                        r.firstTouch.fingerId, Vector2.zero, Vector2.zero,
                        0, 1, TouchPhase.Ended);
                TouchEnded(dummyEnd, r.first, r.cam, NULL_RAYCAST_HIT);
                records.Remove(k); //!
            }
        }
        //if(keys.Count>0) Debug.Log(">>> "+KZUtil.Join(keys));
        //if (records.Keys.Count > 0) Debug.Log(">>>>>> " + KZUtil.Join(new List<int>(records.Keys)));
        records.Clear();
    }

    private class TouchRecord {
        public GameObject first;
        public GameObject last;
        public Camera cam;
        public KZTouch firstTouch;

        public TouchRecord(KZTouch t, GameObject o, Camera c) {
            firstTouch = t;
            first = last = o;
            cam = c;
        }
    }
    
    //[ debug
    public void OnGUI() {
        if( ! enableDebugMode) return;
        if(touches == null) return;

        int fontSize = 48;
        GUIStyle labelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
        labelStyle.fontSize = fontSize;

        GUILayout.BeginVertical();
        for (int i=0; i<touches.Length; i++) {
            if(touches[i] != null  /*&& 
                    touches[i].fingerId < maxConcurrentTouch*/) {
                GUILayout.Label(touches[i].ToString());
            }
        }   
        GUILayout.EndVertical();

        for(int i=0; i<touches.Length; i++) {
            GUI.color = halfTransparent;
            KZTouch t = touches[i];
            Texture2D circle = GetCircle(touches[i].phase);
            Rect rect = new Rect(
                    t.position.x - circle.width/2, 
                    (Screen.height-t.position.y-circle.height/2), 
                    circle.width,
                    circle.height);
            GUI.DrawTexture(rect, circle);

            //GUI.color = original;
            //Rect labelRect = new Rect(
            //        t.position.x - circle.width/2, 
            //        (Screen.height-t.position.y-circle.height/2 - fontSize), 
            //        fontSize*2,
            //        fontSize*2);
            //GUI.Label(labelRect, t.fingerId.ToString(), labelStyle);
            //Debug.Log("drawing texture at "+t.position);        
        }
    }

    private Texture2D GetCircle(TouchPhase phase) {
        switch(phase) {
        case TouchPhase.Began:
            return redCircle;
        case TouchPhase.Stationary:
            return yellowCircle;
        case TouchPhase.Moved:
            return greenCircle;
        case TouchPhase.Ended:
        case TouchPhase.Canceled:
        default:
            return blueCircle;
        }
    }
    
}
