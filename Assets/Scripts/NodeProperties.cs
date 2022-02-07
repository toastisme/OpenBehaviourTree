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
        public static Color RunningTint(){return new Color(0f, .9f, 0);}
        public static Color FailedTint(){return new Color(.9f, 0f, 0f);}
        public static Color DefaultTint(){return new Color(1f, 1f, 1f);}
        public static Color SelectedTint(){return new Color(.8f, .8f, .8f);}

        // Style

        public static GUIStyle GUINodeStyle(){
            GUIStyle nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.alignment = TextAnchor.UpperLeft;
            nodeStyle.fontStyle = FontStyle.Bold;
            nodeStyle.padding=new RectOffset(10,0,0,0);
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

        public static GUIStyle BlackboardIntStyle(){
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleRight;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(226f/255f,226f/255f, 223f/255f);
            return style;
        }

        public static GUIStyle BlackboardFloatStyle(){
            GUIStyle style = BlackboardIntStyle();
            style.normal.textColor = new Color(247f/255f, 217f/255f, 196f/255f);
            return style;
        }
        public static GUIStyle BlackboardBoolStyle(){
            GUIStyle style = BlackboardIntStyle();
            style.normal.textColor = new Color(250f/255f,237f/255f, 203f/255f);
            return style;
        }
        public static GUIStyle BlackboardStringStyle(){
            GUIStyle style = BlackboardIntStyle();
            style.normal.textColor = new Color(201f/255f, 228f/255f, 222f/255f);
            return style;
        }
        public static GUIStyle BlackboardGameObjectStyle(){
            GUIStyle style = BlackboardIntStyle();
            style.normal.textColor = new Color(242f/255f, 198f/255f, 222f/255f);
            return style;
        }

        public static GUIStyle BlackboardVector3Style(){
            GUIStyle style = BlackboardIntStyle();
            style.normal.textColor = new Color(198f/255f, 222f/255f, 241f/255f);
            return style;
        }

        public static GUIStyle BlackboardVector2Style(){
            GUIStyle style = BlackboardIntStyle();
            style.normal.textColor = new Color(219f/255f, 205f/255f, 240f/255f);
            return style;
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

        // GUIContent

        public static GUIContent BlackboardContent(){
            Texture2D icon = Resources.Load("Icons/blackboard_icon", typeof(Texture2D)) as Texture2D;
            string text = "Blackboard";
            string tooltip = "Variable dictionary shared between nodes.";
            return new GUIContent(text, icon, tooltip);
        }
        public static GUIContent AddKeyContent(){
            Texture2D icon = Resources.Load("Icons/add_key_icon", typeof(Texture2D)) as Texture2D;
            string text = "Add Key";
            string tooltip = "Add new key to the blackboard.";
            return new GUIContent(text, icon, tooltip);
        }
        public static GUIContent ClearTreeContent(){
            Texture2D icon = Resources.Load("Icons/delete_icon", typeof(Texture2D)) as Texture2D;
            string text = "Clear Tree";
            string tooltip = "Remove all nodes and connections.";
            return new GUIContent(text, icon, tooltip);
        }
        public static GUIContent SaveContent(){
            Texture2D icon = Resources.Load("Icons/save_icon", typeof(Texture2D)) as Texture2D;
            string text = "Save";
            string tooltip = "Save changes to .asset file.";
            return new GUIContent(text, icon, tooltip);
        }
        public static GUIContent SelectedNodeContent(){
            Texture2D icon = Resources.Load("Icons/selected_node_icon", typeof(Texture2D)) as Texture2D;
            string text = "Selected";
            string tooltip = "Properties of selected node.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent ActionContent(){
            Texture2D icon = Resources.Load("Icons/action_icon", typeof(Texture2D)) as Texture2D;
            string text = "Action";
            string tooltip = "Task for the gameobject to execute.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent CooldownContent(){
            Texture2D icon = Resources.Load("Icons/cooldown_icon", typeof(Texture2D)) as Texture2D;
            string text = "Cooldown";
            string tooltip = "Disable after success for duration.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent DecoratorContent(){
            Texture2D icon = Resources.Load("Icons/decorator_icon", typeof(Texture2D)) as Texture2D;
            string text = "Decorator";
            string tooltip = "Prevent child node running if false.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent PrioritySelectorContent(){
            Texture2D icon = Resources.Load("Icons/priority_selector_icon", typeof(Texture2D)) as Texture2D;
            string text = "Priority Selector";
            string tooltip = "Run child nodes in order until one is successful.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent ProbabilitySelectorContent(){
            Texture2D icon = Resources.Load("Icons/probability_selector_icon", typeof(Texture2D)) as Texture2D;
            string text = "Probability Selector";
            string tooltip = "Randomly select which child node to run.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent SequenceSelectorContent(){
            Texture2D icon = Resources.Load("Icons/sequence_selector_icon", typeof(Texture2D)) as Texture2D;
            string text = "Sequence Selector";
            string tooltip = "Run child nodes in order but stop if any fail.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent TimeoutContent(){
            Texture2D icon = Resources.Load("Icons/timer_icon", typeof(Texture2D)) as Texture2D;
            string text = "Timeout";
            string tooltip = "Stop this node if it runs longer than duration.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent WaitContent(){
            Texture2D icon = Resources.Load("Icons/wait_icon", typeof(Texture2D)) as Texture2D;
            string text = "Wait";
            string tooltip = "Gameobject does nothing for duration.";
            return new GUIContent(text, icon, tooltip);
        }

        public static GUIContent ProbabilityWeightContent(){
            Texture2D icon = Resources.Load("Icons/probability_weight_icon", typeof(Texture2D)) as Texture2D;
            string text = "Probability Weight";
            string tooltip = "Sets the probability of the child node being selected, relative to other weights.";
            return new GUIContent(text, icon, tooltip);
        }
        public static GUIContent ParentConnectionPointContent(){
            string text = "";
            string tooltip = "Click to start creating a connection, click again to create a child node. Right click to cancel.";
            return new GUIContent(text, tooltip);
        }

        public static GUIContent ChildConnectionPointContent(){
            string text = "";
            string tooltip = "When drawing a connection click to connect the nodes.";
            return new GUIContent(text, tooltip);
        }
    }
}