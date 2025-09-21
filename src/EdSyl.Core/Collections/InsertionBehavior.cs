namespace EdSyl.Collections;

/// <summary>
/// Defines behaviour for collection insertion operations.
/// </summary>
internal enum InsertionBehavior
{
    /// <summary>
    /// Do not modify collection if equal element already present.
    /// </summary>
    None = 0,

    /// <summary>
    /// Overwrite an existing element if equal element already present.
    /// </summary>
    OverwriteExisting = 1,

    /// <summary>
    /// Throw exception if equal element already present.
    /// </summary>
    ThrowOnExisting = 2,
}
