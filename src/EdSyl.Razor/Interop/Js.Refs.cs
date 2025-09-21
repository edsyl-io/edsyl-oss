using EdSyl.Async;
using Microsoft.JSInterop;
using static System.Threading.Tasks.TaskContinuationOptions;

namespace EdSyl.Razor.Interop;

public static partial class Js
{
    private const string IdentityFn = "edsyl.identity";

    /// <summary> Get JavaScript object reference for the provided element. </summary>
    /// <param name="element">Element to get reference for.</param>
    /// <exception cref="InvalidOperationException">When unable to acquire JS runtime.</exception>
    public static ValueTask<IJSObjectReference> JsRef(this ElementReference element)
        => (element.JsRuntime() ?? throw NoRuntime()).JsRef(element);

    /// <summary> Get JavaScript object reference for the provided element. </summary>
    /// <param name="runtime">JS runtime.</param>
    /// <param name="element">Element to get reference for.</param>
    public static ValueTask<IJSObjectReference> JsRef(this IJSRuntime runtime, ElementReference element)
        => runtime.Execute<IJSObjectReference>(IdentityFn, element);

    /// <summary> Get JS object reference from the backing field. </summary>
    /// <param name="frugal">Field holding JS object or promise.</param>
    public static ValueTask<IJSObjectReference> ReadJsRef(object frugal) => frugal switch
    {
        IJSObjectReference js => new(js),
        Task<IJSObjectReference> task => new(task),
        _ => default,
    };

    /// <summary> Acquire JS object reference for the given element. </summary>
    /// <param name="self">Object holding a backing field.</param>
    /// <param name="element">Element to get reference for.</param>
    /// <param name="consume">Action to modify the backing field.</param>
    /// <typeparam name="T">Type of the object holding a backing field.</typeparam>
    public static ValueTask<IJSObjectReference> CreateJsRef<T>(T self, ElementReference element, Action<T, object> consume)
    {
        var async = element.JsRef();
        SetJsRef(self, async, consume);
        return async;
    }

    /// <summary> Set value of JS object reference backing field. </summary>
    /// <param name="self">Object holding a backing field.</param>
    /// <param name="value">JS object reference promise.</param>
    /// <param name="consume">Action to modify the backing field.</param>
    /// <typeparam name="T">Type of the object holding a backing field.</typeparam>
    public static void SetJsRef<T>(T self, ValueTask<IJSObjectReference> value, Action<T, object> consume)
    {
        if (value.IsCompleted) consume(self, value.Result);
        else consume(self, Wait(self, value, consume));

        static async Task<IJSObjectReference> Wait(T self, ValueTask<IJSObjectReference> async, Action<T, object> consume)
        {
            var value = await async;
            consume(self, value);
            return value;
        }
    }

    /// <summary> Dispose provided js reference. </summary>
    /// <param name="js">JS object to dispose.</param>
    public static void Dispose(this IJSObjectReference js)
    {
        if (js is IJSInProcessObjectReference sync) sync.Dispose();
        else js.DisposeAsync().Forget();
    }

    /// <summary> Dispose JS object reference from the backing field. </summary>
    /// <param name="frugal">Field holding JS object or promise.</param>
    public static void DisposeJsRef(ref object? frugal)
    {
        switch (Interlocked.Exchange(ref frugal, default))
        {
            case IJSObjectReference js:
                js.Dispose();
                break;

            case Task<IJSObjectReference> task:
                Dispose(task);
                break;
        }
    }

    private static void Dispose(Task<IJSObjectReference> task)
    {
        if (task.IsCompleted) task.Result.Dispose();
        else task.ContinueWith(Dispose, OnlyOnRanToCompletion).Forget();
    }
}
