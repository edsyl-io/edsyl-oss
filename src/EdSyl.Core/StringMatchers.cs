namespace EdSyl;

public static class StringMatchers
{
    public static readonly StringMatchOperator Exact = System.MemoryExtensions.Equals;
    public static readonly StringMatchOperator Contains = System.MemoryExtensions.Contains;
    public static readonly StringMatchOperator StartsWith = System.MemoryExtensions.StartsWith;
    public static readonly StringMatchOperator EndsWith = System.MemoryExtensions.EndsWith;

    public static bool Match(this SpanPredicate<char> predicate, object value)
    {
        return predicate(value.ToString());
    }

    public static bool Match(this SpanPredicate<char> predicate, string value)
    {
        return predicate(value);
    }
}
