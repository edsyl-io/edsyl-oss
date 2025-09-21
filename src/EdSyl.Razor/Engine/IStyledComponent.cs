namespace EdSyl.Razor;

public interface IStyledComponent
{
    /// <summary> List of CSS class names to supply for element rendering. </summary>
    ClassList Class { get; }
}
