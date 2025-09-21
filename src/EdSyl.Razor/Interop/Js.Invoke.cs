using EdSyl.Async;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace EdSyl.Razor.Interop;

public static partial class Js
{
    private const string InvokeFn = "edsyl.invoke";
    private const DynamicallyAccessedMemberTypes InvokeDynamicTypes = PublicConstructors | PublicFields | PublicProperties;

    /// <summary> Call the specified JavaScript function without waiting for a result. </summary>
    /// <param name="element">Reference to a JavaScript element.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <exception cref="InvalidOperationException">When unable to acquire JS runtime.</exception>
    public static void Call(this ElementReference element, string identifier, params object?[]? args)
        => (element.JsRuntime() ?? throw NoRuntime()).Call(InvokeFn, element, identifier, args);

    /// <summary> Call the specified JavaScript function without waiting for a result. </summary>
    /// <param name="runtime">Reference to a JavaScript runtime.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    public static void Call(this IJSRuntime runtime, string identifier, params object?[]? args)
    {
        if (runtime is IJSInProcessRuntime sync) sync.Invoke<IJSVoidResult>(identifier, args);
        else runtime.InvokeAsync<IJSVoidResult>(identifier, args).Forget();
    }

    /// <summary> Call the specified JavaScript function without waiting for a result. </summary>
    /// <param name="js">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    public static void Call(this IJSObjectReference js, string identifier, params object?[]? args)
    {
        if (js is IJSInProcessObjectReference sync) sync.Invoke<IJSVoidResult>(identifier, args);
        else js.InvokeAsync<IJSVoidResult>(identifier, args).Forget();
    }

    /// <summary> Call the specified JavaScript function without waiting for a result. </summary>
    /// <param name="async">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    public static void Call(this ValueTask<IJSObjectReference> async, string identifier, params object?[]? args)
    {
        if (async.IsCompleted) async.Result.Call(identifier, args);
        else async.ThenInvokeAsync<IJSVoidResult>(identifier, args).Forget();
    }

    /// <summary> Try to call the specified JavaScript function without waiting for a result. </summary>
    /// <param name="async">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    public static bool TryCall(this ValueTask<IJSObjectReference> async, string identifier, params object?[]? args)
    {
        if (!async.IsCompleted) async.ThenInvokeAsync<IJSVoidResult>(identifier, args).Forget();
        else if (async.Result is { } js) js.Call(identifier, args);
        else return false;
        return true;
    }

