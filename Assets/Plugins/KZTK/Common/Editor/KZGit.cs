 #if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;

class KZGit {

    [MenuItem("KZTK/Development/TestKZGit")]
    public static void TestKZGit() {
        Debug.Log(GetLatestCommit());
        Debug.Log(GetBranch());
        Debug.Log(GetCommitCount());
    }

    public static string GetLatestCommit() {
        //return Trim(KZUtil.Run("git", "log -n 1 --oneline"));
        return Trim(KZUtil.Run("git", 
                "log -n 1 --pretty=format:\"%h %an \\\"%s\\\""));
                               
    }

    public static string GetHash()
    {
        return Trim(KZUtil.Run("git",
                "log -n 1 --pretty=format:%h"));
    }

    public static string GetBranch() {
        return Trim(KZUtil.Run("git", "rev-parse --abbrev-ref HEAD").Trim());
    }

    public static string GetCommitCount() {
        return Trim(KZUtil.Run("git", "rev-list HEAD --count")); 
    }

    public static string Trim(string input) {
        return input.TrimEnd('\r', '\n');
    }

}
#endif
