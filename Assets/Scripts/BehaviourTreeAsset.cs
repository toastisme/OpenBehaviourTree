using UnityEngine;
using UnityEditor;
using System;
namespace BehaviourBase{
    public class BehaviourTreeAsset
    {
        [MenuItem("Assets/Create/BehaviourTree")]
        public static void CreateAsset ()
        {
            ScriptableObjectUtility.CreateAsset<BehaviourTree>();
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string assetPath = AssetDatabase.GetAssetPath(instanceID);
            BehaviourTree scriptableObject = AssetDatabase.LoadAssetAtPath<BehaviourTree>(assetPath);
            if (scriptableObject != null)
            {
                NodeBasedEditor window = EditorWindow.GetWindow(typeof(NodeBasedEditor)) as NodeBasedEditor;
                window.SetScriptableObject(scriptableObject);
                window.Show();
                return true;
            }
            return false; //let unity open it.
        }

    }

}