using System.Runtime.CompilerServices;

namespace EdSyl.Razor.Components;

public sealed class Scene : ComponentBase, IDisposable
{
    /// <summary> Most recently activated scene. </summary>
    /// <exception cref="InvalidOperationException">When there is no active scene.</exception>
    public static Scene Latest => Scenes.LastOrDefault() ?? throw new InvalidOperationException("No active Scene available");

    private static readonly List<Scene> Scenes = [];
    private readonly HashSet<Actor> actors = [];

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    public IActor<T> Spawn<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(T? example = default) where T : IComponent
    {
        var actor = new Actor<T>(this, example.GetParameters());
        actors.Add(actor);
        StateHasChanged();
        return actor;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Scenes.Remove(this);
        actors.Clear();
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Scenes.Add(this);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascade(this, ChildContent);
        builder.OpenRegion(10);

        foreach (var actor in actors)
            actor.Render(builder);

        builder.CloseRegion();
    }

    private abstract class Actor(Scene scene) : IActor
    {
        /// <inheritdoc />
        public ILifecycle Lifecycle { get; private set; } = ILifecycle.New();

        public abstract void Render(RenderTreeBuilder builder);

        /// <inheritdoc />
        public void Dispose()
        {
            scene.actors.Remove(this);
            Lifecycle.Dispose();
            Lifecycle = ILifecycle.Dead;
            scene.StateHasChanged();
        }
    }

    private sealed class Actor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T> : Actor, IActor<T>
    {
        private readonly RenderFragment rf;
        private readonly Action<object> capture;
        private readonly TaskCompletionSource<T> reference = new();
        private Dictionary<string, object>? parameters;

        public Actor(Scene scene, Dictionary<string, object>? parameters) : base(scene)
        {
            capture = Capture;
            rf = RenderComponent;
            this.parameters = parameters;
        }

        public TaskAwaiter<T> GetAwaiter()
        {
            return reference.Task.GetAwaiter();
        }

        public override void Render(RenderTreeBuilder builder)
        {
            builder.Cascade(this, rf);
        }

        private void RenderComponent(RenderTreeBuilder builder)
        {
            builder.OpenComponent(0, typeof(T));
            builder.SetKey(this);
            builder.AddMultipleAttributes(1, parameters);
            builder.AddComponentReferenceCapture(2, capture);
            builder.CloseComponent();

            // attach parameters only once
            parameters = null;
        }

        private void Capture(object instance)
        {
            reference.SetResult((T)instance);
        }
    }
}
