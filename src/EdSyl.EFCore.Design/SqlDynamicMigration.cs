using System.Reflection;
using System.Reflection.Emit;

namespace EdSyl.EFCore.Design;

public class SqlDynamicMigration : Migration
{
    private static readonly Dictionary<string, State> Migrations = new(StringComparer.OrdinalIgnoreCase);

    /// <summary> Register operations for a migration with particular name. </summary>
    /// <param name="name">Name of a migration.</param>
    /// <param name="up">Operations that will migrate the database 'up'.</param>
    /// <param name="down">Operations that will migrate the database 'down'.</param>
    /// <seealso cref="Migration.UpOperations" />
    /// <seealso cref="Migration.DownOperations" />
    public static void RegisterOperations(string name, IReadOnlyList<MigrationOperation> up, IReadOnlyList<MigrationOperation> down)
    {
        Migrations[name] = new()
        {
            UpOperations = up,
            DownOperations = down,
        };
    }

    /// <summary> Register a migration into an assembly with a runtime generate type instance. </summary>
    /// <param name="id">Identifier to use for <see cref="MigrationAttribute" />.</param>
    /// <param name="name">Name of the migration to use for operations lookup.</param>
    /// <param name="model">A model that the database will map to after the migration has been applied.</param>
    /// <param name="assembly">An assembly containing <see cref="IMigrationsAssembly.Migrations" />.</param>
    public static void RegisterMigration(string id, string name, IModel model, IMigrationsAssembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var config = Migrations[name];
        config.TargetModel = model;
        Migrations[id] = config;
        ((IDictionary<string, TypeInfo>)assembly.Migrations).Add(id, DefineDynamicMigration(id));
    }

    internal static TypeInfo DefineDynamicMigration(string id)
    {
        var an = new AssemblyName("SqlDynamicMigration.Dyanamic");
        var ab = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
        var mb = ab.DefineDynamicModule(an.Name!);
        var tb = mb.DefineType(id, TypeAttributes.Public, typeof(SqlDynamicMigration));

        tb.SetCustomAttribute(new(
            typeof(MigrationAttribute).GetConstructor(new[] { typeof(string) })!,
            new object[] { id }
        ));

        return tb.CreateTypeInfo();
    }

    /// <summary>
    /// Migration identifier with the same name as the type.
    /// </summary>
    public string Id => GetType().Name;

    /// <inheritdoc />
    public sealed override IModel TargetModel => Migrations[Id].TargetModel;

    /// <inheritdoc />
    public sealed override IReadOnlyList<MigrationOperation> UpOperations => Migrations[Id].UpOperations;

    /// <inheritdoc />
    public sealed override IReadOnlyList<MigrationOperation> DownOperations => Migrations[Id].DownOperations;

    /// <inheritdoc />
    protected sealed override void Up(MigrationBuilder migrationBuilder)
        => throw new NotSupportedException();

    /// <inheritdoc />
    protected sealed override void Down(MigrationBuilder migrationBuilder)
        => throw new NotSupportedException();

    private struct State
    {
        public IModel TargetModel;
        public IReadOnlyList<MigrationOperation> UpOperations;
        public IReadOnlyList<MigrationOperation> DownOperations;
    }
}
