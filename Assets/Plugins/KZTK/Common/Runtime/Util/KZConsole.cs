using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//An HUD console for debugging
//2013.3.6  ken  initial version
public class KZConsole : MonoBehaviour {
    public int bufferSize = 10;
    public bool collapse = false;
    public bool receiveUnityLogs = true;

    private KZCircularBuffer<Message> messages;
    //private int start = 0;
    //private int size = 0;
    private Rect bounds;
    private float borderWidth = 128f;
    private float borderHeight= 20f;
    public float lineDistance = 0f;
    //private Vector2 scrollPosition;
    public Font font;
    private GUIStyle style;

    //private Color red = new Color(232f / 255, 44f / 255, 12f / 255);
    private Texture2D bg;
    private Texture2D bg2;

    public void Awake() {
        if (!Debug.isDebugBuild) Destroy(this);

        KZNC.Receive(this, "Log");
        messages = new KZCircularBuffer<Message>(bufferSize);
        //scrollPosition = Vector2.zero; //new Vector2(x, y);

        bounds = new Rect(borderWidth, borderHeight, 
            Screen.width - 2 * borderWidth, Screen.height - 2 * borderHeight);
        Application.RegisterLogCallback(HandleUnityLog);

        KZTexture t = new KZTexture(1, 1);
        t.SetPixel(0, 0, new Color(0f, 0f, 0f, .1f));
        bg = t.ToTexture2D();
        t.SetPixel(0, 0, new Color(0f, 0f, 0f, .2f));
        bg2 = t.ToTexture2D();

    }

    void HandleUnityLog(string logString, string stackTrace, LogType type)
    {
        if(receiveUnityLogs)
        {
            WriteMessage(new Message(logString, type));
        }
    }

    public void Log(KZNotice notice) {
        WriteLine(notice.data);
    }
    public void WriteLine(System.Object o) {
        WriteLine(o.ToString());
    }
    public void WriteLine(string msg)
    {
        WriteMessage(new Message(msg));
    }

    private void WriteMessage(Message m)
    {
        Message last = messages.GetLast();
        if (collapse && last != null && last.Equals(m))
        {
            last.repeat++;
        }
        else
        {
            messages.Add(m);
        }
    }
    

    public void Clear() {
        //messages = new Message[BUFFER_SIZE]; //not necessary
        //start = 0; //not necessary
        messages.Clear();
    }


    public void OnGUI() {
        if (style == null)
        {
            style = new GUIStyle("label");
            if (font == null) font = style.font;
            else style.font = font;
            //style.normal.background = bg;
            //style.fontSize = 14;
        }
        //GUI.color = Color.black;
        GUILayout.BeginArea(bounds);
        //GUILayout.Box(bg, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        GUILayout.BeginVertical();

        //scrollPosition = GUILayout.BeginScrollView(
        //        scrollPosition,
        //        false, true);
        foreach(var m in messages)
        {
            Color flagColor;
            switch (m.type)
            {
                case LogType.Log: 
                    flagColor = Color.green; 
                    break;
                case LogType.Warning: 
                    flagColor = Color.yellow; 
                    break;
                default: 
                    flagColor = Color.red; 
                    break;
            }
            if(m.id % 2 == 0)
                style.normal.background = bg;
            else
                style.normal.background = bg2;

            GUI.contentColor = flagColor;
            GUILayout.BeginHorizontal();
            GUILayout.Label("\u25CF");
            GUI.contentColor = Color.white;
            GUILayout.Label((collapse) ? m.ToString() : m.value, style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(lineDistance);
        }

        //GUILayout.Label(output);

        //GUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUILayout.EndArea();
        
    }

    class Message
    {
        private static int uid = 0;
        public string value;
        public LogType type;
        public int repeat = 1;
        public readonly int id;

        public Message(string s, LogType t = LogType.Log)
        {
            value = s;
            type = t;
            id = uid++;
        }

        public override string ToString()
        {
            if (repeat == 1)
                return string.Format("{0} {1}", GetTypeString(type), value);
            else
                return string.Format("{0} {1} x {2}",
                    GetTypeString(type), value, repeat);
        }

        public string GetTypeString(LogType type)
        {
            //return "[" + type.ToString().ToUpper() +"]";
            return "";
            //switch (type)
            //{ 
            //    case LogType.Log:
            //        return "☀";
            //    case LogType.Warning:
            //        return "☁";
            //    default:
            //        return "☂";
            //}
        }

        public override bool Equals(object obj)
        {
            Message m = obj as Message;
            if (m != null)
            {
                return value.Equals(m.value);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }

}
