namespace EdSyl;

/// <summary> Predicate expecting <see cref="ReadOnlySpan{T}" /> as input parameter. </summary>
/// <param name="item">Item to compare against the criteria.</param>
/// <typeparam name="T">Type of items in the <see cref="ReadOnlySpan{T}" />.</typeparam>
public delegate bool SpanPredicate<T>(ReadOnlySpan<T> item);
