using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Linq;
using System.Reflection;
using System;

public class KZUtil {
    //======== Game ==========
    public static void SetFrameRate(int fps) {
        Application.targetFrameRate = fps; 
    }

    //======== GameObject =========
    public static T Instantiate<T>(T target) where T: UnityEngine.Object {
        return UnityEngine.Object.Instantiate(
                target, Vector3.zero, Quaternion.identity) as T;
    }

    private static Dictionary<string, UnityEngine.Object> loadedResources=
        new Dictionary<string, UnityEngine.Object>();

    public static T LoadResource<T>(string name) 
            where T:UnityEngine.Object {
        if(loadedResources.ContainsKey(name)) {
            return (T)loadedResources[name];
        } else {
            T t=Resources.Load(name) as T;
            loadedResources[name]=t;
            return t;
        }
    }

    //blink(disable renderer) a gameobject <repeat> times 
    //for <duration> seconds, e.g.
    //  0101 or 1010 if repeat == 2
    public static IEnumerator Blink(
            GameObject obj, float duration, int repeat) {
        for(int i=0; i<repeat*2; i++) {
            obj.renderer.enabled = ! obj.renderer.enabled;
            yield return new WaitForSeconds(duration / (repeat*2));
        }
    }
    //place GameObjects in a diamond, used in the farm project 
    [Obsolete("Use KZPlacer instead", false)]
    public static void PlaceDiamond(
            GameObject[] targets, int numCol, int numRow,
            Vector3 start, Vector3 colDiff, Vector3 rowDiff) {
        if(numCol<0) numCol = 0;
        if(numRow<0) numRow = 0;
        Vector3 rowStart=start;
        for(int row=0; row<numRow; row++) {
            Vector3 rowRunner=rowStart;
            for(int col=0; col<numCol; col++) {
                targets[row * numCol + col].transform.position=rowRunner;
                rowRunner += colDiff;
            }
            rowStart += rowDiff;
        }
    }

    //[ auto shrink/expand game objects to fill the grid
    private static List<GameObject> diamondElements = new List<GameObject>(); 
    //] >>> how about two diamonds?

    [Obsolete("Use KZPlacer instead", false)]
    public static void PlaceDiamond(GameObject prefab, int numCol, int numRow,
            Vector3 start, Vector3 colDiff, Vector3 rowDiff) {
        if(numCol<0) numCol = 0;
        if(numRow<0) numRow = 0;
        int size = numCol * numRow;
        while(size > diamondElements.Count) {
            diamondElements.Add(Instantiate(prefab));
        }
        while(size < diamondElements.Count) {
            int last = diamondElements.Count - 1;
            GameObject o = diamondElements[last];
            diamondElements.RemoveAt(last);
            GameObject.Destroy(o);
        }
        GameObject[] targets = diamondElements.ToArray();
        PlaceDiamond(targets, numCol, numRow, start, colDiff, rowDiff);
    }


    public static IEnumerable<GameObject> SortByY(
            IList<GameObject> targets) {
        return targets.OrderBy(x => -x.transform.position.y);
    }

    public static void AdjustZOrder(IList<GameObject> targets, float zGap) {
        IEnumerable<GameObject> sortedList=SortByY(targets);
        float z=0;
        foreach(GameObject o in sortedList) {
            o.transform.Translate(Vector3.forward * z);
            z -= zGap;
        }
    }

    //======== string =========
    //e.g. ToTitleCase("father") > "Father"
    public static string ToTitleCase(string input) {
        if(string.IsNullOrEmpty(input)) return input;
        return char.ToUpper(input[0])+input.Substring(1);
    }

    //======== List or array =========
    //e.g. GetString(["a", "b", "c"]) > "a, b, c"
    [Obsolete("Use Join() instead", false)]
    public static string GetString<T>(IList<T> arr) {
        return Join(arr);
    }
    public static string Join<T>(IList<T> arr) {
        return Join(arr, ", ");
    }

