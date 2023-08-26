using System.Data.Common;
using bowling_backend_persistence;
using Microsoft.EntityFrameworkCore;

namespace bowling_backend_api;

internal static class PersistenceAppExtensions
{
    /// <summary>
    /// Migrates the database at startup. This method waits up to 120 s for the database to be available. This is 
    /// currently necessary for docker compose, because SQL Server Express takes some time to spin up.
    /// </summary>
    internal static async Task MigrateDatabase(this WebApplication app)
    {

        if (app == null)
        {
            return;
        }

        BowlingDataContext dataContext = new(app.Configuration);
        var logger = app.Logger;

        TimeSpan timeout = TimeSpan.FromSeconds(120);
        DateTime startedAt = DateTime.Now;

        logger.LogInformation("Migrating database");

        var completed = false;
        do
        {
            try
            {
                await dataContext.Database.MigrateAsync();
                completed = true;
                logger.LogInformation("Migrated database.");
            }
            catch (DbException)
            {
                if (DateTime.Now - startedAt > timeout)
                {
                    throw new TimeoutException();
                }

                logger.LogInformation("Database connection failed. Retrying in 3 s.");
                await Task.Delay(3000);
            }
        } while (!completed);
    }
}
