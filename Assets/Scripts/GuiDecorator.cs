using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviour{
public class GuiDecorator : CallableGuiNode
{
    GuiNode parentNode;
    Action<GuiDecorator> OnRemoveDecorator;
    Decorator decorator;

    public GuiDecorator(
        Decorator decorator,
        string displayTask,
        string displayName,
        Vector2 pos,
        Action<GuiNode> UpdatePanelDetails,
        Action<GuiDecorator> OnRemoveDecorator,
        ref BehaviourTreeBlackboard blackboard,
        GuiNode parentNode
    ) :base(
        node:decorator,
        displayTask:displayTask,
        displayName:displayName,
        pos:pos,
        UpdatePanelDetails:UpdatePanelDetails,
        blackboard: ref blackboard
    )
    {
        this.decorator = decorator;
        this.parentNode = parentNode;
        this.OnRemoveDecorator = OnRemoveDecorator;
    }
    protected override void ApplyDerivedSettings(){
        defaultStyle = NodeProperties.GUINodeStyle();
        selectedStyle = NodeProperties.SelectedGUINodeStyle();
        activeStyle = defaultStyle;
        color = NodeProperties.DecoratorColor();
        taskRectColor = NodeProperties.DefaultColor();
        rect.size = NodeProperties.SubNodeSize();
    }

    private void Remove(){
        if (OnRemoveDecorator != null){
            OnRemoveDecorator(this);
        }
    }

    public override void Drag(Vector2 delta)
    {
        if (IsSelected){
            parentNode.Drag(delta);
        }
        rect.position += delta;
        callNumber.NodeRect.position += delta;
    }

    protected override void ProcessContextMenu(){
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove decorator"), false, Remove);
        genericMenu.ShowAsContext();
    }

    public override bool ProcessEvents(Event e){
        return base.ProcessSubNodeEvents(e);
    }

    public override void Draw()
    {
        Color currentColor = GUI.backgroundColor;
        GUI.backgroundColor = color;
        string displayTaskAndCondition = displayTask;
        if (decorator.invertCondition){displayTaskAndCondition = "!" + displayTaskAndCondition;}
        GUI.Box(rect, "\n" + displayName + "\n" + displayTaskAndCondition, activeStyle);
        callNumber.Draw();
        GUI.backgroundColor = currentColor;
    }

    public override void DrawDetails()
    {
        base.DrawDetails();
        invertCondition = EditorGUILayout.Toggle("Invert condition", invertCondition);
        
    }
}
}