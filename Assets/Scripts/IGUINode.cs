namespace BehaviourTree{
interface IGuiNode 
{
    void Draw();
    void Drag(Vector2 delta);
    bool IsSelected();
}
}