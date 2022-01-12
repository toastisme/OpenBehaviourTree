using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Behaviour{

    public class NodeProperties
    {
        // Size
        public static Vector2 GuiNodeSize(){return new Vector2(200, 100);}
        public static Vector2 SubNodeSize(){return new Vector2(200, 60);}
        public static Vector2 CallNumberSize(){
            Vector2 guiNodeSize = NodeProperties.GuiNodeSize();
            return new Vector2(
                guiNodeSize.x/6f,
                guiNodeSize.x/6f
            );
        }

        // Color
        public static Color DefaultColor(){return new Color(87f/255.0f, 117f/255.0f, 144f/255.0f);}
        public static Color CallNumberColor(){return NodeProperties.DefaultColor();}
        public static Color DecoratorColor(){return new Color(67f/255.0f, 170f/255.0f, 139f/255.0f);}
        public static Color SequenceSelectorColor(){return new Color(249f/255.0f, 199f/255.0f, 79f/255.0f);}
        public static Color ProbabilitySelectorColor(){return new Color(249f/255.0f, 132f/255.0f, 74f/255.0f);}
        public static Color ProbabilityWeightColor(){return new Color(248f/255.0f, 150f/255.0f, 30f/255.0f);}
        public static Color PrioritySelectorColor(){return new Color(39f/255.0f, 125f/255.0f, 161f/255.0f);}
        public static Color ActionColor(){return new Color(249f/255.0f, 65f/255.0f, 68f/255.0f);}
        public static Color RunningTint(){return new Color(0f, 1f, 0f, .5f);}
        public static Color DefaultTint(){return new Color(1f, 1f, 1f);}

        // Style

        public static GUIStyle GUINodeStyle(){
            GUIStyle nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.alignment = TextAnchor.UpperCenter;
            return nodeStyle;
        }

        public static GUIStyle SelectedGUINodeStyle(){
            GUIStyle selectedNodeStyle = GUINodeStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0 on.png") as Texture2D;
            return selectedNodeStyle;
        }

        public static GUIStyle RootStyle(){
            GUIStyle rootStyle = new GUIStyle();
            rootStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            rootStyle.border = new RectOffset(12, 12, 12, 12);
            rootStyle.normal.textColor = Color.white;
            rootStyle.alignment = TextAnchor.UpperCenter;
            return rootStyle;
        }

        public static GUIStyle DecoratorStyle(){
            GUIStyle decoratorStyle = new GUIStyle();
            decoratorStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            decoratorStyle.border = new RectOffset(12, 12, 12, 12);
            decoratorStyle.normal.textColor = Color.white;
            decoratorStyle.alignment = TextAnchor.MiddleCenter;
            return decoratorStyle;
        }

        public static GUIStyle SelectedDecoratorStyle(){
            GUIStyle selectedDecoratorStyle =  DecoratorStyle();
            selectedDecoratorStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0 on.png") as Texture2D;
            return selectedDecoratorStyle;
        }

        public static GUIStyle SequenceSelectorStyle(){
            GUIStyle sequenceSelectorStyle = new GUIStyle();
            sequenceSelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            sequenceSelectorStyle.border = new RectOffset(12, 12, 12, 12);
            sequenceSelectorStyle.normal.textColor = Color.white;
            sequenceSelectorStyle.alignment = TextAnchor.UpperCenter;
            return sequenceSelectorStyle;
        }

        public static GUIStyle PrioritySelectorStyle(){
            GUIStyle prioritySelectorStyle = new GUIStyle();
            prioritySelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            prioritySelectorStyle.border = new RectOffset(12, 12, 12, 12);
            prioritySelectorStyle.normal.textColor = Color.white;
            prioritySelectorStyle.alignment = TextAnchor.UpperCenter;
            return prioritySelectorStyle;
        }

        public static GUIStyle ProbabilitySelectorStyle(){
            GUIStyle probabilitySelectorStyle = new GUIStyle();
            probabilitySelectorStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            probabilitySelectorStyle.border = new RectOffset(12, 12, 12, 12);
            probabilitySelectorStyle.normal.textColor = Color.white;
            probabilitySelectorStyle.alignment = TextAnchor.UpperCenter;
            return probabilitySelectorStyle;
        }

        public static GUIStyle ActionStyle(){
            GUIStyle actionStyle = new GUIStyle();
            actionStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            actionStyle.border = new RectOffset(12, 12, 12, 12);
            actionStyle.normal.textColor = Color.white;
            actionStyle.alignment = TextAnchor.UpperCenter;
            return actionStyle;
        }

        public static GUIStyle ProbabilityWeightStyle(){
            GUIStyle probabilityWeightStyle = new GUIStyle();
            probabilityWeightStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            probabilityWeightStyle.border = new RectOffset(12, 12, 12, 12);
            probabilityWeightStyle.normal.textColor = Color.white;
            probabilityWeightStyle.alignment = TextAnchor.UpperCenter;
            return probabilityWeightStyle;
        }

        public static GUIStyle SelectedProbabilityWeightStyle(){
            GUIStyle selectedProbabilityWeightStyle = ProbabilityWeightStyle();
            selectedProbabilityWeightStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0 on.png") as Texture2D;
            return selectedProbabilityWeightStyle;
        }

        public static GUIStyle CallNumberStyle(){
            GUIStyle callNumberStyle = new GUIStyle();
            callNumberStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
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

        public static Vector2 InitDecoratorPos(){
            return new Vector2(0, NodeProperties.GuiNodeSize().y*.21f);
        }

        public static string GetDefaultStringFromNodeType(NodeType nodeType){
            switch(nodeType){
                case NodeType.Root:
                    return "Root";
                case NodeType.SequenceSelector:
                    return "Sequence Selector";
                case NodeType.PrioritySelector:
                    return "Priority Selector";
                case NodeType.ProbabilitySelector:
                    return "Probability Selector";
                case NodeType.Decorator:
                    return "Decorator";
                case NodeType.ProbabilityWeight:
                    return "Constant weight";
                default:
                    return "Action";
            }
        }
    }
}