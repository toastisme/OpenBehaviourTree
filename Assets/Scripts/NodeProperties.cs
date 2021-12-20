using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
public enum NodeType{
    Root,
    SequenceSelector,
    ProbabilitySelector,
    PrioritySelector,
    Action,
    Decorator,
    ProbabilityWeight,
    CallNumber
}

public class NodeColors
{
    public Color defaultColor = NodeProperties.GetDefaultColor();
    public Color callNumberColor = NodeProperties.GetDefaultColor();
    public Color actionColor = NodeProperties.GetDefaultColor();
    public Color probabilityWeightColor = NodeProperties.GetDefaultColor();
    public Color decoratorColor = NodeProperties.GetDecoratorColor();
    public Color sequenceSelectorColor = NodeProperties.GetSequenceSelectorColor();
    public Color prioritySelectorColor = NodeProperties.GetPrioritySelectorColor();
    public Color probabilitySelectorColor = NodeProperties.GetProbabilitySelectorColor();

    public Color GetColor(NodeType nodeType){
        switch(nodeType){
            case NodeType.Root:
                return defaultColor;
            case NodeType.Decorator:
                return decoratorColor;
            case NodeType.SequenceSelector:
                return sequenceSelectorColor;
            case NodeType.PrioritySelector:
                return prioritySelectorColor;
            case NodeType.ProbabilitySelector:
                return probabilitySelectorColor;
            case NodeType.CallNumber:
                return callNumberColor;
            case NodeType.ProbabilityWeight:
                return probabilityWeightColor;
            case NodeType.Action:
                return actionColor;
            default:   
                throw new Exception("Unknown NodeType");
        }
    }
}

public class NodeSizes
{
    public Vector2 guiNodeSize = NodeProperties.GUINodeSize();    
    public Vector2 subNodeSize = NodeProperties.SubNodeSize();
}

public class NodeStyles
{
    public GUIStyle guiNodeStyle = NodeProperties.GUINodeStyle();
    public GUIStyle selectedGuiNodeStyle = NodeProperties.SelectedGUINodeStyle();
    public GUIStyle rootStyle = NodeProperties.RootStyle();
    public GUIStyle sequenceSelectorStyle = NodeProperties.SequenceSelectorStyle();
    public GUIStyle prioritySelectorStyle = NodeProperties.PrioritySelectorStyle();
    public GUIStyle probabilitySelectorStyle = NodeProperties.ProbabilitySelectorStyle();
    public GUIStyle probabilityWeightStyle = NodeProperties.ProbabilityWeightStyle();
    public GUIStyle selectedProbabilityWeightStyle = NodeProperties.SelectedProbabilityWeightStyle();
    public GUIStyle decoratorStyle = NodeProperties.DecoratorStyle();
    public GUIStyle selectedDecoratorStyle = NodeProperties.SelectedDecoratorStyle();
    public GUIStyle callNumberStyle = NodeProperties.CallNumberStyle();
    public GUIStyle actionStyle = NodeProperties.ActionStyle();
    public GUIStyle childPointStyle = NodeProperties.ChildPointStyle();
    public GUIStyle parentPointStyle = NodeProperties.ParentPointStyle();

    public GUIStyle GetStyle(NodeType nodeType){
        switch(nodeType){
            case NodeType.Root:
                return rootStyle;
            case NodeType.SequenceSelector:
                return sequenceSelectorStyle;
            case NodeType.ProbabilitySelector:
                return probabilitySelectorStyle;
            case NodeType.ProbabilityWeight:
                return probabilityWeightStyle;
            case NodeType.PrioritySelector:
                return prioritySelectorStyle;
            case NodeType.Action:
                return actionStyle;
            case NodeType.CallNumber:
                return callNumberStyle;
            case NodeType.Decorator:
                return decoratorStyle;
            default:
                throw new Exception("Unknown node type");
        }
    }
}

public class NodeProperties
{
    // Size
    public static Vector2 GUINodeSize(){return new Vector2(200, 100);}
    public static Vector2 SubNodeSize(){return new Vector2(200, 60);}

    // Color
    public static Color GetDefaultColor(){return new Color(38.0f/255.0f, 70.0f/255.0f, 83.0f/255.0f);}
    public static Color GetDecoratorColor(){return new Color(42.0f/255.0f, 157.0f/255.0f, 143.0f/255.0f);}
    public static Color GetSequenceSelectorColor(){return new Color(233.0f/255.0f, 196.0f/255.0f, 106.0f/255.0f);}
    public static Color GetProbabilitySelectorColor(){return new Color(244.0f/255.0f, 162.0f/255.0f, 97.0f/255.0f);}
    public static Color GetPrioritySelectorColor(){return new Color(231.0f/255.0f, 111.0f/255.0f, 81.0f/255.0f);}

    // Style

    public static GUIStyle GUINodeStyle(){
        GUIStyle nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.alignment = TextAnchor.UpperCenter;
        return nodeStyle;
    }

    public static GUIStyle SelectedGUINodeStyle(){
        GUIStyle selectedNodeStyle = GUINodeStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        return selectedNodeStyle;
    }

    public static GUIStyle RootStyle(){
        GUIStyle rootStyle = new GUIStyle();
        rootStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        rootStyle.border = new RectOffset(12, 12, 12, 12);
        rootStyle.normal.textColor = Color.white;
        rootStyle.alignment = TextAnchor.UpperCenter;
        return rootStyle;
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
        sequenceSelectorStyle.alignment = TextAnchor.UpperCenter;
        return sequenceSelectorStyle;
    }

    public static GUIStyle PrioritySelectorStyle(){
        GUIStyle prioritySelectorStyle = new GUIStyle();
        prioritySelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        prioritySelectorStyle.border = new RectOffset(12, 12, 12, 12);
        prioritySelectorStyle.normal.textColor = Color.white;
        prioritySelectorStyle.alignment = TextAnchor.UpperCenter;
        return prioritySelectorStyle;
    }

    public static GUIStyle ProbabilitySelectorStyle(){
        GUIStyle probabilitySelectorStyle = new GUIStyle();
        probabilitySelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        probabilitySelectorStyle.border = new RectOffset(12, 12, 12, 12);
        probabilitySelectorStyle.normal.textColor = Color.white;
        probabilitySelectorStyle.alignment = TextAnchor.UpperCenter;
        return probabilitySelectorStyle;
    }

    public static GUIStyle ActionStyle(){
        GUIStyle actionStyle = new GUIStyle();
        actionStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        actionStyle.border = new RectOffset(12, 12, 12, 12);
        actionStyle.normal.textColor = Color.white;
        actionStyle.alignment = TextAnchor.UpperCenter;
        return actionStyle;
    }

    public static GUIStyle ProbabilityWeightStyle(){
        GUIStyle probabilityWeightStyle = new GUIStyle();
        probabilityWeightStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        probabilityWeightStyle.border = new RectOffset(12, 12, 12, 12);
        probabilityWeightStyle.normal.textColor = Color.white;
        probabilityWeightStyle.alignment = TextAnchor.UpperCenter;
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
