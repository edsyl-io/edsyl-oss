using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using EdSyl.Async;
using Microsoft.EntityFrameworkCore.Query;

namespace EdSyl.EFCore;

[SuppressMessage("Roslynator", "RCS1047", Justification = "Interface Implementation")]
[RequiresUnreferencedCode("Enumerating in-memory collections as IQueryable can require unreferenced code because expressions referencing IQueryable extension methods can get rebound to IEnumerable extension methods. The IEnumerable extension methods could be trimmed causing the application to fail at runtime.")]
public sealed class EmptyQuery<T> : IListSource, IAsyncEnumerable<T>, IAsyncQueryProvider, IOrderedQueryable<T>
{
    private static readonly IQueryProvider EnumerableProvider = new EnumerableQuery<T>([]);

    public EmptyQuery() => Expression = Expression.Constant(this);
    public EmptyQuery(Expression expression) => Expression = expression;

    /// <inheritdoc />
    public Type ElementType => typeof(T);

    /// <inheritdoc />
    public Expression Expression { get; }

    /// <inheritdoc />
    public IQueryProvider Provider => this;

    /// <inheritdoc />
    public bool ContainsListCollection => true;

    /// <inheritdoc />
    public IList GetList() => ImmutableList<T>.Empty;

    /// <inheritdoc />
    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = EnumerableProvider.CreateQuery(expression).ElementType;
        var genericType = typeof(EmptyQuery<>).MakeGenericType(elementType);
        return (IQueryable)Activator.CreateInstance(genericType, expression)!;
    }

    /// <inheritdoc />
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return new EmptyQuery<TElement>(expression);
    }

    /// <inheritdoc />
    public object? Execute(Expression expression)
        => EnumerableProvider.Execute(expression);

    /// <inheritdoc />
    public TResult Execute<TResult>(Expression expression)
        => EnumerableProvider.Execute<TResult>(expression);

    /// <inheritdoc />
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        => Tasks<TResult>.Empty;

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => EmptyAsyncEnumerator.Default;

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
        => EmptyAsyncEnumerator.Default;

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
        => EmptyAsyncEnumerator.Default;

    private class EmptyAsyncEnumerator : IAsyncEnumerator<T>, IEnumerator<T>
    {
        public static readonly EmptyAsyncEnumerator Default = new();

        /// <inheritdoc cref="IAsyncEnumerator{T}.Current" />
        public T Current => default!;

        /// <inheritdoc cref="IEnumerator{T}.Current" />
        object IEnumerator.Current => default!;

        /// <inheritdoc />
        public ValueTask DisposeAsync() => default;

        /// <inheritdoc />
        public ValueTask<bool> MoveNextAsync() => default;

        /// <inheritdoc />
        public bool MoveNext() => default;

        /// <inheritdoc />
        public void Reset() { }

        /// <inheritdoc />
        public void Dispose() { }
    }
}

file static class Tasks<TResult>
{
    public static readonly TResult Empty =
        (TResult)Tasks.FromResultMethod
            .MakeGenericMethod(typeof(TResult).GetGenericArguments()[0])
            .Invoke(null, [null])!;
}
