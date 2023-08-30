namespace editable_table_backend.Persistence;

static class ServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services)
    {
        // We are registering the repositories as singletons to keep the data between
        // calls to the API
        services.AddSingleton<IPersonsRepository, InMemoryPersonsRepository>();
        services.AddSingleton<IDepartmentsRepository, InMemoryDepartmentsRepository>();
    }
}