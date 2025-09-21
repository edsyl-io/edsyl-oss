namespace EdSyl;

/// <summary> Defines how to match query against the text. </summary>
/// <param name="text">A text to search with the given query.</param>
/// <param name="query">A pattern to look for within a text.</param>
/// <param name="comparison">Comparison rule to use while looking for match.</param>
public delegate bool StringMatchOperator(ReadOnlySpan<char> text, ReadOnlySpan<char> query, StringComparison comparison);
