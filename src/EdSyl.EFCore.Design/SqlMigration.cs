namespace EdSyl.EFCore.Design;

/// <summary>
/// Base class for SQL migrations.
/// </summary>
[SuppressMessage("Usage", "EF1001", Justification = "Internal EF Core API usage.")]
public abstract class SqlMigration : Migration
{
    private const string Delimiter = "DELIMITER";

    private bool? isScript;
    private EmbeddedFileProvider? provider;
    private IReadOnlyList<MigrationOperation>? up;
    private IReadOnlyList<MigrationOperation>? down;

    public sealed override IReadOnlyList<MigrationOperation> UpOperations
        => up ??= IsScript
            ? base.UpOperations
            : SqlOperations(".up.sql");

    public sealed override IReadOnlyList<MigrationOperation> DownOperations
        => down ??= IsScript
            ? base.DownOperations
            : down ??= SqlOperations(".down.sql");

    /// <summary> Check if currently generating an SQL script. </summary>
    /// <seealso cref="IMigrator.GenerateScript" />
    private bool IsScript => isScript ??= HasGenerateScriptMethod(new(1, false));

    private EmbeddedFileProvider Provider => provider ??= new(GetType().Assembly, string.Empty);

    private IReadOnlyList<MigrationOperation> SqlOperations(string suffix)
    {
        var builder = new MigrationBuilder(ActiveProvider);
        RunSql(builder, suffix);
        return builder.Operations;
    }

    private void RunSql(MigrationBuilder builder, string suffix)
    {
        var file = GetFile(suffix);
        var sql = ReadAllText(file);
        sql = PrepareSql(sql);
        builder.Sql(sql, IsTransactional(sql));
    }

    private IFileInfo GetFile(string suffix)
    {
        return Provider.GetFileInfo(string.Concat(this.GetId(), suffix));
    }

    private static bool IsTransactional(string sql)
    {
        return sql.Contains("START TRANSACTION;", StringComparison.OrdinalIgnoreCase);
    }

    private static string ReadAllText(IFileInfo file)
    {
        using var stream = file.CreateReadStream();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static bool HasGenerateScriptMethod(StackTrace stackTrace)
    {
        for (int i = 0, count = Math.Min(10, stackTrace.FrameCount); i < count; i++)
        {
            if (IsGenerateScriptMethod(stackTrace.GetFrame(i)!))
                return true;
        }

        return false;
    }

    private static bool IsGenerateScriptMethod(StackFrame frame)
    {
        return frame.GetMethod()?.Name == nameof(IMigrator.GenerateScript);
    }

    private static string PrepareSql(string sql)
    {
        return sql.Contains(Delimiter, StringComparison.OrdinalIgnoreCase)
            ? string.Join('\n', PrepareSql(sql.Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)))
            : sql;
    }

    private static IEnumerable<string> PrepareSql(IEnumerable<string> lines)
    {
        string? delimiter = null;
        foreach (var line in lines)
        {
            // check if delimiter definition
            if (line.StartsWith(Delimiter, StringComparison.OrdinalIgnoreCase))
            {
                delimiter = line.AsSpan().Trim()[(Delimiter.Length + 1)..].ToString();
                if (delimiter == ";") delimiter = null;
                continue;
            }

            // check if ends with delimiter
            if (delimiter != null && line.EndsWith(delimiter, StringComparison.OrdinalIgnoreCase))
            {
                yield return line[..^delimiter.Length] + ";";
                continue;
            }

            yield return line;
        }
    }
}
