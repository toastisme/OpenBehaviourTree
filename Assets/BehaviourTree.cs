using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : ScriptableObject 
{
    public List<GUINode> nodes;
    public List<Connection> connections;

}
