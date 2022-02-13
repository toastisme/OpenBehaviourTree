using UnityEngine;
using UnityEditor;
using System.IO;
 
public static class ScriptableObjectUtility
{
    /**
     * \class ScriptableObjectUtility
     * Helper functions for using ScirptableObjects.
     * Taken from https://forum.unity.com/threads/saving-scriptable-objects-solved.551866/
     */

    public static void CreateAsset<T> () where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T> ();
 
        string path = AssetDatabase.GetAssetPath (Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension (path) != "")
        {
            path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
        }
 
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + typeof(T).ToString() + ".asset");
 
        AssetDatabase.CreateAsset (asset, assetPathAndName);
 
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow ();
        Selection.activeObject = asset;
 
    }
 
    public static void OnDestroy()
    {
        AssetDatabase.SaveAssets();
 
    }
 
    public static void OnApplicationQuit()
    {
        AssetDatabase.SaveAssets();
    }
}
 