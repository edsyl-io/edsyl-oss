using System.Reflection;
using System.Runtime.CompilerServices;
using EdSyl.Reflection;

namespace EdSyl.Razor;

public static class EventCallbacks
{
    private static readonly Func<EventCallback, IHandleEvent> GetReceiver;
    private static readonly Func<EventCallback, MulticastDelegate> GetDelegate;

    static EventCallbacks()
    {
        typeof(EventCallback)
            .RequireField("Receiver", BindingFlags.Instance | BindingFlags.NonPublic)
            .Getter(out GetReceiver);

        typeof(EventCallback)
            .RequireField("Delegate", BindingFlags.Instance | BindingFlags.NonPublic)
            .Getter(out GetDelegate);
    }

    /// <summary> Whether provided callback has no delegate. </summary>
    /// <param name="callback">Callback to check.</param>
    public static bool IsEmpty(this EventCallback callback) => !callback.HasDelegate;

    /// <summary> Whether provided callback has no delegate. </summary>
    /// <param name="callback">Callback to check.</param>
    /// <typeparam name="T">Type of delegate input parameter.</typeparam>
    public static bool IsEmpty<T>(this EventCallback<T> callback) => !callback.HasDelegate;

    /// <inheritdoc cref="EventCallbackFactory.Create(object,Action)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventCallback Callback(this IHandleEvent? receiver, Action callback) => new(receiver, callback);

    /// <inheritdoc cref="EventCallbackFactory.Create{T}(object,Action{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventCallback<T> Callback<T>(this IHandleEvent? receiver, Action callback) => new(receiver, callback);

    /// <inheritdoc cref="EventCallbackFactory.Create{T}(object,Action{T})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventCallback<T> Callback<T>(this IHandleEvent? receiver, Action<T> callback) => new(receiver, callback);

    /// <inheritdoc cref="EventCallbackFactory.Create(object,Func{Task})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventCallback Callback(this IHandleEvent? receiver, Func<Task> callback) => new(receiver, callback);

    /// <inheritdoc cref="EventCallbackFactory.Create(object,Func{object,Task})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventCallback Callback(this IHandleEvent? receiver, Func<object, Task> callback) => new(receiver, callback);

    /// <inheritdoc cref="EventCallbackFactory.Create{T}(object,Func{Task})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventCallback<T> Callback<T>(this IHandleEvent? receiver, Func<Task> callback) => new(receiver, callback);

    /// <inheritdoc cref="EventCallbackFactory.Create{T}(object,Func{T,Task})" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EventCallback<T> Callback<T>(this IHandleEvent? receiver, Func<T, Task> callback) => new(receiver, callback);

    /// <summary> Substitute the callback when provided value is not empty. </summary>
    /// <typeparam name="T">Type of the delegate to substitute. </typeparam>
    /// <param name="input">Current input parameter value.</param>
    /// <param name="callback">Callback to use in place of input parameter.</param>
    /// <param name="param">Field to receive original input parameter value.</param>
    /// <returns><paramref name="callback" /> value.</returns>
    public static T? Decorate<T>(this T? input, T callback, out T? param) where T : Delegate
    {
        param = input;
        return input != null ? callback : null;
    }

    /// <summary> Substitute the callback when provided value is not empty. </summary>
    /// <param name="input">Current input parameter value.</param>
    /// <param name="callback">Callback to use in place of input parameter.</param>
    /// <param name="param">Field to receive original input parameter value.</param>
    /// <returns><paramref name="callback" /> value.</returns>
    public static EventCallback Decorate(this EventCallback input, EventCallback callback, out EventCallback param)
    {
        param = input;
        return input.HasDelegate ? callback : default;
    }

    /// <summary> Substitute the callback when provided value is not empty. </summary>
    /// <typeparam name="T">Type of the callback parameter.</typeparam>
    /// <param name="input">Current input parameter value.</param>
    /// <param name="callback">Callback to use in place of input parameter.</param>
    /// <param name="param">Field to receive original input parameter value.</param>
    /// <returns><paramref name="callback" /> value.</returns>
    public static EventCallback<T> Decorate<T>(this EventCallback<T> input, EventCallback<T> callback, out EventCallback<T> param)
    {
        param = input;
        return input.HasDelegate ? callback : default;
    }

    /// <summary> Replace input parameter with given callback. </summary>
    /// <param name="input">Current input parameter value.</param>
    /// <param name="callback">Callback to use in place of input parameter.</param>
    /// <param name="param">Field to receive original input parameter value.</param>
    /// <returns><paramref name="callback" /> value.</returns>
    public static EventCallback Replace(this EventCallback input, EventCallback callback, out EventCallback param)
    {
        param = EqualityComparer<EventCallback>.Default.Equals(input, callback) ? default : input;
        return callback;
    }

