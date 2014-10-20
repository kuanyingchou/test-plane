#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

public class KZBuilder {

    public static readonly string MANIFEST_TEMPLATE_PATH = 
            Application.dataPath +
            "/Plugins/Android/android_manifest_template.xml";
    public static readonly string MANIFEST_PATH =
            Application.dataPath + 
            "/Plugins/Android/AndroidManifest.xml";
    public static readonly string BUNDLE_ID_PATH =
            Application.dataPath + 
            "/Plugins/Android/bundle_id.txt";
    public static readonly string ENG_NAME_PATH =
            Application.dataPath + 
            "/Plugins/Android/eng.txt";

    private const string DEFAULT_OUTPUT_DIR="C:/";
    private const string OUTPUT_DIR_KEY="kztk_apk_output_dir";
    private const string TIME_FORMAT="yyMMdd";
    private const string COMPANY_NAME = "Kizilab";

    /*
    public static void BuildProductionFromCMD()
    {
        BuildApkFromCMD(false);
    }
    public static void BuildDevelopmentFromCMD()
    {
        BuildApkFromCMD(true);
    }
    */

    public static void BuildApkFromCMD() {
        string[] args = System.Environment.GetCommandLineArgs();
        string optionString = args[args.Length - 1];
        KZKeyValue options = new KZKeyValue(optionString);
        bool isDevelopment = options.GetBool("debug");
        bool isKizipad = options.GetBool("kizipad");

/*
        List<string> apkArg = args.Where(x => x.EndsWith(".apk")).ToList();
        apkArg.ForEach((x) => Debug.Log(x));
        if(apkArg.Count != 1) {
            throw new Exception("please specify output apk path");
        }
        string path = apkArg[0];
*/
        ProjectData pdata = GetProjectDataFrom(GetProjectFolderName());
        PlayerSettings.companyName = COMPANY_NAME;
        PlayerSettings.productName = pdata.productName;
        PlayerSettings.bundleIdentifier = pdata.bundleId;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
                BuildTargetGroup.Android, isKizipad?"KIZIPAD":"");
        FillEngName(pdata);
        CreateLocalizedFiles(pdata);
        SetKeyStore();
        KZIconCopier.CopyIcon();
        KZRevisionHelper.UpdateRevision();