    /// <summary> Execute the specified JavaScript function synchronously if possible, otherwise asynchronously. </summary>
    /// <param name="element">Reference to a JavaScript element.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    /// <exception cref="InvalidOperationException">When unable to acquire JS runtime.</exception>
    public static ValueTask<T> Execute<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this ElementReference element, string identifier, params object?[]? args)
        => (element.JsRuntime() ?? throw NoRuntime()).Execute<T>(InvokeFn, element, identifier, args);

    /// <summary> Execute the specified JavaScript function synchronously if possible, otherwise asynchronously. </summary>
    /// <param name="runtime">Reference to a JavaScript runtime.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    [SuppressMessage("Performance", "CA1849", Justification = "By Design")]
    public static ValueTask<T> Execute<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this IJSRuntime runtime, string identifier, params object?[]? args)
    {
        return runtime is IJSInProcessRuntime sync
            ? new(sync.Invoke<T>(identifier, args))
            : runtime.InvokeAsync<T>(identifier, args);
    }

    /// <summary> Execute the specified JavaScript function synchronously if possible, otherwise asynchronously. </summary>
    /// <param name="js">Reference to a JavaScript runtime.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    [SuppressMessage("Performance", "CA1849", Justification = "By Design")]
    public static ValueTask<T> Execute<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this IJSObjectReference js, string identifier, params object?[]? args)
    {
        return js is IJSInProcessObjectReference sync
            ? new(sync.Invoke<T>(identifier, args))
            : js.InvokeAsync<T>(identifier, args);
    }

    /// <summary> Execute the specified JavaScript function synchronously if possible, otherwise asynchronously. </summary>
    /// <param name="async">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    [SuppressMessage("Performance", "CA1849", Justification = "By Design")]
    public static ValueTask<T> Execute<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this ValueTask<IJSObjectReference> async, string identifier, params object?[]? args)
    {
        return async.IsCompleted
            ? async.Result.Execute<T>(identifier, args)
            : async.ThenExecute<T>(identifier, args);
    }

    /// <summary> Invokes the specified JavaScript function. </summary>
    /// <param name="element">Reference to a JavaScript element.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    /// <exception cref="InvalidOperationException">When unable to complete synchronously.</exception>
    public static T Invoke<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this ElementReference element, string identifier, params object?[]? args)
        => (element.JsRuntime() ?? throw NoRuntime()).Invoke<T>(InvokeFn, element, identifier, args);

    /// <summary> Invokes the specified JavaScript function. </summary>
    /// <param name="runtime">Reference to a JavaScript runtime.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    /// <exception cref="InvalidOperationException">When unable to complete synchronously.</exception>
    public static T Invoke<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this IJSRuntime runtime, string identifier, params object?[]? args)
    {
        if (runtime is IJSInProcessRuntime sync)
            return sync.Invoke<T>(identifier, args);

        var async = runtime.InvokeAsync<T>(identifier, args);
        return async.IsCompleted ? async.Result : throw UnableToSync();
    }

    /// <summary> Invokes the specified JavaScript function. </summary>
    /// <param name="js">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    /// <exception cref="InvalidOperationException">When unable to complete synchronously.</exception>
    public static T Invoke<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this IJSObjectReference js, string identifier, params object?[]? args)
    {
        if (js is IJSInProcessObjectReference sync)
            return sync.Invoke<T>(identifier, args);

        var async = js.InvokeAsync<T>(identifier, args);
        return async.IsCompleted ? async.Result : throw UnableToSync();
    }

    /// <summary> Invokes the specified JavaScript. </summary>
    /// <param name="async">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    /// <exception cref="InvalidOperationException">When unable to complete synchronously.</exception>
    public static T Invoke<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this ValueTask<IJSObjectReference> async, string identifier, params object?[]? args)
    {
        return async.IsCompleted
            ? async.Result.Invoke<T>(identifier, args)
            : throw new InvalidOperationException("JS reference is not available synchronously.");
    }

    /// <summary> Invokes the specified JavaScript function asynchronously. </summary>
    /// <param name="element">Reference to a JavaScript element.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <exception cref="InvalidOperationException">When unable to complete synchronously.</exception>
    public static ValueTask InvokeAsync(this ElementReference element, string identifier, params object?[]? args)
        => (element.JsRuntime() ?? throw NoRuntime()).InvokeAsync(InvokeFn, element, identifier, args);

    /// <summary> Invokes the specified JavaScript function asynchronously. </summary>
    /// <param name="runtime">Reference to a JavaScript runtime.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    public static ValueTask InvokeAsync(this IJSRuntime runtime, string identifier, params object?[]? args)
    {
        var async = runtime.InvokeAsync<IJSVoidResult>(identifier, args);
        return async.IsCompleted ? default : new(async.AsTask());
    }

    /// <summary> Invokes the specified JavaScript function asynchronously. </summary>
    /// <param name="js">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments.</param>
    public static ValueTask InvokeAsync(this IJSObjectReference js, string identifier, params object?[]? args)
    {
        var async = js.InvokeAsync<IJSVoidResult>(identifier, args);
        return async.IsCompleted ? default : new(async.AsTask());
    }

    /// <summary> Invokes the specified JavaScript function asynchronously. </summary>
    /// <param name="async">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    public static ValueTask InvokeAsync(this ValueTask<IJSObjectReference> async, string identifier, params object?[]? args)
    {
        return async.IsCompleted
            ? async.Result.InvokeAsync(identifier, args)
            : async.ThenInvokeAsync(identifier, args);
    }

    /// <summary> Invokes the specified JavaScript function asynchronously. </summary>
    /// <param name="element">Reference to a JavaScript element.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    /// <exception cref="InvalidOperationException">When unable to complete synchronously.</exception>
    public static ValueTask<T> InvokeAsync<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this ElementReference element, string identifier, params object?[]? args)
        => (element.JsRuntime() ?? throw NoRuntime()).InvokeAsync<T>(InvokeFn, element, identifier, args);

    /// <summary> Invokes the specified JavaScript function asynchronously. </summary>
    /// <param name="async">Reference to a JavaScript object.</param>
    /// <param name="identifier">An identifier for the function to invoke.</param>
    /// <param name="args">JSON-serializable arguments</param>
    /// <typeparam name="T">The JSON-serializable return type.</typeparam>
    public static ValueTask<T> InvokeAsync<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this ValueTask<IJSObjectReference> async, string identifier, params object?[]? args)
    {
        return async.IsCompleted
            ? async.Result.InvokeAsync<T>(identifier, args)
            : async.ThenInvokeAsync<T>(identifier, args);
    }

    private static async ValueTask<T> ThenExecute<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this ValueTask<IJSObjectReference> lazy, string identifier, object?[]? args)
    {
        Debug.Assert(!lazy.IsCompleted, "Lazy should not be completed");
        return await (await lazy).Execute<T>(identifier, args);
    }

    private static async ValueTask ThenInvokeAsync(this ValueTask<IJSObjectReference> lazy, string identifier, object?[]? args)
    {
        Debug.Assert(!lazy.IsCompleted, "Lazy should not be completed");
        await (await lazy).InvokeAsync<IJSVoidResult>(identifier, args);
    }

    private static async ValueTask<T> ThenInvokeAsync<[DynamicallyAccessedMembers(InvokeDynamicTypes)] T>(this ValueTask<IJSObjectReference> lazy, string identifier, object?[]? args)
    {
        Debug.Assert(!lazy.IsCompleted, "Lazy should not be completed");
        return await (await lazy).InvokeAsync<T>(identifier, args);
    }

    private static InvalidOperationException UnableToSync() => new("Unable to complete synchronously.");
}
