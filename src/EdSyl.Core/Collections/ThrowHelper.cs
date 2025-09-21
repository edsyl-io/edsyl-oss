using System.Diagnostics;

namespace EdSyl.Collections;

[StackTraceHidden]
internal static class ThrowHelper
{
    internal static ArgumentException AddingDuplicateWithKeyArgumentException(object value, string name)
        => new("An item with the same key has already been added. Key: " + value, name);
}
