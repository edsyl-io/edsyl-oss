namespace EdSyl.EFCore.Design;

/// <summary>
/// Source: https://github.com/dotnet/efcore/blob/main/src/EFCore.Design/Migrations/Design/MigrationsScaffolder.cs .
/// </summary>
public class SqlMigrationsScaffolder : MigrationsScaffolder
{
    public SqlMigrationsScaffolder(MigrationsScaffolderDependencies dependencies) : base(dependencies) { }

    /// <inheritdoc />
    public override ScaffoldedMigration ScaffoldMigration(string migrationName, string? rootNamespace, string? subNamespace = null, string? language = null, bool dryRun = false)
    {
        ArgumentNullException.ThrowIfNull(migrationName);

        // replace illegible characters with underscores
        migrationName = migrationName.Replace('-', '_');

        // allow repetitive names by adding a timestamp
        migrationName = Dependencies.MigrationsIdGenerator.GenerateId(migrationName);

        return base.ScaffoldMigration(migrationName, rootNamespace, subNamespace, language, dryRun);
    }

    /// <inheritdoc />
    public override MigrationFiles Save(string projectDir, ScaffoldedMigration migration, string? outputDir, bool dryRun)
    {
        ArgumentNullException.ThrowIfNull(migration);
        var files = base.Save(projectDir, migration, outputDir, dryRun);

        // remove unwanted .Design files
        if (migration.MetadataCode == SqlMigrationsGenerator.EmptyFile)
        {
            File.Delete(files.MetadataFile!);
            files.MetadataFile = null;
        }

        var prev = migration.PreviousMigrationId ?? Migration.InitialDatabase;
        var curr = migration.MigrationId;

        var up = Dependencies.Migrator.GenerateScript(prev, curr);
        File.WriteAllText(Path.ChangeExtension(files.MigrationFile, ".up.sql")!, up);

        var down = Dependencies.Migrator.GenerateScript(curr, prev);
        File.WriteAllText(Path.ChangeExtension(files.MigrationFile, ".down.sql")!, down);

        return files;
    }
}
