using System.Collections;
using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
using static System.Runtime.CompilerServices.Unsafe;
using static System.Runtime.InteropServices.CollectionsMarshal;

namespace EdSyl.Razor;

/// <summary> Manages a collection of component class names using a smart multi-layered approach. </summary>
public partial class ClassList : IEnumerable<string>
{
    private readonly Dictionary<ReadOnlyMemory<char>, Layer> tokens;
    private Dictionary<string, ClassName>? variants;

    private int size; // stores the number of cached tokens
    private bool excess; // set when unflag to empty layer
    private string? cached;
    private string? external;

    /// <summary> Initializes a new instance of the <see cref="ClassList" /> class. </summary>
    public ClassList() => tokens = new(StringMemoryComparer);

    /// <summary> Initializes a new instance of the <see cref="ClassList" /> class. </summary>
    /// <param name="klass">Computed class name.</param>
    public ClassList([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string klass)
    {
        tokens = new(StringMemoryComparer);
        Flag(klass, Layer.Explicit);
        size = tokens.Count;
        cached = klass;
    }

    /// <summary> Initializes a new instance of the <see cref="ClassList" /> class. </summary>
    /// <param name="other">The other instance to copy values from.</param>
    public ClassList(ClassList other)
    {
        size = other.size;
        excess = other.excess;
        cached = other.cached;
        external = other.external;
        tokens = new(other.tokens, StringMemoryComparer);

        if (other.variants != null)
            variants = new(other.variants, StringComparer);
    }

    /// <summary> Check if the class list evaluates to an empty string. </summary>
    public bool IsEmpty => ToString() == null;

    /// <summary> Check if provided class present in a class list. </summary>
    /// <param name="klass">Class to check.</param>
    public bool Has([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string klass)
    {
        var memory = klass.AsMemory();
        foreach (var range in klass.EnumerateSplits(Separator, SplitOptions))
            if (!tokens.TryGetValue(memory[range], out var layer) || layer == default)
                return false;

        return true;
    }

    /// <summary> Sets the presence of an explicit class. </summary>
    /// <param name="klass">Class to add to the list.</param>
    /// <seealso cref="Layer.Explicit" />
    public ClassList Add([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string? klass)
    {
        if (klass != null)
            Flag(klass, Layer.Explicit);

        return this;
    }

    /// <summary> Sets the presence of an explicit class. </summary>
    /// <param name="klass">Class to add to the list.</param>
    /// <seealso cref="Layer.Explicit" />
    public ClassList Add(ClassName klass)
    {
        Flag(klass, Layer.Explicit);
        return this;
    }

    /// <summary> Removes an explicit class. </summary>
    /// <param name="klass">Class to remove from the list.</param>
    /// <seealso cref="Layer.Explicit" />
    public ClassList Remove([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string? klass)
    {
        if (klass != null)
            Unflag(klass, Layer.Explicit);

        return this;
    }

    /// <summary> Removes an explicit class. </summary>
    /// <param name="klass">Class to remove from the list.</param>
    /// <seealso cref="Layer.Explicit" />
    public ClassList Remove(ClassName klass)
    {
        Unflag(klass, Layer.Explicit);
        return this;
    }

    /// <summary> Add or remove classes from each element depending on the value of the state argument. </summary>
    /// <param name="klass">A set of classes to be added or removed. </param>
    /// <param name="state">Whether the class should be added or remove</param>
    public ClassList Toggle([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string klass, bool state)
    {
        Toggle(state, klass, Layer.Explicit);
        return this;
    }

    /// <summary> Replace a class previously set for a specific variant. </summary>
    /// <param name="variant">A unique name associated with a component variant.</param>
    /// <param name="klass">A new class to use in place of previously provided. </param>
    /// <seealso cref="Layer.Variant" />
    public ClassList ToggleVariant(string variant, [LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string? klass)
    {
        Variate(variant, klass);
        return this;
    }

    /// <summary> Replace a class provided by the 'class' attribute. </summary>
    /// <param name="klass">A new class to use in place of previously provided. </param>
    /// <seealso cref="Layer.External" />
    public ClassList ToggleExternal([LanguageInjection("HTMLInBlazor", Prefix = "<br class='", Suffix = "'/>")] string? klass)
    {
        // don't modify if the same external class
        if (string.Equals(klass, external, Comparison))
            return this;

        // apply changes
        external = klass;
        Unflag(Layer.External);

        if (klass != null)
            Flag(klass, Layer.External);

        return this;
    }

    /// <inheritdoc />
    public IEnumerator<string> GetEnumerator()
    {
        foreach (var (key, layer) in tokens)
            if (layer != default)
                yield return key.ToString();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        foreach (var (key, layer) in tokens)
            if (layer != default)
                yield return key.ToString();
    }

    /// <inheritdoc />
    public override string? ToString()
    {
        return excess || size != tokens.Count
            ? cached = Flush()
            : cached;
    }

    private string? Flush()
    {
        if (excess)
            foreach (var (key, layer) in tokens)
                if (layer == default)
                    tokens.Remove(key);

        excess = false;
        size = tokens.Count;
        return ClassNameOf(tokens);
    }

    private void Variate(string key, ClassName klass)
    {
        if (klass.Empty)
        {
            // remove variant
            if (variants?.Remove(key, out var curr) == true)
                Unflag(curr, Layer.Variant);
        }
        else if (variants != null)
        {
            // replace variant
            Swap(ref GetValueRefOrAddDefault(variants, key, out _), klass, Layer.Variant);
        }
        else
        {
            // initialize variant
            variants = new(StringComparer) { [key] = klass };
            Flag(klass, Layer.Variant);
        }
    }

    private void Swap(ref ClassName klass, ClassName value, Layer layer)
    {
        if (klass == value) return;
        Unflag(klass, layer);
        Flag(value, layer);
        klass = value;
    }

    [MethodImpl(AggressiveInlining)]
    private void Toggle(bool state, string klass, Layer layer)
    {
        if (state) Flag(klass, layer);
        else Unflag(klass, layer);
    }

    [MethodImpl(AggressiveInlining)]
    private void Flag(string klass, Layer layer)
    {
        var memory = klass.AsMemory();
        if (klass.Contains(' '))
        {
            foreach (var range in klass.EnumerateSplits(Separator, SplitOptions))
                Flag(memory[range], layer);
        }
        else
        {
            Flag(memory, layer);
        }
    }

    [MethodImpl(AggressiveInlining)]
    private void Flag(ClassName klass, Layer layer)
    {
        foreach (var key in klass.Span)
            Flag(key, layer);
    }

    [MethodImpl(AggressiveInlining)]
    private void Flag(ReadOnlyMemory<char> key, Layer layer)
        => GetValueRefOrAddDefault(tokens, key, out _) |= layer;

    [MethodImpl(AggressiveInlining)]
    private static Layer Flag(Layer curr, Layer layer)
        => curr | layer;

    [MethodImpl(AggressiveInlining)]
    private void Unflag(string klass, Layer layer)
    {
        var memory = klass.AsMemory();
        if (klass.Contains(' '))
        {
            foreach (var range in klass.EnumerateSplits(Separator, SplitOptions))
                Unflag(memory[range], layer);
        }
        else
        {
            Unflag(memory, layer);
        }
    }

    [MethodImpl(AggressiveInlining)]
    private void Unflag(ClassName klass, Layer layer)
    {
        foreach (var key in klass.Span)
            Unflag(key, layer);
    }

    [MethodImpl(AggressiveInlining)]
    private void Unflag(ReadOnlyMemory<char> key, Layer layer)
    {
        ref var curr = ref GetValueRefOrNullRef(tokens, key);
        if (!IsNullRef(ref curr))
            if ((curr &= ~layer) == default)
                excess = true;
    }

    [MethodImpl(AggressiveInlining)]
    private void Unflag(Layer layer)
    {
        foreach (var (key, curr) in tokens)
            if ((tokens[key] = Unflag(curr, layer)) == default)
                excess = true;
    }

    [MethodImpl(AggressiveInlining)]
    private static Layer Unflag(Layer curr, Layer layer)
        => curr & ~layer;
}
