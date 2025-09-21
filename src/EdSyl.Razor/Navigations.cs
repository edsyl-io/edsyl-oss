namespace EdSyl.Razor;

public static class Navigations
{
    public static ReadOnlySpan<char> Path(this NavigationManager navigation)
        => navigation.Uri.AsSpan(navigation.BaseUri.Length);

    public static bool Match(this NavigationManager navigation, ReadOnlySpan<char> prefix)
        => navigation.Path().StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
}
