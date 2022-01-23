using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public class NodeTimer 
{
    float timerVal;
    bool isActive;
    Coroutine timerFunc;
    MonoBehaviour monoBehaviour;

    public NodeTimer(
        float timerVal
    ){
        this.timerVal = timerVal;
        isActive = false;
    }

    public void LoadTask(MonoBehaviour monoBehaviour){
        this.monoBehaviour = monoBehaviour;
    }

    public void StartTimer(){
        timerFunc = this.monoBehaviour.StartCoroutine(ExecuteTimer());
    }

    public void StopTimer(){
        this.monoBehaviour.StopCoroutine(timerFunc);
    }

    IEnumerator ExecuteTimer(){
        isActive = true;
        yield return new WaitForSeconds(timerVal);
        isActive = false;
    }

    public bool IsActive(){return isActive;}
}
}