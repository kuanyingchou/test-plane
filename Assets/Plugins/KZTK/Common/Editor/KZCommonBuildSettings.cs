#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class KZCommonBuildSettings : EditorWindow 
{

    private static Texture2D splash = null;
    private static Texture2D icon = null;
    private static string bundleId = "";
    private static string[] orientationStrs = new string[]
    {
        "Portrait",
        "Portrait Upside Down",
        "Landscape Right",
        "Landscape Left",
        "Auto Rotation",
    };
    private Vector3 scrollPos = Vector3.zero;

    [MenuItem ("KZTK/Common Build Settings...")]
    public static void Show () {
        KZCommonBuildSettings window = 
                (KZCommonBuildSettings)EditorWindow.GetWindow(
                typeof (KZCommonBuildSettings));
        window.autoRepaintOnSceneChange = false;
    }

    public KZCommonBuildSettings()
    {
        title = "Common Build Settings";
    }

    public void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        EditorGUILayout.BeginVertical();
        PlayerSettings.companyName = 
                EditorGUILayout.TextField("Company Name", 
                PlayerSettings.companyName);
        PlayerSettings.productName = 
                EditorGUILayout.TextField("Product Name", 
                PlayerSettings.productName);
        PlayerSettings.bundleIdentifier = 
                EditorGUILayout.TextField("Bundle ID", 
                PlayerSettings.bundleIdentifier);
        PlayerSettings.resolutionDialogBanner = 
                EditorGUILayout.ObjectField(
                "Splash Image", 
                PlayerSettings.resolutionDialogBanner, 
                typeof(Texture2D), false) as Texture2D;

        Texture2D[] icons = PlayerSettings.GetIconsForTargetGroup(
                BuildTargetGroup.Unknown);
        icons[0] = EditorGUILayout.ObjectField(
                "Default Icon", (icons.Length>0)?icons[0]:null, 
                typeof(Texture2D), false) 
                as Texture2D;
        
        PlayerSettings.SetIconsForTargetGroup(
                BuildTargetGroup.Unknown, icons);
        PlayerSettings.defaultInterfaceOrientation = 
                ToUIOrientation(
                    EditorGUILayout.Popup(
                        label:"Orientation", 
                        selectedIndex:ToOrientationIndex(
                            PlayerSettings.defaultInterfaceOrientation
                        ),
                        displayedOptions:orientationStrs
                    )
                );
        PlayerSettings.bundleVersion = 
                EditorGUILayout.TextField(
                "Version Name",
                PlayerSettings.bundleVersion); 
        PlayerSettings.Android.bundleVersionCode = 
                EditorGUILayout.IntField(
                "Android Version Code",
                PlayerSettings.Android.bundleVersionCode);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    private static int ToOrientationIndex(UIOrientation o)
    {
        switch(o)
        {
        case UIOrientation.Portrait:
            return 0;
        case UIOrientation.PortraitUpsideDown:
            return 1;
        case UIOrientation.LandscapeRight:
            return 2;
        case UIOrientation.LandscapeLeft:
            return 3;
        case UIOrientation.AutoRotation:
            return 4;
        default:
            Debug.LogError("what?");
            return 3;
        }
    }
    private static UIOrientation ToUIOrientation(int index)
    {
        switch(index)
        {
        case 0:
            return UIOrientation.Portrait;
        case 1:
            return UIOrientation.PortraitUpsideDown;
        case 2:
            return UIOrientation.LandscapeRight;
        case 3:
            return UIOrientation.LandscapeLeft;
        case 4:
            return UIOrientation.AutoRotation;
        default:
            Debug.LogError("what?");
            return UIOrientation.LandscapeLeft;
        }
    }
}
#endif
