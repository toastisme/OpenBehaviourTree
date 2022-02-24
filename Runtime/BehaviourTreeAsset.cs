using UnityEngine;
using UnityEditor;
using System;
namespace Behaviour{
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
                BehaviourTreeEditor window = EditorWindow.GetWindow(typeof(BehaviourTreeEditor)) as BehaviourTreeEditor;
                window.SetScriptableObject(scriptableObject);
                window.Show();
                AssetDatabase.Refresh();
                return true;
            }
            return false; //let unity open it.
        }

    }

}