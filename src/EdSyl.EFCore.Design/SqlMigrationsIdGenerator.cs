namespace EdSyl.EFCore.Design;

/// <summary>
/// Source: https://github.com/dotnet/efcore/blob/main/src/EFCore.Relational/Migrations/Internal/MigrationsIdGenerator.cs .
/// </summary>
[SuppressMessage("Usage", "EF1001", Justification = "Internal EF Core API usage.")]
public class SqlMigrationsIdGenerator : MigrationsIdGenerator
{
    /// <summary> Last generated identifier. </summary>
    public string? Last { get; private set; }

    /// <inheritdoc />
    public override string GenerateId(string name)
    {
        return Last = !IsValidId(name)
            ? base.GenerateId(name)
            : name;
    }
}
