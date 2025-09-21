namespace EdSyl.Collections;

/// <summary> Defines methods to support both: <see cref="IComparer{T}" /> and <see cref="IEqualityComparer{T}" /> </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
public interface IUnifiedComparer<in T> : IComparer<T>, IEqualityComparer<T>;
