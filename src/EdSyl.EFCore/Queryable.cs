namespace EdSyl.EFCore;

/// <summary> <see cref="IQueryable{T}" /> generic utilities. </summary>
/// <typeparam name="T">Type of the elements.</typeparam>
[SuppressMessage("Design", "MA0018:Do not declare static members on generic types", Justification = "Caching")]
public static class Queryable<T>
{
    /// <summary> An empty <see cref="IQueryable{T}" /> which yields no results. </summary>
    public static readonly IQueryable<T> Empty = new EmptyQuery<T>();
}