        string err = BuildAndroid(
                path:options["path"], 
                options:isDevelopment?
                        BuildOptions.Development:
                        BuildOptions.None,
                visibleInLauncher:
                        (isKizipad && !isDevelopment)?false:true);
        if(!string.IsNullOrEmpty(err)) {
            throw new Exception(err);
        }
    }

    private static void SetKeyStore() {
        PlayerSettings.Android.keystoreName = 
                "C:/Users/user/Downloads/kizipad.keystore";
        PlayerSettings.Android.keystorePass = "kizipad";
        PlayerSettings.Android.keyaliasName = "kizipad";
        PlayerSettings.Android.keyaliasPass = "kizipad";
    }

    private static string GetDateTime(string format) {
        DateTime now = DateTime.Now;
        return now.ToString(format);
    }

    public static void BuildAndroidFromCMD() {
        string dir = GetOutputDirFromArgs();
        string folderName = GetProjectFolderName();
        ProjectData pdata = GetProjectDataFrom(folderName);
        //Debug.Log("project data from config: "+pdata);

        DateTime now = DateTime.Now;
        string dateTime = now.ToString(TIME_FORMAT);
        string fileName = pdata.id +"_"+ dateTime +".apk";
        string path = dir + "/" + fileName;

        PlayerSettings.companyName = COMPANY_NAME;
        PlayerSettings.productName = pdata.productName; //+ "_" + dateTime; //pdata.id + "_" + dateTime; 
        PlayerSettings.bundleIdentifier = pdata.bundleId;

        SetKeyStore();

        KZIconCopier.CopyIcon();

        //string rev = KZGit.GetCommit()+"("+KZGit.GetBranch()+")";
        //KZRevisionHelper.Increment(now, rev);

        KZRevisionHelper.UpdateRevision();

        string err = BuildAndroid(path, BuildOptions.None);

        if(err != null && err != "") {
            throw new Exception(err);
        }
    }
    public static void BuildAndroidFromGUI(BuildOptions options) {

        DateTime now = DateTime.Now;

        string fileName = GetProjectFolderName()+"_"+
                now.ToString(TIME_FORMAT);
        string path = GetPathFromDialog(fileName); 
        if(path==null) {
            return; 
        }

        //string rev = KZGit.GetCommit()+"("+KZGit.GetBranch()+")";
        //KZRevisionHelper.Increment(now, rev);
        KZRevisionHelper.UpdateRevision();

        string err = BuildAndroid(path, options); 

        if(err!=null && err.Length>0) {
            EditorUtility.DisplayDialog(
                    "KZ Toolkit", "Building Failed: "+err, "OK");
        } else {
            //EditorUtility.DisplayDialog("KZ Toolkit", "Building Done! Please see "+path, "OK");
        }
    }

    public static string BuildAndroid(string path, BuildOptions options, 
            bool visibleInLauncher = true) {
        GenerateAndroidManifest(visibleInLauncher);
        string err = Build( path, BuildTarget.Android, options);
        return err;
    }

    //private: 

    private static string Build(
            string path, 
            BuildTarget target, 
            BuildOptions opt) {

        string[] scenes = GetScenesInBuildSettings();

        EditorUserBuildSettings.SwitchActiveBuildTarget(
                target);

        Debug.Log("writing to "+path);

        return BuildPipeline.BuildPlayer(
                scenes, path, target, opt); 
    }
    private static string GetPathFromDialog(string fileName) {
        if( ! PlayerPrefs.HasKey(OUTPUT_DIR_KEY)) {
            PlayerPrefs.SetString(OUTPUT_DIR_KEY, DEFAULT_OUTPUT_DIR);
        }
        string path = EditorUtility.SaveFilePanel(
                "Select a path for APK file", 
                PlayerPrefs.GetString(OUTPUT_DIR_KEY), fileName, "apk");
        string dir = path.Substring(0, path.LastIndexOf("/")+1);

        if(Directory.Exists(dir)) {
            PlayerPrefs.SetString(OUTPUT_DIR_KEY, dir);
            return path;
        } else {
            return null;
        }
    }
    private static void GenerateAndroidManifest(bool visibleInLauncher) {
        string output = GetAndroidManifest(
                PlayerSettings.bundleIdentifier, 
                visibleInLauncher);
        KZAsset.WriteTextFile(MANIFEST_PATH, output);
    }

    //[MenuItem("KZBuild/Test Prjoect Name")]
    private static string GetProjectFolderName() {
        string dataPath = Application.dataPath; //.../ProjectDir/Assets
        string[] pathElements = dataPath.Split('/');
        //Debug.Log("project = " + pathElements[pathElements.Length-2]);
        if (pathElements.Contains(".jenkins"))
            return pathElements[pathElements.Length - 3]; //skip "workspace"
        else
            return pathElements[pathElements.Length - 2]; //skip "Assets"
    }

    [MenuItem ("KZTK/Development/TestGetProjectData")]
    public static void TestGetProjectDataFrom() {
        ProjectData p = 
                GetProjectDataFrom("family");
        KZUtil.Assert(p.id == "ENG_05"); 
        p = GetProjectDataFrom("mcdonald");
        KZUtil.Assert(p.id == "CS_17"); 
        p = GetProjectDataFrom("traffic");
        KZUtil.AssertEquals(p.id, "SOC_02"); 
        //Debug.Log(p);
    }

    //a folderName can be a codename(ex. family) or an id(ex. MAT_09)
    private static ProjectData GetProjectDataFrom(string folderName) {
        if(folderName == null) throw new Exception("illegal argument");
        string bundleIds = KZAsset.ReadTextFile(BUNDLE_ID_PATH);
        using (StringReader reader = new StringReader(bundleIds)) {
            string record;
            while((record = reader.ReadLine()) != null) { 
                record = record.Trim();
                if(String.IsNullOrEmpty(record)) continue;
                if(record.StartsWith("#")) continue;
//Debug.Log("read line: \""+record +"\"");
                string[] values = record.Split(';');
                //[
                //if(values.Length != 3 || values.Length != 4) continue; 
                //] bug!
                if(!(values.Length == 3 || values.Length == 4)) continue;
//Debug.Log(KZUtil.Join(values, ", ") + " >>> "+values.Length);
                string id = values[0].Trim();
                string bundleId = values[1];
                string packageName = values[2];

                if(values.Length == 4) {
                    string codename = values[3].Trim();
                    if(folderName.Equals(codename)) {
                        return new ProjectData(id, bundleId, packageName, codename);
                    }
                } else if(values.Length == 3) {
                    if(folderName.Equals(id)) {
                        return new ProjectData(id, bundleId, packageName, null);
                    }
                }
            }
        }
        //[ non found, create a dummy one
        return new ProjectData(
            folderName, 
            "com.allproducts.kizi."+folderName.ToLower().Replace("_", ""),
            PlayerSettings.productName,
            folderName
        );
    }

    private static void FillEngName(ProjectData pdata)
    {
        string names = KZAsset.ReadTextFile(ENG_NAME_PATH);
        Dictionary<string, string> idNamePair = new Dictionary<string, string>();
        using (StringReader reader = new StringReader(names)) {
            string record;
            while((record = reader.ReadLine()) != null) { 
                if(string.IsNullOrEmpty(record)) continue;
                string[] values = record.Split(';');
                if(values.Length != 2) continue;
                string id = values[0];
                string eng = values[1];
                idNamePair[id] = eng;
            }
        }
        if(idNamePair.ContainsKey(pdata.id))
        {
            pdata.productNameEng = idNamePair[pdata.id];
        } else 
        {
            pdata.productNameEng = pdata.productName;
        }
    }

    private static void CreateLocalizedFiles(ProjectData pdata)
    {
        string[] supportedLocales = {"", "-zh-rtw", "-en"};
        string[] appNames = {pdata.productName, pdata.productName, pdata.productNameEng};
        for(int i=0; i<supportedLocales.Length; i++)
        {
            string locale = supportedLocales[i];
            string path = Application.dataPath+"/Plugins/Android/res/values"+locale;
            if(System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, recursive:true);
            }

            System.IO.Directory.CreateDirectory(path);
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();

            string xml = string.Format(
"<?xml version=\"1.0\" encoding=\"utf-8\"?>"+System.Environment.NewLine+
"<resources>"+System.Environment.NewLine+
" <string name=\"app_name\">{0}</string>"+System.Environment.NewLine+
"</resources>", 
            appNames[i]);
            
            KZAsset.WriteTextFile(path+"/strings.xml", xml);
        }

    }

    private class ProjectData {
        public readonly string id;          //ex. MAT_09
        public readonly string codename;    //ex. family
        public readonly string bundleId;    //ex. com.allproducts.kizi.mat9
        public readonly string productName; //ex. Mr. McDonald
        public string productNameEng;

        public ProjectData(string i, string bid, string pn, string cn) {
            id=i; bundleId=bid; productName=pn; codename=cn;
KZDebug.Log("create project data: "+pn);
        }
        public override string ToString() {
            return "id = " + id + 
                   ", codename = " + codename +
                   ", bundle id = " + bundleId + 
                   ", product name = " + productName;
        }
    }

    private static string GetOutputDirFromArgs() {
        string[] args=System.Environment.GetCommandLineArgs();
        return args[args.Length - 1];
    }

    //[ modified from http://answers.unity3d.com/questions/33263/how-to-get-names-of-all-available-levels.html
    public static string[] GetScenesInBuildSettings() {
        List<string> scenePaths = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene s in UnityEditor.EditorBuildSettings.scenes) {
            if (s.enabled) {
                scenePaths.Add(s.path);
            }
        }
        return scenePaths.ToArray();
    }

    private static string GetAndroidManifest(string bundleId, bool visibleInLauncher) {
        string template = KZAsset.ReadTextFile(
                MANIFEST_TEMPLATE_PATH);
        string manifest = 
                template.Replace("__bundleid__", bundleId);

        string category;
        if(visibleInLauncher) {
            category = "<category android:name=\"android.intent.category.LAUNCHER\" />";
        } else {
            category = "<category android:name=\"android.intent.category.INFO\" /> ";
        }
        manifest = manifest.Replace("__category__", category);

        if(IsQuestionnaire(bundleId)) {
            //Debug.Log("replacing landscape settings");
            manifest = manifest.Replace("landscape", "portrait");
        }
        return manifest;
    }
    private static bool IsQuestionnaire(string bundleId) {
        if("com.allproducts.kizi.sur1;com.allproducts.kizi.sur2;com.allproducts.kizi.sur3".
                Contains(bundleId)) return true;
        else return false;
    }
}
#endif
