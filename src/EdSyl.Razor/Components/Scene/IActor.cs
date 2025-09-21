using System.Runtime.CompilerServices;

namespace EdSyl.Razor.Components;

public interface IActor : IDisposable
{
    ILifecycle Lifecycle { get; }
}

public interface IActor<T> : IActor
{
    TaskAwaiter<T> GetAwaiter();
}
