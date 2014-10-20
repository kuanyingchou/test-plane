using UnityEngine;
using System;
using System.IO;

/*
A revision number should be updated with each build, and can be accessed by
calling KZRevision.GetRevision(). A revision number should also be carried 
to different devices(so users can see it on their device), so we saved it
in Unity's Resources directory. PlayerPref can not be used here because the 
location of the data is platform-dependent(it uses registry on Windows, 
and /data/data/<bundle-id>/shared_prefs/ on Android). 
*/

public class KZRevision {
    public DateTime updateTime;
    public string branch;
    public string commitCount; //TODO: use int instead
    public string latestCommit;
    public string hash;

    //public static string WIN_EDITOR_FILE_PATH = 
    //        Application.dataPath + "Plugins/Resources/version.xml";

    //[ we need this for XML Serializer
    public KZRevision() {}

    public KZRevision(DateTime t, string b, string cc, string lc, string h) {
        this.updateTime = t;
        this.branch = b;
        this.commitCount = cc;
        this.latestCommit = lc;
        this.hash = h;
    }

    public static KZRevision Load() {
        KZXML<KZRevision> x = new KZXML<KZRevision>();
        TextAsset textFile = Resources.Load("version") as TextAsset;
        if(textFile == null) {
            return null;
        } else {
            return x.LoadString(textFile.text);
        }
    }

    public override string ToString() {
        return string.Format(
                "On branch {0}, {1} commits. {2}",
                branch,
                commitCount, 
                latestCommit);
    }

    public string ToHTML() {
        return string.Format(
                "On branch <b>{0}</b>, <b>{1}</b> commits ( {2} ).",
                branch,
                commitCount, 
                latestCommit);
    }
}
