using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public class NodeTimer 
{
    float timerVal;

    bool timerExceeded;
    bool isActive;
    Coroutine timerFunc;
    MonoBehaviour monoBehaviour;

    public NodeTimer(
        float timerVal
    ){
        this.timerVal = timerVal;
        isActive = false;
        timerExceeded = false;
    }

    public void LoadTask(MonoBehaviour monoBehaviour){
        this.monoBehaviour = monoBehaviour;
    }

    public void StartTimer(){
        ResetTimer();
        timerFunc = this.monoBehaviour.StartCoroutine(ExecuteTimer());
    }

    public void StopTimer(){
        this.monoBehaviour.StopCoroutine(timerFunc);
    }

    public void ResetTimer(){
        StopTimer();
        timerExceeded = false;
    }

    IEnumerator ExecuteTimer(){
        isActive = true;
        yield return new WaitForSeconds(timerVal);
        isActive = false;
        timerExceeded = true;
    }

    public bool IsActive(){return isActive;}

    public bool TimerExceeded(){return timerExceeded;}
}
}