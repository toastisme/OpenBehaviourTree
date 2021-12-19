using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public class NodeProperties
{
    public static Vector2 GUINodeSize(){return new Vector2(200, 100);}
    public static Vector2 SubNodeSize(){return new Vector2(200, 50);}
    public static GUIStyle GUINodeStyle(){
        GUIStyle nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.alignment = TextAnchor.UpperCenter;
        return nodeStyle;
    }

    public static GUIStyle SelectedGUINodestyle(){
        GUIStyle selectedNodeStyle = GUINodeStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        return selectedNodeStyle;
    }

    public static GUIStyle DecoratorStyle(){
        GUIStyle decoratorStyle = new GUIStyle();
        decoratorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        decoratorStyle.border = new RectOffset(12, 12, 12, 12);
        decoratorStyle.normal.textColor = Color.white;
        decoratorStyle.alignment = TextAnchor.MiddleCenter;
        return decoratorStyle;
    }

    public static GUIStyle SelectedDecoratorStyle(){
        GUIStyle selectedDecoratorStyle =  DecoratorStyle();
        selectedDecoratorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        return selectedDecoratorStyle;
    }

    public static GUIStyle SequenceSelectorStyle(){
        GUIStyle sequenceSelectorStyle = new GUIStyle();
        sequenceSelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        sequenceSelectorStyle.border = new RectOffset(12, 12, 12, 12);
        sequenceSelectorStyle.normal.textColor = Color.white;
        sequenceSelectorStyle.alignment = TextAnchor.MiddleCenter;
        return sequenceSelectorStyle;
    }

    public static GUIStyle SelectedSequenceSelectorStyle(){
        GUIStyle selectedSequenceSelectorStyle =  SequenceSelectorStyle();
        selectedSequenceSelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        return selectedSequenceSelectorStyle;
    }

    public static GUIStyle PrioritySelectorStyle(){
        GUIStyle prioritySelectorStyle = new GUIStyle();
        prioritySelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        prioritySelectorStyle.border = new RectOffset(12, 12, 12, 12);
        prioritySelectorStyle.normal.textColor = Color.white;
        prioritySelectorStyle.alignment = TextAnchor.MiddleCenter;
        return prioritySelectorStyle;
    }

    public static GUIStyle SelectedPrioritySelectorStyle(){
        GUIStyle selectedPrioritySelectorStyle =  PrioritySelectorStyle();
        selectedPrioritySelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        return selectedPrioritySelectorStyle;
    }

    public static GUIStyle ProbabilitySelectorStyle(){
        GUIStyle probabilitySelectorStyle = new GUIStyle();
        probabilitySelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        probabilitySelectorStyle.border = new RectOffset(12, 12, 12, 12);
        probabilitySelectorStyle.normal.textColor = Color.white;
        probabilitySelectorStyle.alignment = TextAnchor.MiddleCenter;
        return probabilitySelectorStyle;
    }

    public static GUIStyle SelectedProbabilitySelectorStyle(){
        GUIStyle selectedProbabilitySelectorStyle =  ProbabilitySelectorStyle();
        selectedProbabilitySelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        return selectedProbabilitySelectorStyle;
    }
    public static GUIStyle ActionStyle(){
        GUIStyle actionStyle = new GUIStyle();
        actionStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        actionStyle.border = new RectOffset(12, 12, 12, 12);
        actionStyle.normal.textColor = Color.white;
        actionStyle.alignment = TextAnchor.MiddleCenter;
        return actionStyle;
    }

    public static GUIStyle SelectedActionStyle(){
        GUIStyle selectedActionStyle =  ActionStyle();
        selectedActionStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        return selectedActionStyle;
    }

    public static GUIStyle ProbabilityWeightStyle(){
        GUIStyle probabilityWeightStyle = new GUIStyle();
        probabilityWeightStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        probabilityWeightStyle.border = new RectOffset(12, 12, 12, 12);
        probabilityWeightStyle.normal.textColor = Color.white;
        probabilityWeightStyle.alignment = TextAnchor.MiddleCenter;
        return probabilityWeightStyle;
    }

    public static GUIStyle SelectedProbabilityWeightStyle(){
        GUIStyle selectedProbabilityWeightStyle = ProbabilityWeightStyle();
        selectedProbabilityWeightStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        return selectedProbabilityWeightStyle;
    }

    public static GUIStyle CallNumberStyle(){
        GUIStyle callNumberStyle = new GUIStyle();
        callNumberStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        callNumberStyle.border = new RectOffset(12, 12, 12, 12);
        callNumberStyle.normal.textColor = Color.white;
        callNumberStyle.alignment = TextAnchor.MiddleCenter;
        return callNumberStyle;
    }

    public static GUIStyle ChildPointStyle(){
        GUIStyle childPointStyle = new GUIStyle();
        childPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn.png") as Texture2D;
        childPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn on.png") as Texture2D;
        childPointStyle.border = new RectOffset(4, 4, 12, 12);
        childPointStyle.alignment = TextAnchor.MiddleCenter;
        return childPointStyle;
    }

    public static GUIStyle ParentPointStyle(){
        GUIStyle parentPointStyle = new GUIStyle();
        parentPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn.png") as Texture2D;
        parentPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn on.png") as Texture2D;
        parentPointStyle.border = new RectOffset(4, 4, 12, 12);
        parentPointStyle.alignment = TextAnchor.MiddleCenter;
        return parentPointStyle;
    }
}
