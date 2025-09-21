namespace EdSyl.Design.Components;

[ClassName("drag-handle surface-bright zen:hidden")]
public class DragHandle : Button
{
    public DragHandle()
    {
        Textual = true;
        Secondary = true;
        Icon = Icons.Drag;
    }
}
