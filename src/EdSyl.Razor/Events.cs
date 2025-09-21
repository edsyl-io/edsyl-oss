namespace EdSyl.Razor;

public static class Events
{
    public static bool IsChecked(this ChangeEventArgs e) => e.Value is true or "on";
}
