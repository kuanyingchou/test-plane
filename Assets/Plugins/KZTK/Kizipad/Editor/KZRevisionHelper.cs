#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class KZRevisionHelper {
    public static string PATH = Application.dataPath + "/Plugins/KZTK/Common/Runtime/Resources/version.xml";

    private static KZRevision Load() {
        return KZRevision.Load();
    }

    private static void Save(KZRevision r) {
        KZXML<KZRevision> x = new KZXML<KZRevision>();
        string output = x.SaveString(r);
        KZAsset.WriteTextFile(PATH, output);
    }
    /*
    public static void Increment(DateTime buildTime, string rev) {
        KZRevision r = Load();
        if(r == null) {
            r = new KZRevision(0, buildTime, rev);
            Save(r);
        } else {
            r.id += 1;
            r.buildTime = buildTime;
            r.revision = rev;
            Save(r);
        } 
    }
    */

    [MenuItem("KZTK/Update Revision")]
    public static void UpdateRevision() {
        Save(new KZRevision(
                DateTime.Now, 
                KZGit.GetBranch(), 
                KZGit.GetCommitCount(), 
                KZGit.GetLatestCommit(),
                KZGit.GetHash()));
        Debug.Log(PATH + " updated.");
    }


}
#endif