    /// <summary> Replace input parameter with given callback. </summary>
    /// <typeparam name="T">Type of the callback parameter.</typeparam>
    /// <param name="input">Current input parameter value.</param>
    /// <param name="callback">Callback to use in place of input parameter.</param>
    /// <param name="param">Field to receive original input parameter value.</param>
    /// <returns><paramref name="callback" /> value.</returns>
    public static EventCallback<T> Replace<T>(this EventCallback<T> input, EventCallback<T> callback, out EventCallback<T> param)
    {
        param = EqualityComparer<EventCallback<T>>.Default.Equals(input, callback) ? default : input;
        return callback;
    }

    /// <summary> Try to invoke a callback of an unknown type. </summary>
    /// <param name="callback">Event callback to invoke.</param>
    /// <param name="arg">Argument to pass for <see cref="EventCallback" /></param>
    /// <typeparam name="T">Type of the callback parameter.</typeparam>
    public static Task InvokeAsync<T>(object? callback, T arg) => callback switch
    {
        EventCallback cb => cb.InvokeAsync(arg),
        EventCallback<T> cb => cb.InvokeAsync(arg),
        MulticastDelegate fn => new EventCallbackWorkItem(fn).InvokeAsync(arg),
        _ => Task.CompletedTask,
    };

    /// <summary> Invoke callback with the given arguments and make sure to propagate task cancellation. </summary>
    /// <remarks> Blazor components silently suppress task cancellations by implementing <see cref="IHandleEvent" />. </remarks>
    /// <param name="callback">Event callback to invoke.</param>
    /// <param name="arg">Argument to pass for <see cref="EventCallback" /></param>
    public static Task InvokeAsyncRethrowCancelled(this EventCallback callback, object? arg)
    {
        if (callback.HasDelegate && GetReceiver(callback) is { } receiver)
        {
            switch (GetDelegate(callback))
            {
                case Func<Task> func: return InvokeAsyncRethrowCancelled(receiver, func);
                case Func<object, Task> func: return InvokeAsyncRethrowCancelled(receiver, func, arg);
            }
        }

        return callback.InvokeAsync(arg);
    }

    /// <summary> Invoke callback with the given arguments and make sure to propagate task cancellation. </summary>
    /// <typeparam name="T">Type of the callback parameter.</typeparam>
    /// <remarks> Blazor components silently suppress task cancellations by implementing <see cref="IHandleEvent" />. </remarks>
    /// <param name="callback">Event callback to invoke.</param>
    /// <param name="arg">Argument to pass for <see cref="EventCallback{T}" /></param>
    public static Task InvokeAsyncRethrowCancellation<T>(this EventCallback<T> callback, T arg)
    {
        if (callback.HasDelegate && Generics<T>.GetReceiver(callback) is { } receiver)
        {
            switch (Generics<T>.GetDelegate(callback))
            {
                case Func<Task> func: return InvokeAsyncRethrowCancelled(receiver, func);
                case Func<object, Task> func: return InvokeAsyncRethrowCancelled(receiver, func, arg);
                case Func<T, Task> func: return InvokeAsyncRethrowCancelled(receiver, func, arg);
            }
        }

        return callback.InvokeAsync(arg);
    }

    private static async Task InvokeAsyncRethrowCancelled(IHandleEvent receiver, Func<Task> callback)
    {
        Task? task = null;
        var interceptor = () => task = callback();
        await receiver.HandleEventAsync(new(interceptor), null);

        if (task!.IsCanceled)
            throw new TaskCanceledException(task);
    }

    private static async Task InvokeAsyncRethrowCancelled<T>(IHandleEvent receiver, Func<T, Task> callback, T? arg)
    {
        Task? task = null;
        Func<T, Task> interceptor = x => task = callback(x);
        await receiver.HandleEventAsync(new(interceptor), arg);

        if (task!.IsCanceled)
            throw new TaskCanceledException(task);
    }

    private static class Generics<T>
    {
        public static readonly Func<EventCallback<T>, IHandleEvent> GetReceiver;
        public static readonly Func<EventCallback<T>, MulticastDelegate> GetDelegate;

        static Generics()
        {
            typeof(EventCallback<T>)
                .RequireField("Receiver", BindingFlags.Instance | BindingFlags.NonPublic)
                .Getter(out GetReceiver);

            typeof(EventCallback<T>)
                .RequireField("Delegate", BindingFlags.Instance | BindingFlags.NonPublic)
                .Getter(out GetDelegate);
        }
    }
}
