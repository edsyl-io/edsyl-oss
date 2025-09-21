using System.Runtime.InteropServices.JavaScript;

namespace EdSyl.Razor.Interop;

[SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "emulate native naming")]
public static partial class Browser
{
    [JSImport("globalThis.requestAnimationFrame")]
    public static partial void RequestAnimationFrame([JSMarshalAs<JSType.Function>] Action callback);

    [JSImport("globalThis.setTimeout")]
    public static partial void SetTimeout([JSMarshalAs<JSType.Function>] Action callback, int delay);

    [JSImport("globalThis.print")]
    public static partial void Print();

    [JSImport("globalThis.navigator.clipboard.writeText")]
    public static partial Task CopyTextToClipboard(string text);

    public static partial class Navigation
    {
        [JSImport("globalThis.navigation.back")]
        public static partial void Back();
    }
}
