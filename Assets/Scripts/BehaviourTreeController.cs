using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Behaviour{
public class BehaviourTreeController : MonoBehaviour
{
    public BehaviourTree behaviourTree;
    private BehaviourTree treeInstance;
    private BehaviourTreeBlackboard blackboardInstance;
    private bool showingTree;
    public bool showTree;

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
        showingTree = false;
        Selection.selectionChanged += ShowWindowIfSelected;
    }

    void Start(){
        ShowWindowIfSelected();
    }

    void ShowWindowIfSelected(){
        if (Selection.activeObject == this.gameObject && !showingTree && showTree){
            showingTree = true;
            ShowTree();
        }
        else if (Selection.activeObject != this.gameObject && showingTree){
            showingTree = false;
            StopShowingTree();
        }

    }

    void ShowTree(){
        BehaviourTreeEditor window = EditorWindow.GetWindow(typeof(BehaviourTreeEditor)) as BehaviourTreeEditor;
        window.SetScriptableObject(treeInstance, BehaviourTreeEditorMode.Runtime);
    }

    void StopShowingTree(){}

    public void RunTree(){
        StartCoroutine(runTree);
    }

    public void StopTree(){
        treeInstance.ResetTree();
        StopCoroutine(runTree);
    }

    void OnDestroy(){
        Selection.selectionChanged -= ShowWindowIfSelected;
    }

}
}