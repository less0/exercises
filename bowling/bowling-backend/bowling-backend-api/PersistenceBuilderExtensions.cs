using System.Data.Common;
using bowling_backend_applicaton;
using bowling_backend_applicaton.Interfaces;
using bowling_backend_core.DomainModel;
using bowling_backend_persistence;
using Microsoft.EntityFrameworkCore;

namespace bowling_backend_api;

static class PersistenceBuilderExtensions
{
    internal static void AddPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddScoped<BowlingDataContext>();
        builder.Services.AddScoped<IRepository<BowlingGame>, BowlingGameRepository>();
        builder.Services.AddScoped<IBowlingCommands, BowlingCommands>();
        builder.Services.AddScoped<IBowlingQueries, BowlingQueries>();       
    }

    
}