    //e.g. Join(["a", "b", "c"], ";") > "a;b;c"
    public static string Join<T>(IList<T> arr, string del) {
        return Join(arr, o => (o==null?"null":o.ToString()) , del);
    }
    public static string Join<T>(
            IList<T> arr, 
            System.Func<T, string> fun, 
            string del) {
        //return string.Join(del, arr); 
        //] .net framework 4+, not yet supported in unity
        if(arr == null) return "null";
        if(arr.Count == 0) return "";
        StringBuilder sb=new StringBuilder(fun(arr[0]));
        for(int i=1; i<arr.Count; i++) {
            sb.Append(del).Append(fun(arr[i]));
        }
        return sb.ToString();
    }
    public static object[] Array(params object[] t) {
        return t;
    }

    public static void Shuffle<T>(IList<T> arr) {
        if(arr == null) throw new ArgumentNullException();
        if(arr.Count == 1) return;
        for(int i=0; i<arr.Count-1; i++) {
            T t=arr[i];
            int j=UnityEngine.Random.Range(i+1, arr.Count);
            arr[i]=arr[j];
            arr[j]=t;
        }
    }

    public static void Reload() {
        Application.LoadLevel(Application.loadedLevel);
    }

    //[ assertions, necessary?
    public static bool Assert(bool exp) {
        if(!exp) PrintAssertError(null);
        else PrintAssertOK(null);
        return exp;
    }
    public static bool AssertEquals(System.Object a, System.Object b) {
        bool res = a.Equals(b);
        if(!res) PrintAssertError("\""+a + "\" is not \"" + b+"\"");
        else PrintAssertOK(null);
        return res;
    }
    private static void PrintAssertError(string msg) {
        Debug.LogError("Wrong Assertion! "+msg);
    }
    private static void PrintAssertOK(string msg) {
        Debug.LogWarning("Right Assertion! "+msg);
    }
    //]

    //Unlike SendMessage(), Call is not limited to GameObjects, and it returns value.
    //and it doesn't produce warnings when no receiver is found.
    //Like SendMessage(), Call only allows one argument.
    public static System.Object Call(System.Object target, string method) {
        return Call(target, method, null);
    }
    public static System.Object Call(
            System.Object target, 
            string method, 
            System.Object arg) {
        //Debug.Log("Calling " + method + "("+arg+") on "+target);
        if(target == null || string.IsNullOrEmpty(method)) return null;
        System.Type targetType = target.GetType();

        MethodInfo m = GetMethodInfo(targetType, method); 
        if(m == null) return null;

        System.Object[] p = PrepareParameters(m.GetParameters(), arg);

        //Debug.Log("processing "+m.Name);
        if(target is MonoBehaviour && m.ReturnType.Equals(typeof(IEnumerator))) {
            //Debug.Log("calling coroutine");
            return ((MonoBehaviour)target).StartCoroutine(
                (IEnumerator)(m.Invoke(target, p)));
        } else {
            //Debug.Log("calling normal routine");
            return m.Invoke(target, p);
        }
    }

    private static System.Object[] PrepareParameters(ParameterInfo[] pars, System.Object arg) {
        if(pars.Length == 0) {
            return new System.Object[0];
        } else {
            return new System.Object[] { arg };
        }
    }

    private static MethodInfo GetMethodInfo(System.Type target, string method) {
        MethodInfo m = null;
        try {
            m = target.GetMethod(method);
        } catch(AmbiguousMatchException) {
            Debug.LogError(target + " has multiple \""+method+"()\"!");
            return null;
        }
        if(m == null) {
            //Debug.LogWarning("method not found: "+
            //        target+"."+method+"("+(arg==null?"":arg)+")");
            return null;
        }
        return m;
    }

