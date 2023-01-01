using UnityEngine;
using UnityEditor;
using System;
namespace OpenBehaviourTree{
public class BehaviourTreeBlackboardAsset
{
    [MenuItem("Assets/Create/BehaviourTreeBlackboard")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<BehaviourTreeBlackboard>();
    }
}
}
