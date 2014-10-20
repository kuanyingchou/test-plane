#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

class KZExporter {
    private const string DEFAULT_OUTPUT_PATH="C:/";
    private const string OUTPUT_PATH_KEY="kztk_pkg_output_path";

    [MenuItem("KZTK/Development/Export Package")]
    public static void ExportPackage() {
        DateTime now = DateTime.Now;
        string fileName = "kztk_"+now.ToString("yyyyMMdd_HHmmss");
        string path = EditorUtility.SaveFilePanel(
                "Select path for package file", 
                PlayerPrefs.GetString(OUTPUT_PATH_KEY), 
                fileName, 
                "unitypackage");
        if(string.IsNullOrEmpty(path)) {
            return;
        }

        ExportPackage(path);
    }

    public static void ExportPackageFromCLI() {
        string[] args = System.Environment.GetCommandLineArgs();
        ExportPackage(args[8]); //>>>
    }

    public static void ExportPackage(string path) {
        AssetDatabase.ExportPackage(
            new string[] {"Assets/KZTK", "Assets/Plugins"},
            path,
            /*ExportPackageOptions.Interactive | */
            ExportPackageOptions.Default | 
            ExportPackageOptions.Recurse);
    }

    public static void ExportSourcePackageFromCLI() {
        string[] args = System.Environment.GetCommandLineArgs();
        List<string> arg = args.Where(x => x.EndsWith(".unitypackage"))
                .ToList();
        AssetDatabase.ExportPackage(
            new string[] {"Assets/Plugins"},
            arg[0],
            ExportPackageOptions.Default | 
            ExportPackageOptions.Recurse);
        
    }
    public static void ExportSourcePackage(string path) {
        AssetDatabase.ExportPackage(
            new string[] {"Assets/Plugins"},
            path,
            ExportPackageOptions.Default | 
            ExportPackageOptions.Recurse);
    }
}
#endif