    //modified from http://forum.unity3d.com/threads/31351-GUIText-width-and-height
    public static Rect AutoWordWrap(
            GUIText guiText, float width) {
        string[] words = guiText.text.Split(); 
        //Debug.Log(KZUtil.Join(words, "', '"));
        string result = "";
        Rect textArea = new Rect();

        for(int i = 0; i < words.Length; i++) {
            if(words[i].Trim() == "") continue;
            // set the gui text to the current string including new word
            guiText.text = (result + words[i] + " ");
            // measure it
            textArea = guiText.GetScreenRect();
            // if it didn't fit, put word onto next line, otherwise keep it
            if(textArea.width > width) {
                result += ("\n" + words[i] + " ");
            } else {
                result = guiText.text;
            }
        }
        return textArea;
    }

    public static Rect AutoCharWrap(
            GUIText guiText, float width) {
        //Debug.Log(KZUtil.Join(words, "', '"));
        string words = guiText.text;
        string result = "";
        Rect textArea = new Rect();

        for(int i = 0; i < words.Length; i++) {
            if(words[i] == '\n') continue;
            // set the gui text to the current string including new word
            guiText.text = (result + words[i]);
            // measure it
            textArea = guiText.GetScreenRect();
            // if it didn't fit, put word onto next line, otherwise keep it
            if(textArea.width > width) {
                result += ("\n" + words[i]);
            } else {
                result = guiText.text;
            }
        }
        return textArea;
    }

    public static bool Similar(float a, float b, float tolerance) {
        return Mathf.Abs(a - b) < tolerance;
    }
    public static bool Similar(Vector3 a, Vector3 b, float tolerance) {
        if(!Similar(a.x, b.x, tolerance) ||
           !Similar(a.y, b.y, tolerance) || 
           !Similar(a.z, b.z, tolerance)) return false;
        else return true;
    }

    public static double Benchmark(System.Action f, int repeat) {
        System.Diagnostics.Stopwatch stopWatch = 
                    new System.Diagnostics.Stopwatch();
        stopWatch.Start();
        for(int i=0; i<repeat; i++) {
            f();
        }
        stopWatch.Stop();
        System.TimeSpan ts = stopWatch.Elapsed;
        return ts.TotalSeconds;
    }

    public static void CompareBenchmarks(System.Action a, System.Action b, int repeat) {
        double ra = Benchmark(a, repeat);
        double rb = Benchmark(b, repeat);
        string format = "0.00";
        if(ra < rb) {
            Debug.Log(string.Format(
                    "a({0}s) is faster than b({1}s) by {2}%.", 
                    ra, 
                    rb, 
                    ((rb-ra)/rb * 100).ToString(format)));
        } else if(ra > rb) {
            Debug.Log(string.Format(
                    "b({0}s) is faster than a({1}s) by {2}%.", 
                    rb, 
                    ra,
                    ((ra-rb)/ra * 100).ToString(format)));
        } else {
            Debug.Log("a and b both take " + ra + "s.");
        }
    }

    public static string Run(string command) {
        return Run(command, "");
    }

    //from http://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results
    public static string Run(string command, string paras) {
        System.Diagnostics.Process p = 
                new System.Diagnostics.Process();
        p.StartInfo.FileName = command;
        p.StartInfo.Arguments = paras;
        p.StartInfo.CreateNoWindow = true;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardOutput = true;   
        p.Start();
        string output = p.StandardOutput.ReadToEnd();
        p.WaitForExit();
        return output;
    }

    public static int Fibonacci(int n) {
        if(n < 2) return 1;
        else return Fibonacci(n-1) + Fibonacci(n-2);
    }

    public static bool Contains(
            string a, string b, bool ignoreCase = false) {
        return a.IndexOf(b, 
                ignoreCase?
                StringComparison.OrdinalIgnoreCase:
                StringComparison.Ordinal) >= 0;
    }

    public static Vector3 V3(float x=0, float y=0, float z=0) {
        return new Vector3(x, y, z);
    }

}
