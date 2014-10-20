#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//2013.3.21  ken  add "Build", "Build and Run"
//2013.3.14  ken  initial version
public class KZBuilderMenu : EditorWindow {

    //private static string outputPath=null;
    //[MenuItem("KZBuild/Build Number++")]
    //public static void IncrdmentBuildNumber() {
    //    SetupRevisionNumber(true);
    //}
    //[MenuItem("KZBuild/Setup Revision Number")]
    //public static void GenerateRevisionClass() {
    //    SetupRevisionNumber(false);
    //}
    
    [MenuItem("KZBuild/Build")]
    public static void Build() {
        KZBuilder.BuildAndroidFromGUI(BuildOptions.None);
    }
    
    [MenuItem("KZBuild/Build And Run")]
    public static void BuildAndRun(){
        KZBuilder.BuildAndroidFromGUI(
                BuildOptions.AutoRunPlayer);
    }
    
    [MenuItem("KZBuild/Build (Development)")]
    public static void BuildDevelopment() {
        KZBuilder.BuildAndroidFromGUI(BuildOptions.Development);
    }

    [MenuItem("KZBuild/Build And Run (Development)")]
    public static void BuildAndRunDevelopment(){
        KZBuilder.BuildAndroidFromGUI(
                BuildOptions.AutoRunPlayer | BuildOptions.Development);
    }
}
#endif
