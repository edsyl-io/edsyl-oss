namespace EdSyl.EFCore.Design;

/// <summary>
/// Source: https://github.com/dotnet/efcore/blob/main/src/EFCore.Design/Migrations/Design/CSharpMigrationsGenerator.cs .
/// </summary>
[SuppressMessage("Usage", "EF1001", Justification = "Internal EF Core API usage.")]
public class SqlMigrationsGenerator : CSharpMigrationsGenerator
{
    internal const string EmptyFile = "// file is intentionally left blank for deleteion";
    private const bool DisableDesignFiles = false;

    private static readonly NamespaceComparer NamespaceComparer = new();

    private static readonly HashSet<string> UnnecessaryImports = new(StringComparer.Ordinal)
    {
        "System",
    };

    private readonly IModel currentModel;
    private readonly ICurrentDbContext currentContext;
    private readonly IMigrationsAssembly migrationsAssembly;
    private readonly SqlMigrationsIdGenerator migrationsIdGenerator;

    private ICSharpHelper Code => CSharpDependencies.CSharpHelper;

    public SqlMigrationsGenerator(
        MigrationsCodeGeneratorDependencies dependencies,
        CSharpMigrationsGeneratorDependencies csharpDependencies,
        IModel currentModel,
        ICurrentDbContext currentContext,
        IMigrationsAssembly migrationsAssembly,
        IMigrationsIdGenerator migrationsIdGenerator) : base(dependencies, csharpDependencies)
    {
        this.currentModel = currentModel;
        this.currentContext = currentContext;
        this.migrationsAssembly = migrationsAssembly;
        this.migrationsIdGenerator = (SqlMigrationsIdGenerator)migrationsIdGenerator;
    }

    /// <inheritdoc />
    public override string GenerateMigration(
        string? migrationNamespace,
        string migrationName,
        IReadOnlyList<MigrationOperation> upOperations,
        IReadOnlyList<MigrationOperation> downOperations)
    {
        var migrationId = migrationsIdGenerator.Last!;
        var contextType = currentContext.Context.GetType();

        SqlDynamicMigration.RegisterOperations(migrationName, upOperations, downOperations);
        SqlDynamicMigration.RegisterMigration(migrationId, migrationName, currentModel, migrationsAssembly);

        var builder = new IndentedStringBuilder();
        var namespaces = new HashSet<string>(StringComparer.Ordinal)
        {
            "Microsoft.EntityFrameworkCore.Infrastructure",
            "Microsoft.EntityFrameworkCore.Migrations",
            typeof(SqlMigration).Namespace!,
        };

        namespaces.UnionWith(GetNamespaces(upOperations));
        namespaces.UnionWith(GetNamespaces(downOperations));
        namespaces.ExceptWith(UnnecessaryImports);

        WriteImports(builder, namespaces);
        if (migrationNamespace != null)
            WriteNamespace(builder, migrationNamespace);

        builder
            .Append("[DbContext(typeof(").Append(Code.Reference(contextType)).AppendLine("))]")
            .Append("[Migration(").Append(Code.Literal(migrationId)).AppendLine(")]")
            .Append("public partial class ").Append(Code.Identifier(migrationId))
            .Append(" : ").AppendLine(nameof(SqlMigration))
            .AppendLine("{");

        using (builder.Indent())
        {
            builder
                .AppendLine("/// <inheritdoc />")
                .AppendLine("protected override void Up(MigrationBuilder migrationBuilder)")
                .AppendLine("{");

            using (builder.Indent())
                CSharpDependencies.CSharpMigrationOperationGenerator.Generate("migrationBuilder", upOperations, builder);

            builder
                .AppendLine()
                .AppendLine("}")
                .AppendLine()
                .AppendLine("/// <inheritdoc />")
                .AppendLine("protected override void Down(MigrationBuilder migrationBuilder)")
                .AppendLine("{");

            using (builder.Indent())
                CSharpDependencies.CSharpMigrationOperationGenerator.Generate("migrationBuilder", downOperations, builder);

            builder
                .AppendLine()
                .AppendLine("}");
        }

        builder.AppendLine("}");

        return builder.ToString();
    }

    /// <inheritdoc />
    public override string GenerateMetadata(
        string? migrationNamespace,
        Type contextType,
        string migrationName,
        string migrationId,
        IModel targetModel)
    {
        if (DisableDesignFiles)
            return EmptyFile;

        var builder = new IndentedStringBuilder();
        var namespaces = new HashSet<string>(StringComparer.Ordinal)
        {
            "Microsoft.EntityFrameworkCore.Infrastructure",
            "Microsoft.EntityFrameworkCore.Migrations",
        };

        namespaces.UnionWith(GetNamespaces(targetModel));
        namespaces.ExceptWith(UnnecessaryImports);

        WriteImports(builder, namespaces);
        if (migrationNamespace != null)
            WriteNamespace(builder, migrationNamespace);

        builder
            .Append("[DbContext(typeof(").Append(Code.Reference(contextType)).AppendLine("))]")
            .Append("[Migration(").Append(Code.Literal(migrationId)).AppendLine(")]")
            .Append("public partial class ").Append(Code.Identifier(migrationName)).Append("Migration")
            .AppendLine(" { }");

        return builder.ToString();
    }

    /// <inheritdoc />
    public override string GenerateSnapshot(
        string? modelSnapshotNamespace,
        Type contextType,
        string modelSnapshotName,
        IModel model)
    {
        var builder = new IndentedStringBuilder();
        var namespaces = new HashSet<string>(StringComparer.Ordinal)
        {
            "Microsoft.EntityFrameworkCore",
            "Microsoft.EntityFrameworkCore.Infrastructure",
            "Microsoft.EntityFrameworkCore.Storage.ValueConversion",
        };

        namespaces.UnionWith(GetNamespaces(model));
        namespaces.ExceptWith(UnnecessaryImports);
        WriteImports(builder, namespaces);
        if (modelSnapshotNamespace != null)
            WriteNamespace(builder, modelSnapshotNamespace);

        builder
            .Append("[DbContext(typeof(").Append(Code.Reference(contextType)).AppendLine("))]")
            .Append("public class ").Append(Code.Identifier(modelSnapshotName)).AppendLine(" : ModelSnapshot")
            .AppendLine("{");

        using (builder.Indent())
        {
            builder
                .AppendLine("protected override void BuildModel(ModelBuilder modelBuilder)")
                .AppendLine("{");

            using (builder.Indent())
                CSharpDependencies.CSharpSnapshotGenerator.Generate("modelBuilder", model, builder);

            builder.AppendLine("}");
        }

        builder.AppendLine("}");

        return builder.ToString();
    }

    private static void WriteImports(IndentedStringBuilder builder, IEnumerable<string> namespaces)
    {
        foreach (var n in namespaces.OrderBy(x => x, NamespaceComparer))
            builder.Append("using ").Append(n).AppendLine(";");
    }

    private void WriteNamespace(IndentedStringBuilder builder, string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            builder
                .AppendLine()
                .Append("namespace ").Append(Code.Namespace(name))
                .AppendLine(";")
                .AppendLine();
        }
    }
}
