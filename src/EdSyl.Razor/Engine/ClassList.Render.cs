using System.Reflection;
using System.Runtime.CompilerServices;
using EdSyl.Reflection;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;
using static System.Reflection.BindingFlags;
using static System.Runtime.CompilerServices.MethodImplOptions;
using static System.Runtime.InteropServices.CollectionsMarshal;

namespace EdSyl.Razor;

public partial class ClassList
{
    /// <summary> Automatically render component class names from its attributes. </summary>
    /// <param name="component">Component to render.</param>
    public ClassList Render(object? component)
    {
#pragma warning disable IL2072
        for (var meta = Meta.Of(component?.GetType()); meta != null; meta = meta.Subgraph)
#pragma warning restore IL2072
        {
            // TODO: detect overrides

            // process toggles
            if (meta.Toggles.AsSpan() is { Length: > 0 } toggles)
                foreach (var toggle in toggles)
                    Variate(toggle.Name, toggle.Get(component!) ? toggle.Data.On : toggle.Data.Off);

            // process variants
            if (meta.Variants != null && meta.Holders.AsSpan() is { Length: > 0 } holders)
                foreach (var holder in holders)
                    if (holder.Resolve(component!) is { } variant)
                        if (meta.Variants.TryGetValue(variant, out var toggle))
                            Variate(holder.Name, toggle.Klass);
        }

        return this;
    }
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401", Justification = "Performance")]
file class Toggle(MemberInfo member, ClassNameToggleAttribute attribute)
{
    public readonly string Name = member.Name;
    public readonly ClassNameToggleAttribute Data = attribute;
    public readonly Func<object, bool> Get = member.Getter<object, bool>();
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401", Justification = "Performance")]
file class Holder(MemberInfo member, ClassVariantHolderAttribute attribute)
{
    public readonly string Name = member.Name;
    public readonly ClassVariantHolderAttribute Data = attribute;
    public readonly Func<object, string?> Get = member.Getter<object, string?>();
    public string? Resolve(object instance) => Get(instance) ?? Data.Default;
}

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401", Justification = "Performance")]
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1205", Justification = "File-scoped")]
file partial class Meta(Toggle[]? toggles, Holder[]? holders, Dictionary<string, ClassVariantToggleAttribute>? variants, Meta? subgraph)
{
    public readonly Meta? Subgraph = subgraph;
    public readonly Toggle[] Toggles = toggles ?? [];
    public readonly Holder[] Holders = holders ?? [];
    public readonly Dictionary<string, ClassVariantToggleAttribute>? Variants = variants?.Trim();
}

file partial class Meta
{
    private static readonly Dictionary<Type, Meta?> Cache = [];

    [MethodImpl(AggressiveInlining)]
    public static Meta? Of([DynamicallyAccessedMembers(PublicProperties | PublicFields | NonPublicProperties | NonPublicFields)] Type? type)
    {
        return type != null
            ? Get(type) ??= Compute(type)
            : null;
    }

    private static ref Meta? Get([DynamicallyAccessedMembers(PublicProperties | NonPublicProperties | PublicFields | NonPublicFields)] Type type)
    {
        lock (Cache)
            return ref GetValueRefOrAddDefault(Cache, type, out _);
    }

    [MethodImpl(AggressiveInlining)]
    private static Meta? Compute([DynamicallyAccessedMembers(PublicProperties | NonPublicProperties | PublicFields | NonPublicFields)] Type type)
    {
#pragma warning disable IL2072
        return Compute(type, Of(type.BaseType));
#pragma warning restore IL2072
    }

    [SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop", Justification = "Impossible")]
    private static Meta? Compute([DynamicallyAccessedMembers(PublicProperties | NonPublicProperties | PublicFields | NonPublicFields)] Type type, Meta? subgraph)
    {
        // limit to custom components for slim graph
        if (!typeof(IStyledComponent).IsAssignableFrom(type))
            return null;

        const BindingFlags bindings = Instance | Public | NonPublic | DeclaredOnly;
        var builder = default(Builder);
        builder.Add(type.GetFields(bindings));
        builder.Add(type.GetProperties(bindings));
        return builder.Build(subgraph);
    }

    private ref struct Builder
    {
        private Dictionary<string, Toggle>? toggles;
        private Dictionary<string, Holder>? holders;
        private Dictionary<string, ClassVariantToggleAttribute>? variants;

        public void Add(ReadOnlySpan<MemberInfo> members)
        {
            foreach (var member in members)
                foreach (var attribute in Attribute.GetCustomAttributes(member))
                    switch (attribute)
                    {
                        case ClassNameToggleAttribute toggle:
                            toggles ??= [];
                            toggles[member.Name] = new(member, toggle);
                            break;

                        case ClassVariantToggleAttribute variant:
                            variants ??= [];
                            variants[member.Name] = variant;
                            break;

                        case ClassVariantHolderAttribute holder:
                            holders ??= [];
                            holders[member.Name] = new(member, holder);
                            break;
                    }
        }

        public readonly Meta Build(Meta? subgraph) => new(
            toggles?.Values.ToArray(),
            holders?.Values.ToArray(),
            variants,
            subgraph
        );
    }
}
