using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Behaviour{

    public class NodeProperties
    {

        public static float DefaultTimerVal(){return 5f;}
        public static float DefaultWaitTime(){return 5f;}

        // Size
        public static Vector2 GuiNodeSize(){return new Vector2(200, 140);}
        public static Vector2 SubNodeSize(){return new Vector2(200, 60);}
        public static Vector2 TaskNodeSize(){return new Vector2(200, 80);}
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
        public static Color RunningTint(){return new Color(0f, .9f, 0f);}
        public static Color FailedTint(){return new Color(.9f, 0f, 0f);}
        public static Color DefaultTint(){return new Color(1f, 1f, 1f);}

        // Style

        public static GUIStyle GUINodeStyle(){
            GUIStyle nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.alignment = TextAnchor.UpperCenter;
            nodeStyle.fontStyle = FontStyle.Bold;
            return nodeStyle;
        }

        public static GUIStyle SelectedGUINodeStyle(){
            GUIStyle selectedNodeStyle = GUINodeStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0 on.png") as Texture2D;
            return selectedNodeStyle;
        }

        public static GUIStyle TaskNodeStyle(){
            GUIStyle taskNodeStyle = GUINodeStyle();
            taskNodeStyle.fontSize = 16;
            return taskNodeStyle;
        }

        public static GUIStyle SelectedTaskNodeStyle(){
            GUIStyle selectedTaskNodeStyle = TaskNodeStyle();
            selectedTaskNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0 on.png") as Texture2D;
            return selectedTaskNodeStyle;
        }

        public static GUIStyle DecoratorStyle(){
            GUIStyle decoratorStyle = GUINodeStyle();
            decoratorStyle.alignment = TextAnchor.MiddleCenter;
            return decoratorStyle;
        }

        public static GUIStyle SelectedDecoratorStyle(){
            GUIStyle selectedDecoratorStyle =  DecoratorStyle();
            selectedDecoratorStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0 on.png") as Texture2D;
            return selectedDecoratorStyle;
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
                    return "Probability Weight";
                default:
                    return "Action";
            }
        }

        public static Texture2D BlackboardIcon(){
            return Resources.Load("Icons/blackboard_icon", typeof(Texture2D)) as Texture2D;
        }
        public static Texture2D AddKeyIcon(){
            return Resources.Load("Icons/add_key_icon", typeof(Texture2D)) as Texture2D;
        }
        public static Texture2D DeleteIcon(){
            return Resources.Load("Icons/delete_icon", typeof(Texture2D)) as Texture2D;
        }
        public static Texture2D SaveIcon(){
            return Resources.Load("Icons/save_icon", typeof(Texture2D)) as Texture2D;
        }
        public static Texture2D SelectedNodeIcon(){
            return Resources.Load("Icons/selected_node_icon", typeof(Texture2D)) as Texture2D;
        }

    }
}