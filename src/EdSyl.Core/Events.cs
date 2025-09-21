namespace EdSyl;

public static class Events
{
    public static void Invoke(this IEnumerable<Action>? handlers)
    {
        if (handlers != null)
            foreach (var handler in handlers)
                handler();
    }

    public static void Invoke(this IEnumerable<EventHandler>? handlers, object sender, EventArgs? args = null)
    {
        if (handlers == null) return;
        args ??= EventArgs.Empty;
        foreach (var handler in handlers)
            handler(sender, args);
    }
}
