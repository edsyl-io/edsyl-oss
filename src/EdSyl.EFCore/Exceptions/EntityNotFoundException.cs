namespace EdSyl.EFCore.Exceptions;

[SuppressMessage("Roslynator", "RCS1194:Implement exception constructors.", Justification = "By Design")]
[SuppressMessage("Design", "CA1032:Implement standard exception constructors", Justification = "By Design")]
public class EntityNotFoundException : Exception
{
    public Type Type { get; }

    public EntityNotFoundException(Type type)
    {
        Type = type;
    }
}
