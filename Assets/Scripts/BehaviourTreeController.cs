using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public class BehaviourTreeController : MonoBehaviour
{
    public BehaviourTree behaviourTree;
    private BehaviourTree treeInstance;
    private BehaviourTreeBlackboard blackboardInstance;

    IEnumerator runTree;

    void Awake(){

        /**
        * Get instances of the behaviour tree and blackboard, and update
        * the nodes of the tree with the blackboard instance
        */

        treeInstance = Object.Instantiate(behaviourTree);
        blackboardInstance = Object.Instantiate(behaviourTree.blackboard);
        treeInstance.LoadTree(monoBehaviour:this, blackboard:ref blackboardInstance);
        runTree = treeInstance.RunTree();
    }

    public void RunTree(){
        StartCoroutine(runTree);
    }

    public void StopTree(){
        treeInstance.ResetTree();
        StopCoroutine(runTree);
    }

}
}