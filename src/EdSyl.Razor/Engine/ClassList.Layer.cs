namespace EdSyl.Razor;

public partial class ClassList
{
    [Flags]
    protected enum Layer : byte
    {
        /// <summary>
        /// Class that should be excluded from the final class list.
        /// Used mostly as a temporary state for modification tracking.
        /// </summary>
        None = 0,

        /// <summary>
        /// Class applied via direct manipulation of a class list.
        /// </summary>
        Explicit = 1 << 0,

        /// <summary>
        /// Class applied via applying a specific variant to a component.
        /// Variants have higher precedence over <see cref="Explicit" /> as they're controlled by the component internal logic.
        /// In case when you want to remove the presence of a class defined by variant, you provide logic to affect the variant itself.
        /// </summary>
        Variant = 1 << 1,

        /// <summary>
        /// Class externally provided to a component via the 'class' attribute.
        /// The highest precedence in a hierarchy, as class provided externally, implies an intent to append additional classes to a list.
        /// </summary>
        External = 1 << 2,
    }
}
