
using UnityEngine;
using UnityEditor;
using System;
public class BehaviourTreeBlackboardAsset
{
    [MenuItem("Assets/Create/BehaviourTreeBlackboard")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<BehaviourTreeBlackboard>();
    }
}
