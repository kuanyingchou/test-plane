#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class KZIconCopier {
    private static string ICON_NAME="app_icon.png";
    private static string DEST
            = "Assets/Plugins/Android/res/drawable";
    //private static string DEST_XHDPI
    //        = "Assets/Plugins/Android/res/drawable-xhdpi";

    [MenuItem("KZTK/Development/CopyIcon")]
    public static void CopyIcon() {
        Texture2D icon = GetIcon();
        if(icon == null) {
            return;
        } else {
            CopyIcon(icon, DEST);
            //CopyIcon(icon, DEST_XHDPI);
            ClearIcon();
        }
    }

    private static void CopyIcon(Texture2D icon, string path) {
        Directory.CreateDirectory(path);
        AssetDatabase.CopyAsset(
                AssetDatabase.GetAssetPath(icon), path + "/"+ICON_NAME); 
        AssetDatabase.Refresh();
    }

    private static Texture2D GetIcon() {
        Texture2D[] icons = PlayerSettings.GetIconsForTargetGroup(
                BuildTargetGroup.Unknown);
        if(icons.Length <= 0 || icons[0] == null) {
            icons = PlayerSettings.GetIconsForTargetGroup(
                BuildTargetGroup.Android);
        } 
        //Debug.Log(KZUtil.Join(icons, ", "));
        if(icons.Length <= 0) return null;
        else return icons[0];
    }
    private static void ClearIcon() {
        ClearIcon(BuildTargetGroup.Android);
        ClearIcon(BuildTargetGroup.Unknown);
    }
    private static void ClearIcon(BuildTargetGroup t) {
        int[] sizes = PlayerSettings.GetIconSizesForTargetGroup(t);
        Texture2D[] icons = new Texture2D[sizes.Length];
        PlayerSettings.SetIconsForTargetGroup(t, icons);
    }
}
#endif
