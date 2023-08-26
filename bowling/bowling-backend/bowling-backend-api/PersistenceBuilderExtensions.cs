using System.Data.Common;
using bowling_backend_applicaton;
using bowling_backend_applicaton.Interfaces;
using bowling_backend_core.DomainModel;
using bowling_backend_persistence;
using Microsoft.EntityFrameworkCore;

namespace bowling_backend_api;

static class PersistenceBuilderExtensions
{
    internal static async Task AddPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddScoped<BowlingDataContext>();
        builder.Services.AddScoped<IRepository<BowlingGame>, BowlingGameRepository>();
        builder.Services.AddScoped<IBowlingCommands, BowlingCommands>();
        builder.Services.AddScoped<IBowlingQueries, BowlingQueries>();

        await builder.MigrateDatabase();        
    }

    private static async Task MigrateDatabase(this WebApplicationBuilder? builder)
    {
        if (builder == null)
        {
            return;
        }

        BowlingDataContext dataContext = new(builder.Configuration);

        TimeSpan timeout = TimeSpan.FromSeconds(120);
        DateTime startedAt = DateTime.Now;

        Console.Write("Migrating database");

        var completed = false;
        do
        {
            try
            {
                Console.Write(".");
                await dataContext.Database.MigrateAsync();
                completed = true;
                Console.WriteLine();
                Console.WriteLine("Migrated database.");
            }
            catch (DbException)
            {
                if (DateTime.Now - startedAt > timeout)
                {
                    throw new TimeoutException();
                }
                await Task.Delay(3000);
            }
        } while (!completed);
    }
}