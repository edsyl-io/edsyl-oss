using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
using static System.Runtime.InteropServices.CollectionsMarshal;

namespace EdSyl.Razor;

public partial class ClassList
{
    private const char Separator = ' ';
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    private const StringSplitOptions SplitOptions = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
    private static StringComparer StringComparer => StringComparer.OrdinalIgnoreCase;
    private static StringMemoryComparer StringMemoryComparer => StringMemoryComparer.OrdinalIgnoreCase;
    private static readonly Dictionary<Type, ClassList> Cache = new(ReferenceEqualityComparer.Instance);

    /// <summary> Get a list of classes to use for a specific component by default. </summary>
    /// <param name="component">Type of component.</param>
    public static ClassList For(IStyledComponent? component)
    {
        return component != null
            ? For(component.GetType())
            : [];
    }

    /// <summary> Get a list of classes to use for a specific component by default. </summary>
    /// <param name="type">Type of component.</param>
    public static ClassList For(Type type)
    {
        // clone from a cache
        return new(Get(type) ??= Compute(type));
    }

    private static ref ClassList? Get(Type type)
    {
        lock (Cache)
            return ref GetValueRefOrAddDefault(Cache, type, out _);
    }

    [SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop", Justification = "Impossible")]
    private static ClassList Compute(Type type)
    {
        var list = new ClassList();

        // default class names
        foreach (ClassNameAttribute attribute in Attribute.GetCustomAttributes(type, typeof(ClassNameAttribute)))
            list.Add(attribute.Klass);

        list.cached = list.ToString();
        return list;
    }

    private static string? ClassNameOf(Dictionary<ReadOnlyMemory<char>, Layer> tokens)
    {
        var length = LengthOf(tokens) - 1;
        if (length < 1)
            return null;

        Span<char> buffer = stackalloc char[length];

        length = 0;
        foreach (var token in tokens.Keys)
        {
            if (length > 0) buffer[length++] = Separator;
            token.Span.CopyTo(buffer[length..]);
            length += token.Length;
        }

        return new(buffer[..length]);
    }

    [MethodImpl(AggressiveInlining)]
    private static int LengthOf(Dictionary<ReadOnlyMemory<char>, Layer> tokens)
    {
        var length = 0;
        foreach (var (token, layer) in tokens)
            if (layer != default)
                length += 1 + token.Length;

        return length;
    }
}
