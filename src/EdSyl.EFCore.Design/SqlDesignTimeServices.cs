namespace EdSyl.EFCore.Design;

public class SqlDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMigrationsCodeGenerator, SqlMigrationsGenerator>();
        serviceCollection.AddSingleton<IMigrationsIdGenerator, SqlMigrationsIdGenerator>();
        serviceCollection.AddSingleton<IMigrationsScaffolder, SqlMigrationsScaffolder>();
    }
}
