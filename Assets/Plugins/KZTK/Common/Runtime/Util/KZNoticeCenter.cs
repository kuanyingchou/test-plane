using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
 
//2013.3.29  ken  modified from 
//                http://wiki.unity3d.com/index.php/NotificationCenter3_5

public class KZNoticeCenter {
 
    private static KZNoticeCenter instance;

    public static bool DEBUG=false;
 
    public static KZNoticeCenter Instance {
        get {
            if (instance == null) {
                instance = new KZNoticeCenter();
            }
            return instance;
        }
    }

    private Dictionary<string, List<Observer>> registrations =
       new Dictionary<string, List<Observer>>();

    
    #region Subscribe
    public void RemoveAll() {
        registrations.Clear();
    }
 
    
    // since the == operator on System.Object doesn't handle Unity's 
    // Destroy(), we use UnityEngine.Object as observer type.
    public void AddObserver(System.Object observer, string noticeName) {
        Observer o = new ObjectObserver(observer);
        AddObserver(o, noticeName);
    }

    public void AddObserver(System.Action<KZNotice> action, string noticeName)
    {
        Observer o = new ActionObserver(action);
        AddObserver(o, noticeName);
    }

    private void AddObserver(Observer observer, string noticeName)
    {
        if (noticeName == null || noticeName == "")
        {
            Debug.LogError("Please specify a notice for this observer!");
            return;
        }
        if (registrations.ContainsKey(noticeName) == false)
        {
            registrations[noticeName] = new List<Observer>();
        }

        List<Observer> notifyList = registrations[noticeName];

        if (notifyList.Contains(observer))
        {
            Debug.LogWarning("Observer had been added before!");
        }
        else
        {
            notifyList.Add(observer);
        }
    }



    public void RemoveObserver(System.Object observer, string name)
    {
        if(!registrations.ContainsKey(name)) return;

        List<Observer> notifyList = registrations[name];

        Observer o = new ObjectObserver(observer);
        if(notifyList.Contains(o)) {
            notifyList.Remove(o);
        }
        if(notifyList.Count == 0) {
            registrations.Remove(name);
        }
    }

    public List<Observer> GetObservers(string name)
    {
        if(string.IsNullOrEmpty(name) || 
           !registrations.ContainsKey(name) ||
           registrations[name] == null)
            return new List<Observer>();
        return registrations[name];
    }
    #endregion

    #region Publish

    public void PostNotice (System.Object aSender, string aName) {
        PostNotice(aSender, aName, null);
    }
 
    public void PostNotice(System.Object aSender, string aName, Hashtable aData)
    {
        PostNotice(new KZNotice(aSender, aName, aData));
    }
    public void PostNotice(System.Object aSender, string aName, System.Object aData)
    {
        PostNotice(new KZNotice(aSender, aName, aData));
    }
 
    private void PostNotice(KZNotice aNotice)
    {
        if(aNotice.name == null || aNotice.name == "")
        {
            Debug.Log("Null name sent to PostNotice.");
            return;
        }

        List<Observer> notifyList = null;
        if(registrations.ContainsKey(aNotice.name)) {
            notifyList = registrations[aNotice.name];
        }
 
        if(notifyList == null)
        {
            Debug.Log("No observer found for notice \""+aNotice.name+"\"");
            return;
        }

        List<Observer> observersToRemove = new List<Observer>();
        List<Observer> receiver = new List<Observer>();

        for(int i=0; i<notifyList.Count; i++)
        {
            Observer observer = notifyList[i];
            if(!observer.IsAlive()) { 
                observersToRemove.Add(observer);
                //since the observer may be destroyed after subscription
            } else {
                if(DEBUG) receiver.Add(observer);
                observer.Notify(aNotice);
            }
        }
        if(DEBUG) {
            string list = KZUtil.Join(receiver, 
                    o => o.GetType() + 
                    ((o.GetType() == typeof(MonoBehaviour))
                    ?
                    "(" + ((MonoBehaviour)o).GetInstanceID() +
                    ") of \"" + ((MonoBehaviour)o).gameObject.name + "\""
                    :
                    "")
                    , 
                    ", ");
            string msg="Sent \""+aNotice.name+"\" to ";
            Debug.Log(msg + list);
        }
 
        foreach(Observer observer in observersToRemove) {
            notifyList.Remove(observer);
        }
    }

    #endregion

    #region Observers

    public interface Observer
    {
        bool IsAlive();
        void Notify(KZNotice notice);
    }
    class ObjectObserver : Observer
    {
        System.Object _target;
        public ObjectObserver(System.Object target)
        {
            _target = target;
        }

        public bool IsAlive()
        {
            return !(_target == null || IsDestroyed(_target));
        }

        public void Notify(KZNotice notice)
        {
            KZUtil.Call(_target, notice.name, notice);
            //Since the target type of SendMessage() is GameObject,
            //when multiple scripts attached to the same GameObject
            //while listening to the same event, the event would be 
            //sent to the same GameObject several times and produce 
            //undesirable results. In order to fix the problem, we 
            //bypass GameObject, and use reflection to send the event 
            //directly to the observing Component.
        }

        public override bool Equals(object obj)
        {
            ObjectObserver oo = obj as ObjectObserver;
            if (oo == null)
                return false;
            else
                return this._target == oo._target;
        }

        public override int GetHashCode()
        {
            return _target.GetHashCode() + 13;
        }

        private static bool IsDestroyed(System.Object o)
        {
            return (o is UnityEngine.Object && ((UnityEngine.Object)o) == null) ?
                    true : false; //note the cast is important in Unity 3 and 4
        }

    }
    class ActionObserver : Observer
    {
        System.Action<KZNotice> _action;
        public ActionObserver(System.Action<KZNotice> action)
        {
            _action = action;
        }
        public bool IsAlive()
        {
            return _action != null;
        }
        public void Notify(KZNotice notice)
        {
            _action(notice);
        }
        public override bool Equals(object obj)
        {
            ActionObserver oo = obj as ActionObserver;
            if (oo == null)
                return false;
            else
                return this._action == oo._action;
        }
        public override int GetHashCode()
        {
            return _action.GetHashCode() + 13;
        }
    }

    #endregion


}


public class KZNotice
{
    public System.Object sender;
    public string name;
    public System.Object data;

    public KZNotice(System.Object aSender, string aName)
    {
        sender = aSender;
        name = aName;
    }

    public KZNotice(System.Object aSender, string aName, Hashtable aData)
    {
        sender = aSender;
        name = aName;
        data = aData;
    }

    public KZNotice(System.Object aSender, string aName, System.Object aData)
    {
        sender = aSender;
        name = aName;
        data = aData;
    }
}