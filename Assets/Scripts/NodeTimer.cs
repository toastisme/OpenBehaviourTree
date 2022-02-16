using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public class NodeTimer 
{

    /**
     * \class NodeTimer
     * Class used to allow different timer decorators to be added to nodes
     * in BehaviourTrees (e.g. timeout and cooldown nodes).
     */

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
        if (timerFunc != null){
            this.monoBehaviour.StopCoroutine(timerFunc);
        }
        isActive = false;
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

    public void SetTimerVal(float timerVal){
        this.timerVal = timerVal;
    }

    public float GetTimerVal(){return this.timerVal;}
}
}