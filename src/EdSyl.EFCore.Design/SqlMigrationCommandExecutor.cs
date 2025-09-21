namespace EdSyl.EFCore.Design;

/// <summary>
/// Filters out commands migrations history altering as they already part of the SQL script.
/// </summary>
[SuppressMessage("Usage", "EF1001", Justification = "Internal EF Core API usage.")]
public class SqlMigrationCommandExecutor : MigrationCommandExecutor
{
    private readonly string insertHistory;
    private readonly string deleteHistory;

    public SqlMigrationCommandExecutor(IExecutionStrategy executionStrategy, ISqlGenerationHelper sqlGenerationHelper) : base(executionStrategy)
    {
        ArgumentNullException.ThrowIfNull(sqlGenerationHelper);
        var table = sqlGenerationHelper.DelimitIdentifier("__EFMigrationsHistory", schema: null);
        insertHistory = $"INSERT INTO {table} (";
        deleteHistory = $"DELETE FROM {table} WHERE";
    }

    /// <inheritdoc />
    public override void ExecuteNonQuery(IEnumerable<MigrationCommand> migrationCommands, IRelationalConnection connection)
        => base.ExecuteNonQuery(migrationCommands.Where(Match), connection);

    /// <inheritdoc />
    public override int ExecuteNonQuery(IReadOnlyList<MigrationCommand> migrationCommands, IRelationalConnection connection, MigrationExecutionState executionState, bool commitTransaction, IsolationLevel? isolationLevel = null)
        => base.ExecuteNonQuery([.. migrationCommands.Where(Match)], connection, executionState, commitTransaction, isolationLevel);

    /// <inheritdoc />
    public override Task ExecuteNonQueryAsync(IEnumerable<MigrationCommand> migrationCommands, IRelationalConnection connection, CancellationToken cancellationToken = default)
        => base.ExecuteNonQueryAsync(migrationCommands.Where(Match), connection, cancellationToken);

    /// <inheritdoc />
    public override Task<int> ExecuteNonQueryAsync(IReadOnlyList<MigrationCommand> migrationCommands, IRelationalConnection connection, MigrationExecutionState executionState, bool commitTransaction, IsolationLevel? isolationLevel = null, CancellationToken cancellationToken = default)
        => base.ExecuteNonQueryAsync([.. migrationCommands.Where(Match)], connection, executionState, commitTransaction, isolationLevel, cancellationToken);

    private bool Match(MigrationCommand command)
        => !IsHistory(command.CommandText);

    private bool IsHistory(string sql)
        => sql.StartsWith(insertHistory, StringComparison.Ordinal)
        || sql.StartsWith(deleteHistory, StringComparison.Ordinal);
}
